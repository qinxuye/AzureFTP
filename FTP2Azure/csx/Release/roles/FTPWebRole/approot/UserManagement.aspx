<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="FTPWebRole.UserManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        CellPadding="4" DataSourceID="FtpAccountsDataSource" ForeColor="#333333" 
        GridLines="None" Width="736px" DataKeyNames="Username">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:BoundField HeaderText="用户名" ReadOnly="True" DataField="Username" 
                SortExpression="Username" />
            <asp:BoundField HeaderText="创建日期" ReadOnly="True" DataField="CreateDate" 
                SortExpression="CreateDate" />
            <asp:CheckBoxField DataField="IsSuperUser" HeaderText="是否超级用户" 
                SortExpression="IsSuperUser" />
            <asp:CheckBoxField HeaderText="是否活跃" DataField="IsActive" 
                SortExpression="IsActive" />
            <asp:CommandField ShowEditButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </asp:GridView>
    <asp:ObjectDataSource ID="FtpAccountsDataSource" runat="server" 
        DataObjectTypeName="FTPWebRole.FtpAccount" DeleteMethod="DeleteFtpAccount" 
        SelectMethod="GetAllFtpAccount" TypeName="FTPWebRole.UserManagementProvider" 
        UpdateMethod="UpdateFtpAccount">
    </asp:ObjectDataSource>
</asp:Content>
