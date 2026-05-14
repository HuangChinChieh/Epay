using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SkyPay.Backend
{
    public partial class sendManualOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";

            var amountStr = Request.Params["amount"];
            var serviceType = Request.Params["ServiceType"];
            var currencyType = Request.Params["CurrencyType"];
            var companyIDStr = Request.Params["CompanyID"];

            if (string.IsNullOrWhiteSpace(amountStr) || string.IsNullOrWhiteSpace(serviceType) ||
                string.IsNullOrWhiteSpace(currencyType) || string.IsNullOrWhiteSpace(companyIDStr))
            {
                Response.Write(JsonConvert.SerializeObject(new { Status = -1, Message = "缺少必要參數" }));
                Response.End();
                return;
            }

            decimal amount;
            int companyID;
            if (!decimal.TryParse(amountStr, out amount) || !int.TryParse(companyIDStr, out companyID))
            {
                Response.Write(JsonConvert.SerializeObject(new { Status = -1, Message = "參數格式錯誤" }));
                Response.End();
                return;
            }

            var result = CreateOrder(amount, serviceType, currencyType, companyID);
            Response.Write(JsonConvert.SerializeObject(result));
            Response.End();
        }

        private object CreateOrder(decimal amount, string serviceType, string currencyType, int companyID)
        {
            BackendDB backendDB = new BackendDB();
            var companyModel = backendDB.GetCompanyWithKeyByCompanyID(companyID);

            if (companyModel == null)
                return new { Status = -1, Message = "找不到商戶資料" };

            string apiUrl = Pay.IsTestSite
                ? "http://gpay.dev4.mts.idv.tw/api/Gateway/RequirePayment"
                : "https://www.richpay888.com/api/Gateway/RequirePayment";

            string returnUrl = Pay.IsTestSite
                ? "http://gpay.dev4.mts.idv.tw/api/ProviderResult/GPayTestCompanyReturn?result=AAA"
                : "https://www.richpay888.com/api/ProviderResult/GPayTestCompanyReturn?result=AAA";

            var companyCode = companyModel.CompanyCode;
            var companyKey = companyModel.CompanyKey;
            var orderID = Guid.NewGuid().ToString("N");
            var orderDate = DateTime.Now;

            var sign = GetGPaySign(orderID, amount, orderDate, serviceType, currencyType, companyCode, companyKey);

            var formData = new System.Collections.Specialized.NameValueCollection();
            formData.Add("CompanyCode", companyCode);
            formData.Add("CurrencyType", currencyType);
            formData.Add("ServiceType", serviceType);
            formData.Add("ClientIP", GetClientIP());
            formData.Add("OrderID", orderID);
            formData.Add("OrderDate", orderDate.ToString("yyyy-MM-dd HH:mm:ss"));
            formData.Add("OrderAmount", amount.ToString("#.##"));
            formData.Add("ReturnURL", returnUrl);
            formData.Add("Sign", sign);

            try
            {
                var responseText = PostFormData(apiUrl, formData);

                // 閘道可能回傳 JSON（有 Url 欄位）或直接 HTML 跳轉
                if (!string.IsNullOrWhiteSpace(responseText) &&
                    (responseText.TrimStart().StartsWith("{") || responseText.TrimStart().StartsWith("[")))
                {
                    var json = JObject.Parse(responseText);
                    string status = json["Status"]?.ToString() ?? json["status"]?.ToString() ?? "-1";
                    string payUrl = json["Url"]?.ToString() ?? json["url"]?.ToString() ?? string.Empty;
                    string message = json["Message"]?.ToString() ?? json["message"]?.ToString() ?? responseText;

                    if (status == "0" && !string.IsNullOrEmpty(payUrl))
                        return new { Status = 0, OrderID = orderID, PayUrl = payUrl };
                    else
                        return new { Status = -1, Message = message };
                }
                else
                {
                    // 閘道直接回傳 HTML 頁面（部分渠道行為），此時無法取得純URL
                    // 回傳臨時頁面 URL 讓客服使用
                    return new { Status = -1, Message = "閘道未回傳支付連結，請改用舊版測試頁手動確認" };
                }
            }
            catch (Exception ex)
            {
                return new { Status = -1, Message = "呼叫閘道失敗：" + ex.Message };
            }
        }

        private static string PostFormData(string url, System.Collections.Specialized.NameValueCollection data)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var pairs = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>();
                foreach (string key in data)
                    pairs.Add(new System.Collections.Generic.KeyValuePair<string, string>(key, data[key]));

                using (var content = new FormUrlEncodedContent(pairs))
                {
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
        }

        private static string GetClientIP()
        {
            var request = HttpContext.Current.Request;
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = request.ServerVariables["REMOTE_ADDR"];
            return ip ?? "127.0.0.1";
        }

        private static string GetGPaySign(string orderID, decimal orderAmount, DateTime orderDate,
                                           string serviceType, string currencyType,
                                           string companyCode, string companyKey)
        {
            string signStr = "CompanyCode=" + companyCode;
            signStr += "&CurrencyType=" + currencyType;
            signStr += "&ServiceType=" + serviceType;
            signStr += "&OrderID=" + orderID;
            signStr += "&OrderAmount=" + orderAmount.ToString("#.##");
            signStr += "&OrderDate=" + orderDate.ToString("yyyy-MM-dd HH:mm:ss");
            signStr += "&CompanyKey=" + companyKey;

            return GetSHA256(signStr, false).ToUpper();
        }

        private static string GetSHA256(string data, bool base64 = true)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = new System.Security.Cryptography.SHA256CryptoServiceProvider().ComputeHash(bytes);
            if (base64) return Convert.ToBase64String(hash);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                var s = b.ToString("x");
                sb.Append(new string('0', 2 - s.Length) + s);
            }
            return sb.ToString();
        }
    }
}
