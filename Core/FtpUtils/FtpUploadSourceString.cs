namespace GameCore.Core.Ftp
{
    public class FtpUploadSourceString : FtpUploadSourceBytpes
    {
        public string text;

        public FtpUploadSourceString(string text)
        {
            this.text = text;
            bytes = System.Text.Encoding.UTF8.GetBytes(text);
        }
    }
}