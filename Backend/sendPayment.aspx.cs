using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using SkyPay.Backend;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using Newtonsoft.Json;
namespace SkyPay.Backend
{
    public partial class sendPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var amount = decimal.Parse(Request.Params["amount"]);
            var ServiceType = Request.Params["ServiceType"];
            var currencyType = Request.Params["currencyType"];
            var isTestSite = Request.Params["isTestSite"];
            SendPayment(amount, ServiceType, isTestSite, currencyType);
            //SendHeartBeat();
        }

        public void SendPayment(decimal amount, string ServiceType, string isTestSite, string currencyType)
        {
            string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
            bool IsTestSite = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsTestSite"]);
           
            var CompanyCode = "VPayTest";
            var CurrencyType = currencyType;
            var OrderID = Guid.NewGuid().ToString("N");
            var OrderDate = DateTime.Now;
            var OrderAmount = amount;
            var ReturnURL = ApiUrl + "/CallBack/TestCompanyReturn?result=AAA";
            var URL = ApiUrl + "/Gate/RequirePayingUrl";
            var CompanyKey = IsTestSite
                ? "81a5ad6e8048459590f47a13c4a48e09"
                : "1aa1a43a2b5c4e0abf5b674943d1597d";
            var Description = string.Empty;
            var BankCode = string.Empty;
            var BankCardNo = string.Empty;
            var PhoneNo = string.Empty;

            if (ServiceType.ToUpper() == "BITPAY") {
                //銀行帳號；銀行代碼
                Description = "12354489184；KKP";
            }
            //泰國 PromptPay（二维码）強制要求實名制
            if (ServiceType.ToUpper() == "CUP05") {
                URL = ApiUrl + "/Gate/RequirePayingExtended";

                BankCode = "Tisco bank";
                BankCardNo = "521478526";
                PhoneNo = "886952145236";
            }

                var Sign = GetGPaySign(OrderID, OrderAmount, OrderDate, ServiceType, CurrencyType, CompanyCode, CompanyKey);
            var payload = new
            {
                ManageCode = CompanyCode,
                Currency = CurrencyType,
                Service = ServiceType,
                CustomerIP = "121.1.1.1",
                OrderID = OrderID,
                OrderDate = OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderAmount = OrderAmount.ToString("#.##"),
                RevolveURL = ReturnURL,
                UserName = "vince",
                Description= Description,
                BankCode= BankCode,
                BankCardNo = BankCardNo,
                PhoneNo = PhoneNo,
                Sign = Sign
            };

            // 轉成 JSON
            string jsonBody = JsonConvert.SerializeObject(payload);

            // 呼叫 API
            var requestData = RequestJsonAPI(URL, jsonBody);

            if (!string.IsNullOrWhiteSpace(requestData) &&
            (requestData.TrimStart().StartsWith("{") || requestData.TrimStart().StartsWith("[")))
            {
                try
                {
                    var jsonData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(requestData);
                    jsonData = JsonConvert.DeserializeObject<JObject>(requestData);

                    // 🔑 判斷 Status == 0
                    if (jsonData != null && jsonData["Status"] != null && jsonData["Status"].ToString() == "0")
                    {
                        string payUrl = jsonData["Url"]?.ToString();

                        if (!string.IsNullOrEmpty(payUrl))
                        {
                            // 在 ASP.NET WebForms 中注入 JS 開新視窗
                            string script = $"window.location.href='{payUrl}';";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenPaymentWindow", script, true);
                        }
                    }
                    else
                    {
                        // ⚠️ Status 不為 0，打印原始回傳資料
                        string errorMsg = $"API Response: {requestData}";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowError", $"alert('{errorMsg}');", true);

              
                        // 或用 Response.Write
                        // Response.Write(errorMsg);
                    }
                }
                catch (Exception ex)
                {

                    string errorMsg = $"JSON Parse Error: {ex.Message}\\nResponse: {requestData}";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowError", $"alert('{errorMsg}');", true);
                    System.Diagnostics.Debug.WriteLine(errorMsg);
                }
            }
            else
            {
                string errorMsg = $"Non-JSON Response: {requestData}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowError", $"alert('{errorMsg}');", true);
                System.Diagnostics.Debug.WriteLine(errorMsg);
            }
        }

        public void SendHeartBeat()
        {
            string ApiUrl = "http://api-jason.payrich888.com";
            string Url = ApiUrl + "/api/Gate/HeartBeat?EchoString=123";

            // Body 如果 API 不需要，可以給空字串
            string jsonBody = "{}";

            // 呼叫 API
            var requestData = RequestJsonAPI(Url, jsonBody);

            if (!string.IsNullOrWhiteSpace(requestData) &&
                (requestData.TrimStart().StartsWith("{") || requestData.TrimStart().StartsWith("[")))
            {
                try
                {
                    var jsonData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(requestData);

                    if (jsonData != null)
                    {
                        // ✅ 這裡依 HeartBeat API 規格，通常會回傳 EchoString
                        string echo = jsonData["EchoString"]?.ToString();

                        string msg = $"HeartBeat Success. EchoString={echo}";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowEcho", $"alert('{msg}');", true);
                    }
                    else
                    {
                        string errorMsg = $"Empty JSON Response: {requestData}";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowError", $"alert('{errorMsg}');", true);
                    }
                }
                catch (Exception ex)
                {
                    string errorMsg = $"JSON Parse Error: {ex.Message}\\nResponse: {requestData}";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowError", $"alert('{errorMsg}');", true);
                    System.Diagnostics.Debug.WriteLine(errorMsg);
                }
            }
            else
            {
                string errorMsg = $"Non-JSON Response: {requestData}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowError", $"alert('{errorMsg}');", true);
                System.Diagnostics.Debug.WriteLine(errorMsg);
            }
        }

        public static string RequestJsonAPI(string Url, string JsonString)
        {
            bool IsTestSite = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsTestSite"]);
            string ProxyServerUrl = System.Configuration.ConfigurationManager.AppSettings["ProxyServerUrl"];

            string result = string.Empty;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                if (!IsTestSite)
                {
                    //handler.Proxy = new WebProxy(ProxyServerUrl);
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        #region 呼叫遠端 Web API

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Url);
                        HttpResponseMessage response = null;

                        #region  設定相關網址內容

                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        //client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Content-Type 用於宣告遞送給對方的文件型態
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


                        // 將 data 轉為 json
                        string json = JsonString;
                        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = client.SendAsync(request).GetAwaiter().GetResult();

                        #endregion
                        #endregion

                        #region 處理呼叫完成 Web API 之後的回報結果
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode == true)
                            {
                                // 取得呼叫完成 API 後的回報內容
                                result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            }
                            else
                            {
                                //PayDB.InsertPaymentTransferLog("状态码有误:" + response.StatusCode + ", RequestJsonAPI 回传结果:" + response.Content, 2, "", "");
                            }

                        }

                        #endregion

                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                        //PayDB.InsertPaymentTransferLog("ex:" + ex.Message, 2, "", "");
                    }
                }
            }

            return result;
        }

        public static void RedirectAndPOST(System.Web.UI.Page page, string destinationUrl,
                                       System.Collections.Specialized.NameValueCollection data)
        {
            //Prepare the Posting form
            string strForm = PreparePOSTForm(destinationUrl, data);
            //Add a literal control the specified page holding 
            //the Post Form, this is to submit the Posting form with the request.
            page.Controls.Add(new System.Web.UI.LiteralControl(strForm));
        }

        private static String PreparePOSTForm(string url, System.Collections.Specialized.NameValueCollection data)
        {
            //Set a name for the form
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            System.Text.StringBuilder strForm = new System.Text.StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            var a = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            foreach (string key in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + key +
                               "\" value=\"" + data[key] + "\">");
            }

            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            System.Text.StringBuilder strScript = new System.Text.StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }

        public static string GetSHA256(string DataString, bool Base64Encoding = true)
        {
            return GetSHA256(System.Text.Encoding.UTF8.GetBytes(DataString), Base64Encoding);
        }

        public static string GetSHA256(byte[] Data, bool Base64Encoding = true)
        {
            System.Security.Cryptography.SHA256 SHA256Provider = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] hash;
            System.Text.StringBuilder RetValue = new System.Text.StringBuilder();

            hash = SHA256Provider.ComputeHash(Data);
            SHA256Provider = null;

            if (Base64Encoding)
            {
                RetValue.Append(System.Convert.ToBase64String(hash));
            }
            else
            {
                foreach (byte EachByte in hash)
                {
                    // => .ToString("x2")
                    string ByteStr = EachByte.ToString("x");

                    ByteStr = new string('0', 2 - ByteStr.Length) + ByteStr;
                    RetValue.Append(ByteStr);
                }
            }


            return RetValue.ToString();
        }


        public static string GetGPaySign(string OrderID, decimal OrderAmount, DateTime OrderDateTime, string ServiceType, string CurrencyType, string CompanyCode, string CompanyKey)
        {
            string sign;
            string signStr = "ManageCode=" + CompanyCode;
            signStr += "&Currency=" + CurrencyType;
            signStr += "&Service=" + ServiceType;
            signStr += "&OrderID=" + OrderID;
            signStr += "&OrderAmount=" + OrderAmount.ToString("#.##");
            signStr += "&OrderDate=" + OrderDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            signStr += "&CompanyKey=" + CompanyKey;

            sign = GetSHA256(signStr, false).ToUpper();

            return sign;
        }

    }
}