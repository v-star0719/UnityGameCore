namespace GameCore.Core
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