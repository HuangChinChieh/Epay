<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net.Http" %>
<%@ Import Namespace="System.Configuration" %>
<%
    string apiUrl = ConfigurationManager.AppSettings["ApiUrl"] ?? "(ApiUrl 未設定)";
    string echoString = "HelloTest123";
    string url = apiUrl + "/Gate/HeartBeat?EchoString=" + Uri.EscapeDataString(echoString);
    string result = "";
    string error = "";

    try {
        using (var client = new HttpClient()) {
            client.Timeout = TimeSpan.FromSeconds(10);
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            var resp = client.SendAsync(req).GetAwaiter().GetResult();
            result = "HTTP " + (int)resp.StatusCode + " " + resp.StatusCode + "<br/>" +
                     Server.HtmlEncode(resp.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }
    } catch (Exception ex) {
        var sb = new System.Text.StringBuilder();
        var e2 = ex;
        int depth = 0;
        while (e2 != null && depth < 5) {
            sb.AppendFormat("[{0}] {1}: {2}<br/>", depth, Server.HtmlEncode(e2.GetType().Name), Server.HtmlEncode(e2.Message));
            e2 = e2.InnerException;
            depth++;
        }
        error = sb.ToString();
    }
%>
<b>ApiUrl：</b><%=apiUrl%><br />
<b>呼叫 URL：</b><%=url%><br />
<b>回傳：</b><%=result%><br />
<% if (!string.IsNullOrEmpty(error)) { %><b style="color:red">錯誤：</b><br/><%=error%><% } %>
