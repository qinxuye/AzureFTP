<%@ Page Title="注册" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="FTPWebRole.Account.Register" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="RegisterUser" runat="server">
         <span class="failureNotification">
            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
        </span>

        <div class="accountInfo">
            <fieldset class="register">
                <legend>帐户信息</legend>
                <p>
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">用户名:</asp:Label>
                    <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                            CssClass="failureNotification" ErrorMessage="必须填写“用户名”。" ToolTip="必须填写“用户名”。" 
                            ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">密码:</asp:Label>
                    <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                            CssClass="failureNotification" ErrorMessage="必须填写“密码”。" ToolTip="必须填写“密码”。" 
                            ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">确认密码:</asp:Label>
                    <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="ConfirmPassword" CssClass="failureNotification" Display="Dynamic" 
                            ErrorMessage="必须填写“确认密码”。" ID="ConfirmPasswordRequired" runat="server" 
                            ToolTip="必须填写“确认密码”。" ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                            CssClass="failureNotification" Display="Dynamic" ErrorMessage="“密码”和“确认密码”必须匹配。"
                            ValidationGroup="RegisterUserValidationGroup">*</asp:CompareValidator>
                </p>
            </fieldset>
            <p class="submitButton">
                <asp:Button ID="CreateUserButton" runat="server" CommandName="MoveNext" Text="创建用户" 
                        ValidationGroup="RegisterUserValidationGroup" 
                    onclick="RegisterUser_CreatedUser"/>
            </p>
        </div>
    </asp:Panel>

</asp:Content>
