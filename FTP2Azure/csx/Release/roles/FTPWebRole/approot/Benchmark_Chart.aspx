<%@ Page Title="监测图" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Benchmark_Chart.aspx.cs" Inherits="FTPWebRole.Benchmark_Chart" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
  Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %> 
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />
           <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
<p>
    CPU Performance(%)
</p>
<p>
    <asp:Chart ID="Chart1" runat="server" DataSourceID="ObjectDataSource1" 
        Width="500px">
        <Series>
            <asp:Series Name="Series1" ChartType="Spline" XValueMember="TimeCur" 
                YValueMembers="CounterValue">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
    SelectMethod="GetProcessorTime" TypeName="FTPWebRole.queryDataProvider">
</asp:ObjectDataSource>
</p>
<p>
    Memory Performance(M available)</p>
<p>
    <asp:Chart ID="Chart2" runat="server" DataSourceID="ObjectDataSource2" 
        Width="500px">
        <Series>
            <asp:Series Name="Series1" ChartType="Spline" XValueMember="TimeCur" 
                YValueMembers="CounterValue">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
<asp:ObjectDataSource ID="ObjectDataSource2" runat="server" 
    SelectMethod="GetMemoryTime" TypeName="FTPWebRole.queryDataProvider">
</asp:ObjectDataSource>
</p>
<p>
    TCP fail(Number)</p>
<p>
    <asp:Chart ID="Chart3" runat="server" DataSourceID="ObjectDataSource3" 
        Width="500px">
        <Series>
            <asp:Series Name="Series1" ChartType="Spline" XValueMember="TimeCur" 
                YValueMembers="CounterValue">
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1">
            </asp:ChartArea>
        </ChartAreas>
    </asp:Chart>
<asp:ObjectDataSource ID="ObjectDataSource3" runat="server" 
    SelectMethod="GetTcpFail" TypeName="FTPWebRole.queryDataProvider">
</asp:ObjectDataSource>
</p>
<asp:Timer ID="Timer1" runat="server" Interval="30000">
</asp:Timer>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
