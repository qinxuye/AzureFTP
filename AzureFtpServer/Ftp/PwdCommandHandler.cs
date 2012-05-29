using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    internal class PwdCommandHandler : PwdCommandHandlerBase
    {
        public PwdCommandHandler(FtpConnectionObject connectionObject)
            : base("PWD", connectionObject)
        {
        }
    }
}