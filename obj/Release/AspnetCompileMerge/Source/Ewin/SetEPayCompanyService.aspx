<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetEPayCompanyService.aspx.cs" Inherits="SetEPayCompanyService" %>

<%
    string paymentResult = "";
    int CompanyID = -1;
    string CompanyCode = Request.Params["CompanyCode"];
    string Sign = Request.Params["Sign"];
    string Timestamp = Request.Params["Timestamp"];

    Common.APIResult R = new Common.APIResult() { ResultState = Common.APIResult.enumResultCode.ERR };

    if (CompanyCode == null)
    {
        R.ResultState = Common.APIResult.enumResultCode.ERR;
        R.Message = "The parameter CompanyCode not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    if (Sign == null)
    {
        R.ResultState = Common.APIResult.enumResultCode.ERR;
        R.Message = "The parameter Sign not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

    if (!Common.CheckTimestamp(long.Parse(Timestamp)))
    {
        R.ResultState = Common.APIResult.enumResultCode.ERR;
        R.Message = "Timestamp Expired";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }
    //if (Common.CheckInIP(InIP))
    //{

    if (Common.CheckProviderListSign(CompanyCode,Sign,Timestamp))
    {
        CompanyID=Common.GetCompanyIDByCompanyCode(CompanyCode);
        if (CompanyID != -1)
        {
            //ListProviderListResult = Common.GetProviderListResult(CompanyID);

            //if (ListProviderListResult != null)
            //{
            //    ServiceDatas = Common.GetProviderListServiceData(CompanyID);
            //    ProviderPoints = Common.GetAllProviderPoint(CompanyID);

            //    foreach (var item in ListProviderListResult)
            //    {
            //        if (ServiceDatas != null)
            //        {
            //            item.ServiceDatas = ServiceDatas.Where(w => w.ProviderCode == item.ProviderCode).ToList();
            //        }

            //        if (ProviderPoints != null)
            //        {
            //            item.ProviderListPoints = ProviderPoints.Where(w => w.ProviderCode == item.ProviderCode).ToList();
            //        }
            //    }

            //    strListProviderListResult=Newtonsoft.Json.JsonConvert.SerializeObject(ListProviderListResult);
            //}
            //else
            //{
            //    R.ResultState = Common.APIResult.enumResultCode.ERR;
            //    R.Message = "No Provider Data";
            //             Response.Write(R.Message);
            //        Response.Flush();
            //        Response.End();
            //}
        }
        else
        {
            R.ResultState = Common.APIResult.enumResultCode.ERR;
            R.Message = "CompanyCode Error";
            Response.Write(R.Message);
            Response.Flush();
            Response.End();
        }
    }
    else
    {
        R.ResultState = Common.APIResult.enumResultCode.ERR;
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
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet"/>
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.8.1/css/all.css" integrity="sha384-50oBUHEmvpQ+1lW4y57PTFmhCaXp0ML5d60M1M7uH2+nqUivzIebhndOJK28anvf" crossorigin="anonymous"/>
    <link rel="icon" href="favicon.ico" type="image/x-icon"/>
<link rel="stylesheet" href="/VPay/assets/plugins/bootstrap/css/bootstrap.min.css">
    <!-- Bootstrap Select Css -->
    <link href="/VPay/assets/plugins/bootstrap-select/css/bootstrap-select.css" rel="stylesheet" />
    <link rel="stylesheet" href="/VPay/assets/plugins/jvectormap/jquery-jvectormap-2.0.3.min.css" />
    <link rel="stylesheet" href="/VPay/assets/plugins/morrisjs/morris.min.css" />
    <!-- Custom Css -->
    <link rel="stylesheet" href="/VPay/assets/css/main.css">
    <link rel="stylesheet" href="/VPay/assets/css/color_skins.css">
    <!-- JQuery DataTable Css -->
    <link rel="stylesheet" href="/VPay/assets/plugins/jquery-datatable/dataTables.bootstrap4.min.css">

    <!-- JQuery sweetalert Css -->
    <link href="/VPay/assets/plugins/sweetalert/sweetalert.css" rel="stylesheet" />
    <!-- JQuery jquery-nestable Css -->
    <link href="/VPay/assets/plugins/nestable/jquery-nestable.css" rel="stylesheet" />
    <!-- datetimepicker Css-->
    <link href="/VPay/assets/plugins/bootstrap-material-datetimepicker/css/bootstrap-material-datetimepicker.css" rel="stylesheet" />
    <!-- JQuery multi-select Css -->
    <link href="/VPay/assets/plugins/multi-select/css/multi-select.css" rel="stylesheet" />
    <link href="/VPay/assets/css/ecommerce.css" rel="stylesheet" />
    <!-- jquery-ui Css -->
    <link href="/VPay/assets/css/jquery-ui.css" rel="stylesheet" />
    <link href="/VPay/assets/css/ui.tabs.overflowResize.css" rel="stylesheet" />
    <!-- Jquery Core Js -->
    <script src="/VPay/assets/js/jquery-3.3.1.min.js"></script>
    <script src="/VPay/assets/js/D3/d3.min.js"></script>
    <script src="/VPay/assets/bundles/libscripts.bundle.js"></script> <!--Lib Scripts Plugin Js ( jquery.v3.2.1, Bootstrap4 js)-->
    <script src="/VPay/assets/bundles/vendorscripts.bundle.js"></script>  <!--slimscroll, waves Scripts Plugin Js-->
    <script src="/VPay/assets/plugins/multi-select/js/jquery.multi-select.js"></script>
    <script src="/VPay/assets/plugins/momentjs/moment.js"></script> <!-- Moment Plugin Js -->
    <script src="/VPay/assets/bundles/morrisscripts.bundle.js"></script><!-- Morris Plugin Js -->
    <script src="/VPay/assets/bundles/jvectormap.bundle.js"></script> <!-- JVectorMap Plugin Js -->
    <script src="/VPay/assets/bundles/knob.bundle.js"></script> <!-- Jquery Knob-->
    <script src="/VPay/assets/bundles/mainscripts.bundle.js"></script>
    <script src="/VPay/assets/js/pages/index.js"></script>
    <script src="/VPay/assets/plugins/nestable/jquery.nestable.js"></script>
    <script src="/VPay/assets/plugins/jquery-spinner/js/jquery.spinner.js"></script>
    <!-- Jquery DataTable Plugin Js -->
    <script src="/VPay/assets/bundles/datatablescripts.bundle.js"></script>
    <script src="/VPay/assets/plugins/jquery-datatable/buttons/dataTables.buttons.min.js"></script>
    <script src="/VPay/assets/plugins/jquery-datatable/buttons/buttons.bootstrap4.min.js"></script>
    <script src="/VPay/assets/plugins/jquery-datatable/buttons/buttons.colVis.min.js"></script>
    <script src="/VPay/assets/plugins/jquery-datatable/buttons/buttons.html5.min.js"></script>
    <script src="/VPay/assets/plugins/jquery-datatable/buttons/buttons.print.min.js"></script>
    <script src="/VPay/assets/js/pages/tables/jquery-datatable.js"></script>

    <script src="/VPay/assets/plugins/jquery-datatable/dataTables.responsive.min.js"></script>
    <script src="/VPay/assets/plugins/bootstrap-notify/bootstrap-notify.min.js"></script>
    <script src="/VPay/assets/plugins/jquery-inputmask/jquery.inputmask.bundle.js"></script>
    <!-- basic-form-elements Js -->
    <!-- datetimepicker Js -->
    <script src="/VPay/assets/plugins/bootstrap-material-datetimepicker/js/bootstrap-material-datetimepicker.js"></script>
    <!-- sweetalert Plugin Js -->
    <script src="/VPay/assets/plugins/sweetalert/sweetalert.min.js"></script>
    <script src="/VPay/assets/plugins/jquery-spinner/js/jquery.spinner.js"></script>
    <script src="/VPay/assets/js/BackendJS/BackendAPI.js?20200326"></script>
    <script src="/VPay/assets/js/AutoNumeric.js"></script>
    <script src="/VPay/assets/js/ion.sound.min.js"></script>
    <!-- JQuery 頁籤 Js -->
    <script src="/VPay/assets/js/jquery-ui.js"></script>
    <script src="/VPay/assets/js/ui.tabs.overflowResize.js"></script>
    <script src="/VPay/assets/js/ui.tabs.addTab.js"></script>
    <script src="/VPay/assets/js/ui.tabs.closable.js"></script>
    <script src="/Ewin/Common.js"></script>
    <style>


        ::placeholder { /* Chrome, Firefox, Opera, Safari 10.1+ */
            color: black !important;
            opacity: 1; /* Firefox */
        }

        .filter-option {
            color: black !important;
            opacity: 0.8;
        }
        /* fallback */
        @@font-face {
            font-family: 'Material Icons';
            font-style: normal;
            font-weight: 400;
            src: url(https://fonts.gstatic.com/s/materialicons/v47/flUhRq6tzZclQEJ-Vdg-IuiaDsNc.woff2) format('woff2');
        }

        button:hover {
            cursor: pointer;
        }

        .material-icons {
            font-family: 'Material Icons';
            font-weight: normal;
            font-style: normal;
            font-size: 24px;
            line-height: 1;
            letter-spacing: normal;
            text-transform: none;
            display: inline-block;
            white-space: nowrap;
            word-wrap: normal;
            direction: ltr;
            -webkit-font-feature-settings: 'liga';
            -webkit-font-smoothing: antialiased;
        }

        .dtp-buttons {
            display: none;
        }

        .navbar

        .divided:hover {
            cursor: pointer;
        }

        button, input, optgroup, select, textarea {
            margin: 0;
            font-family: "Microsoft JhengHei";
            font-size: inherit;
            line-height: inherit;
        }

        form > label {
            font-size: 14px;
            font-weight: bold;
        }

        [data-notify="container"][class*="alert-pastel-"] {
            background-color: rgb(255, 255, 238);
            border-width: 0px;
            border-left: 15px solid rgb(255, 240, 106);
            border-radius: 0px;
            box-shadow: 0px 0px 5px rgba(51, 51, 51, 0.3);
            font-family: 'Old Standard TT', serif;
            letter-spacing: 1px;
            opacity: 0.9;
            width: 90%;
        }

        [data-notify="container"].alert-pastel-info {
            border-left-color: rgb(255, 179, 40);
        }

        [data-notify="container"].alert-pastel-danger {
            border-left-color: rgb(255, 103, 76);
        }

        [data-notify="container"][class*="alert-pastel-"] > [data-notify="title"] {
            color: rgb(80, 80, 57);
            display: block;
            font-weight: 700;
            margin-bottom: 5px;
        }

        [data-notify="container"][class*="alert-pastel-"] > [data-notify="message"] {
            font-weight: 400;
            color: black;
        }

        .alert .close {
            color: black !important;
            margin-right: 20px;
        }

        .selectedItem {
            color: forestgreen !important;
            background: #DDDDDD !important;
            border-color: black !important;
        }

            .selectedItem:hover {
                color: forestgreen;
                background: #DDDDDD;
            }

        .dd-handle:hover {
            cursor: pointer;
        }

        table .selected {
            background-color: #dff0d8 !important;
        }

        .show > .dropdown-toggle {
            color: #fff !important;
        }

        .table-responsive > div > table {
            width: 100%;
        }

        .btn-timeZone:hover {
            cursor: pointer;
        }
        /*.dtr-data {
            word-break: normal;
            width: auto;
            display: block;
            white-space: pre-wrap;
            word-wrap: break-word;
            overflow: hidden;
        }*/
        .dtr-title {
            display: inline-block;
            width: 150px;
            font-weight: bold;
            white-space: pre-wrap;
            word-wrap: break-word;
            word-break: normal;
        }

        table.dataTable > tbody > tr.child ul.dtr-details {
            display: block;
            list-style-type: none;
            margin: 0;
            padding: 0;
        }



        .card_title {
            font-size: 22px;
        }

        .card_subtitle {
            padding: 20px;
        }

        .h1, .h2, .h3, .h4, .h5, .h6, h1, h2, h3, h4, h5, h6 {
            font-family: "Microsoft JhengHei";
        }

        * {
            font-family: 微軟正黑體, 微軟雅黑體, Arial;
        }
        /*body{min-width: 1200px;}*/
        /*body > .bg{background-image: url('../bg/davide-ragusa-716-unsplash.jpg');}*/
        body > .bg {
            background-image: url('assets/background/bg.jpg');
        }
        /*body > .bg{background-image: url('backgrounds/1.jpg');}*/
        /*body > .bg{background-image: url('../bg/jeremy-bishop-171845-unsplash.jpg');}*/
        /*body > .bg{background-image: url('../bg/paul-carmona-276464-unsplash.jpg');}*/
        /*body > .bg{background-image: url('../bg/vinoth-ragunathan-96670-unsplash.jpg');}*/
        body > .bg {
            background-size: cover;
            position: fixed;
            width: 100%;
            height: 100%;
        }

        .navbar-brand img {
            filter: brightness(10);
        }

        .navbar-brand span {
            color: #fff;
        }
        /*.navbar-nav > li > a i.fa-sync-alt, .navbar-nav > li > a i.fa-bell, .navbar-nav > li > a i.fa-caret-down{font-size: inherit;}*/
        nav.navbar li > a {
            color: #fff !important;
        }

        li.divided > a {
            display: inline-block;
            line-height: 30px;
            padding: 0 15px;
        }

        span.label.label-transparent-black {
            position: absolute;
            top: -10px;
            left: 26px;
            z-index: 2;
        }

            span.label.label-transparent-black:before {
                content: '';
                background-color: #FF6B6B;
                position: absolute;
                width: 18px;
                height: 18px;
                border-radius: 50%;
                left: 50%;
                top: 50%;
                transform: translate(-50%,-50%);
                z-index: -1;
            }

        .displaynone, .overlay, .sidebar .menu .list .ml-menu, .menu_sm .sidebar .list li.header, .menu_sm .sidebar .list .detail, .menu_sm .sidebar .list a::before, .menu_sm .sidebar .list a::after, .menu_sm .sidebar .list a span {
            background-color: rgba(0,0,0,0.2);
        }

        .user .profile-photo {
            display: inline-block;
            width: 92px;
            height: 92px;
            margin: 0 0 4px 0;
            border-radius: 50%;
            border: 4px solid #fff;
            box-shadow: 0 2px 16px rgba(0,0,0,0.4);
            transition: 0.2s all;
        }

            .user .profile-photo img {
                width: 100%;
            }

        #current-user {
            text-align: center;
            list-style: none;
            margin: 16px 0 0;
        }

            #current-user .account p {
                color: #fff;
            }

            #current-user .account #layoutCompanyName {
                font-size: 18px;
                margin: 6px 0;
            }

            #current-user .account #layoutLoginAccount {
                font-size: 14px;
                font-weight: lighter;
                margin: 0;
                padding-bottom: 12px;
                border-bottom: 1px solid rgba(255,255,255,0.2);
            }

        #leftsidebar {
            background-color: rgba(0,0,0,0.1);
        }

        .sidebar .menu .list a {
            color: #eee;
            padding: 12px 16px;
        }

            .sidebar .menu .list a span {
                font-size: 16px;
            }

        .sidebar .menu .list .ml-menu li a {
            padding: 10px 0 10px 60px;
        }

            .sidebar .menu .list .ml-menu li a:before {
                content: '';
            }

        #timeShowBox {
            color: #fff;
            margin: 0;
        }

        #timeZoneSelect {
            width: 90%;
            height: 0;
            margin: 8px auto 0;
            background-color: #fff;
            border-radius: 4px;
            box-shadow: 0 2px 32px rgba(0,0,0,0.1);
            position: relative;
            text-align: center;
            transition: 0.3s all;
            font-size: 0;
        }

            #timeZoneSelect:before {
                content: '';
                position: absolute;
                top: -4px;
                left: 50%;
                transform: translateX(-50%);
                width: 0;
                height: 0;
                border-left: 5px solid transparent;
                border-right: 5px solid transparent;
                border-bottom: 5px solid transparent;
                transition: 0.3s all;
            }

            #timeZoneSelect.active {
                height: 54px;
                padding: 12px 0;
            }

                #timeZoneSelect.active:before {
                    border-bottom: 5px solid #fff;
                }

            #timeZoneSelect .btn-timeZone {
                display: -webkit-inline-flex;
                display: -moz-inline-flex;
                display: -ms-inline-flex;
                display: -o-inline-flex;
                display: inline-flex;
                justify-content: center;
                -ms-align-items: center;
                align-items: center;
                width: 88px;
                height: 30px;
                margin: 0 4px;
                font-size: 12px;
                color: transparent;
                background-color: transparent;
                border-radius: 4px;
                transition: 0.3s all;
            }

            #timeZoneSelect.active .btn-timeZone {
                color: #fff;
                background-color: #65C3DF;
            }

        .navbar {
            min-height: 65px;
            background-image: url('assets/background/nav_bg.jpg');
            background-size: cover;
            background-position: center;
        }

        ul.flex, .navLeft, .navRight {
            display: -webkit-inline-flex !important;
            display: -moz-inline-flex !important;
            display: -ms-inline-flex !important;
            display: -o-inline-flex !important;
            display: inline-flex !important;
            justify-content: space-between;
            -ms-align-items: center;
            align-items: center;
            flex-direction: inherit;
        }

            .navLeft li:last-child {
                position: absolute;
                left: 214px;
            }

            .navRight li:nth-child(1), .navRight li:nth-child(2), .navRight li:nth-child(3) {
                border-right: 1px solid #888;
                height: 30px;
            }

        .navRight {
            margin-right: 12px;
        }
        /*.navbar ul.nav li{position: absolute; top: 50%; transform: translateY(-50%);}*/
        /*.navbar ul.nav li:nth-child(2){left: 150px;}
        .navbar ul.nav li:nth-child(3){right: 340px; border-right: 1px solid #888; height: 30px;}
        .navbar ul.nav li:nth-child(3) p{margin: 0;}
        .navbar ul.nav li:nth-child(4){right: 290px; border-right: 1px solid #888; height: 30px;}
        .navbar ul.nav li:nth-child(5){right: 242px; border-right: 1px solid #888; height: 30px;}
        .navbar ul.nav li:nth-child(6){right: 52px;}
        .navbar ul.nav li:nth-child(7){right: 6px; border-left: 1px solid #888; height: 30px;}*/
        .copyright {
            position: absolute;
            left: 28px;
            bottom: 12px;
            transition: 0.2s all;
        }

        body .copyright p {
            margin: 0;
            font-size: 12px;
            color: #eee;
        }

        body.theme-cyan.menu_sm .copyright {
            left: 4px;
            transition: 0.2s all;
        }

        body.theme-cyan.menu_sm .profile-photo {
            width: 52px;
            height: 52px;
            border-width: 2px;
            transition: 0.2s all;
        }

        body.theme-cyan.menu_sm .account p {
            display: none;
        }

        section.content {
            margin: 65px 0px 0 250px;
            background-color: #f5f5f5;
            border-radius: 0;
        }

        .container-fluid > .row {
            padding: 12px 0;
        }

        .dd-handle {
            background: none;
            border: 0;
        }

            .dd-handle:hover {
                color: #45B6B0;
            }

        ol.dd-list > li.dd-item > ol.dd-list > li.dd-item > .dd-handle {
            color: #575b61;
            padding-left: 24px;
        }

        ol.dd-list > li.dd-item > ol.dd-list > li.dd-item:before {
            content: '';
            position: absolute;
            left: 0;
            top: -5px;
            width: 1px;
            height: calc(100% + 10px);
            height: -moz-calc(100% + 10px);
            height: -webkit-calc(100% + 10px);
            border-left: 0.5px solid #ddd;
        }

        .divCompanyTree > ol.dd-list > li.dd-item {
            /*border-bottom: 1px solid #ddd;*/
            margin: 6px 0;
        }

        li.dd-item {
            padding: 0 12px;
        }

            li.dd-item > button {
                position: absolute;
                left: 10px;
            }

        .divCompanyTree > ol.dd-list > li.dd-item:last-child {
            border: 0;
        }

        .divCompanyTree > ol.dd-list > li.dd-item > .dd-handle {
            font-weight: bold;
            font-size: 15px;
            padding-left: 24px;
        }

        .theme-cyan .sidebar .menu .list {
            padding: 0;
        }

            .theme-cyan .sidebar .menu .list > li {
                padding: 0;
                margin: 4px 12px;
            }
            /*.theme-cyan .sidebar .menu .list li ul li{padding: 0 12px;}*/
            .theme-cyan .sidebar .menu .list li a:hover {
                background-color: rgba(0,0,0,0.2);
            }

            .theme-cyan .sidebar .menu .list li ul {
                background: rgba(0,0,0,0.35);
            }

            .theme-cyan .sidebar .menu .list li a:hover {
                color: #fff;
            }
        /*.strongtitle{color: #6f7884; font-size: 22px;}*/
        .col-lg-7.col-md-7.col-sm-7 > p {
            margin: 0;
        }

        .block-header {
            padding: 0 64px;
            margin-bottom: 16px;
            border-bottom: 1px solid rgba(0,0,0,0.1);
        }

            .block-header h2 {
                color: #565d67;
                font-weight: bold;
            }

            .block-header .col {
                padding: 16px;
            }

                .block-header .col.col-lg-9.col-md-9.col-sm-9 {
                    display: -webkit-inline-flex;
                    display: -moz-inline-flex;
                    display: -ms-inline-flex;
                    display: -o-inline-flex;
                    display: inline-flex;
                    -ms-align-items: center;
                    align-items: center;
                    border-left: 1px dashed #ddd;
                }

        .tips {
            font-size: 12px;
            color: #565d67;
            padding-left: 12px;
        }

        #divtree {
            padding: 0 24px;
        }

        @@media screen and (min-width: 1600px) {
            #divtree {
                padding: 0 56px;
            }
        }

        .divtreeTool {
            font-size: 0;
        }

            .divtreeTool button {
                width: calc((100% - 6px * 2)/ 3 );
                margin: 0 6px 6px 0;
                font-size: 12px;
            }

                .divtreeTool button:last-child {
                    margin-right: 0;
                }

            .divtreeTool .btn-divtreeTool {
                padding: 12px 0;
            }

        .paper-wrap:before {
            content: "";
            position: absolute;
            top: 0;
            left: 0;
            border-width: 0 42px 42px 0;
            border-style: solid;
            border-color: #ffffff #253856;
            -webkit-box-shadow: 0 0 0 rgba(0, 0, 0, 0.4), 0 0 10px rgba(0, 0, 0, 0.3);
            -moz-box-shadow: 0 0 0 rgba(0, 0, 0, 0.4), 0 0 10px rgba(0, 0, 0, 0.3);
            box-shadow: 0 0 0 rgba(0, 0, 0, 0.4), 0 0 10px rgba(0, 0, 0, 0.3);
            -webkit-transform: rotate(-90deg);
            -moz-transform: rotate(-90deg);
            -ms-transform: rotate(-90deg);
            -o-transform: rotate(-90deg);
            filter: progid: DXImageTransform.Microsoft.BasicImage(rotation=3);
        }

        .card .header .row .col-lg-7.col-md-7.col-sm-7 {
            display: -webkit-flex;
            display: -moz-flex;
            display: -ms-flex;
            display: -o-flex;
            display: flex;
            -ms-align-items: center;
            align-items: center;
        }

        table {
            font-size: 14px !important;
            color: #575b61 !important;
        }

        .card .body {
            font-size: 14px !important;
            color: #575b61 !important;
            padding: 0 20px !important;
        }

            .card .body .row, #divCompanyPoint.card .body {
                margin: 0 0 8px 0;
            }

        /*.card.wallet {
            width: 120px;
        }*/

        .footer {
            padding: 16px 0;
        }
        /*.theme-cyan .card .header h2:before{background: #45B6B0; top: -17px;}*/
        .ortherText button {
            margin: 0 8px;
        }

        .theme-cyan .btn-primary {
            background: #65C3DF;
        }

        .dataTables_wrapper input[type="search"]:focus, .dataTables_wrapper input[type="search"]:active {
            border-bottom: 2px solid #65C3DF;
        }

        div.dataTables_wrapper div.dataTables_paginate ul.pagination {
            margin: 16px 0 8px;
        }


        .theme-cyan .btn-primary:active, .theme-cyan .btn-primary:hover, .theme-cyan .btn-primary:focus {
            background: #45B6B0;
        }

        .sorting_1:focus {
            outline: 0;
        }

        .btn-primary:not(:disabled):not(.disabled).active, .btn-primary:not(:disabled):not(.disabled):active, .show > .btn-primary.dropdown-toggle {
            background-color: #446288;
        }

        button.btn-circle {
            width: 36px;
            height: 36px;
            padding: 0;
            border-radius: 50%;
            line-height: 36px;
        }

        button.btn-round {
            padding: 9px 24px;
        }

        table.dataTable.dtr-inline.collapsed > tbody > tr[role="row"] > td:first-child:before, table.dataTable.dtr-inline.collapsed > tbody > tr[role="row"] > th:first-child:before {
            background-color: #65C3DF;
        }

        footer.footer {
            text-align: right;
            padding: 0;
            background-color: #fff;
            border-top: 1px solid #ccc;
        }

            footer.footer p {
                display: inline-block;
                margin: 0 20px 0 0;
                font-size: 12px;
                color: #aaa;
            }

        [data-notify="container"][class*="alert-pastel-"] {
            background-color: rgb(255, 255, 238);
            border-width: 0px;
            border-left: 15px solid rgb(255, 240, 106);
            border-radius: 0px;
            box-shadow: 0px 0px 5px rgba(51, 51, 51, 0.3);
            font-family: 'Old Standard TT', serif;
            letter-spacing: 1px;
            opacity: 0.9;
            width: 90%;
        }

        [data-notify="container"].alert-pastel-info {
            border-left-color: rgb(255, 179, 40);
        }

        [data-notify="container"].alert-pastel-danger {
            border-left-color: rgb(255, 103, 76);
        }

        [data-notify="container"][class*="alert-pastel-"] > [data-notify="title"] {
            color: rgb(80, 80, 57);
            display: block;
            font-weight: 700;
            margin-bottom: 5px;
        }

        [data-notify="container"][class*="alert-pastel-"] > [data-notify="message"] {
            font-weight: 400;
            color: black;
        }

        .alert .close {
            color: black !important;
            margin-right: 20px;
        }

        .selectedItem {
            /*color: forestgreen;*/
            /*background: #DDDDDD;*/
            border-color: black;
        }

            .selectedItem:hover {
                /*color: forestgreen;*/
                /*background: #DDDDDD;*/
            }

        .dd-handle:hover {
            cursor: pointer;
        }

        table .selected {
            background-color: #a2aec7 !important;
        }

        .show > .dropdown-toggle {
            /*color: #fff !important;*/
        }

        .table-responsive > div > table {
            width: 100%;
        }

        .dtr-data {
            word-break: normal;
            width: auto;
            display: block;
            white-space: pre-wrap;
            word-wrap: break-word;
            overflow: hidden;
        }


        .divDetailData .card {
            background-color: #fff;
        }

        .divDetailData > .card > .header {
            padding: 12px 16px;
            margin-bottom: 30px;
            border-bottom: 1px solid rgba(0,0,0,0.1);
        }

        .divPointDetail {
            font-size: 0;
            white-space: nowrap;
        }

            .divPointDetail .footer {
                padding: 0;
            }

            .divPointDetail .card.wallet {
                border-right: 1px solid rgba(0,0,0,0.1) !important;
                margin-bottom: 30px;
                border-radius: 0;
            }

                .divPointDetail .card.wallet:last-child {
                    border-right: 0;
                }

            .divPointDetail .card.wallet {
                background: none;
                box-shadow: none;
            }

                .divPointDetail .card.wallet .header {
                    padding: 4px 20px 8px;
                }

                .divPointDetail .card.wallet h2:before {
                    width: 16px;
                    height: 2px;
                    top: -4px;
                }

                .divPointDetail .card.wallet p.text-muted, .divPointDetail .card.wallet p.wallettext-muted {
                    color: #6f7884 !important;
                    margin: 0;
                    white-space: pre-wrap;
                    font-size: 18px;
                }

        .table-responsive table th {
            font-weight: normal;
        }

    .leftItem {
        float: left;
    }

    .cg-Item {
        padding: 5px 10px;
    }

    .list-group-item {
        border: 0;
        background: none;
    }
    </style>
