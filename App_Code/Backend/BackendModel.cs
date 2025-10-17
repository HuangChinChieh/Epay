using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


#region DBModel

public class DBModel {
    public class ProviderBankCard
    {
        public string BankCardGUID { get; set; }
        public string forProviderCode { get; set; }
        public int BankCardState { get; set; }
        public string CurrencyType { get; set; }
        //0=銀行卡/1=支付寶
        public int Type { get; set; }
        //0=輪詢卡/1=非輪詢卡
        public int CardType { get; set; }

        public string BankCardName { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BankNumber { get; set; }
        public string AccountName { get; set; }
        public string BankProvince { get; set; }
        public string BankCity { get; set; }
        public string Description { get; set; }
        public decimal AmountMax { get; set; }
        public decimal AmountMin { get; set; }
        public decimal TotalCreditValueDay { get; set; }
        public decimal TotalCreditValueWeek { get; set; }
        public decimal TotalCreditValueMonth { get; set; }
        public decimal CreditValueDay { get; set; }
        public decimal CreditValueWeek { get; set; }
        public decimal CreditValueMonth { get; set; }
        public DateTime LastDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ServiceStartDate { get; set; }
        public DateTime ServiceEndDate { get; set; }
        public string CompanyName { get; set; }

        public string CompanyCode { get; set; }
        public string[] AlipayArray { get; set; }
    }

    public class ConfigSetting
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }

    public class BlockChainRate
    {
        public string CurrencyType { get; set; }
        public decimal Rate { get; set; }
        public decimal UpFloatRate { get; set; }
        public decimal DownFloatRate { get; set; }
    }

    public class ASPStateTempSessions {
        public string SessionId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime LockDate { get; set; }
        public DateTime LockDateLocal { get; set; }
        public int LockCookie { get; set; }
        public int Timeout { get; set; }
        public bool Locked { get; set; }
        public byte[] SessionItemShort { get; set; }
        public int Flags { get; set; }
    }

    public class PaymentTransferLog {
        public string forPaymentSerial { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public string forProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string CreateDate2 { get; set; }

    }

    public class API_WithdrawResult {
        // 0=即時/1=非即時
        public int SendType;
        public string WithdrawSerial;
        public string UpOrderID;
        public int SendStatus;
        public decimal DidAmount;
        public decimal Balance;
        //OK = 0,ERR = 1,SignErr = 2,Invalidate = 3
        public int Status;
        public string Message;
    }

    public class ProxyProviderGroup {
        public string forProviderCode { get; set; }
        public decimal ProxyProviderPoint { get; set; }
        public decimal CanUsePoint { get; set; }
        public int GroupID { get; set; }
        public int Weight { get; set; }
        public string GroupName { get; set; }
        public DateTime CreateDate { get; set; }
        public int State { get; set; }
        public string CreateDate2 { get; set; }
        public string GroupAccounts { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int WithdrawalCount { get; set; }
        public decimal CanUsePoint2 { get; set; }
        public decimal PaymentRate { get; set; }
        public decimal WithdrawalCharge { get; set; }
        public decimal ProfitAmount { get; set; }

    }

    public class ProxyProviderGroupWithdrawingCount : ProxyProviderGroup {
        public int WithdrawingCount { get; set; }
    }

