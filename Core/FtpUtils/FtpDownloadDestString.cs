namespace GameCore.Core
{
    public class FtpDownloadDestString : FtpDownloadDestBytes
    {
        public string text;

        public FtpDownloadDestString()
        {
        }

        public override void Close()
        {
            base.Close();
            text = System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}