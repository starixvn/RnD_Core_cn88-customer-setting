namespace CN88.CustomerSetting.SettingType1.UnitTests.Services
{
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.SettingType1.Services;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
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
            var failedResult = passwordService.ValidateAndHashPassword("Y9gQnGWQnTO1O3yS2o+spg==", "v2e4", Level.Super);

            Assert.IsFalse(failedResult.IsOk);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void ValidateAndHashPassword_PasswordIsValid_ReturnEncodedPassword(byte levelId)
        {
            var result = passwordService.ValidateAndHashPassword(
                "GngmtfNnjleecX2dhZqqHg==",
                "v2e4",
                Enumeration.FromValue<Level>(levelId));

            // Password 1234aaAA
            Assert.AreEqual("C268E7827BABD7657E85A85811D281", result.Message);
        }

        [TestCase("", false)]
        [TestCase("1234aa", false)]
        [TestCase("123456", false)]
        [TestCase("fdghhj", false)]
        [TestCase("@#@#@#@#@$$#", false)]
        [TestCase("ĐFFSDFDSFSDSFD", false)]
        public void IsMatchedRules_PasswordInvalid_ReturnFalse(string password, bool isMember)
        {
            Assert.IsFalse(PasswordServiceImpl.IsMatchedRules(password, isMember));
        }
    }
}