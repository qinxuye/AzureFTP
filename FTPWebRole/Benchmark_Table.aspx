<%@ Page Title="检测表" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Benchmark_Table.aspx.cs" Inherits="FTPWebRole.WebForm1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
  Namespace="System.Web.UI" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
           <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
    <p>
        CPU Performance(%)
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            DataSourceID="ObjectDataSource1">
            <Columns>
                <asp:BoundField DataField="TimeCur" HeaderText="TimeCur" 
                    SortExpression="TimeCur" />
                <asp:BoundField DataField="DeploymentId" HeaderText="DeploymentId" 
                    SortExpression="DeploymentId" />
                <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" />
                <asp:BoundField DataField="RoleInstance" HeaderText="RoleInstance" 
                    SortExpression="RoleInstance" />
                <asp:BoundField DataField="CounterValue" HeaderText="CounterValue" 
                    SortExpression="CounterValue" />
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            SelectMethod="GetProcessorTime" TypeName="FTPWebRole.queryDataProvider">
        </asp:ObjectDataSource>
    </p>
    <p>
        Memory Performance(M available)
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
            DataSourceID="ObjectDataSource2">
            <Columns>
                <asp:BoundField DataField="TimeCur" HeaderText="TimeCur" 
                    SortExpression="TimeCur" />
                <asp:BoundField DataField="DeploymentId" HeaderText="DeploymentId" 
                    SortExpression="DeploymentId" />
                <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" />
                <asp:BoundField DataField="RoleInstance" HeaderText="RoleInstance" 
                    SortExpression="RoleInstance" />
                <asp:BoundField DataField="CounterValue" HeaderText="CounterValue" 
                    SortExpression="CounterValue" />
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" 
            SelectMethod="GetMemoryTime" TypeName="FTPWebRole.queryDataProvider">
        </asp:ObjectDataSource>
    </p>
    <p>
        TCP fail(Number)
        <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" 
            DataSourceID="ObjectDataSource3">
            <Columns>
                <asp:BoundField DataField="TimeCur" HeaderText="TimeCur" 
                    SortExpression="TimeCur" />
                <asp:BoundField DataField="DeploymentId" HeaderText="DeploymentId" 
                    SortExpression="DeploymentId" />
                <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" />
                <asp:BoundField DataField="RoleInstance" HeaderText="RoleInstance" 
                    SortExpression="RoleInstance" />
                <asp:BoundField DataField="CounterValue" HeaderText="CounterValue" 
                    SortExpression="CounterValue" />
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="ObjectDataSource3" runat="server" 
            SelectMethod="GetTcpFail" TypeName="FTPWebRole.queryDataProvider">
        </asp:ObjectDataSource>
    </p>
    <asp:Timer ID="Timer1" runat="server" Interval="30000">
</asp:Timer>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
