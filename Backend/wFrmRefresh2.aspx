<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wFrmRefresh2.aspx.cs" Inherits="SkyPay.Backend.wFrmRefresh2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<script type="text/javascript" src="../Scripts/jquery-1.6.4.js"></script>
<script type="text/javascript" src="../Scripts/jquery.signalR-0.5.2.js"></script>
<script type="text/javascript" src="../signalr/hubs"></script>
<script src="../Scripts/jquery.url.js"></script>
</head>

 
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
    <script>
        $(function () {
             var Hub = $.connection.myHub;//大大們都說這裡 myHub 的 m 必須小寫 我也不知為什麼 照做就對了 不然會報錯            

            //開始連線
            $.connection.hub.start();
            //server 接收到 發言的時候會叫用的 client 端方法
            Hub.clientNoticeMessage = function (content) {
                console.log(content);
            }
        })
    </script>
</body>
  
</html>
