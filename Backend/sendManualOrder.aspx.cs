using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

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
                WriteJson(new { Status = -1, Message = "缺少必要參數" });
                return;
            }

            decimal amount;
            int companyID;
            if (!decimal.TryParse(amountStr, out amount) || !int.TryParse(companyIDStr, out companyID))
            {
                WriteJson(new { Status = -1, Message = "參數格式錯誤" });
                return;
            }

            var result = CreateOrder(amount, serviceType, currencyType, companyID);
            WriteJson(result);
        }

        private void WriteJson(object obj)
        {
            Response.Write(JsonConvert.SerializeObject(obj));
            Response.End();
        }

        private object CreateOrder(decimal amount, string serviceType, string currencyType, int companyID)
        {
            var company = GetCompanyByID(companyID);
            if (company == null)
                return new { Status = -1, Message = "找不到商戶資料" };

            string apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            string returnUrl = apiUrl + "/ResultSuccess.cshtml";
            string url = apiUrl + "/Gate/RequirePayingUrl";

            var orderID = Guid.NewGuid().ToString("N");
            var orderDate = DateTime.Now;
            var sign = GetGPaySign(orderID, amount, orderDate, serviceType, currencyType, company.CompanyCode, company.CompanyKey);

            var payload = new
            {
                ManageCode = company.CompanyCode,
                Currency = currencyType,
                Service = serviceType,
                CustomerIP = GetClientIP(),
                OrderID = orderID,
                OrderDate = orderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderAmount = amount.ToString("#.##"),
                RevolveURL = returnUrl,
                UserName = "cs",
                Description = string.Empty,
                Sign = sign
            };

            string jsonBody = JsonConvert.SerializeObject(payload);

            try
            {
                var responseText = RequestJsonAPI(url, jsonBody);

                if (!string.IsNullOrWhiteSpace(responseText) &&
                    (responseText.TrimStart().StartsWith("{") || responseText.TrimStart().StartsWith("[")))
                {
                    var json = JObject.Parse(responseText);
                    string status = json["Status"]?.ToString() ?? "-1";
                    string payUrl = json["Url"]?.ToString() ?? string.Empty;
                    string message = json["Message"]?.ToString() ?? responseText;

                    if (status == "0" && !string.IsNullOrEmpty(payUrl))
                        return new { Status = 0, OrderID = orderID, PayUrl = payUrl };
                    else
                        return new { Status = -1, Message = message };
                }
                else
                {
                    return new { Status = -1, Message = "閘道未回傳支付連結：" + responseText };
                }
            }
            catch (Exception ex)
            {
                return new { Status = -1, Message = "呼叫閘道失敗：" + ex.Message };
            }
        }

        public static string RequestJsonAPI(string Url, string JsonString)
        {
            bool IsTestSite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsTestSite"]);
            string result = string.Empty;

            using (HttpClientHandler handler = new HttpClientHandler())
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(JsonString, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();

                    if (response != null && response.IsSuccessStatusCode)
                        result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return result;
        }

        private class CompanyInfo
        {
            public string CompanyCode { get; set; }
            public string CompanyKey { get; set; }
        }

        private static CompanyInfo GetCompanyByID(int companyID)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DBConnStr"].ConnectionString;
            string sql = "SELECT CompanyCode, CompanyKey FROM CompanyTable WITH (NOLOCK) WHERE CompanyID = @CompanyID";

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = companyID;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return new CompanyInfo
                        {
                            CompanyCode = reader["CompanyCode"].ToString(),
                            CompanyKey = reader["CompanyKey"].ToString()
                        };
                }
            }
            return null;
        }

        private static string GetClientIP()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return ip ?? "127.0.0.1";
        }

        private static string GetGPaySign(string orderID, decimal orderAmount, DateTime orderDate,
                                           string serviceType, string currencyType,
                                           string companyCode, string companyKey)
        {
            string signStr = "ManageCode=" + companyCode
                + "&Currency=" + currencyType
                + "&Service=" + serviceType
                + "&OrderID=" + orderID
                + "&OrderAmount=" + orderAmount.ToString("#.##")
                + "&OrderDate=" + orderDate.ToString("yyyy-MM-dd HH:mm:ss")
                + "&CompanyKey=" + companyKey;

            using (var sha256 = new SHA256CryptoServiceProvider())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(signStr));
                var sb = new StringBuilder();
                foreach (var b in hash)
                {
                    var s = b.ToString("x");
                    sb.Append(s.Length == 1 ? "0" + s : s);
                }
                return sb.ToString().ToUpper();
            }
        }
    }
}
