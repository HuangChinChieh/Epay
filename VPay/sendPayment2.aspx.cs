using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SkyPay.Backend;
namespace SkyPay.Backend
{
    public partial class sendPayment2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var amount = decimal.Parse(Request.Params["amount"]);
            var serviceType = Request.Params["serviceType"];
            var isTestSite = Request.Params["isTestSite"];
            SendPayment(amount, serviceType, isTestSite);
        }

        public void SendPayment(decimal amount, string serviceType,string isTestSite)
        {
    
            var CompanyCode = "test01";
            var CurrencyType = "CNY";
            var ServiceType = serviceType;
            var ClientIP = "";
            var OrderID = Guid.NewGuid().ToString("N");
            var OrderDate = DateTime.Now;
            var OrderAmount = amount;
            var ReturnURL = "";
            var URL = "";
            var CompanyKey = "";
            if (isTestSite.ToUpper()=="TRUE")
            {
                ReturnURL = "http://gpay.dev4.mts.idv.tw" + "/api/ProviderResult/GPayTestCompanyReturn?result=AAA";
                URL = "http://gpay.dev4.mts.idv.tw" + "/api/Gateway/RequirePayment";
                //URL = "https://cn.richpay888.com:1443" + "/api/Gateway/RequirePayment";
                CompanyKey = "5489880dfba5443cb935fa3b998c1a0e";
         
            }
            else
            {
                ReturnURL = "https://www.richpay888.com" + "/api/ProviderResult/GPayTestCompanyReturn?result=AAA";
                URL = "https://www.richpay888.com" + "/api/Gateway/RequirePayment";
                CompanyKey = "7a7a4b7e31474e048f8fe47147fabc65";
            }

          

            var Sign = GetGPaySign(OrderID, OrderAmount, OrderDate, ServiceType, CurrencyType, CompanyCode, CompanyKey);

            System.Collections.Specialized.NameValueCollection data = new System.Collections.Specialized.NameValueCollection();
            data.Add("CompanyCode", CompanyCode);
            data.Add("CurrencyType", CurrencyType);
            data.Add("ServiceType", ServiceType);
            data.Add("ClientIP", "121.1.1.1");
            data.Add("OrderID", OrderID);
            data.Add("OrderDate", OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
            data.Add("OrderAmount", OrderAmount.ToString("#.##"));
            data.Add("ReturnURL", ReturnURL);
            data.Add("Sign", Sign);

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
            string signStr = "CompanyCode=" + CompanyCode;
            signStr += "&CurrencyType=" + CurrencyType;
            signStr += "&ServiceType=" + ServiceType;
            signStr += "&OrderID=" + OrderID;
            signStr += "&OrderAmount=" + OrderAmount.ToString("#.##");
            signStr += "&OrderDate=" + OrderDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            signStr += "&CompanyKey=" + CompanyKey;

            sign = GetSHA256(signStr, false).ToUpper();

            return sign;
        }

    }
}