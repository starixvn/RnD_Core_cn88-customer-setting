namespace CN88.CustomerSetting.UnitTests.SpParams.Commands
{
    using CN88.CustomerSetting.SpParams.Commands;
    using NUnit.Framework;

    [TestFixture(Category = "Core")]
    public class CheckValidUsernameCommandTests
    {
        [TestCase("")]
        [TestCase(null)]
        public void IsValid_UsernameIsEmtpy_ReturnFalse(string username)
        {
            //// Arrange
            var criteria = CreateCheckValidUsernameCommandFakeObject(username);

            //// Act
            var actualResult = criteria.IsValid();

            //// Arrange
            Assert.IsFalse(actualResult);
        }

        [TestCase("username")]
        [TestCase("username_1")]
        public void IsValid_ProductIdIsGreaterThanZero_ReturnTrue(string username)
        {
            //// Arrange
            var criteria = CreateCheckValidUsernameCommandFakeObject(username);

            //// Act
            var actualResult = criteria.IsValid();

            //// Arrange
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void GetSettingKey_ReturnSettingKey()
        {
            //// Arrange
            var criteria = CreateCheckValidUsernameCommandFakeObject("username");

            //// Act
            var actualSettingKey = criteria.GetSettingKey();

            //// Arrange
            Assert.AreEqual("Age_CustomerCheckExists", actualSettingKey);
        }

        private CheckValidUsernameCommand CreateCheckValidUsernameCommandFakeObject(string username)
            => new CheckValidUsernameCommand(username);
    }
}