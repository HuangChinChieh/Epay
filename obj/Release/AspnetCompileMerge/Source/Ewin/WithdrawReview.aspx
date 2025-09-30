<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WithdrawReview.aspx.cs" Inherits="WithdrawReview" %>
<%
    dynamic paymentReport;
    string paymentResult = "";

    string Timestamp = Request.Params["Timestamp"];
    string OrderID = Request.Params["OrderID"];
    string CompanyCode = Request.Params["CompanyCode"];
    string Sign = Request.Params["Sign"];
    int CompanyID = -1;
    Common.APIResult R = new Common.APIResult() { ResultState = Common.APIResult.enumResultCode.ERR };

    if (OrderID == null)
    {
        R.ResultState = Common.APIResult.enumResultCode.ERR;
        R.Message = "The parameter orderID not Exist";
        Response.Write(R.Message);
        Response.Flush();
        Response.End();
    }

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

    if (Common.CheckWithdrawReviewSign(CompanyCode, OrderID, Sign, Timestamp))
    {

        CompanyID = Common.GetCompanyKeyByCompanyID(CompanyCode);

        if (CompanyID == -1)
        {
            R.ResultState = Common.APIResult.enumResultCode.ERR;
            R.Message = "Company Code Error";
            Response.Write(R.Message);
            Response.Flush();
            Response.End();
        }

        paymentReport = Common.GetWithdrawalByOrderID(OrderID, CompanyID);
        if (paymentReport != null)
        {
            paymentResult = Newtonsoft.Json.JsonConvert.SerializeObject(paymentReport);
        }
        else
        {
            R.ResultState = Common.APIResult.enumResultCode.ERR;
            R.Message = "Payment Not Exist";
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

    <!-- Custom Css -->
    <link rel="stylesheet" href="/VPay/assets/css/main.css">
    <link rel="stylesheet" href="/VPay/assets/css/color_skins.css">
    <!-- Jquery Core Js -->
    <script src="/VPay/assets/js/jquery-3.3.1.min.js"></script>
    <script src="/VPay/assets/bundles/libscripts.bundle.js"></script> <!--Lib Scripts Plugin Js ( jquery.v3.2.1, Bootstrap4 js)-->



    <script src="/VPay/assets/js/BackendJS/BackendAPI.js?20200508"></script>
    <script src="/VPay/assets/js/AutoNumeric.js"></script>
    <script src="/Ewin/Common.js"></script>

    <style>
        ::placeholder { /* Chrome, Firefox, Opera, Safari 10.1+ */
            color: black !important;
            opacity: 1; /* Firefox */
        }

        .bootstrap-select {
            display: none;
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
    var paymentResult = '<%=paymentResult%>';
    var data;
    var apiURL = "/Ewin/WithdrawReview.aspx";
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
    var orderid = '<%=OrderID%>';
    var companyid = <%=CompanyID%>;
    $(function () {
        if (paymentResult != "") {
            data = JSON.parse(paymentResult);
            updateBaseInfo(data);
        }
    });

    function updateBaseInfo(data) {
        var ProcessStatus = "";
        $('#modal_ServiceType').text('');
        $('#modal_CompanyName').text(data.CompanyName);
        $('#modal_WithdrawSerial').text(data.WithdrawSerial);
        $('#modal_DownOrderID').text(data.DownOrderID);
        $('#modal_BankCard').text(data.BankCard);
        $('#modal_CurrencyType').text(data.CurrencyType);
        $('#modal_Amount').text(toCurrency(data.Amount));
        $('#modal_BankName').text(data.BankName);
        $('#modal_BankBranchName').text(data.BankBranchName);
        $('#modal_BankCardName').text(data.BankCardName);
        $('#modal_OwnCity').text(data.OwnCity);
        $('#modal_OwnProvince').text(data.OwnProvince);

        switch (data.Status) {
            case 0:
                ProcessStatus = '未处理';
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
        $('#modal_Status').html(ProcessStatus);
        if (data.Status == 0) {
            getProviderCodeResult(data.forCompanyID, data.ServiceType, data.CurrencyType);
            $('#span_select_providercode').hide();
        } else {
            $('#div_select_providercode').hide();
            $('#div_select_companyservicepoint').hide();
            $('#span_select_providercode').show();
            $('#modal_ProviderName').text(data.ProviderName);

            $('#modal-footer').hide();
        }

        $('#WithdrawalModal').modal('show');
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

    function getProviderCodeResult(companyid, ServiceType, currencyType) {

        parent.wrapperFadeIn();
        $('#modal_select_providercode').find('option').remove();
     
        postObj = {
            CompanyID: companyid,
            CurrencyType: currencyType
        }

        c.callService(apiURL + "/GetWithdrawDetailPointResult", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    var providerDataSort = o.ProviderPointResults.sort(function (a, b) {

                        if (a.ProviderCode > b.ProviderCode) {
                            return 1;
                        } else if (a.ProviderCode < b.ProviderCode) {
                            return -1;
                        }

                        return 0;
                    });

                    createProviderSelect(providerDataSort);
                    //checkCompanyServiceExist(o.CompanyServicePointResults, ServiceType, currencyType);
                    parent.wrapperFadeOut();
                }
                else {
                    //var message = ""
                    //switch (o.ResultCode) {
                    //    case 7:
                    //        alert("您已断线请重新登入");
                    //        break;
                    //    case 5:

                    //        break;
                    //    default:
                    //        message = "其他错误";
                    //        alert(message);
                    //        break;
                    //}
                    wrapperFadeOut();

                }

            } else {
                wrapperFadeOut();
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });

    }

    function createProviderSelect(providerdata) {

        if (providerdata) {
            $("#modal_select_providercode").append('<option value="-1">' + '选择供应商' + '</option>');

            $.each(providerdata, function (i, item) {
                var boolProviderAPIType = false;
                var ProviderAPIType = providerdata[i].ProviderAPIType;
                if ((ProviderAPIType & 2) == 2) {
                    boolProviderAPIType = true;
                }

                $("#modal_select_providercode").append('<option data-supportapi=' + boolProviderAPIType + ' value="' + providerdata[i].ProviderCode + '">' + providerdata[i].ProviderName + " / " + toCurrency(providerdata[i].SystemPointValue - providerdata[i].WithdrawPoint) + '</option>');
            })
        }

        $('#div_select_providercode').find("[data-supportapi=false]").each(function () {
            $(this).show();
        });
    }

    function saveReviewResult(modifyStatus) {
        var withdrawSerial = $('#modal_WithdrawSerial').text();
        var status = modifyStatus;
        var providerCode = $('#modal_select_providercode').val();
        var ServiceType = $('#modal_ServiceType').text().trim();
        if (modifyStatus != 3) {
            if (providerCode == "-1") {
                alert("尚未选择供应商");
                return;
            }
        }


        wrapperFadeIn();

        postObj = {
            Status: status,
            WithdrawSerial: withdrawSerial,
            ProviderCode: providerCode,
            ServiceType: ServiceType
        }
        wrapperFadeIn();
        c.callService(apiURL + "/UpdateWithdrawalResultByWithdrawSerial", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultState == 0) {
                    updateOrder();
                    alert("处理完成");
                }
                else {
                    alert(o.Message);
                    wrapperFadeOut();

                }

            } else {
                wrapperFadeOut();
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });

    }

    function updateOrder() {
        postObj = {
            OrderID: orderid,
            CompanyID: companyid
        }
 
        c.callService(apiURL + "/GetWithdrawalByOrderID", postObj, function (success, o) {
            if (success) {
                o = c.getJSON(o);
                if (o.ResultCode == 0) {
                    updateBaseInfo(o.data);
                }
                else {
                    alert(o.Message);
                    wrapperFadeOut();

                }

            } else {
                wrapperFadeOut();
                alert("网路错误:" + o);
            }

            wrapperFadeOut();
        });
    }

</script>
<body class="theme-violet">
     <div class="page-loader-wrapper" style="display:none;">
        <div class="loader">
            <div class="m-t-30"><img class="zmdi-hc-spin" src="/VPay/assets/images/logo_S_w.svg" width="48" height="48" alt="sQuare"></div>
            <p>Please wait...</p>
        </div>
    </div>

    <div class="modal fade" style="background-color: rgba(0, 0, 0, 0.4);" data-backdrop="false" id="WithdrawalModal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="title" id="WithdrawalModalModalLabel">出款审核</h4>
                </div>
                <div class="modal-body">
                    <span style="display: block">平台订单号:<span id="modal_WithdrawSerial" style="margin-left: 5px"></span></span>
                    <span style="display: block">商户订单号:<span id="modal_DownOrderID" style="margin-left: 5px"></span></span>
                    <span style="display: block">银行名称:<span id="modal_BankName" style="margin-left: 5px"></span></span>
                    <span style="display: block">分行名称:<span id="modal_BankBranchName" style="margin-left: 5px"></span></span>
                    <span style="display: block">银行卡号:<span id="modal_BankCard" style="margin-left: 5px"></span></span>
                    <span style="display: block">持卡人姓名:<span id="modal_BankCardName" style="margin-left: 5px"></span></span>
                    <span style="display: block">城市:<span id="modal_OwnCity" style="margin-left: 5px"></span></span>
                    <span style="display: block">省份:<span id="modal_OwnProvince" style="margin-left: 5px"></span></span>
                    <span style="display: block">币别:<span id="modal_CurrencyType" style="margin-left: 5px"></span></span>
                    <span style="display: block">金额:<span id="modal_Amount" style="margin-left: 5px"></span></span>
                    <span style="display: block">订单状态:<span id="modal_Status" style="margin-left: 5px"></span></span>
                    <div id="div_select_providercode"><span>渠道:
                        <select class="show-tick" id="modal_select_providercode"></select></span></div>
                        <span id="span_select_providercode" style="display: block">
                        渠道:<span id="modal_ProviderName" style="margin-left: 5px"></span></span>           
                    <span id="modal_ServiceType" style="display: none"></span>
                </div>
                <div class="modal-footer" id="modal-footer">
                    <button onclick="saveReviewResult(1)" type="button" class="btn btn-primary btn-round waves-effect">确认</button>
                    <button onclick="saveReviewResult(3)" type="button" style="background-color: black;" class="btn btn-primary btn-round waves-effect">失敗</button>
            </div>
        </div>
    </div>
</body>
</html>




