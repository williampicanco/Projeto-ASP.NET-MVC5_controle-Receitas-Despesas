<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mvc_App_Crud_ControleReceitasDespesas.Views.Shared.WebForm1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>relatório</title>
    <div>
        <script runat="server">
            public void Page_Load(object sender, System.EventArgs e)
            {
                ReportViewer1.LocalReport.ReportPath = ""; //("../Report/RelatorioDespesa.rdlc");   
            }
        </script>    
    </div>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" height="500" width="500" AsyncRendering="false"></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
