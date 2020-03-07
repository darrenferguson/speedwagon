namespace SpeedWagon.Web.Extension
{
    public static class StringExtension
    {
        public static string MaskEmail(this string userName)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(userName);
                userName = userName.Substring(0, userName.IndexOf("@"));
                
            }
            catch
            {
                return userName;
            }

            return userName;
        }
    }
}
