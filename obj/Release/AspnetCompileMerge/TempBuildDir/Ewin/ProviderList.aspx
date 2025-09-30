<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProviderList.aspx.cs" Inherits="ProviderList" %>

<%
    string paymentResult = "";
    int CompanyID = -1;
    string CompanyCode = Request.Params["CompanyCode"];
    string Sign = Request.Params["Sign"];
    string Timestamp = Request.Params["Timestamp"];
    string strListProviderListResult = "";
    List<Common.ProviderListResult> ListProviderListResult = null;
    List<Common.ServiceData> ServiceDatas = null;
    List<Common.ProviderPointVM> ProviderPoints = null;
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
            ListProviderListResult = Common.GetProviderListResult(CompanyID);

            if (ListProviderListResult != null)
            {
                ServiceDatas = Common.GetProviderListServiceData(CompanyID);
                ProviderPoints = Common.GetAllProviderPoint(CompanyID);

                foreach (var item in ListProviderListResult)
                {
                    if (ServiceDatas != null)
                    {
                        item.ServiceDatas = ServiceDatas.Where(w => w.ProviderCode == item.ProviderCode).ToList();
                    }

                    if (ProviderPoints != null)
                    {
                        item.ProviderListPoints = ProviderPoints.Where(w => w.ProviderCode == item.ProviderCode).ToList();
                    }
                }

                strListProviderListResult=Newtonsoft.Json.JsonConvert.SerializeObject(ListProviderListResult);
            }
            else
            {
                R.ResultState = Common.APIResult.enumResultCode.ERR;
                R.Message = "No Provider Data";
                         Response.Write(R.Message);
                    Response.Flush();
                    Response.End();
            }
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

        ::placeholder {
            color:gray !important;
        }
    </style>
