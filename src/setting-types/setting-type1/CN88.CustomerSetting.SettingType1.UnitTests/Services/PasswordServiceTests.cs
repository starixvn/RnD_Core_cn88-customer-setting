namespace CN88.CustomerSetting.SettingType1.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.SettingType1.Services;
    using NUnit.Framework;

    public class PasswordServiceTests
    {
        private PasswordServiceImpl passwordService = new PasswordServiceImpl();

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void ValidateAndHashPassword_EncryptedPasswordIsNullOrEmpty_ReturnFailedValidation(string encryptedPassword)
        {
            var failedResult = passwordService.ValidateAndHashPassword(encryptedPassword, "key", Level.Super);

            Assert.IsFalse(failedResult.IsOk);
        }

        [Test]
        public void ValidateAndHashPassword_SuperPasswordDoesNotMeetRule_ReturnFailedValidation()
        {
            var failedResult = passwordService.ValidateAndHashPassword("6708b1a58a", "w4yyuhre5d2xb4avwzgvxrye", Level.Super);

            Assert.IsFalse(failedResult.IsOk);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void ValidateAndHashPassword_PasswordIsValid_ReturnEncodedPassword(byte levelId)
        {
            var result = passwordService.ValidateAndHashPassword(
                "761aa2b1cb964258", 
                "w4yyuhre5d2xb4avwzgvxrye", 
                Enumeration.FromValue<Level>(levelId));

            // Password 1234aaAA
            Assert.AreEqual("C268E7827BABD7657E85A85811D281", result.Message);
        }

        [TestCase("", false)]
        public void IsMatchedRules_PasswordInvalid_ReturnFalse(string password, bool isMember)
        {
            Assert.IsFalse(passwordService.IsMatchedRules(password, isMember));
        }
    }
}
