using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{

    internal class XPwdCommandHandler : PwdCommandHandlerBase
    {
        public XPwdCommandHandler(FtpConnectionObject connectionObject)
            : base("XPWD", connectionObject)
        {
        }
    }
}