</head>
<script>
    var strListProviderListResult = '<%=strListProviderListResult%>';
    var companyID = <%=CompanyID%>;
    var jsonListProviderListResult;
    var apiURL = "/Ewin/ProviderList.aspx";
    var isCreatedTable = false;
    var c = new common();
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
    var ProviderData;
    var isInit = false;
    var isInit2 = false;
    $(function () {
        if (strListProviderListResult != "") {
            jsonListProviderListResult = JSON.parse(strListProviderListResult);
            CreateProviderListTable(jsonListProviderListResult);
            ProviderData = jsonListProviderListResult;
        }

        var updateProviderPointModal = `<div class="modal fade" style="background-color: rgba(0, 0, 0, 0.4);" data-backdrop=false id="updateProviderPointModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog modal-sm" role="document">
                            <div class="modal-content">
                            <div class="modal-header">
                            <h4 class="title">渠道金额调整</h4>
                            </div>
                            <div class="modal-body" id="updateProviderPointModalBody">
                                <span style="display: block;margin-bottom:5px;">渠道:<span id="updateProviderPointModal_ProviderName" style="margin-left: 5px"></span></span>
                                <span style="display: block;margin-bottom:5px;">金額:<input class="thousand-symbols" placeholder="加負號(-)為扣除額度" id="updateProviderPointModal_Amount" style="margin-left: 5px"/></span>
                                <input id="updateProviderPointModal_ProviderCode" style="display:none;" />
                            </div>
                            <div class="modal-footer">
                            <button onclick="updateProviderPointModalSave()" type="button" class="btn btn-primary btn-round waves-effect">修改</button>
                            <button onclick="updateProviderPointModalCancel()" type="button" class="btn btn-secondary btn-round waves-effect">取消</button>
                            </div>
                            </div>
                            </div>
                            </div>`;
        $('body').append(updateProviderPointModal);

        var withdrawLimitModal = `<div class="modal fade" style="background-color: rgba(0, 0, 0, 0.4);" data-backdrop=false id="withdrawLimitModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog modal-sm" role="document">
                            <div class="modal-content">
                            <div class="modal-header">
                            <h4 class="title">代付金额调整</h4>
                            </div>
                            <div class="modal-body" id="withdrawLimitModalBody">
                                <span style="display: block;margin-bottom:5px;">渠道:<span id="withdrawLimitModal_ProviderName" style="margin-left: 5px"></span></span>
                                <span style="display: block;margin-bottom:5px;">最低金額:<input class="thousand-symbols" id="withdrawLimitModal_MinLimit" style="margin-left: 5px"/></span>
                                <span style="display: block;margin-bottom:5px;">最高金额:<input class="thousand-symbols" id="withdrawLimitModal_MaxLimit" style="margin-left: 5px"/></span>
                                <span style="display: block">手续费:<input class="thousand-symbols" id="withdrawLimitModal_Charge" style="margin-left: 5px"/></span>
                                <input id="withdrawLimitModal_ProviderCode" style="display:none;" />
                            </div>
                            <div class="modal-footer">
                            <button onclick="withdrawLimitModalSave()" type="button" class="btn btn-primary btn-round waves-effect">修改</button>
                            <button onclick="withdrawLimitModalCancel()" type="button" class="btn btn-secondary btn-round waves-effect">取消</button>
                            </div>
                            </div>
                            </div>
                            </div>`;
        $('body').append(withdrawLimitModal);


        var providerServiceModal = `<div class="modal fade" style="background-color: rgba(0, 0, 0, 0.4);" data-backdrop=false id="providerServiceModal" tabindex="-1" role="dialog" aria-hidden="true">
                            <div class="modal-dialog modal-sm" role="document">
                            <div class="modal-content">
                            <div class="modal-header">
                            <h4 class="title">代收金额调整</h4>
                            </div>
                            <div class="modal-body" id="providerServiceModalBody">
                                <span style="display: block;margin-bottom:5px;">渠道:<span id="providerServiceModal_ProviderName" style="margin-left: 5px"></span></span>
                                <span style="display: block;margin-bottom:5px;">支付通道:<span id="providerServiceModal_ServiceTypeName" style="margin-left: 5px"></span></span>
                
                                <span style="display: block">费率(%):<input class="percent" id="providerServiceModal_Charge" style="margin-left: 5px"/></span>
                                <input id="providerServiceModal_ProviderCode" style="display:none;" />
                                <input id="providerServiceModal_ServiceType" style="display:none;" />
                            </div>
                            <div class="modal-footer">
                            <button onclick="providerServiceModalSave()" type="button" class="btn btn-primary btn-round waves-effect">修改</button>
                            <button onclick="providerServiceModalCancel()" type="button" class="btn btn-secondary btn-round waves-effect">取消</button>
                            </div>
                            </div>
                            </div>
                            </div>`;
        $('body').append(providerServiceModal);


         AutoNumeric.multiple('.thousand-symbols', 'floatPos', { allowDecimalPadding: false, decimalPlaces: 4, modifyValueOnWheel: false });
         AutoNumeric.multiple('.percent', 'integer', { allowDecimalPadding: false, decimalPlaces: 4, minimumValue: 0, maximumValue: 100 });
    });

    function withdrawLimitModalSave() {

        wrapperFadeIn();
        var MinLimit = toNumber($('#withdrawLimitModal_MinLimit').val());
        var MaxLimit = toNumber($('#withdrawLimitModal_MaxLimit').val());
        var Charge = toNumber($('#withdrawLimitModal_Charge').val());
        var ProviderCode =  $('#withdrawLimitModal_ProviderCode').val();

        postObj = {
            ProviderCode: ProviderCode,
            CompanyID: companyID,
            MaxLimit: MaxLimit,
            MinLimit: MinLimit,
            Charge: Charge
        }

        c.callService(apiURL + "/UpdateProviderWithdrawLimitResult", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    withdrawLimitModalCancel();
                    updateProviderList();
                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
            }
        
            wrapperFadeOut();
        });
    }

    function providerServiceModalSave() {

        wrapperFadeIn();
        //var MinOnceAmount = toNumber($('#providerServiceModal_MinLimit').val());
        //var MaxOnceAmount = toNumber($('#providerServiceModal_MaxLimit').val());
        var CostRate = toNumber($('#providerServiceModal_Charge').val());
        var ProviderCode = $('#providerServiceModal_ProviderCode').val();
        var ServiceType = $('#providerServiceModal_ServiceType').val();
       
        postObj = {
            ProviderCode: ProviderCode,
            CompanyID: companyID,
            CostRate: CostRate,
            ServiceType: ServiceType
        }

        c.callService(apiURL + "/UpdateProviderServiceResult", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    providerServiceModalCancel();
                    updateProviderList();
                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });
    }

    function updateProviderPointModalSave() {

        wrapperFadeIn();
        var Amount = toNumber($('#updateProviderPointModal_Amount').val());
        var ProviderCode = $('#updateProviderPointModal_ProviderCode').val();

        postObj = {
            ProviderCode: ProviderCode,
            CompanyID: companyID,
            Amount: Amount,
            Description:""
        }

        c.callService(apiURL + "/InsertManualHistory", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    alert("更新成功");
                    updateProviderPointModalCancel();
                    updateProviderList();
                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });
    }

    function toNumber(num) {
        var returnNum = Number(num.trim().replace(/,/g, ""));
        return returnNum;
    }

    function withdrawLimitModalCancel() {
        $('#withdrawLimitModal').modal('hide');
    }

    function updateProviderPointModalCancel(){
        $('#updateProviderPointModal').modal('hide');
    }

    function providerServiceModalCancel() {
        $('#providerServiceModal').modal('hide');
    }

    function CreateProviderListTable(data) {
        isCreatedTable = true;
        ProviderServiceTable = $('#table_ProviderList').DataTable({
            data: data,
            columns: [
                {
                    "title": "", "data": "ProviderState", "visible": false
                },
                {
                    "title": "供应商名称", "data": "",
                    "render": function (data, display, rowdata) {
                        var numProviderAPIType = parseInt(rowdata.ProviderAPIType);

                        var retValue = "";
                        retValue +=
                            `<div class='row'><div class="col-sm-4" style="padding-top: 10px;">${rowdata.ProviderName}&nbsp;&nbsp</div>
                             <div class="col-sm"><input type="checkbox" onclick="changeProviderCodeState('${rowdata.ProviderCode}')" ${rowdata.ProviderState == 0 ? checked = "checked" : checked = ""}/><text>全部功能</text></div>
                             <div class="col-sm"><input type="checkbox" onclick="changeProviderAPIType('${rowdata.ProviderCode}',1)" ${(numProviderAPIType & 1) == 1 ? checked = "checked" : checked = ""}/><text>充值</text></br>
                             <input type="checkbox" onclick="changeProviderAPIType('${rowdata.ProviderCode}',2)" ${(numProviderAPIType & 2) == 2 ? checked = "checked" : checked = ""}/><text>提款</text></div>`;


                        return retValue;
                    }
                },
                {
                    "title": "可提取金额范围", "data": "",
                    "render": function (data, display, rowdata) {
                        let retValue = "";

                        retValue = `<div style="padding-bottom:10px;">
                                     最&nbsp;&nbsp;低：${toCurrency(rowdata.MinLimit)}</br>最&nbsp;&nbsp;高：${toCurrency(rowdata.MaxLimit)}</br>手续费：${rowdata.Charge} 元 
                                      <button style="margin-left:5px;" onclick="showWithdrawLimitModal('${rowdata.ProviderCode}','${rowdata.ProviderName}')">调整</button></div>`;

                        return retValue;
                    }
                },
                {
                    "title": "支付方式", "data": "",
                    "render": function (data, display, rowdata) {
                        let retValue = "";
                        if (rowdata.ServiceDatas != null) {
                            for (var i = 0; i < rowdata.ServiceDatas.length; i++) {
                                let data = rowdata.ServiceDatas[i];
                              
                                retValue +=
                                    `<div style="padding-bottom:10px;">
                                     <div style='display: inline-block;'><input type="checkbox" onclick="changeProviderServiceState('${data.ProviderCode}','${data.ServiceType}','${data.CurrencyType}')" ${data.State == 0 ? checked = "checked" : checked = ""}/></div>
                                     <div style='width:100px;display: inline-block;'>${data.ServiceTypeName}</div>       
                                     <div style='width:100px;display: inline-block;'>費率 :${data.CostRate}% </div><button style="margin-left:5px;" onclick="showProviderServiceModal('${data.ProviderCode}','${data.ServiceType}','${data.ServiceTypeName}','${rowdata.ProviderName}')">調整</button></div>
                                     `;
                            }
                            retValue = '<table><tbody>' + retValue + '</tbody></table>';
                        }
                        return retValue;
                    }
                },
                {
                    "title": "帐户余额", "data": "",
                    "render": function (data, display, rowdata) {
                        let retValue = "";
                        if (rowdata.ProviderListPoints != null) {
                            for (var i = 0; i < rowdata.ProviderListPoints.length; i++) {
                                let data = rowdata.ProviderListPoints[i];
                                retValue += `<span style="color:green">可用额度：${toCurrency(data.SystemPointValue - data.WithdrawPoint)} </span></br><span style="color:blue">冻结：${toCurrency(data.ProviderFrozenAmount)} </span></br><span style="color:red">提领中：${toCurrency(data.WithdrawPoint)} </span> <button style="margin-left:5px;" onclick="showUpdateProviderPointModal('${rowdata.ProviderCode}','${rowdata.ProviderName}')">額度调整</button>`;
                            }
                        }
                        return retValue;
                    }
                }
                //{
                //    "title": "提供功能", "data": "ProviderAPIType",
                //    "render": function (data, display, rowdata) {
                //        let retValue = "";
                //        let aa = checkedProviderAPIType(data);
                //        let bb = aa.split(',');
                //        for (var i = 0; i < bb.length; i++) {
                //            retValue += bb[i] + "</br>";
                //        }
                //        return retValue;
                //    }
                //}
            ],
            rowCallback: function (row, data) { },
            filter: true,         //右上搜尋
            info: false,
            ordering: true, //排序
            processing: true,
            retrieve: true,
            order: [[1, "asc"]],
            paging: true,    //分頁
            scrollX: true,
            autoWidth: false,
            language: chineseTableLang
        });
    }

    function showWithdrawLimitModal(providerCode,providerName) {
        var providerdata = ProviderData.find(w => w.ProviderCode == providerCode);
        if (providerdata) {
            $('#withdrawLimitModal_ProviderName').text(providerName);
            $('#withdrawLimitModal_MinLimit').val(providerdata.MinLimit);
            $('#withdrawLimitModal_MaxLimit').val(providerdata.MaxLimit);
            $('#withdrawLimitModal_Charge').val(providerdata.Charge);
            $('#withdrawLimitModal_ProviderCode').val(providerdata.ProviderCode);
            $('#withdrawLimitModal').modal('show');
        }

    }

    function showUpdateProviderPointModal(providerCode,providerName) {
        var providerdata = ProviderData.find(w => w.ProviderCode == providerCode);
        if (providerdata) {
   
                $('#updateProviderPointModal_ProviderName').text(providerName);
                $('#updateProviderPointModal_ProviderCode').val(providerdata.ProviderCode);
                $('#updateProviderPointModal_Amount').val('');
                $('#updateProviderPointModal').modal('show');
        }

    }

    function showProviderServiceModal(providerCode, ServiceType, ServiceTypeName, providerName) {
        var providerdata = ProviderData.find(w => w.ProviderCode == providerCode);
        if (providerdata) {
            var serviceData = providerdata.ServiceDatas.find(w => w.ServiceType == ServiceType);

            if (serviceData) {
           
                $('#providerServiceModal_ProviderName').text(providerName);
                $('#providerServiceModal_ServiceTypeName').text(ServiceTypeName);
                //$('#providerServiceModal_MinLimit').val(serviceData.MinOnceAmount);
               // AutoNumeric.getAutoNumericElement('#providerServiceModal_MinLimit').set(serviceData.MinOnceAmount);
              //  $('#providerServiceModal_MaxLimit').val(serviceData.MaxOnceAmount);
               // AutoNumeric.getAutoNumericElement('#providerServiceModal_MaxLimit').set(serviceData.MaxOnceAmount);
                //$('#providerServiceModal_Charge').val(serviceData.CostRate);
                AutoNumeric.getAutoNumericElement('#providerServiceModal_Charge').set(serviceData.CostRate);
                $('#providerServiceModal_ProviderCode').val(providerdata.ProviderCode);
                $('#providerServiceModal_ServiceType').val(serviceData.ServiceType);
                $('#providerServiceModal').modal('show');
            }
            
        }

    }

    function updateProviderList(){
        postObj = {
            CompanyID: companyID
        }
        wrapperFadeIn();
        c.callService(apiURL + "/GetProviderListResult", postObj, function (success, o) {
            if (success) {
                if (isCreatedTable) {
                    ProviderServiceTable.clear().draw();
                }
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    ProviderData = o.ProviderListResult;
                    if (!isCreatedTable) {
                        $("#div_table_ProviderList").empty();
                        $("#div_table_ProviderList").append('<table style="width:100%" class="table table-bordered table-striped table-hover dataTable tt-table nowrap" id="table_ProviderList"></table >');
                        CreateProviderListTable(o.ProviderListResult);   
                    } else {
                        ProviderServiceTable.rows.add(o.ProviderListResult).draw(); // Add new data
                    }
                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
                wrapperFadeOut();
            }
            wrapperFadeOut();
        });
    }

    function changeProviderServiceState(providerCode, ServiceType, currencyType) {
        wrapperFadeIn();
  
        postObj = {
            ServiceType: ServiceType,
            CurrencyType: currencyType,
            ProviderCode: providerCode
        }

        c.callService(apiURL + "/ChangeProviderServiceState", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {

                }
                else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
            }
            wrapperFadeOut();
        });
    }

    function changeProviderCodeState(providerCode) {
        var MinLimit = $('#withdrawLimitModal_MinLimit').val();
        var MaxLimit = $('#withdrawLimitModal_MaxLimit').val();
        var Charge = $('#withdrawLimitModal_Charge').val();
        var ProviderCode = $('#withdrawLimitModal_ProviderCode').val();

        wrapperFadeIn();

        postObj = {
            ProviderCode: providerCode,
            Charge: Charge,
            MaxLimit: MaxLimit,
            MinLimit: MinLimit
        }

        c.callService(apiURL + "/ChangeProviderCodeState", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {

                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });

    }

    function changeProviderAPIType(providerCode, setAPIType) {
        parent.wrapperFadeIn();
        postObj = {
            ProviderCode: providerCode,
            setAPIType: setAPIType
        }

        c.callService(apiURL + "/ChangeProviderAPIType", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {

                } else {
                    switch (o.ResultCode) {
                        case 4:
                            alert("权限不足");
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
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });
    }

    function wrapperFadeOut() {
        $(".page-loader-wrapper").fadeOut();
    }

    function wrapperFadeIn() {
        $(".page-loader-wrapper").fadeIn();
    }

    function toCurrency(num) {

        num = parseFloat(Number(num).toFixed(2));
        var parts = num.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }

</script>
<body class="theme-cyan">

      <!-- Page Loader -->
    <div class="page-loader-wrapper" style="display:none;">
        <div class="loader">
            <div class="m-t-30"><img class="zmdi-hc-spin" src="/VPay/assets/images/logo_S_w.svg" width="48" height="48" alt="sQuare"></div>
            <p>Please wait...</p>
        </div>
    </div>

    <div class="block-header paper-wrap">
        <div class="row">
            <div class="col col-lg-3 col-md-3 col-sm-3">
                <h2>渠道列表</h2>
            </div>
            <div class="col col-lg-9 col-md-9 col-sm-9 hideDiv">
                <div class="tips">
                    <span></span>
                </div>
            </div>
        </div>
    </div>

    <div class="container-fluid">
        <div class="row clearfix">
            <div class="col-lg-12 divDetailData">
                <div class="card">
                    <div class="header">
                        <div class="row">
                            <div class="col-lg-7 col-md-7 col-sm-7">
                                <h2><strong></strong></h2>
                            </div>
             
                        </div>
                    </div>
                    <div class="body">
                        <div id="div_table_ProviderList">
                            <table style="width:100%" class="table table-bordered table-striped table-hover dataTable tt-table nowrap" id="table_ProviderList">
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>


