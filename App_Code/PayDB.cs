using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// EWinDB 的摘要描述
/// </summary>


    public static class PayDB {
        public enum enumPaymentType {
            CNBankCard,
            CreditCard,
            Paypal,
            WechatCode,
            WechatH5,
            AlipayCode,
            AlipayH5
        }

        public static System.Data.DataTable GetCompanyByCode(string CompanyCode) {
            string SS;
            System.Data.SqlClient.SqlCommand DBCmd = null;
            System.Data.DataTable DT;

            SS = "SELECT * FROM CompanyTable WITH (NOLOCK) WHERE CompanyCode=@CompanyCode AND CompanyState=0";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@CompanyCode", System.Data.SqlDbType.VarChar).Value = CompanyCode;
            DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

            return DT;
        }

        #region 反代IP名單
        public static List<string> GetProxyIPList() {

            string SS;
            System.Data.SqlClient.SqlCommand DBCmd;
            List<string> returnValue = new List<string>();
            DataTable DT;
            //System.Collections.Generic.Dictionary<int, string> SummaryDict = new Dictionary<int, string>();
            DBCmd = new System.Data.SqlClient.SqlCommand();
            SS = " SELECT IP FROM ProxyIP ";

            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;

            DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

            if (DT != null) {
                if (DT.Rows.Count > 0) {
                    foreach (DataRow dr in DT.Rows) {
                        returnValue.Add(dr["IP"].ToString());
                    }
                    //returnValue = DataTableExtensions.ToList<string>(DT).ToList();
                }
            }
            return returnValue;
        }
        #endregion

        #region PaymentTransferLog
        public static void InsertPaymentTransferLog(string Message, int Type, string PaymentSerial, string ProviderCode) {
            string SS;
            System.Data.SqlClient.SqlCommand DBCmd = null;

            SS = "INSERT INTO PaymentTransferLog (forPaymentSerial, Message, Type,forProviderCode)" +
                 "                  VALUES (@PaymentSerial,@Message, @Type,@ProviderCode);";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = System.Data.CommandType.Text;
            DBCmd.Parameters.Add("@Type", System.Data.SqlDbType.Int).Value = Type;
            DBCmd.Parameters.Add("@PaymentSerial", System.Data.SqlDbType.VarChar).Value = PaymentSerial;
            DBCmd.Parameters.Add("@ProviderCode", System.Data.SqlDbType.VarChar).Value = ProviderCode;
            DBCmd.Parameters.Add("@Message", System.Data.SqlDbType.NVarChar).Value = Message;

            DBAccess.ExecuteDB(Pay.DBConnStr, DBCmd);

        }
        #endregion

        public static DBModel.Admin GetAdminByLoginAccount(string LoginAccount) {
            DBModel.Admin returnValue = null;
            String SS = String.Empty;
            System.Data.SqlClient.SqlCommand DBCmd;
            DataTable DT;

            SS = "SELECT *,CompanyType,SortKey,CompanyCode,CompanyName FROM AdminTable WITH (NOLOCK) " +
                " Left Join CompanyTable WITH (NOLOCK) On AdminTable.forCompanyID = CompanyTable.CompanyID" +
                " WHERE LoginAccount=@LoginAccount And AdminState=0";
            DBCmd = new System.Data.SqlClient.SqlCommand();
            DBCmd.CommandText = SS;
            DBCmd.CommandType = CommandType.Text;
            DBCmd.Parameters.Add("@LoginAccount", SqlDbType.VarChar).Value = LoginAccount;

            DT = DBAccess.GetDB(Pay.DBConnStr, DBCmd);

            if (DT != null) {
                if (DT.Rows.Count > 0) {
                    returnValue = DataTableExtensions.ToList<DBModel.Admin>(DT).FirstOrDefault();
                }
            }

            return returnValue;
        }
    }