</head>
    
<script>
    var CompanyServiceTable;
    var WithdrawLimitTable;
    var CompanyWithdrawRelationTable;
    var CData;
    var CSeleData;
    var seleCompanyid;
    var selectedParentCompanyID;
    var boolCreatedCompanyServiceTable = false;
    var isCreatedWithdrawLimitTable = false;
    var isCreatedCompanyWithdrawRelationTable = false;
    var isCreatedTable = false;
    var selectCurrencyType;
    var tempCompanyPoint = null;
    var CompanyServicePointTable;
    var CompanyID =<%=CompanyID%>;
    var apiURL = "/Ewin/SetEPayCompanyService.aspx";
    var chineseTableLang = {
        "processing": "处理中...",
        "loadingRecords": "载入中...",
        "lengthMenu": "显示 _MENU_ 项结果",
        "zeroRecords": "没有符合的结果",
        "info": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项",
        "infoEmpty": "显示第 0 至 0 项结果，共 0 项",
        "infoFiltered": "(從 _MAX_ 项结果中过滤)",
        "infoPostFix": "",
        "search": "搜寻:",
        "paginate": {
            "first": "第一页",
            "previous": "上一页",
            "next": "下一页",
            "last": "最后一页"
        },
        "aria": {
            "sortAscending": ": 升幂划分",
            "sortDescending": ": 降幂划分"
        }
    };
    var c = new common();

    $(document).ready(function () {
        pageLoad();
    });
    function pageLoad() {
   
        AdminObject = parent.AdminObject;
        api = parent.api;

        createCompanyServicePointTable();

        ShowCompanyService();
        //新增商戶渠道資料
        $('#btnCompanyServiceAdd').on("click", function () {

        });

        //編輯商戶渠道資料
        $("#btnCompanyServiceEdit").on("click", function () {
            updateCompanyService();
        });

        //停用商戶渠道資料
        $('#btnCompanyServiceDisable').on("click", function () {
            if (CompanyServiceTable.row(".selected").data() == undefined) {
                alert("请先选择要修改的资料");
            } else {
                swal({
                    title: "是否要(停用/启用)？",
                    type: "warning",
                    showCancelButton: true,
                    closeOnConfirm: true,
                    confirmButtonText: "确定",
                    cancelButtonText: "取消",
                    confirmButtonColor: "#DD6B55",
                }, function (isConfirm) {
                    if (isConfirm) {
                        disableCompanyService(CompanyID, CompanyServiceTable.row(".selected").data().ServiceType, CompanyServiceTable.row(".selected").data().CurrencyType);
                    }
                });
            }
        })
        //檢視設定的商戶
        $("#btnShowGPayRelation").on("click", function () {

            var targetData = CompanyServiceTable.row(".selected").data();


            if (CompanyServiceTable.row(".selected").data() != undefined) {
                var companyServiceData = CompanyServiceTable.row(".selected").data();
                var postObj = {
                    CompanyID: companyServiceData.forCompanyID,
                    ServiceType: companyServiceData.ServiceType,
                    CurrencyType: companyServiceData.CurrencyType
                };
                c.callService(apiURL + "/GetGPayRelationResult", postObj, function (s, obj) {
                    if (s) {
                        wrapperFadeOut();
                        obj = c.getJSON(obj);

                        if (obj.ResultCode == 0) {
                            var strswaltext = "";
                            strswaltext += '<div class="row clearfix justify-content-center">';
                            $.each(obj.GPayRelations, function (i, item) {
                                strswaltext += '<div class="col-lg-6 col-md-6 col-sm-6">' +
                                    '<div class="card" style="border:1px solid #49c5b6" >' +
                                    '<div class="header">' +
                                    '<strong style="color:#49c5b6;font-size:18px">' + obj.GPayRelations[i].ProviderName + '</strong>' +
                                    '</div> ' + '</div ></div >';
                            })
                            strswaltext += '</div> ';
                            swal({
                                title: targetData.ServiceTypeName,
                                text: strswaltext,
                                html: true,
                                confirmButtonText: "确认"
                            });

                        }
                        else {
                            var message = ""
                            switch (obj.ResultCode) {
                                case 4:
                                    alert("权限不足");
                                    break;
                                case 7:
                                    alert("您已断线请重新登入");
                                    break;
                                case 5:
                                    alert("尚未设定供应商渠道");
                                    break;
                                default:
                                    break;
                            }
                        }
                    } else {
                        wrapperFadeOut();
                        alert("其他错误");
                    }
                });
            } else {
                alert("请先选择渠道资料");
            }
        })
        //檢視設定的商戶(代付)
        $("#btnShowGPayWithdrawRelation").on("click", function () {

            var targetData = CompanyWithdrawRelationTable.row(".selected").data();

            if (CompanyWithdrawRelationTable.row(".selected").data() != undefined) {

                api.getGPayWithdrawRelationByCompanyID(seleCompanyid, function (success, obj) {
                    if (obj.ResultCode == 0) {
                        var strswaltext = "";
                        strswaltext += '<div class="row clearfix justify-content-center">';
                        for (var i = 0; i < obj.GPayWithdrawRelations.length; i++) {
                            strswaltext += '<div class="col-lg-6 col-md-6 col-sm-6">' +
                                '<div class="card" style="border:1px solid #49c5b6" >' +
                                '<div class="header">' +
                                '<strong style="color:#49c5b6;font-size:18px">' + obj.GPayWithdrawRelations[i].ProviderName + '</strong>' +
                                '</div> ' + '</div ></div >';
                        }



                        strswaltext += '</div> ';
                        swal({
                            title: "代付渠道供应商",
                            text: strswaltext,
                            html: true,
                            confirmButtonText: "确认"
                        });
                    } else {
                        alert("尚未设定代付渠道");
                    }
                });



            } else {
                alert("请先选择要设定的资料");
            }
        })
        //新增供應商代付限額
        $("#btnCompanyWithdrawLimitCreate").on("click", function () {
            var data = {
                CompanyID: seleCompanyid,
                CompanyName: $('#pCompanyCode').text(),
                backID: window.frameElement.id.split('_')[1]
            }

            var tmpdata = {
                type: "CompanyWithdrawLimitCreate",
                data: data
            };
            sessionStorage.setItem("TempData", JSON.stringify(tmpdata));
            parent.GoPage("CompanyWithdrawLimitCreate", `商户代付限额新增(${CSeleData[0].CompanyName})`, "CompanyWithdrawLimit_Create.cshtml");

        })
        //編輯供應商代付限額
        $("#btnCompanyWithdrawLimitEdit").on("click", function () {
            if (WithdrawLimitTable == undefined) {
                alert("尚未有商户提现资料");
            }
            else if (WithdrawLimitTable.row(".selected").data() == undefined) {
                alert("请先选择要编辑的资料");
            } else {
                var backID = window.frameElement.id.split('_')[1];
                WithdrawLimitTable.row(".selected").data().CompanyName = $('#pCompanyCode').text();
                WithdrawLimitTable.row(".selected").data().forCompanyID = seleCompanyid;
                parent.CreateSessionStorage("CompanyWithdrawLimitUpdate", WithdrawLimitTable.row(".selected").data(), `商户代付限额修改(${CSeleData[0].CompanyName})`, "CompanyWithdrawLimit_Update.cshtml", backID, "CompanyWithdrawLimit_Update_" + WithdrawLimitTable.row(".selected").data().CompanyName + "_" + + WithdrawLimitTable.row(".selected").data().forCompanyID);

            }
        })
        //新增自动代付渠道
        $("#btnCompanyWithdrawRelationCreate").on("click", function () {
            var data = {
                CompanyID: seleCompanyid,
                CompanyName: $('#pCompanyCode').text(),
                ParentCompanyID: selectedParentCompanyID,
                backID: window.frameElement.id.split('_')[1]
            }

            var tmpdata = {
                type: "CompanyWithdrawRelationCreate",
                data: data
            };

            sessionStorage.setItem("TempData", JSON.stringify(tmpdata));
            parent.GoPage("CompanyWithdrawRelationCreate", `商户自动代付新增(${CSeleData[0].CompanyName})`, "CompanyWithdrawRelation_Create.cshtml");

        })
        //編輯供應商代付渠道
        $("#btnCompanyWithdrawRelationEdit").on("click", function () {
            if (CompanyWithdrawRelationTable == undefined) {
                alert("尚未有商户代付渠道资料");
            }
            else if (CompanyWithdrawRelationTable.row(".selected").data() == undefined) {
                alert("请先选择要编辑的资料");
            } else {
                CompanyWithdrawRelationTable.row(".selected").data().CompanyName = $('#pCompanyCode').text();
                CompanyWithdrawRelationTable.row(".selected").data().CompanyID = seleCompanyid;
                CompanyWithdrawRelationTable.row(".selected").data().ParentCompanyID = selectedParentCompanyID;
                var backID = window.frameElement.id.split('_')[1];
                parent.CreateSessionStorage("CompanyWithdrawRelationUpdate", CompanyWithdrawRelationTable.row(".selected").data(), `商户自动代付修改(${CSeleData[0].CompanyName})`, "CompanyWithdrawRelation_Update.cshtml", backID, "CompanyWithdrawRelation_Update_" + CompanyWithdrawRelationTable.row(".selected").data().CompanyName + "_" + CompanyWithdrawRelationTable.row(".selected").data().CompanyID);
            }
        })

    }

    function GetSelectedCompanyService() {

        var parentCompanyID = EditData.data.ParentCompanyID
        var ServiceType = EditData.data.ServiceType;
        var currencyType = EditData.data.CurrencyType;

        api.getSelectedCompanyService(parentCompanyID, ServiceType, currencyType, function (success, obj) {
            if (obj.ResultCode == 0) {
                uplineCompanyService = obj.CompanyServiceResult;
                setNotifyLabelText(obj.CompanyServiceResult);
            } else {
                var message = ""
                switch (obj.ResultCode) {
                    case 4:
                        alert("权限不足");
                        break;
                    case 5:
                        alert("上线尚未设定渠道");
                        break;
                    case 7:
                        alert("您已断线请重新登入");
                        break;
                    default:
                        alert("其他错误");
                        break;
                }
            }
        });

    }

    function updateCompanyService() {
        var html = ` <div class="container-fluid">
        <div class="row clearfix">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="card">
                    <div class="header"></div>
                    <div class="body">
                        <form>

                            <label for="">渠道 : </label>
                            <div class="form-group">
                                <label id="label_ServiceType" style="margin-left:20px;font-size:16px"></label>
                            </div>

                            <label for="">币别 : </label>
                            <div class="form-group">
                                <label style="margin-left:20px;font-size:16px" id="label_CurrencyType"></label>
                            </div>

                            <label for="select_ServiceType">渠道设定 : </label>
                            <div id="providerServiceSet" style="display:none;">
                                <div class="form-group" style="margin-top:5px">
                                    <div class="col-lg-12 col-md-12 col-sm-12">

                                        <div class="row clearfix" id="divGPayRelationDetail">
                                            &nbsp;&nbsp;&nbsp;&nbsp;尚未设定供应商
                                        </div>

                                    </div>
                                </div>
                            </div>


                            <label for="input_CollectRate">单笔交易费率(%) : </label><label id="notifyCollectRate" style="color:red;margin:5px"></label>
                            <div class="form-group">
                                <input data-rule="percent" class="form-control percent" id="input_CollectRate" type="text" placeholder="請輸入單筆交易費率(%)">
                            </div>

                            <label style="display:none" for="input_CollectCharge">单笔交易费(/次) : </label><label id="notifyCollectCharge" style="color:red;margin:5px;display:none"></label>
                            <div class="form-group">
                                <input style="display:none" data-rule="currency" class="form-control thousand-symbols" id="input_CollectCharge" type="text" placeholder="請輸入單筆交易費(/次)">
                            </div>

                            <label for="input_MinOnceAmount">单笔交易最低金额 : </label><label id="notifyMinOnceAmount" style="color:red;margin:5px"></label>
                            <div class="form-group">
                                <input data-rule="currency" class="form-control thousand-symbols" id="input_MinOnceAmount" type="text" placeholder="請輸入單筆交易最低金額">
                            </div>

                            <label for="input_MaxOnceAmount">单笔交易最高金额 : </label><label id="notifyMaxOnceAmount" style="color:red;margin:5px"></label>
                            <div class="form-group">
                                <input data-rule="currency" class="form-control thousand-symbols" id="input_MaxOnceAmount" type="text" placeholder="請輸入單筆交易最高金額">
                            </div>

                            <label for="input_MaxDaliyAmount">每日交易最大用量 : </label><label id="notifyMaxDaliyAmount" style="color:red;margin:5px;display:none"></label>
                            <div class="form-group">
                                <input data-rule="currency" class="form-control thousand-symbols" id="input_MaxDaliyAmount" type="text" placeholder="請輸入每日交易最大用量">
                            </div>
                            <div id="divCheckoutType" style="display:none">
                                <label for="">结算时间 : </label>
                                <div class="radio">
                                    <input type="radio" name="rdoCheckoutType" id="CheckoutType0" value="0">
                                    <label for="CheckoutType0">
                                        当日结算
                                    </label>
                                </div>
                                <div class="radio">
                                    <input type="radio" name="rdoCheckoutType" id="CheckoutType1" value="1">
                                    <label for="CheckoutType1">
                                        隔日结算
                                    </label>
                                </div>
                                <div class="radio">
                                    <input type="radio" name="rdoCheckoutType" id="CheckoutType2" value="2">
                                    <label for="CheckoutType2">
                                        当周结算
                                    </label>
                                </div>
                            </div>

                            <label for="">状态 : </label>
                            <div class="radio">
                                <input type="radio" name="radioState" id="radioState0" value="0">
                                <label for="radioState0">
                                    启用
                                </label>
                            </div>
                            <div class="radio">
                                <input type="radio" name="radioState" id="radioState1" value="1">
                                <label for="radioState1">
                                    停用
                                </label>
                            </div>
                        </form>
                    </div>
                    <div class="footer"></div>
                </div>
            </div>
        </div>
    </div>`;
        
        swal({
            title: "商户渠道修改",
            text: html,
            showCancelButton: true,
            closeOnConfirm: false,
            confirmButtonText: "确认",
            cancelButtonText: "取消",
            html: true,
        },function (inputValue) {
                if (inputValue === false) return false;

                if (inputValue === "") {
                    swal.showInputError("尚未输入 Google 验证码");
                    return false
                }
                $('.sweet-alert').find('.confirm').attr("disabled", true);

                api.WithdrawalCreate(array_data, inputValue, function (s, o) {
                    if (s) {
                        if (o.ResultCode == 0) {
                            var failCount = array_data.length - o.Message;

                            parent.successAlert3("申请完成,成功笔数" + o.Message + ",失败笔数:" + failCount, function () {
                                reloadPage();
                            });

                        } else {
                            switch (o.ResultCode) {

                                case 7:
                                    parent.errorAlert("您已断线请重新登入", "Login.cshtml");
                                    break;
                                case 13:
                                    parent.warningAlert(o.Message);
                                    break;
                                case 14:
                                    parent.warningAlert(o.Message);
                                    break;
                                case 15:
                                    parent.warningAlert(o.Message);
                                    break;
                                default:
                                    parent.successAlert3("目前没有可申请订单,5分钟内只能提交一张相同银行卡资讯订单", function () {
                                        reloadPage();
                                    });

                                    break;
                            }
                        }
                        $('.sweet-alert').find('.confirm').attr("disabled", false);

                    } else {
                        $('.sweet-alert').find('.confirm').attr("disabled", false);

                    }
                });

            });
    }

    function createCompanyServicePointTable() {

        CompanyServicePointTable = $("#table_CompanyServicePointTable").DataTable({
            columns: [
                {
                    "title": "支付方式", "data": "ServiceTypeName"
                },
                {
                    "title": "可结算金额", "data": "SystemPointValue",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data - rowdata.FrozenServicePoint);
                    }
                },
                {
                    "title": "冻结金额", "data": "FrozenServicePoint",
                    "render": function (data) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "冻结笔数", "data": "FrozenServiceCount",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                }
            ],
            rowCallback: function (row, data) { },
            filter: true,
            info: false,
            ordering: true,
            processing: true,
            retrieve: true,
            selected: true,
            scrollX: true,
            'order': [[1, 'desc']],
            autoWidth: false,
            language: chineseTableLang
        });
    }

    //#region 商戶錢包資料
    function ShowCompanyPoint() {
        $("#divCompanyPoint").hide();
        $("#divCompanyPointDetail").empty();
        let strCompanyPointDetail = "";
        var postObj = {
            CompanyID: CompanyID
        }
        wrapperFadeIn();
        c.callService(apiURL + "/GetCompanyPointTableResult", postObj, function (s, o) {
            if (s) {
                wrapperFadeOut();
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    tempCompanyPoint = o.CompanyPoints;

                    $.each(o.CompanyPoints, function (i, item) {

                        //if (o.CompanyPoints[i].CurrencyType=='CNY') {
                        var canUsePoint = o.CompanyPoints[i].PointValue - o.CompanyPoints[i].LockPointValue - o.CompanyPoints[i].FrozenPoint;

                        strCompanyPointDetail += '<strong style="font-size:18px" class="text-muted">帐户金额： ' + toCurrency(o.CompanyPoints[i].PointValue) + ', 可用金额 : ' + toCurrency(canUsePoint) + ', 不可用金额 : ' + toCurrency(o.CompanyPoints[i].LockPointValue) + ', </br>冻结金额 : ' + toCurrency(o.CompanyPoints[i].FrozenPoint) + '</strong > ';
                        //}
                    })

                    $("#divCompanyPointDetail").html(strCompanyPointDetail);
                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
                            break;
                        case 5:
                            //$('#divCompanyPointDetail').html('<div class="span6" style="float: none; margin: 0 auto;"><h6>尚未設定錢包</h6></div>');
                            break;
                        case 7:
                            alert("您已断线请重新登入");
                            break;
                        default:
                            alert("其他错误");
                            break;
                    }
                }
            } else {
                wrapperFadeOut();
                alert("其他错误");
            }
            $("#divCompanyPoint").show();
        });

        c.callService(apiURL + "/GetCompanyServicePointDetail2", postObj, function (success, obj) {
            if (success) {
                wrapperFadeOut();
                obj = c.getJSON(obj);
                if (CompanyServicePointTable) {
                    CompanyServicePointTable.clear().draw();
                }

                if (obj.ResultCode == 0) {
                    CompanyServicePointTable.rows.add(obj.CompanyServicePoints).draw();
                } else {
                    switch (obj.ResultCode) {
                        case 4:
                            alert("权限不足");
                            break;
                        case 5:
                            //warningAlert("沒有資料");
                            break;
                        case 7:
                            alert("您已断线请重新登入");
                            break;
                        default:
                            alert("其他错误");
                            break;
                    }

                    wrapperFadeOut();
                }
            } else {
                wrapperFadeOut();
                alert("其他错误");
            }
         
        });
    }
    //#endregion

    //#region 商戶渠道
    function ShowCompanyService() {
        postObj = {
            CompanyID: CompanyID
        }
        wrapperFadeIn();
        c.callService(apiURL + "/GetCompanyAllServiceDetailData", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                wrapperFadeOut();
                if (o.ResultCode == 0) {
                    //自动代付設定
                    if (isCreatedCompanyWithdrawRelationTable) {
                        CompanyWithdrawRelationTable.clear().draw();
                    }

                    if (o.WithdrawRelations != null) {
                        if (!isCreatedCompanyWithdrawRelationTable) {
                            isCreatedCompanyWithdrawRelationTable = true;
                            CreateCompanyWithdrawRelationTable(o.WithdrawRelations);
                        } else {
                            CompanyWithdrawRelationTable.rows.add(o.WithdrawRelations).draw();
                        }
                    }
                    //充值通道
                    if (isCreatedTable) {
                        CompanyServiceTable.clear().draw();
                    }

                    if (o.CompanyServiceResults != null) {
                        if (!isCreatedTable) {
                            isCreatedTable = true;
                            createCompanyServiceTable(o.CompanyServiceResults);
                        } else {
                            CompanyServiceTable.rows.add(o.CompanyServiceResults).draw();
                        }
                    }

                    //下发限额
                    if (isCreatedWithdrawLimitTable) {
                        WithdrawLimitTable.clear().draw();
                    }

                    if (o.WithdrawLimits != null) {
                        if (!isCreatedWithdrawLimitTable) {
                            isCreatedWithdrawLimitTable = true;
                            CreateWithdrawLimitTable(o.WithdrawLimits);

                        } else {
                            WithdrawLimitTable.rows.add(o.WithdrawLimits).draw();
                        }
                    }

                }
                else {
                    switch (o.ResultCode) {
                        case 7:
                            alert("您已断线请重新登入");
                            break;
                        default:
                            if (isCreatedWithdrawLimitTable) {
                                WithdrawLimitTable.clear().draw();
                            }

                            if (isCreatedTable) {
                                CompanyServiceTable.clear().draw();
                            }

                            if (isCreatedCompanyWithdrawRelationTable) {
                                CompanyWithdrawRelationTable.clear().draw();
                            }
                            break;
                    }
                }
            } else {
                alert("连线失败");
                wrapperFadeOut();
            }

        });

        ShowCompanyPoint();
    }

    function createCompanyServiceTable(data) {
        isCreatedTable = true;

        CompanyServiceTable = $("#table_CompanyServiceTable").DataTable({
            data: data,
            columns: [
                { "title": "支付方式", "data": "ServiceTypeName" },
                { "title": "支付代码", "data": "ServiceType" },
                { "title": "渠道数量", "data": "GPayRelationCount" },

                {
                    "title": "状态", "data": "State",
                    "render": function (data) {
                        if (data == 0) {
                            return '<span style="color:blue;">启用</span>';
                        } else {
                            return '<span style="color:red;">停用</span>';
                        }
                    }
                },
                { "title": "费率(%)", "data": "CollectRate" },
                {
                    "title": "单笔限额(低)", "data": "MinOnceAmount",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "单笔限额(高)", "data": "MaxOnceAmount",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "每日交易最大用量", "data": "MaxDaliyAmount",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "备注", "data": "Description"
                }
            ],
            rowCallback: function (row, data) { },
            autoWidth: true,
            filter: true,         //右上搜尋
            info: false,
            ordering: true, //排序
            processing: true,
            retrieve: true,
            paging: true,    //分頁
            scrollX: true,
            autoWidth: false,
            language: chineseTableLang
        });

        $('#table_CompanyServiceTable tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                CompanyServiceTable.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    }

    function CreateWithdrawLimitTable(data) {

        WithdrawLimitTable = $('#table_ProviderWithdrawLimit').DataTable({
            data: data,
            columns: [
                { "title": "币别", "data": "CurrencyType" },
                { "title": "支付通道", "data": "ServiceTypeName" },
                {
                    "title": "单笔交易最低金额", "data": "MinLimit",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "单笔交易最高金额", "data": "MaxLimit",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "手续费", "data": "Charge",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                }
            ],
            rowCallback: function (row, data) { },
            filter: true,         //右上搜尋
            info: false,
            ordering: true, //排序
            processing: true,
            retrieve: true,
            paging: true,    //分頁
            scrollX: true,
            autoWidth: false,
            language: chineseTableLang
        });

        $('#table_ProviderWithdrawLimit tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                WithdrawLimitTable.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    }

    function CreateCompanyWithdrawRelationTable(data) {

        CompanyWithdrawRelationTable = $('#table_CompanyWithdrawRelation').DataTable({
            data: data,
            columns: [
                { "title": "币别", "data": "CurrencyType" },
                { "title": "手续费", "data": "Charge" },
                {
                    "title": "单笔限额(低)", "data": "MinLimit",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                },
                {
                    "title": "單筆限額(高)", "data": "MaxLimit",
                    "render": function (data, display, rowdata) {
                        return toCurrency(data);
                    }
                }
            ],
            rowCallback: function (row, data) { },
            filter: true,         //右上搜尋
            info: false,
            ordering: true, //排序
            processing: true,
            retrieve: true,
            paging: true,    //分頁
            scrollX: true,
            autoWidth: false,
            language: chineseTableLang
        });

        $('#table_CompanyWithdrawRelation tbody').on('click', 'tr', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                CompanyWithdrawRelationTable.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    }

    function wrapperFadeOut() {
        $(".page-loader-wrapper").fadeOut();
    }

    function wrapperFadeIn() {
        $(".page-loader-wrapper").fadeIn();
    }

    //新增幣別選項
    function disableCompanyService(companyID, ServiceType, currencyType) {
        var postObj = {
            CompanyID: companyID,
            ServiceType: ServiceType,
            CurrencyType: currencyType
        };
        wrapperFadeIn();
        c.callService(apiURL + "/DisableCompanyService", postObj, function (s, o) {
            if (s) {
                wrapperFadeOut();
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    alert("修改完成");
                    ShowCompanyService();
                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
                            break;
                        case 5:
                            //$('#divCompanyPointDetail').html('<div class="span6" style="float: none; margin: 0 auto;"><h6>尚未設定錢包</h6></div>');
                            break;
                        case 7:
                            alert("您已断线请重新登入");
                            break;
                        default:
                            alert("其他错误");
                            break;
                    }
                }
            } else {
                wrapperFadeOut();
                alert("其他错误");
            }
            $("#divCompanyPoint").show();
        });
    }

    function toCurrency(num) {

        num = parseFloat(Number(num).toFixed(2));
        var parts = num.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }
</script>
<body class="theme-cyan">
    <div class="block-header paper-wrap">
        <div class="row">
            <div class="col col-lg-3 col-md-3 col-sm-3">
                <h2>商户设定管理</h2>

            </div>
            <div class="col col-lg-9 col-md-9 col-sm-9 hideDiv hideDiv2">
                <div class="tips">
                </div>
            </div>
        </div>
        <div class="container-fluid">

            <div class="row">
                <div class="" id="divCompanyData" style="width:100%;">
                    <div id="divNoNCompany" class="card" style="display:none">
                        <div class="header">

                        </div>
                        <div class="body">
                            <div class="row clearfix" style="margin:0">
                                <div class="span6" style="float: none; margin: 0 auto;"><h6>尚未新增商户</h6></div>
                            </div>
                        </div>

                        <div class="footer">
                        </div>
                    </div>
                    <div class="divDetailData">
                        <div class="card">
                            <div class="header">
                                <div class="row">
                                    <div class="col-lg-7 col-md-7 col-sm-7">
                                        <h2 style="display:inline-block"><strong class="card_title">商户余额</strong></h2><i style="margin-left:5px" class="zmdi zmdi-refresh" onclick="ShowCompanyPoint()"></i>
                                    </div>
                                
                                </div>

                                <div class="row">
                                    <div class="col-lg-8 col-md-12 col-xs-12">
                                    
                                        <div id="divCompanyPoint" class="divPoint" style="padding:10px;display:inline-block">
                                            <div style="margin:0" id="divCompanyPointDetail">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="divCompanyService">
                                <div class="header companyServiceHeader">
                                    <div class="row">
                                        <div class="col-lg-7 col-md-12 col-sm-12">
                                            <h2><strong>商户渠道资料</strong></h2>
                                        </div>
                                        <div class="col-lg-5 col-md-12 col-sm-12 ortherText hideDiv">
                                            <button id="btnShowGPayRelation" type="button" class="btn btn-raised btn-primary btn-round waves-effect float-right">检视渠道设定</button>
                                            <button title="停用" id="btnCompanyServiceDisable" type="button" class="btn btn-raised btn-primary btn-circle waves-effect float-right" style="background: #FF6B6B;"><i class="fas fa-ban"></i></button>
                                            <button title="修改" id="btnCompanyServiceEdit" type="button" class="btn btn-raised btn-primary btn-circle waves-effect float-right"><i class="fas fa-pen"></i></button>
                                        </div>
                                    </div>
                                </div>
                                <div class="body">
                                    <div>
                                        <table class="table table-bordered table-striped table-hover dataTable nowrap" style="width:100%" id="table_CompanyServiceTable"></table>
                                    </div>
                                </div>
                                <div class="footer">
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-lg-12">
                            <div class="card">
                                <div class="header">
                                    <h2><strong>通道额度</strong></h2>
                                </div>
                                <div class="body">
                                    <div class="table-responsive" id="div_table_CompanyServicePointTable">
                                        <table style="width:100%" class="table table-bordered table-striped table-hover dataTable nowrap" id="table_CompanyServicePointTable"></table>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-lg-12 divDetailData">
                            <div class="card">
                                <div class="header">
                                    <div class="row">
                                        <div class="col-lg-7 col-md-7 col-sm-7">
                                            <h2><strong>下发/代付限额</strong></h2>
                                        </div>
                                        <div class="col-lg-5 col-md-5 col-sm-5 ortherText">

                                            <button title="修改" id="btnCompanyWithdrawLimitEdit" type="button" class="btn btn-raised btn-primary btn-circle waves-effect float-right"><i class="fas fa-pen"></i></button>
                                            <button title="新增" id="btnCompanyWithdrawLimitCreate" type="button" class="btn btn-raised btn-primary btn-circle waves-effect float-right"><i class="fas fa-plus"></i></button>
                                        </div>
                                    </div>
                                </div>
                                <div class="body">

                                    <div>
                                        <table style="width:100%" class="table table-bordered table-striped table-hover dataTable tt-table nowrap" id="table_ProviderWithdrawLimit">
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-lg-12 divDetailData">
                            <div class="card">
                                <div class="header">
                                    <div class="row">
                                        <div class="col-lg-7 col-md-7 col-sm-7">
                                            <h2><strong>自动代付設定</strong></h2>
                                        </div>
                                        <div class="col-lg-5 col-md-5 col-sm-5 ortherText">
                                            <button id="btnShowGPayWithdrawRelation" type="button" class="btn btn-raised btn-primary btn-round waves-effect float-right">检视渠道设定</button>
                                            <button title="修改" id="btnCompanyWithdrawRelationEdit" type="button" class="btn btn-raised btn-primary btn-circle waves-effect float-right"><i class="fas fa-pen"></i></button>
                                            <button title="新增" id="btnCompanyWithdrawRelationCreate" type="button" class="btn btn-raised btn-primary btn-circle waves-effect float-right"><i class="fas fa-plus"></i></button>
                                        </div>
                                    </div>
                                </div>
                                <div class="body">

                                    <div>
                                        <table style="width:100%" class="table table-bordered table-striped table-hover dataTable tt-table nowrap" id="table_CompanyWithdrawRelation">
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>


