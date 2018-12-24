namespace CN88.CustomerSetting.Client.Helpers
{
    using System;

    public static class Encryptor
    {
        public static string RandomNumberString(int length)
        {
            var chars = "0123456789";
            var rndStr = string.Empty;
            var rnd = new Random();

            for (var i = 0; i < length; ++i)
            {
                rndStr += chars[rnd.Next() % chars.Length];
            }
            return rndStr;
        }
    }
}