    public class DownOrderTransferLog {
        public string OrderID { get; set; }
        public string DownOrderID { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class DownOrderTransferLogV2 : DownOrderTransferLog {
        public int TotalCount { get; set; }
    }



    public class PaymentTable {
        public int PaymentID { get; set; }
        public int forCompanyID { get; set; }
        public string PaymentSerial { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public string BankCode { get; set; }
        public string ProviderCode { get; set; }
        public int ProcessStatus { get; set; }
        public string ReturnURL { get; set; }
        public string PaymentResult { get; set; }
        public string State { get; set; }
        public string BankSequenceID { get; set; }
        public string ClientIP { get; set; }
        public string OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDate2 { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime FinishDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDate2 { get; set; }
        public decimal CostRate { get; set; }
        public decimal CostCharge { get; set; }
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public string ProviderOrderID { get; set; }
        public int Accounting { get; set; }
        public decimal PartialOrderAmount { get; set; }
        public string PatchDescription { get; set; }
        public string forPaymentSerial { get; set; }
        public int ConfirmAdminID { get; set; }
        public string ServiceTypeName { get; set; }
        public string BankName { get; set; }
        public string ProviderName { get; set; }
        public string CompanyName { get; set; }
        public string UserIP { get; set; }
        public int SubmitType { get; set; }
    }

    public class CreatePatchPayment {
        public int PatchPaymentState { get; set; }
        public string PaymentSerial { get; set; }
    }

    public class RequireWithdrawalSet {
        public string WithdrawSerial { get; set; }
        public string Sign { get; set; }
    }

    public class CompanyWithGooleKey {
        public int CompanyID { get; set; }
        public int CompanyType { get; set; }
        public int CompanyState { get; set; }
        public int ParentCompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string URL { get; set; }
        public int InsideLevel { get; set; }
        public string SortKey { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateAdminID { get; set; }
        //public string CompanyKey { get; set; }
        public string ContacterName { get; set; }
        public string ContacterMobile { get; set; }
        public int ContacterMethod { get; set; }
        public string ContacterMethodAccount { get; set; }
        public string ContacterEmail { get; set; }
        public int ChildCompanyCount { get; set; }
        public string MerchantCode { get; set; }
        public int WithdrawType { get; set; }
        public string AutoWithdrawalServiceType { get; set; }
        public string CheckCompanyWithdrawUrl { get; set; }
        public int BackendLoginIPType { get; set; }
        public int WithdrawAPIType { get; set; }
        public int ProviderGroupID { get; set; }
        public string ProviderGroups { get; set; }
        public int BackendWithdrawType { get; set; }
        public string GoogleKey { get; set; }
    }

    public class Company {
        public int CompanyID { get; set; }
        public int CompanyType { get; set; }
        public int CompanyState { get; set; }
        public int ParentCompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string URL { get; set; }
        public int InsideLevel { get; set; }
        public string SortKey { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateAdminID { get; set; }
        //public string CompanyKey { get; set; }
        public string ContacterName { get; set; }
        public string ContacterMobile { get; set; }
        public int ContacterMethod { get; set; }
        public string ContacterMethodAccount { get; set; }
        public string ContacterEmail { get; set; }
        public int ChildCompanyCount { get; set; }
        public string MerchantCode { get; set; }
        public int WithdrawType { get; set; }
        public string AutoWithdrawalServiceType { get; set; }
        public string CheckCompanyWithdrawUrl { get; set; }
        public int BackendLoginIPType { get; set; }
        public int WithdrawAPIType { get; set; }
        public int ProviderGroupID { get; set; }
        public int BackendWithdrawType { get; set; }
        public string ProviderGroups { get; set; }
        public int CheckCompanyWithdrawType { get; set; }
        public string Description { get; set; }
        public string CurrencyType { get; set; }
        public int BackendWithdrawIPType { get; set; }
        public decimal Timezone { get; set; }
    }

    public class CompanyWithKey {
        public int CompanyID { get; set; }
        public int CompanyType { get; set; }
        public int CompanyState { get; set; }
        public int ParentCompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string URL { get; set; }
        public int InsideLevel { get; set; }
        public string SortKey { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateAdminID { get; set; }
        //public string CompanyKey { get; set; }
        public string ContacterName { get; set; }
        public string ContacterMobile { get; set; }
        public int ContacterMethod { get; set; }
        public string ContacterMethodAccount { get; set; }
        public string ContacterEmail { get; set; }
        public int ChildCompanyCount { get; set; }
        public string CompanyKey { get; set; }
        public string MerchantCode { get; set; }
        public int WithdrawType { get; set; }
        public string AutoWithdrawalServiceType { get; set; }
        public string CheckCompanyWithdrawUrl { get; set; }

    }

    public class FastCreateCompany {
        public string CompanyCode { get; set; }
    }

    public class Admin {
        public int AdminID { get; set; }
        public int AdminState { get; set; }
        public int forCompanyID { get; set; }
        public int forAdminRoleID { get; set; }
        public int AdminType { get; set; }
        public string LoginAccount { get; set; }
        //public string LoginPassword { get; set; }
        public string RealName { get; set; }
        public string Description { get; set; }
        public int LastBackendNotifyID { get; set; }
        public int TryCount { get; set; }
        public DateTime LockDate { get; set; }
        public string UserIP { get; set; }
        public DateTime CreateDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int CompanyType { get; set; }
        public string SortKey { get; set; }
        //public string GoogleKey { get; set; }
        public int GroupID { get; set; }
    }

    public class AdminWithLoginPassword {
        public int AdminID { get; set; }
        public int AdminState { get; set; }
        public int forCompanyID { get; set; }
        public int forAdminRoleID { get; set; }
        public int AdminType { get; set; }
        public string LoginAccount { get; set; }
        public string LoginPassword { get; set; }
        public string RealName { get; set; }
        public string Description { get; set; }
        public int LastBackendNotifyID { get; set; }
        public int TryCount { get; set; }
        public DateTime LockDate { get; set; }
        public string UserIP { get; set; }
        public DateTime CreateDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int CompanyType { get; set; }
        public string SortKey { get; set; }
        //public string GoogleKey { get; set; }
        public int GroupID { get; set; }
    }


    public class AdminWithGoogleKey {
        public int AdminID { get; set; }
        public int AdminState { get; set; }
        public int forCompanyID { get; set; }
        public int forAdminRoleID { get; set; }
        public int AdminType { get; set; }
        public string LoginAccount { get; set; }
        //public string LoginPassword { get; set; }
        public string RealName { get; set; }
        public string Description { get; set; }
        public int LastBackendNotifyID { get; set; }
        public int TryCount { get; set; }
        public DateTime LockDate { get; set; }
        public string UserIP { get; set; }
        public DateTime CreateDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int CompanyType { get; set; }
        public string SortKey { get; set; }
        public string GoogleKey { get; set; }
        public int GroupID { get; set; }
    }

    public class AdminRole {
        public int forCompanyID { get; set; }
        public int AdminRoleID { get; set; }
        public string RoleName { get; set; }
        public string CompanyName { get; set; }
    }

    public class ImageTable {
        public int ImageID { get; set; }
        public int Type { get; set; }
        public string ImageName { get; set; }
        public string CompanyCode { get; set; }
        public string TransactionID { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class WithdrawalIP {
        public int forCompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string withdrawalIP { get; set; }
        public string ImageName { get; set; }
        public DateTime CreateDate { get; set; }
        public int ImageCount { get; set; }
        public string RealName { get; set; }
    }

    public class UpdateWithdrawalResultsByAdminCheck {
        public UpdateWithdrawalResultsByAdminCheck() {
            ErrorWithdrawal = new List<string>();
        }
        public List<Withdrawal> WithdrawalModel { get; set; }
        public string Message { get; set; }
        public int State { get; set; }
        public List<string> ErrorWithdrawal { get; set; }
    }

    public class ProviderBalanceSet {
        public List<string> ArrayProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string Sign { get; set; }
    }


    public class ReSendPaymentSet {
        public string PaymentSerial { get; set; }
        public string Sign { get; set; }
    }

    public class ReSendWithdrawSet {
        public string WithdrawSerial { get; set; }
        public string Sign { get; set; }
        public bool isReSendWithdraw { get; set; }
    }



    public class ProviderBalance {
        public string ProviderName { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal CashBalance { get; set; }
        public bool IsProviderSupport { get; set; }
    }

    public class AdminRolePermission {

        public int forAdminRoleID { get; set; }
        public int forCompanyID { get; set; }
        public string forPermissionName { get; set; }
    }

    public class SummaryProviderByDate {
        public string SummaryDate2 { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public string ProviderName { get; set; }
        public decimal SummaryAmount { get; set; }
        public decimal SummaryNetAmount { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal SummaryWithdrawalAmount { get; set; }
        public decimal SummaryWithdrawalChargeAmount { get; set; }
    }

    public class ProxySummaryProviderByDate {
        public string SummaryDate2 { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string GroupName { get; set; }
        public decimal SummaryAmount { get; set; }
        public decimal SummaryNetAmount { get; set; }
        public decimal SummaryWithdrawalAmount { get; set; }
        public decimal SummaryWithdrawalChargeAmount { get; set; }
        public int WithdrawalCount { get; set; }
        public int PaymentCount { get; set; }

    }

    public class returnPaymentReportV2 {
        public List<PaymentReportV2> data { get; set; }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public bool IsAutoLoad { get; set; }
        public DBModel.StatisticsPaymentAmount StatisticsPaymentAmount { get; set; }
        public int ResultCode;

    }

    public class PaymentReportV2 : PaymentReport {
        public int TotalCount { get; set; }
    }

    public class PaymentReport {
        public string CreateDate2 { get; set; }
        public string OrderDate2 { get; set; }
        public int PaymentID { get; set; }
        public int forCompanyID { get; set; }
        public string PaymentSerial { get; set; }
        public string forPaymentSerial { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public string BankCode { get; set; }
        public string ProviderCode { get; set; }
        public int ProcessStatus { get; set; }
        public string ReturnURL { get; set; }
        public object PaymentResult { get; set; }
        public string ClientIP { get; set; }
        public string UserIP { get; set; }
        public string OrderID { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal CostRate { get; set; }
        public decimal CostCharge { get; set; }
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public decimal PartialOrderAmount { get; set; }
        public decimal PaymentRate { get; set; }
        public int Accounting { get; set; }
        public string ServiceTypeName { get; set; }
        public string ProviderName { get; set; }
        public string BankName { get; set; }
        public int BankType { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string ProviderOrderID { get; set; }
        public string FinishDate2 { get; set; }
        public int SubmitType { get; set; }
        public string PatchDescription { get; set; }
        public string Description { get; set; }
        public string MerchantCode { get; set; }
        public string RealName { get; set; }
        public string GroupName { get; set; }
        public string UserName { get; set; }
        public int DecimalPlaces { get; set; }
        public string TransferRemark { get; set; }
        public string BranchName { get; set; }
        public string BankNumber { get; set; }
        public string AccountName { get; set; }
        public string BankCardName { get; set; }
        
    }


    public class PointBySearchFilter {
        public int ProcessStatus { get; set; }
        public decimal SumOrderAmount { get; set; }
        public decimal SumSuccessOrderAmount { get; set; }
        public int OrderCount { get; set; }
        public decimal SumCharge { get; set; }
        public decimal SumChargeAndSuccessOrderAmount { get; set; }
        public decimal SumProviderCharge { get; set; }

    }

    public class StatisticsPaymentAmount {
        public int ProcessStatus { get; set; }
        public decimal SumSuccessAmount { get; set; }
        public decimal SumOrderAmount { get; set; }
        public int SuccessOrderCount { get; set; }
        public int OrderCount { get; set; }
        public decimal SumCharge { get; set; }
        public decimal SumChargeAndSuccessOrderAmount { get; set; }
        public decimal ProfitAmount { get; set; }

    }

    public class AgentReceive {
        public int AgentReceiveID { get; set; }
        public string Currency { get; set; }
        public string ServiceType { get; set; }
        public int forCompanyID { get; set; }
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public int forChildCompanyID { get; set; }
        public decimal ChildCollectRate { get; set; }
        public decimal ChildCollectCharge { get; set; }
        public decimal ReceiveAmount { get; set; }
        public int forPaymentID { get; set; }
        public int forAgentCloseID { get; set; }
        public int ReceiveStatus { get; set; }
        public DateTime PaymentFinishDate { get; set; }
        public string PaymentFinishDate2 { get; set; }
        public string ChildCompanyName { get; set; }
        public string ServiceTypeName { get; set; }
        public string PaymentSerial { get; set; }
        public decimal OrderAmount { get; set; }

    }

    public class AgentClose {
        public int AgentCloseID { get; set; }
        public string Currency { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDate2 { get; set; }
        public DateTime EndDate { get; set; }
        public string EndDate2 { get; set; }
        public int forCompanyID { get; set; }
        public decimal TotalReceiveAmount { get; set; }
        public int TotolReceiveCount { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDate2 { get; set; }
    }

    public class Currency {
        public string CurrencyType { get; set; }
    }

    public class Permission {
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public string LinkURL { get; set; }
        public int AdminPermission { get; set; }
        public int PermissionCategoryID { get; set; }
        public int SortIndex { get; set; }
        public string PermissionCategoryDescription { get; set; }

    }

    public class ServiceTypeModel {
        public string ServiceTypeName { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public int AllowCollect { get; set; }
        public int AllowPay { get; set; }
        public int ServiceSupplyType { get; set; }
        public int ServicePaymentType { get; set; }

    }

    public class Provider {
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string Introducer { get; set; }
        public string ProviderUrl { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantKey { get; set; }
        public string NotifyAsyncUrl { get; set; }
        public string NotifySyncUrl { get; set; }
        public int ProviderAPIType { get; set; }
        public int ProviderState { get; set; }
        public int CollectType { get; set; }
        public int forCompanyID { get; set; }
        public decimal WithdrawRate { get; set; }
        public int isPrePaid { get; set; }
        public int BankCardTimeoutSecond { get; set; }
        public int WalletTimeoutSecond { get; set; }
    }

    public class WithdrawLimit {
        public string CurrencyType { get; set; }
        //0=Provider/1=Company/2=代付
        public string ServiceType { get; set; }
        public string ServiceTypeName { get; set; }
        public int WithdrawLimitType { get; set; }
        public string ProviderCode { get; set; }
        public int CompanyID { get; set; }
        public decimal MaxLimit { get; set; }
        public decimal MinLimit { get; set; }
        public decimal Charge { get; set; }
        public decimal BlockChainRate { get; set; }
        public decimal Rate { get; set; }
    }

    public class ProviderService {
        public string ProviderCode { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public decimal CostRate { get; set; }
        public decimal CostCharge { get; set; }
        public decimal MinOnceAmount { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public decimal MaxDaliyAmount { get; set; }
        public int CheckoutType { get; set; }
        public int DeviceType { get; set; }
        public int State { get; set; }
        public string Description { get; set; }
    }



    public class ProxyProvider {
        public string forProviderCode { get; set; }
        public string ProviderCode { get; set; }
        public decimal Charge { get; set; }
        public decimal Rate { get; set; }
        public decimal ProxyProviderPoint { get; set; }
        public decimal CanUsePoint { get; set; }
        public string ProviderName { get; set; }
        public decimal MaxWithdrawalAmount { get; set; }

    }

    public class ProxyProviderOrder {
        public int OrderID { get; set; }
        public string forOrderSerial { get; set; }
        public int Type { get; set; }
        public decimal PaymentRate { get; set; }
        public decimal WithdrawalCharge { get; set; }
    }

    public class PermissionCategory {
        public int PermissionCategoryID { get; set; }
        public string PermissionCategoryName { get; set; }
        public int PageType { get; set; }
        public int SortIndex { get; set; }
        public string Description { get; set; }
    }

    public class GPayRelation {
        public int forCompanyID { get; set; }
        public string ProviderCode { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public string ProviderName { get; set; }
        public int Weight { get; set; }
    }

    public class CompanyService {
        public int CheckoutType { get; set; }
        public int forCompanyID { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public decimal MaxDaliyAmount { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public decimal MinOnceAmount { get; set; }
        public int DeviceType { get; set; }
        public int State { get; set; }
        public decimal MaxDaliyAmountByUse { get; set; }
    }

    public class CompanyPoint {

        public int forCompanyID { get; set; }
        public string CurrencyType { get; set; }
        public decimal PointValue { get; set; }
        public decimal CanUsePoint { get; set; }
        public decimal FrozenPoint { get; set; }
        public decimal InWithdrawProcessPoint { get; set; }
    }

    public class CompanyServicePoint {
        public int CompanyID { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public decimal SystemPointValue { get; set; }
    }

    public class ProviderPoint {
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string exMessage { get; set; }
        public decimal SystemPointValue { get; set; }
        public decimal ProviderPointValue { get; set; }
        public decimal TotalDepositePointValue { get; set; }
        public decimal TotalProfitPointValue { get; set; }
    }

    public class BankCodeTable {
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string ETHContractNumber { get; set; }
        public int BankState { get; set; }
        public int BankType { get; set; }
    }

    public class BackendNotifyTable {
        public int BackendNotifyID { get; set; }
        public int forCompanyID { get; set; }
        public string CompanyName { get; set; }
        public int Type { get; set; }
        public string Data { get; set; }
        public string CreateDate { get; set; }
        public int OpenByAdminID { get; set; }
    }

    public class WithdrawalTotalAmountsByDate {
        public int TotalCount { get; set; }
        public decimal Amount { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
        public string CurrencyType { get; set; }
    }


    public class GetProviderWithdrawalByGroupAmount {
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }

    }

    public class returnWithdrawalV2 {
        public List<WithdrawalV2> data { get; set; }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public bool IsAutoLoad { get; set; }
        public DBModel.StatisticsPaymentAmount StatisticsPaymentAmount { get; set; }
        public int ResultCode;
        public decimal TotalAmount { get; set; }
        public decimal TotalHandlingFee { get; set; }
    }

    public class WithdrawalV2 : Withdrawal {
        public int TotalCount { get; set; }
    }

    public class WithdrawalV2TotalAmount
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalHandlingFee { get; set; }
    }

    public enum WithdrawResultStatus {
        Successs = 0,
        Failure = 1,
        WithdrawProgress = 2,
        ProblemWithdraw = 3
        //CompanyCodeNotFound = 3,
        //SignFail = 4,
        //PaymentNotFound = 5,
        //SystemFailure = 99
    }

    public class GPayReturnByWithdraw {
        public GPayRetunDataByWithdraw GPayRetunData { get; set; }
        public string RetunUrl { get; set; }

        public void SetByWithdraw(Withdrawal withdrawal, WithdrawResultStatus withdrawalStatus) {
            BackendDB backendDB = new BackendDB();
            CompanyWithKey companyModel = backendDB.GetCompanyWithKeyByCompanyID(withdrawal.forCompanyID);

            this.GPayRetunData.ServiceType = withdrawal.ServiceType;
            this.RetunUrl = withdrawal.DownUrl;
            this.GPayRetunData.WithdrawSerial = withdrawal.WithdrawSerial;
            this.GPayRetunData.WithdrawStatus = (int)withdrawalStatus;
            this.GPayRetunData.OrderAmount = withdrawal.Amount;
            this.GPayRetunData.WithdrawAmount = withdrawal.FinishAmount;
            this.GPayRetunData.WithdrawCharge = withdrawal.CollectCharge;
            this.GPayRetunData.CurrencyType = withdrawal.CurrencyType;
            this.GPayRetunData.CompanyCode = companyModel.CompanyCode;
            this.GPayRetunData.OrderID = withdrawal.DownOrderID;
            this.GPayRetunData.OrderDate = withdrawal.DownOrderDate.ToString("yyyy-MM-dd HH:mm:ss");
            this.GPayRetunData.Sign = GetGPayWithdrawSign(GPayRetunData.OrderID, GPayRetunData.OrderAmount, withdrawal.DownOrderDate, GPayRetunData.CurrencyType, GPayRetunData.CompanyCode, companyModel.CompanyKey);

        }

        private string GetGPayWithdrawSign(string OrderID, decimal OrderAmount, DateTime OrderDateTime, string CurrencyType, string CompanyCode, string CompanyKey) {
            string sign;
            string signStr = "CompanyCode=" + CompanyCode;
            signStr += "&CurrencyType=" + CurrencyType;
            signStr += "&OrderID=" + OrderID;
            signStr += "&OrderAmount=" + OrderAmount.ToString("#.##");
            signStr += "&OrderDate=" + OrderDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            signStr += "&CompanyKey=" + CompanyKey;

            sign = CodingControl.GetSHA256(signStr, false);
            return sign.ToUpper();
        }
    }

    public class GPayRetunDataByWithdraw {
        public string WithdrawSerial { get; set; }
        public int WithdrawStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal WithdrawAmount { get; set; }
        public decimal WithdrawCharge { get; set; }
        public string CurrencyType { get; set; }
        public string CompanyCode { get; set; }
        public string ServiceType { get; set; }
        public string OrderID { get; set; }
        public string OrderDate { get; set; }
        public string Sign { get; set; }
    }

    public class Withdrawal {
        public int WithdrawID { get; set; }
        public int WithdrawType { get; set; }
        public string WithdrawSerial { get; set; }
        public string CreateDate2 { get; set; }
        public string SummaryDate { get; set; }
        public int forCompanyID { get; set; }
        public string CurrencyType { get; set; }
        public decimal Amount { get; set; }
        public decimal FinishAmount { get; set; }
        public string FinishDate2 { get; set; }
        public int Status { get; set; } //流程狀態，0=建立/1=進行中/2=成功/3=失敗
        public string BankCard { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string BankCardName { get; set; }
        public string BankName { get; set; }
        public int BankType { get; set; }
        public string OwnProvince { get; set; }
        public string OwnCity { get; set; }
        public string BankBranchName { get; set; }
        public string RealName1 { get; set; }
        public string RealName2 { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string BankDescription { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal CollectCharge { get; set; }//手續費(商户)
        public decimal CostCharge { get; set; }//手續費(供應商)
        public decimal WithdrawalCharge { get; set; }//手續費(供應商)
        public string Description { get; set; }
        public string RejectDescription { get; set; }
        public string ManagerRejectDescription { get; set; }
        public string CashierRejectDescription { get; set; }
        public int FloatType { get; set; } // 0=後台申請提現單=>後台審核 /1=API申請代付=>後台審核 /2=API申請代付=>不經後台審核
        public int DownStatus { get; set; }
        public string DownUrl { get; set; }
        public string DownOrderID { get; set; }
        public DateTime DownOrderDate { get; set; }
        public string DownMobile { get; set; }
        public string DownClientIP { get; set; }
        public int UpStatus { get; set; }
        public string UpResult { get; set; }
        public string UpOrderID { get; set; }
        public decimal UpDidAmount { get; set; }
        public int UpAccounting { get; set; }
        public string forProviderCode { get; set; }
        public int HandleByAdminID { get; set; }
        public string GroupName { get; set; }
        public int GroupID { get; set; }
        public string CompanyDescription { get; set; }
        public decimal WithdrawRate { get; set; }
        public int DecimalPlaces { get; set; }
        public int IsBlockChainOrder { get; set; }
        public decimal UpBlockChainRate { get; set; }
        public decimal DownBlockChainRate { get; set; }
        public decimal OKEXRate { get; set; }
        public decimal UpRate { get; set; }
        public decimal DownRate { get; set; }
    }

    public class RiskControlWithdrawalTable {
        public int RiskControlWithdrawalID { get; set; }
        public string forCompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
        public string BankName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDate2 { get; set; }
    }

    public class WithdrawalTable {
        public int forCompanyID { get; set; }
        public int WithdrawID { get; set; }
        public string CurrencyType { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime FinishDate { get; set; }
        public int Status { get; set; }
        public string BankCard { get; set; }
        public string WithdrawSerial { get; set; }
        public string ProviderCode { get; set; }
        public string BankSequenceID { get; set; }
        public string BankDescription { get; set; }
        public int WithdrawType { get; set; }

    }

        public class SummaryCompanyByDate {
        public string SummaryDate2 { get; set; }
        public int forCompanyID { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public decimal SummaryAmount { get; set; }
        public decimal SummaryNetAmount { get; set; }
        public decimal SummaryProviderNetAmount { get; set; }
        public decimal SummaryAgentAmount { get; set; }
        public decimal SummaryPayAgentAmount { get; set; }
        public decimal SummaryCompanyWithdrawalChargeAmount { get; set; }
        public decimal SummaryProviderWithdrawalChargeAmount { get; set; }
        public string ServiceTypeName { get; set; }
        public string ProviderName { get; set; }
        public int CompanyType { get; set; }

    }

    public class SummaryBlockChainByDate
    {
        public string SummaryDate2 { get; set; }
        public int forCompanyID { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public decimal BlockChainRate { get; set; }
        public decimal UpBlockChainRate { get; set; }
        public decimal DownBlockChainRate { get; set; }
        public decimal SummaryCompanyAmount { get; set; }
        public decimal SummaryProviderAmount { get; set; }
        public decimal UpSummaryBlockChainAmount { get; set; }
        public decimal DownSummaryBlockChainAmount { get; set; }
        public string ServiceTypeName { get; set; }
        public string ProviderName { get; set; }
    }

    public class SummaryCompanyByDateFlot
    {
        public string SummaryDate { get; set; }
        public decimal TotalWithdrawalAmount { get; set; }
        public decimal TotalNetAmount { get; set; }
    }


    public class SummaryCompanyByHour
    {
        public string SummaryTime2 { get; set; }
        public string SummaryDate2 { get; set; }
        public DateTime SummaryTime { get; set; }
        public DateTime SummaryDate { get; set; }
        public int forCompanyID { get; set; }
        public string CurrencyType { get; set; }
        public decimal SummaryAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal ChargeProfit { get; set; }
        public decimal CanUseValue { get; set; }
        public string CompanyName { get; set; }
        
    }


    public class CompanyBlockChainAddress
    {
        public string BlockChainAddress { get; set; }
        public int BlockChainAddressID { get; set; }
        public int forCompanyID { get; set; }
    }

    public class CompanyServicePointHistory {
        public string CreateDate2 { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceType { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public int OperatorType { get; set; }
        public string TransactionOrder { get; set; }
    }

    public class CompanyServicePointLog {
        public DateTime CreateDate { get; set; }
        public string CompanyName { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceType { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public decimal ValueByService { get; set; }
        public decimal BeforeValueByService { get; set; }
        public int OperatorType { get; set; }
        public int TransactionID { get; set; }
        public string TransactionOrder { get; set; }
        public string DownOrderID { get; set; }
    }


    public class CompanyServiceAndProviderPointLog {
        public DateTime CreateDate2 { get; set; }
        public string CompanyName2 { get; set; }
        public int CompanyID2 { get; set; }
        public string ServiceTypeName2 { get; set; }
        public string ProviderName2 { get; set; }
        public string ProviderCode2 { get; set; }
        public string ServiceType2 { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public decimal ValueByService { get; set; }
        public decimal BeforeValueByService { get; set; }
        public int OperatorType2 { get; set; }
        public int TransactionID { get; set; }
        public string TransactionOrder { get; set; }
        public decimal UpValue { get; set; }
        public decimal DownValue { get; set; }
        public decimal ProviderValue { get; set; }
        public decimal ProviderBeforeValue { get; set; }
    }

    public class ProviderPointHistory {
        public string CreateDate2 { get; set; }
        public string ProviderName { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public int OperatorType { get; set; }
        public string TransactionOrder { get; set; }
    }


    public class SummaryCompanyByDateChartData {
        public decimal SummaryNetAmounts { get; set; }
        public int SummaryNetAmountPercent { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceType { get; set; }
    }

    public class CompanyPointHistory {
        public string CompanyPointHistoryID { get; set; }
        public int forCompanyID { get; set; }
        public string CurrencyType { get; set; }
        public int OperatorType { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public decimal ValueByService { get; set; }
        public decimal BeforeValueByService { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDate2 { get; set; }
        public int TransactionID { get; set; }
        public string ServiceTypeName { get; set; }
    }

    public class GoogleQrCode {
        public string ImageUrl { get; set; }
        public string ManualEntryKey { get; set; }
        public string GoogleKey { get; set; }
        public bool IsCreated { get; set; }
    }

    public class AdminOPLog {
        public string OperatingID { get; set; }
        public int forCompanyID { get; set; }
        public int forAdminID { get; set; }
        public int Type { get; set; }
        public string IP { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class ProviderManualHistory {
        public int ProviderManualID { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public int forAdminID { get; set; }
        public int Type { get; set; }
        public string CurrencyType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TransactionSerial { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDate2 { get; set; }
        public string LoginAccount { get; set; }
        public bool isModifyProfit { get; set; }
    }

    public class CompanyWhiteList
    {
        public int forCompanyID { get; set; }
        public string WhiteListID { get; set; }
        public string UserIdentity { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
    }

    public class CompanyManualHistory {
        public int CompanyManualID { get; set; }
        public string ServiceType { get; set; }
        public int forCompanyID { get; set; }
        public int forAdminID { get; set; }
        public int Type { get; set; }
        public string CurrencyType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TransactionSerial { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDate2 { get; set; }
        public string LoginAccount { get; set; }
        public string ServiceTypeName { get; set; }
        public string CompanyName { get; set; }
        public string ProviderCode { get; set; }
    }

    public class FrozenPoint {
        public int FrozenID { get; set; }
        public int forPaymentID { get; set; }
        public string forPaymentSerial { get; set; }
        public int forCompanyID { get; set; }
        public string forProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public decimal CompanyFrozenAmount { get; set; }
        public decimal ProviderFrozenAmount { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
        public string BankName { get; set; }
        public int GroupID { get; set; }
        public decimal ActualProviderFrozenAmount { get; set; }
    }

    public class ProxyProviderGroupFrozenPointHistory {
        public decimal FrozenAmount { get; set; }
        public string GroupName { get; set; }
    }

    public class FrozenPointHistory {
        public int FrozenID { get; set; }
        public string forPaymentSerial { get; set; }
        public int forCompanyID { get; set; }
        public string forProviderCode { get; set; }
        public decimal CompanyFrozenAmount { get; set; }
        public decimal ProviderFrozenAmount { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string CompanyName { get; set; }
        public string ProviderName { get; set; }
        public string CreateDate2 { get; set; }
        public string FrozenAdminName { get; set; }
        public string UnFrozenAdminName { get; set; }
        public DateTime UnFrozenDate { get; set; }
        public string GroupName { get; set; }
        public string ImageName { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
        public string BankName { get; set; }  
        public decimal ActualProviderFrozenAmount { get; set; }

    }

    public class BlackList {
        public int BlackListID { get; set; }
        public int forCompanyID { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
        public string UserIP { get; set; }
        public int Status { get; set; }
        public string CreateDate2 { get; set; }

    }

    public class RiskControlByPaymentSuccessCount {
        public string UserIP { get; set; }
        public int Count { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }

    public class ProviderOrderCount {
        public int PaymentCount { get; set; }
        public int WithdrawCount { get; set; }
        public int WithdrawCountByTimeEnd { get; set; }
    }

}

#endregion

#region APIFromBody

public class FromBody {

    public class ProviderBankCardData : DBModel.ProviderBankCard
    {
        public string BID { get; set; }
    }

    public class InsertServiceType {
        public string BID { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public int AllowCollect { get; set; }
        public int AllowPay { get; set; }
        public int ServiceSupplyType { get; set; }
        public int ServicePaymentType { get; set; }
    }
    public class WithdrawalPostSet {
        public string BID { get; set; }
        public int WithdrawID { get; set; }
        public string WithdrawSerial { get; set; }
        public string WithdrawalSerial { get; set; }
        public bool isReSendWithdraw { get; set; }
    }

    public class TransactionPostSet {
        public string BID { get; set; }
        public string TransactionSerial { get; set; }
    }

    public class CurrencyPostSet {
        public string BID { get; set; }
        public string Currency { get; set; }
    }

    public class BlockChainRate
    {
        public string BID { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public decimal UpFloatRate { get; set; }
        public decimal DownFloatRate { get; set; }
    }

    public class ProviderPostSet {
        public string BID { get; set; }
        public string ProviderCode { get; set; }
    }

    public class SearchIPC {
        public string BID { get; set; }
        public string IP { get; set; }
    }

    public class Company : DBModel.Company {
        public string BID { get; set; }
        public string CurrencyType { get; set; }
    }

    public class GetBankDataSet
    {
        public string BID { get; set; }
        public string CurrencyType { get; set; }
    }

    public class GetAllProviderPointBC 
    {
        public string BID { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public int CompanyID { get; set; }
    }
    

    public class ProviderService : DBModel.ProviderService {
        public string BID { get; set; }
    }

    public class SetPrePaidPoint 
    {
        public string BID { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public decimal Amount { get; set; }
    }

    public class Provider : DBModel.Provider {
        public string BID { get; set; }
    }

    public class ProxyProvider : DBModel.ProxyProvider {
        public string BID { get; set; }
    }

    public class PermissionCategory : DBModel.PermissionCategory {
        public string BID { get; set; }
    }

    public class CompanyPoint : DBModel.CompanyPoint {
        public string BID { get; set; }
    }

    public class CompanyService : DBModel.CompanyService {
        public string BID { get; set; }
    }

    public class GPayRelation : DBModel.GPayRelation {
        public string BID { get; set; }
    }

    public class WithdrawLimit : DBModel.WithdrawLimit {
        public string BID { get; set; }
    }

    public class ProviderManualHistory : DBModel.ProviderManualHistory {
        public string BID { get; set; }
    }

    public class CompanyManualHistory : DBModel.CompanyManualHistory {
        public string BID { get; set; }
    }

    public class FrozenPoint : DBModel.FrozenPoint {
        public string BID { get; set; }
        public bool BoolActualProviderFrozenAmount { get; set; }
    }

    public class BlackList : DBModel.BlackList {
        public string BID { get; set; }
    }

    public class ThawPoint {
        public string BID { get; set; }
        public int FrozenID { get; set; }
    }

    public class GPayRelationSet {
        public string BID { get; set; }
        public int forCompanyID { get; set; }
        public string ProviderCode { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public string ProviderName { get; set; }
        public int Weight { get; set; }
        public bool isAddRelation { get; set; }
    }

    public class GetOffLineResultSet {
        public string BID { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyState { get; set; }
    }

    public class Login {
        public string LoginAccount { get; set; }
        public string Password { get; set; }
        public string CompanyCode { get; set; }
        public string UserKey { get; set; }
    }

    public class SummaryProviderByDate {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public int GroupID { get; set; }
    }

    public class PaymentTable {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceType { get; set; }
        public int ProcessStatus { get; set; }
        public string PaymentSerial { get; set; }
        public string OrderID { get; set; }
        public int OperatorType { get; set; }
    }

    public class PaymentTransferLogSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderCode { get; set; }
        public int ProcessStatus { get; set; }
        public string PaymentSerial { get; set; }

    }

    public class DownOrderTransferLogSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CompanyCode { get; set; }
        public int ProcessStatus { get; set; }
        public string OrderID { get; set; }
        public int isErrorOrder { get; set; }

    }


    public class DownOrderTransferLogSetV2 : DownOrderTransferLogSet {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<JQueryDataTableColumns> columns { get; set; }
        public List<JQueryDataTableOrder> order { get; set; }
        public JQueryDataTableSearch search { get; set; }
    }

    public class GetPaymentForAdminV2 : GetPaymentForAdmin {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<JQueryDataTableColumns> columns { get; set; }
        public List<JQueryDataTableOrder> order { get; set; }
        public JQueryDataTableSearch search { get; set; }
        public bool IsAutoLoad { get; set; }
    }

    public class JQueryDataTableSearch {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class JQueryDataTableOrder {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class JQueryDataTableColumns {
        public string data { get; set; }
        public string name { get; set; }
        public string orderable { get; set; }
        public string searchable { get; set; }

    }

    public class GetPaymentForAdmin {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartAmount { get; set; }
        public decimal EndAmount { get; set; }
        public int CompanyID { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string CompanyName { get; set; }
        public string CurrencyType { get; set; }
        public List<int> ProcessStatus { get; set; }
        public string PaymentSerial { get; set; }
        public string OrderID { get; set; }
        public string UserIP { get; set; }
        public string ClientIP { get; set; }
        public int SubmitType { get; set; }
        public string TimeType { get; set; }
        public string ServiceType { get; set; }
        public string PatchDescription { get; set; }
        public List<int> PaymentIDs { get; set; }
    }

    public class GetAbnormalPaymentSet {
        public string BID { get; set; }
        public string ProviderOrderID { get; set; }
        public string OrderID { get; set; }
        public int CheckType { get; set; }// 0=订单上下十笔纪录 / 1=订单前后十分钟笔纪录
        public int SearchType { get; set; }// 0=商户订单 / 1=营运商订单
        public string Providercode { get; set; }
        public int CompanyID { get; set; }
        public string IP { get; set; }
    }

    public class GetPayment {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public List<int> ProcessStatus { get; set; }
        public string PaymentSerial { get; set; }
        public string OrderID { get; set; }
    }

    public class RemoveGoogleQrCode {
        public string BID { get; set; }
        public int CompanyID { get; set; }
        public string Password { get; set; }
    }

    public class PaymentSet {
        public string BID { get; set; }
        public string PaymentSerial { get; set; }
        public decimal Amount { get; set; }
        public string PatchDescription { get; set; }
        public decimal PatchAmount { get; set; }
        public string ProviderOrderID { get; set; }
        public int ProcessStatus { get; set; }
        public string ProviderCode { get; set; }
        public string ServiceType { get; set; }
        public string Password { get; set; }
        public int CompanyID { get; set; }
        public int GroupID { get; set; }
        public string Description { get; set; }
        public string UserKey { get; set; }
        public decimal RealAmount { get; set; }
    }

    public class AgentReceiveSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public string CurrencyType { get; set; }
    }

    public class AgentReceiveCloseSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public string CurrencyType { get; set; }
        public decimal TotalReceiveAmount { get; set; }
        public int TotolReceiveCount { get; set; }

    }

    public class GetAdminTableResult {
        public string BID { get; set; }
        public int CompanyID { get; set; }
    }

    public class GetAdminRoleTableResult {
        public string BID { get; set; }
        public int CompanyID { get; set; }
    }

    public class GetWithdrawalIPTableResult {
        public string BID { get; set; }
        public int CompanyID { get; set; }
        public string WithdrawalIP { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public string Type { get; set; }
        public int ImageID { get; set; }
    }

    public class SetFrozenPointImageResult {
        public string BID { get; set; }
        public int FrozenID { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public string Type { get; set; }
        public int ImageID { get; set; }
    }

    public class GetPermissionByAdminRoleID {
        public string BID { get; set; }
        public int AdminRoleID { get; set; }
    }

    public class InsertAdmin {
        public string BID { get; set; }
        public int CompanyID { get; set; }
        public int AdminroleID { get; set; }
        public string LoginAccount { get; set; }
        public string Password { get; set; }
        public string RealName { get; set; }
        public string Description { get; set; }
        public int AdminType { get; set; }
        public int AdminState { get; set; }
        public int AdminID { get; set; }
        public int GroupID { get; set; }
    }

    public class ProxyProviderGroupSet {
        public string BID { get; set; }
        public int GroupID { get; set; }
        public int State { get; set; }
        public int Weight { get; set; }
        public string GroupName { get; set; }
        public string OrderSerial { get; set; }
        public string ProviderCode { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }

    }

    public class ChangeProviderGroupOrdersByAdmin {
        public string BID { get; set; }
        public List<string> Withdrawals { get; set; }
        public int GroupID { get; set; }
    }

    public class ProxyProviderGroupWeightSet {
        public string BID { get; set; }
        public List<DBModel.ProxyProviderGroup> GroupData { get; set; }
    }

    public class UpdateLoginPassword {
        public string BID { get; set; }
        public string Newpassword { get; set; }
        public string Password { get; set; }
    }

    public class GetAdminOPLogResult {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public int Type { get; set; }
    }

    public class GetProviderManualHistory {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
    }

    public class GetCompanyWhiteListProviderBankCard
    {
        public string BID { get; set; }
        public string BankCardGUID { get; set; }
        public int forCompanyID { get; set; }
        public string UserIdentity { get; set; }
        public string Description { get; set; }
        public string WhiteListID { get; set; }
    }

    public class GetFrozenPointHistory {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentSerial { get; set; }
        public int CompanyID { get; set; }
        public string ProviderCode { get; set; }
        public int Status { get; set; }
        public int GroupID { get; set; }
    }

    public class GetBlackList {
        public string BID { get; set; }
        public int BlackListID { get; set; }
        public string UserIP { get; set; }
        public int CompanyID { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
    }

    public class GetCompanyManualHistory {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int forCompanyID { get; set; }
        public string ServiceType { get; set; }
    }

    public class SummaryCompanyByDateResultByCurrencyTypeSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }

    }

    public class PermissionSet {
        public string BID { get; set; }
        public string PermissionName { get; set; }
        public int AdminPermission { get; set; } //0:一般權限/1:管理者權限
        public string Description { get; set; }
        public string LinkURL { get; set; }
        public int SortIndex { get; set; }
        public int PermissionCategoryID { get; set; }
        public List<string> PermissionRoles { get; set; }
    }

    public class GoogleKeySet {
        public string BID { get; set; }
        public string UserKey { get; set; }
        public string GoogleKey { get; set; }
    }

    public class GoogleKeySetByAdmin {
        public string BID { get; set; }
        public string UserKey { get; set; }
        public string GoogleKey { get; set; }
        public string LoginAccount { get; set; }
    }

    public class ConfigSetting
    {
        public string BID { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
    }

    public class CompanyServiceSet {
        public string BID { get; set; }
        public int CompanyID { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public string CurrencyType { get; set; }
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public decimal BeforeMaxDaliyAmount { get; set; }
        public decimal MaxDaliyAmount { get; set; }
        public decimal MinOnceAmount { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public int CheckoutType { get; set; }
        public int State { get; set; }
        public List<ProviderCodeAndWeight> ProviderCodeAndWeight { get; set; }
    }

    public class ProviderCodeAndWeight {
        public string BID { get; set; }
        public string ProviderCode { get; set; }
        public int Weight { get; set; }
    }

    public class DeleteProviderService {
        public string BID { get; set; }
        public string ProviderCode { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
    }

    public class GPayWithdrawRelationSet {
        public string BID { get; set; }
        public int CompanyID { get; set; }
        public decimal Charge { get; set; }
        public decimal MinLimit { get; set; }
        public decimal MaxLimit { get; set; }
        public string CurrencyType { get; set; }
        public decimal Rate { get; set; }
        public List<ProviderCodeAndWeight> ProviderCodeAndWeight { get; set; }
    }

    public class GetProviderPointHistorySet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProviderCode { get; set; }
        public int OperatorType { get; set; }
        public int GroupID { get; set; }
    }


    public class BankCodeSet {
        public string BID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string ETHContractNumber { get; set; }
        public int BankState { get; set; }
        public int BankType { get; set; }
    }

    public class WithdrawalSetV2 : WithdrawalSet {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<JQueryDataTableColumns> columns { get; set; }
        public List<JQueryDataTableOrder> order { get; set; }
        public JQueryDataTableSearch search { get; set; }
        public bool IsAutoLoad { get; set; }
        public bool IsSearchWaitReview { get; set; }
    }

    public class WithdrawalSet {
        public string BID { get; set; }
        //流程狀態，0=建立/1=進行中/2=成功/3=失敗/99=全部資料
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WithdrawSerial { get; set; }
        public int CompanyID { get; set; }
        public int WithdrawType { get; set; }
        public string UserKey { get; set; }
        public string ProviderCode { get; set; }
        public string BankDescription { get; set; }
        public string RejectDescription { get; set; }
        public List<int> LstStatus { get; set; }
        public List<int> WithdrawIDs { get; set; }
        public string ServiceType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int GroupID { get; set; }
        public string TimeType { get; set; }
    }

    public class WithdrawalReportSet {
        public string BID { get; set; }
        public List<int> Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WithdrawSerial { get; set; }
        public string OrderID { get; set; }
        public string BankCardName { get; set; }
        public int CompanyID { get; set; }

    }

    public class RemoveAllWithdrawal {
        public string BID { get; set; }
        //流程狀態，0=建立/1=進行中/2=成功/3=失敗/99=全部資料
        public List<int> WithdrawIDs { get; set; }
    }

    public class SummaryCompanyByDateSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public string CurrencyType { get; set; }
    }

    public class CompanyPointHistoryeSet {
        public string BID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyID { get; set; }
        public string CurrencyType { get; set; }
        public string SearchDays { get; set; }
    }


    public class BankCardSet {
        public string BID { get; set; }
        public string BankCode { get; set; }
        public int forCompanyID { get; set; }
        public string BankCard { get; set; }
        public string BankCardName { get; set; }
        public string OwnProvince { get; set; }
        public string OwnCity { get; set; }
        public string BankBranchName { get; set; }
    }

    public class BlockChainAddressSet
    {
        public string BID { get; set; }
        public string BlockChainAddress { get; set; }
        public int BlockChainAddressID { get; set; }
        public int forCompanyID { get; set; }
    }

    public class InsertAdminRole {
        public string BID { get; set; }
        public string RoleName { get; set; }
        public List<string> AdminPermission { get; set; }
        public List<string> NormalPermission { get; set; }
        public int CompanyID { get; set; }
    }

    public class UpdateAdminRole {
        public string BID { get; set; }
        public string RoleName { get; set; }
        public List<string> AdminPermission { get; set; }
        public List<string> NormalPermission { get; set; }
        public int CompanyID { get; set; }
        public int AdminRoleID { get; set; }
    }

    public class WithdrawalCreate {
        public string BID { get; set; }
        public List<DBModel.Withdrawal> WithdrawalData { get; set; }
        public string UserKey { get; set; }
    }

    public class AdjustProviderPointSet {
        public string BID { get; set; }
        public string ProviderCode { get; set; }
        public decimal Amount { get; set; }
        public string WithdrawSerial { get; set; }
    }

    public class WithdrawalUpdate {
        public string BID { get; set; }
        public DBModel.Withdrawal WithdrawalData { get; set; }
    }

    public class UpdateProviderAPIType {
        public string BID { get; set; }
        public string ProviderCode { get; set; }
        public int setAPIType { get; set; }
    }


}

#endregion

#region DBViewModel

public class DBViewModel {


    public class ProviderWithdrawalOrderCount {
        public int TotalCount { get; set; }
        public int TotalCountTimeEnd { get; set; }
    }

    public class LayoutLeftSideBarResult : DBModel.Permission {
        public string CategoryDescription { get; set; }
        public string PermissionCategoryName { get; set; }

    }

    public class InsertCompanyReturn {
        public int CompanyID { get; set; }
        public string CompanyKey { get; set; }
    }

    public class CompanyServiceRelation {
        public string CompanyName { get; set; }
        public int CompanyID { get; set; }
        public int isSelected { get; set; } // 0=未选取/1=已选取
    }

    public class IPCounty {
        public string IP { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
    }

    public class PaymentRow_NunberID {
        public int ROWID { get; set; }
        public DateTime CreateDate { get; set; }

    }

    public class OffLineCompany {
        public int CompanyID { get; set; }
        public int CompanyState { get; set; }
        public string ParentCompanyCode { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreateDate { get; set; }
        public string MerchantCode { get; set; }
    }

    public class AdminRolePermission : DBModel.Permission {
        public Boolean selectedPermission { get; set; }
    }

    public class ApiWithdrawLimit : DBModel.WithdrawLimit {
        public bool selectedWithdrawLimit { get; set; }
        public string ProviderName { get; set; }
        public int Weight { get; set; }
        public int WithdrawType { get; set; }
    }

    public class AdminTableResult : DBModel.Admin {
        public string RoleName { get; set; }
        public string CreateDate2 { get; set; }
        public string GroupName { get; set; }
    }

    public class CompanyServiceTableResult : DBModel.CompanyService {
        public string RoleName { get; set; }
        public string CreateDate2 { get; set; }
        public string ServiceTypeName { get; set; }
        public string Description { get; set; }
        public int GPayRelationCount { get; set; }
        public List<DBModel.GPayRelation> GPayRelations { get; set; }
    }


    public class TestPageCompanyService {
        public string ServiceTypeName { get; set; }
        public string ServiceType { get; set; }
        public string CurrencyType { get; set; }
        public string ProviderName { get; set; }
        public decimal CollectRate { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public decimal MinOnceAmount { get; set; }
        public int State { get; set; }

        public int ProviderState { get; set; }
    }


    public class AdminRolePermissionResult : DBModel.AdminRole {
        public bool selectedAdminRole { get; set; }
    }

    public class CompanyPointVM : DBModel.CompanyPoint {
        public string CompanyName { get; set; }
        public decimal LockPointValue { get; set; }
        public decimal AutoWithdrawAmount { get; set; }
        public decimal MaxLimit { get; set; }
        public decimal MinLimit { get; set; }
        public decimal Charge { get; set; }
        public int CompanyCount { get; set; }
    }

    public class AgentReceiveVM : DBModel.Permission {
        public decimal UnCloseAmount { get; set; }
        public decimal CloseAmount { get; set; }
        public decimal AgentAmount { get; set; }
        public decimal OrderAmount { get; set; }
    }

    public class ProviderPointVM : DBModel.ProviderPoint {
        public string ProviderName { get; set; }
        public int ProviderAPIType { get; set; }
        public decimal ProviderFrozenAmount { get; set; }
        public decimal WithdrawPoint { get; set; }
        public decimal OKEXRate { get; set; }
        public decimal UpBlockChainRate { get; set; }
        public decimal WithdrawProfit { get; set; }
        public decimal PrePaidPoint { get; set; }
    }

    public class CompanyServicePointVM : DBModel.CompanyServicePoint {
        public string CompanyName { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal MaxLimit { get; set; }
        public decimal MinLimit { get; set; }
        public decimal FrozenPoint { get; set; }
        public decimal CanUsePoint { get; set; }
        public decimal Charge { get; set; }
        public decimal WithdrawalPoint { get; set; }
        public int FrozenServiceCount { get; set; }
        public int State { get; set; }
        public decimal FrozenServicePoint { get; set; }
        public decimal Rate { get; set; }
    }

    public class SummaryCompanyByDateVM : DBModel.SummaryCompanyByDate {
        public int CheckoutType { get; set; }

    }

    public class GPayRelationResult : DBModel.GPayRelation {

        public string ServiceTypeName { get; set; }
        public string CompanyName { get; set; }
    }

    public class GPayWithdrawRelation : DBModel.WithdrawLimit {
        public string ProviderName { get; set; }
        public int Weight { get; set; }
        public int WithdrawType { get; set; }

    }

    public class ProviderServiceVM : DBModel.ProviderService {
        public string ServiceTypeName { get; set; }
        public bool selectedProviderService { get; set; }
        public string ProviderName { get; set; }
        public int Weight { get; set; }
    }

    public class ServiceTypeVM : DBModel.ServiceTypeModel {
        public int isUpLine { get; set; } // 是否為最上線 0 :否 1: 是
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public decimal MaxDaliyAmount { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public decimal MinOnceAmount { get; set; }

    }

    public class ProviderServiceTypeVM : DBModel.ServiceTypeModel {
        public int isUpLine { get; set; } // 是否為最上線 0 :否 1: 是
        public decimal CollectRate { get; set; }
        public decimal CollectCharge { get; set; }
        public decimal MaxDaliyAmount { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public decimal MinOnceAmount { get; set; }

    }


    public class BankCardVM : FromBody.BankCardSet {
        public string BankName { get; set; }
    }

    public class WithdrawalVM : DBModel.WithdrawalTable {
        public string CompanyName { get; set; }
        public string BankName { get; set; }
        public string WithdrawDate2 { get; set; }
        public string CreateDate2 { get; set; }
        public string OwnProvince { get; set; }
        public string OwnCity { get; set; }
        public string BankBranchName { get; set; }
        public string BankCardName { get; set; }
        public string RealName { get; set; }

    }

    public class UpdateWithdrawalResult {
        public int Status { get; set; }
        public string Message { get; set; }
        public decimal PaymentAmount { get; set; }
        public DBModel.Withdrawal WithdrawalData { get; set; }

    }

    public class UpdatePatmentResult {
        public int Status { get; set; }
        public string Message { get; set; }
        public DBModel.PaymentReport PaymentData { get; set; }

    }

    public class ProviderPointHistory {
        public string CreateDate2 { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public int OperatorType { get; set; }
        public string ProviderName { get; set; }
        public string TransactionOrder { get; set; }

    }


    public class ProxyProviderPointHistory {
        public string CreateDate2 { get; set; }
        public decimal Value { get; set; }
        public decimal BeforeValue { get; set; }
        public decimal GroupValue { get; set; }
        public decimal GroupBeforeValue { get; set; }
        public int OperatorType { get; set; }
        public string ProviderName { get; set; }
        public string TransactionOrder { get; set; }
        public string GroupName { get; set; }
    }

    public class ProviderListResult {
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public int ProviderAPIType { get; set; }
        public decimal MaxLimit { get; set; }
        public decimal MinLimit { get; set; }
        public decimal Charge { get; set; }
        public string CurrencyType { get; set; }
        public int ProviderState { get; set; }
        public List<ServiceData> ServiceDatas { get; set; }
        public List<ProviderPointVM> ProviderListPoints { get; set; }
        public List<ProviderListPoint> ProviderListFrozenPoints { get; set; }
    }

    public class ServiceData {
        public string ServiceTypeName { get; set; }
        public decimal MaxDaliyAmount { get; set; }
        public decimal MaxOnceAmount { get; set; }
        public decimal MinOnceAmount { get; set; }
        public decimal CostCharge { get; set; }
        public decimal CostRate { get; set; }
        public int CheckoutType { get; set; }
        public string ServiceType { get; set; }
        public string ProviderCode { get; set; }
        public string CurrencyType { get; set; }
        public int State { get; set; }
    }

    public class ProviderListPoint {
        public string CurrencyType { get; set; }
        public decimal SystemPointValue { get; set; }
    }

    public class AllProviderTotal {
        public int Count { get; set; }
        public decimal Total { get; set; }
        public decimal FrozenTotal { get; set; }

    }

    public class AdminOPLogVM : DBModel.AdminOPLog {
        public string LoginAccount { get; set; }
        public string CreateDate2 { get; set; }
    }

    public class AdminWithKey : DBModel.Admin {
        public string GoogleKey { get; set; }
    }


}

#endregion
