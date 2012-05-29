namespace AzureFtpServer.Ftp
{
   
    public class FtpServerMessageHandler
    {
        #region Delegates

        public delegate void MessageEventHandler(int nId, string sMessage);

        #endregion

        protected FtpServerMessageHandler()
        {
        }

        public static event MessageEventHandler Message;

        public static void SendMessage(int nId, string sMessage)
        {
            if (Message != null)
            {
                Message(nId, sMessage);
            }
        }
    }
}