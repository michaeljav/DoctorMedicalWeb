<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ReportViwerMaster.Master" AutoEventWireup="true" CodeBehind="RecetaGinecologica.aspx.cs" Inherits="DoctorMedicalWeb.Reports.CrystalViewer.RecetaGinecologica" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
      <%--<script lang="javaScript" type="text/javascript" src="crystalreportviewers13/js/crviewer/crv.js"></script> --%>
    <%--<link  href="crystalreportviewers13/js/crviewer/images/style.css" rel="stylesheet" />--%>
    <%--<link  href="<%=ResolveUrl("~/crystalreportviewers13/js/crviewer/images/style.css")%>" rel="stylesheet" />--%>
   <%--<link  href="http://localhost/DoctorMedicalWeb/Reports/CrystalViewer/crystalreportviewers13/js/crviewer/crv.js" rel="stylesheet" />--%>
      <%--<script  src="crystalreportviewers13/js/crviewer/crv.js"></script>--%>
    <%--<script type="text/javascript" src="<%= Page.ResolveClientURL("~/scripts/common.js")%>"></script>--%>
  <%--  <script  src="~/crviewer/crv.js"></script>--%>
  <%--  <script runat="server" type="text/javascript" 
  src='<%= Page.ResolveUrl("~/crv.js") %>'></script>--%>
    <%--<script src="<%=ResolveUrl("~/crviewer/crv.js") %>" type="text/javascript"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button value="Preview" Text="Preview" runat="server" ID="Preview" ValidationGroup="view" type="submit" OnClick="Preview_Click" Visible="false" />
    <input type="button" id="btnPrint" value="Imprimir" onclick="Print()" <%--onmouseup="document.getElementById('header').style.display='None';document.getElementById('footer').style.display='None'" --%>/>
    <div id="dvReport" >
        <CR:CrystalReportViewer ID="RecetaGinecologicaListCrystalReportViewer" runat="server" AutoDataBind="true" HasCrystalLogo="False"
            Height="50px" EnableParameterPrompt="false" EnableDatabaseLogonPrompt="false" ToolPanelWidth="200px"
            Width="350px" ToolPanelView="None" PageZoomFactor="80" HasDrillUpButton="False" HasPrintButton="False" HasSearchButton="False" ToolbarStyle-BorderStyle="None" />
    </div>
    <br />
   <%-- <style type="text/css" media="print">
    @page 
    {
        size: auto;   /* auto is the initial value */
        margin: 0mm;  /* this affects the margin in the printer settings */
    }
</style>--%>
    
    <script type="text/javascript">

   


        function Print() {
            //document.getElementById('header').style.display = 'none';
            //document.getElementById('footer').style.display = 'none';
            var dvReport = document.getElementById("dvReport");
            var frame1 = dvReport.getElementsByTagName("iframe")[0];
            if (navigator.appName.indexOf("Internet Explorer") != -1) {
                frame1.name = frame1.id;

                window.frames[frame1.id].focus();
                window.frames[frame1.id].print();
                
            }
            else {
                var frameDoc = frame1.contentWindow ? frame1.contentWindow : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;
                frameDoc.print();
            }

 
        }

        //document.getElementById('header').style.display = 'none';
        //document.getElementById('footer').style.display = 'none';
</script>
</asp:Content>
