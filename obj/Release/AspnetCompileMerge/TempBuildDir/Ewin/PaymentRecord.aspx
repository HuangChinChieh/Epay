<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Common.cs" Inherits="Common" %>

<%
    dynamic paymentReport;
    string paymentResult = "";

    string OrderID = Request.Params["OrderID"];
    string CompanyCode = Request.Params["CompanyCode"];
    string PaymentType = Request.Params["PaymentType"];
    string Timestamp = Request.Params["Timestamp"];
    string Sign = Request.Params["Sign"];
    int CompanyID = -1;

    APIResult R = new APIResult() { ResultState = APIResult.enumResultCode.ERR };

    if (OrderID == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "The parameter orderID not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    if (CompanyCode == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "The parameter CompanyCode not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    if (Sign == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "The parameter Sign not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    if (PaymentType == null)
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "The parameter PaymentType not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    //if (Common.CheckInIP(InIP))
    //{
    if (!Common.CheckTimestamp(long.Parse(Timestamp)))
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "Timestamp Expired";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    if (Common.CheckPaymentSign(CompanyCode,OrderID,PaymentType,Sign,Timestamp))
    {
        CompanyID= Common.GetCompanyKeyByCompanyID(CompanyCode);

        if (CompanyID==-1)
        {
            R.ResultState = Common.APIResult.enumResultCode.ERR;
            R.Message = "Company Code Error";
            Response.Write(R.Message);
            Response.Flush();
            Response.End();
        }

        if (PaymentType == "0")
        {
            paymentReport = Common.GetPaymentByOrderID(OrderID);

            if (paymentReport != null)
            {
                paymentResult = Newtonsoft.Json.JsonConvert.SerializeObject(paymentReport);
            }
            else
            {
                R.ResultState = APIResult.enumResultCode.ERR;
                R.Message = "Payment Not Exist";
                         Response.Write(R.Message);
                    Response.Flush();
                    Response.End();
            }
        }
        else if (PaymentType == "1")
        {
            paymentReport = Common.GetWithdrawalByOrderID(OrderID,CompanyID);

            if (paymentReport != null)
            {
                paymentResult = Newtonsoft.Json.JsonConvert.SerializeObject(paymentReport);
            }
            else
            {
                R.ResultState = APIResult.enumResultCode.ERR;
                R.Message = "Payment Not Exist";
                         Response.Write(R.Message);
                    Response.Flush();
                    Response.End();
            }
        }
        else
        {
            R.ResultState = APIResult.enumResultCode.ERR;
            R.Message = "PaymentType Error";
            Response.Write(R.Message);
            Response.Flush();
            Response.End();
        }
    }
    else
    {
        R.ResultState = APIResult.enumResultCode.ERR;
        R.Message = "Sign Fail";
                 Response.Write(R.Message);
                    Response.Flush();
                    Response.End();
    }
    //}
    //else
    //{
    //    R.ResultState = APIResult.enumResultCode.ERR;
    //    R.Message = "IP Fail:" + InIP;
    //}



%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="pragma" content="no-cache" />
    <title></title>


    <link rel="stylesheet" href="/VPay/assets/plugins/bootstrap/css/bootstrap.min.css">
    <!-- Bootstrap Select Css -->
    <link href="/VPay/assets/plugins/bootstrap-select/css/bootstrap-select.css" rel="stylesheet" />

    <!-- Custom Css -->
    <link rel="stylesheet" href="/VPay/assets/css/main.css">
    <!-- Jquery Core Js -->
    <script src="/VPay/assets/js/jquery-3.3.1.min.js"></script>
    <script src="/VPay/assets/bundles/libscripts.bundle.js"></script> <!--Lib Scripts Plugin Js ( jquery.v3.2.1, Bootstrap4 js)-->



    <script src="/VPay/assets/js/BackendJS/BackendAPI.js?20200508"></script>
    <script src="/VPay/assets/js/AutoNumeric.js"></script>
    <style>
        * {
            font-family: 微軟正黑體, 微軟雅黑體, Arial;
        }
    </style>
</head>
<script>
    var paymentResult = '<%=paymentResult%>';
    var jsonPaymentResult;
    var PaymentType = '<%=PaymentType%>';
    $(function () {
        if (paymentResult != "") {
            jsonPaymentResult = JSON.parse(paymentResult);

            if (PaymentType == '0') {
                var ProcessStatus = "";
                var CostCharge = 0;
                if (jsonPaymentResult.PartialOrderAmount != 0) {
                    CostCharge = toCurrency((jsonPaymentResult.PartialOrderAmount * jsonPaymentResult.CollectRate * 0.01) + jsonPaymentResult.CollectCharge);
                }

                switch (jsonPaymentResult.ProcessStatus) {
                    case 0:
                        ProcessStatus = '<span style="color:red;font-weight:900">未处理</span>';
                        break;
                    case 1:
                        ProcessStatus = '<span style="color:red;font-weight:900">未处理</span>';
                        break;
                    case 2:
                        ProcessStatus = '<span style="color:orange;font-weight:900">交易成功，尚未通知商户</span>';
                        break;
                    case 3:
                        ProcessStatus = '交易失败';
                        break;
                    case 4:
                        ProcessStatus = '<span style="color:green;font-weight:900">交易成功，已通知商户</span>';
                        break;
                    case 5:
                        ProcessStatus = '供应商资料不一致';
                        break;
                    case 6:
                        ProcessStatus = '系统问题';
                        break;
                    case 7:
                        ProcessStatus = '补单';
                        break;
                    default:
                        return '';
                        break;
                }

                $('#DepositModal').modal('show');
                $('#DepositModal_PaymentSerial').text(jsonPaymentResult.PaymentSerial);
                $('#DepositModal_OrderID').text(jsonPaymentResult.OrderID);
                $('#DepositModal_ProviderName').text(jsonPaymentResult.ProviderName);
                $('#DepositModal_ServiceTypeName').text(jsonPaymentResult.ServiceTypeName);
                $('#DepositModal_PaymentAmount').html(toCurrency(jsonPaymentResult.PaymentAmount));
                $('#DepositModal_ProcessStatus').html(ProcessStatus);
                $('#DepositModal_OrderAmount').text(toCurrency(jsonPaymentResult.OrderAmount));
                $('#DepositModal_CostRate').text(jsonPaymentResult.CostRate);
                $('#DepositModal_CostCharge').text(CostCharge);
                $('#DepositModal_CreateDate2').text(jsonPaymentResult.CreateDate2);
                $('#DepositModal_FinishDate2').text(jsonPaymentResult.FinishDate2);
                $('#DepositModal_UserIP').text(jsonPaymentResult.UserIP);
            }
            else if (PaymentType == '1') {
                var ProcessStatus = "";
                var CostCharge = 0;

                if (jsonPaymentResult.DecimalPlaces == 1) {
                    CostCharge = toCurrency(Math.floor((jsonPaymentResult.Amount * jsonPaymentResult.WithdrawRate * 0.01) + jsonPaymentResult.CollectCharge));
                } else {
                    CostCharge = toCurrency((jsonPaymentResult.Amount * jsonPaymentResult.WithdrawRate * 0.01) + jsonPaymentResult.CollectCharge);
                }

                switch (jsonPaymentResult.Status) {
                    case 0:
                        ProcessStatus= '未处理';
                        break;
                    case 1:
                        ProcessStatus = '<span style="color:orange;font-weight:900">进行中</span>';
                        break;
                    case 2:
                        ProcessStatus = '<span style="color:green;font-weight:900">成功</span>';
                        break;
                    case 3:
                        ProcessStatus = '<span style="color:red;font-weight:900">失敗</span>';
                        break;
                    case 9:
                        ProcessStatus = '审核退回';
                        break;
                    case 13:
                        ProcessStatus = '重审';
                        break;
                    case 14:
                        ProcessStatus = '系统问题单';
                        break;
                    default:
                        return '';
                        break;
                }

                $('#WithdrawalModal').modal('show');
                $('#WithdrawalModal_WithdrawSerial').text(jsonPaymentResult.WithdrawSerial);
                $('#WithdrawalModal_DownOrderID').text(jsonPaymentResult.DownOrderID);
                $('#WithdrawalModal_ProviderName').text(jsonPaymentResult.ProviderName);
                $('#WithdrawalModal_Status').html(ProcessStatus);
                $('#WithdrawalModal_BankName').text(jsonPaymentResult.BankName);
                $('#WithdrawalModal_BankCard').text(jsonPaymentResult.BankCard);
                $('#WithdrawalModal_BankCardName').text(jsonPaymentResult.BankCardName);
                $('#WithdrawalModal_Amount').text(toCurrency(jsonPaymentResult.Amount));
                $('#WithdrawalModal_CollectCharge').text(CostCharge);
                $('#WithdrawalModal_CreateDate2').text(jsonPaymentResult.CreateDate2);
                $('#WithdrawalModal_FinishDate2').text(jsonPaymentResult.FinishDate2);
                $('#WithdrawalModal_OwnProvince').text(jsonPaymentResult.OwnProvince);
                $('#WithdrawalModal_OwnCity').text(jsonPaymentResult.OwnCity);
                $('#WithdrawalModal_BankBranchName').text(jsonPaymentResult.BankBranchName);
                $('#WithdrawalModal_FinishAmount').text(toCurrency(jsonPaymentResult.FinishAmount));
            }
      
        }
        
    });

    function toCurrency(num) {

        num = parseFloat(Number(num).toFixed(2));
        var parts = num.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }
</script>
<body>
    <div class="modal fade" style="background-color: rgba(0, 0, 0, 0.4);" data-backdrop="false" id="DepositModal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="title" id="">充值单</h4>
                </div>
                <div class="modal-body">
                    <span style="display: block">平台订单号:<span id="DepositModal_PaymentSerial" style="margin-left: 5px"></span></span>
                    <span style="display: block">商户订单号:<span id="DepositModal_OrderID" style="margin-left: 5px"></span></span>
                    <span style="display: block">渠道:<span id="DepositModal_ProviderName" style="margin-left: 5px"></span></span>
                    <span style="display: block">支付方式:<span id="DepositModal_ServiceTypeName" style="margin-left: 5px"></span></span>
                    <span style="display: block">状态:<span id="DepositModal_ProcessStatus" style="margin-left: 5px"></span></span>
                    <span style="display: block">申请金额:<span id="DepositModal_OrderAmount" style="margin-left: 5px"></span></span>
                    <span style="display: block">费率(%):<span id="DepositModal_CostRate" style="margin-left: 5px"></span></span>
                    <span style="display: block">手续费:<span id="DepositModal_CostCharge" style="margin-left: 5px"></span></span>
                    <span style="display: block">实际金额:<span id="DepositModal_PaymentAmount" style="margin-left: 5px"></span></span>
                    <span style="display: block">提交时间:<span id="DepositModal_CreateDate2" style="margin-left: 5px"></span></span>
                    <span style="display: block">完成时间:<span id="DepositModal_FinishDate2" style="margin-left: 5px"></span></span>
                    <span style="display: block">IP:<span id="DepositModal_UserIP" style="margin-left: 5px"></span></span>
                </div>
                <div class="modal-footer">
      
                </div>
            </div>
        </div>
    </div>

     <div class="modal fade" style="background-color: rgba(0, 0, 0, 0.4);" data-backdrop="false" id="WithdrawalModal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="title" id="">代付单</h4>
                </div>
                <div class="modal-body">
                    <span style="display: block">平台订单号:<span id="WithdrawalModal_WithdrawSerial" style="margin-left: 5px"></span></span>
                    <span style="display: block">商户订单号:<span id="WithdrawalModal_DownOrderID" style="margin-left: 5px"></span></span>
                    <span style="display: block">渠道:<span id="WithdrawalModal_ProviderName" style="margin-left: 5px"></span></span>
                    <span style="display: block">状态:<span id="WithdrawalModal_Status" style="margin-left: 5px"></span></span>
                    <span style="display: block">申请金额:<span id="WithdrawalModal_Amount" style="margin-left: 5px"></span></span>
                    <span style="display: block">手续费:<span id="WithdrawalModal_CollectCharge" style="margin-left: 5px"></span></span>
                    <span style="display: block">银行:<span id="WithdrawalModal_BankName" style="margin-left: 5px"></span></span>
                    <span style="display: block">卡号:<span id="WithdrawalModal_BankCard" style="margin-left: 5px"></span></span>
                    <span style="display: block">持卡人姓名:<span id="WithdrawalModal_BankCardName" style="margin-left: 5px"></span></span>
                    <span style="display: block">省份:<span id="WithdrawalModal_OwnProvince" style="margin-left: 5px"></span></span>
                    <span style="display: block">城市:<span id="WithdrawalModal_OwnCity" style="margin-left: 5px"></span></span>
                    <span style="display: block">分行:<span id="WithdrawalModal_BankBranchName" style="margin-left: 5px"></span></span>
                    <span style="display: block">实际金额:<span id="WithdrawalModal_FinishAmount" style="margin-left: 5px"></span></span>
                    <span style="display: block">提交时间:<span id="WithdrawalModal_CreateDate2" style="margin-left: 5px"></span></span>
                    <span style="display: block">完成时间:<span id="WithdrawalModal_FinishDate2" style="margin-left: 5px"></span></span>
                
                </div>
                <div class="modal-footer">
      
                </div>
            </div>
        </div>
    </div>
</body>
</html>


