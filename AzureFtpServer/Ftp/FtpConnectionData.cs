using System.Net.Sockets;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.Ftp
{
    internal class FtpConnectionData
    {
        #region Member Variables

        private readonly int m_nId;
        private readonly TcpClient m_theSocket;
        private IFileSystem m_fileSystem;
        private int m_nPortCommandSocketPort = 20;
        private string m_sCurrentDirectory = "\\";
        private string m_sPortCommandSocketAddress = "";

        #endregion

        #region Construction

        public FtpConnectionData(int nId, TcpClient socket)
        {
            m_nId = nId;
            m_theSocket = socket;
        }

        #endregion

        #region Properties

     
        public TcpClient Socket
        {
            get { return m_theSocket; }
        }

        public string User { get; set; }

 
        public string CurrentDirectory
        {
            get { return m_sCurrentDirectory; }

            set { m_sCurrentDirectory = value; }
        }

        public int Id
        {
            get { return m_nId; }
        }

 
        public string PortCommandSocketAddress
        {
            get { return m_sPortCommandSocketAddress; }

            set { m_sPortCommandSocketAddress = value; }
        }

  
        public int PortCommandSocketPort
        {
            get { return m_nPortCommandSocketPort; }

            set { m_nPortCommandSocketPort = value; }
        }

 
        public bool BinaryMode { get; set; }

        public TcpClient PasvSocket { get; set; }

        public string FileToRename { get; set; }

    
        public bool RenameDirectory { get; set; }

        public IFileSystem FileSystemObject
        {
            get { return m_fileSystem; }
        }

        protected void SetFileSystemObject(IFileSystem fileSystem)
        {
            m_fileSystem = fileSystem;
        }

        #endregion
    }
}