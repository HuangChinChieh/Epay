using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SkyPay.Backend;
namespace SkyPay.Backend
{
    public partial class sendPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var amount = decimal.Parse(Request.Params["amount"]);
            var serviceType = Request.Params["serviceType"];
            var currencyType = Request.Params["currencyType"];
            var isTestSite = Request.Params["isTestSite"];
            SendPayment(amount, serviceType, isTestSite, currencyType);
        }

        public void SendPayment(decimal amount, string serviceType, string isTestSite,string currencyType)
        {

            var CompanyCode = "VPayTest";
            var CurrencyType = currencyType;
            var ServiceType = serviceType;
            var ClientIP = "";
            var OrderID = Guid.NewGuid().ToString("N");
            var OrderDate = DateTime.Now;
            var OrderAmount = amount;
            var ReturnURL = "";
            var URL = "";
            var CompanyKey = "";
            if (isTestSite.ToUpper() == "TRUE")
            {
                ReturnURL = "http://epay.dev4.mts.idv.tw" + "/api/CallBack/TestCompanyReturn?result=AAA";
                URL = "http://epay.dev4.mts.idv.tw" + "/api/Gate/RequirePaying";
                CompanyKey = "81a5ad6e8048459590f47a13c4a48e09";
                ///"http://cn.thespeedpay.com/Result.cshtml?
                //ReturnURL = "http://cn.thespeedpay.com" + "/api/CallBack/TestCompanyReturn?result=AAA";
                //URL = "http://cn.thespeedpay.com" + "/api/Gate/RequirePaying";
                //CompanyKey = "e2c377e5a6f14019990cc21947e737f1";
            }
            else
            {
                ReturnURL = "https://ewin-pay.com" + "/api/CallBack/TestCompanyReturn?result=AAA";
                URL = "https://ewin-pay.com" + "/api/Gate/RequirePaying";
                CompanyKey = "81a5ad6e8048459590f47a13c4a48e09";
            }

            var Sign = GetGPaySign(OrderID, OrderAmount, OrderDate, ServiceType, CurrencyType, CompanyCode, CompanyKey);

            System.Collections.Specialized.NameValueCollection data = new System.Collections.Specialized.NameValueCollection();
            var data2 = new Newtonsoft.Json.Linq.JObject();
            data.Add("ManageCode", CompanyCode);
            data.Add("Currency", CurrencyType);
            data.Add("Service", ServiceType);
            data.Add("CustomerIP", "121.1.1.1");
            data.Add("OrderID", OrderID);
            data.Add("OrderDate", OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
            data.Add("OrderAmount", OrderAmount.ToString("#.##"));
            data.Add("RevolveURL", ReturnURL);
            data.Add("UserName", "ヨウ アスカ");
            data.Add("Sign", Sign);

            data2.Add("ManageCode", CompanyCode);
            data2.Add("Currency", CurrencyType);
            data2.Add("Service", ServiceType);
            data2.Add("CustomerIP", "121.1.1.1");
            data2.Add("OrderID", OrderID);
            data2.Add("OrderDate", OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
            data2.Add("OrderAmount", OrderAmount.ToString("#.##"));
            data2.Add("RevolveURL", ReturnURL);
            data2.Add("UserName", "ヨウ アスカ");
            data2.Add("Sign", Sign);
            var jsondata= Newtonsoft.Json.JsonConvert.SerializeObject(data2);
            RedirectAndPOST(this.Page, URL, data);
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