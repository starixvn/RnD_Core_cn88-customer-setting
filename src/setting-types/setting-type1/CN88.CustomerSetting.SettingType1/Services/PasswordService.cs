namespace CN88.CustomerSetting.SettingType1.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using CN88.CustomerSetting.Models.Results;
    using CN88.CustomerSetting.SettingType1.AppResources;

    public interface PasswordService
    {
        IResult ValidateAndHashPassword(string encryptedPassword, string encryptionKey, bool isMemberLevel);
    }

    public class PasswordServiceImpl : PasswordService
    {
        private const string HEX_DIGIT = "0123456789abcdef";
        private static readonly Regex PasswordRule1 = new Regex(@"^(?=.*\d).{8,100}$");
        private static readonly Regex PasswordRule2 = new Regex(@"^(?=.*[a-z]).{8,100}$");
        private static readonly Regex PasswordRule3 = new Regex(@"^(?=.*[A-Z]).{8,100}$");
        private static readonly Regex PasswordRule4 = new Regex(@"^(?=.*[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;,]).{8,100}$");
        private static readonly Regex MemberPasswordRule = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$");
        private static readonly Regex NoWhiteSpace = new Regex(@"^\S*$");

        public IResult ValidateAndHashPassword(string encryptedPassword, string encryptionKey, bool isMemberLevel)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                return Result.CreateErrorResult(Messages.PasswordCannotBeNullOrEmpty);
            }

            var rawPassword = Decrypt(encryptedPassword, encryptionKey);
            var validationResult = Validate(rawPassword, isMemberLevel);

            return validationResult.IsOk
                ? Result.CreateSuccessResult(CfsEnCode(rawPassword))
                : validationResult;
        }

        private Result Validate(
            string password,
            bool isMemberLevel)
        {
            if (!IsMatchedRules(password, isMemberLevel))
            {
                return Result.CreateErrorResult(Messages.PasswordRequire);
            }

            return Result.Success;
        }

        /// <summary>
        /// Password must be at least 8 characters without white space and contain at least 3 of the following:
        ///  • Uppercase letter [A-Z]
        ///  • Lowercase leader [a-z]
        ///  • Numeric [0-9]
        ///  • Special character (!,@,#,etc..)
        /// For example: 59D7!4$h
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool IsMatchedRules(string password, bool isResetMemberPassword)
        {
            if (isResetMemberPassword)
            {
                return MemberPasswordRule.Match(password).Success && NoWhiteSpace.Match(password).Success;
            }

            var r1 = PasswordRule1.Match(password).Success ? 1 : 0;
            var r2 = PasswordRule2.Match(password).Success ? 1 : 0;
            var r3 = PasswordRule3.Match(password).Success ? 1 : 0;
            var r4 = PasswordRule4.Match(password).Success ? 1 : 0;

            return (r1 + r2 + r3 + r4 >= 3) && NoWhiteSpace.Match(password).Success;
        }

        private string Decrypt(string password, string encryptKey)
            => RC4(encryptKey, HexDecode(password));

        public static string CfsEnCode(string codeStr)
        {
            var num = 30;
            var num2 = num - codeStr.Length;

            if (num2 > 1)
            {
                for (int m = 1; m <= num2; m++)
                {
                    codeStr += Convert.ToChar(0x15);
                }
            }

            var num3 = 1.0;

            for (int i = 1; i <= num; i++)
            {
                if (num <= codeStr.Length)
                {
                    var result = Convert.ToChar(codeStr.Substring(i - 1, 1));

                    int num5 = num + (result * i);
                    num3 *= num5;
                }
            }

            codeStr = num3.ToString();
            string str = string.Empty;
            for (int j = 1; j <= codeStr.Length; j++)
            {
                var length = 3;

                if (codeStr.Length - j < 2)
                {
                    length = codeStr.Length - j + 1;
                }

                str += CfsCode(codeStr.Substring(j - 1, length));
            }

            string str2 = string.Empty;
            for (int k = 20; k <= (str.Length - 0x12); k += 2)
            {
                str2 += str.Substring(k - 1, 1);
            }

            return str2;
        }

        private static string CfsCode(string value)
        {
            string number = string.Empty;

            for (int i = 1; i <= value.Length; i++)
            {
                number += (int)Convert.ToChar(value.Substring(i - 1, 1));
            }

            return (Convert.ToInt32(number)).ToString("X");
        }

        public static string RC4(string key, string value)
        {
            var ks = key.ToCharArray();
            var vs = value.ToCharArray();
            var c = key.Length;
            var d = value.Length;
            var s = new int[256];
            var i = 0;
            for (i = 0; i < 256; i++)
            {
                s[i] = i;
            }
            var j = 0;
            int x;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + ks[i % c]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
            }
            i = 0;
            j = 0;
            var ct = string.Empty;
            for (var y = 0; y < d; y++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
                ct += Convert.ToChar(vs[y] ^ s[(s[i] + s[j]) % 256]);
            }
            return ct;
        }

        public static string HexDecode(string data)
        {
            var b16_digits = HEX_DIGIT;
            var b16_map = new char[256];
            for (var i = 0; i < 256; i++)
            {
                b16_map[Convert.ToInt32(string.Concat(b16_digits[i >> 4], b16_digits[i & 15]), 16)] = Convert.ToChar(i);
            }
            var c = data.Length;
            if (c % 2 != 0) data = '0' + data;
            var result = new List<char>();

            for (var i = 0; i < c; i += 2)
            {
                result.Add(b16_map[Convert.ToInt32(data.Substring(i, 2), 16)]);
            }
            return new string(result.ToArray());
        }
    }
}