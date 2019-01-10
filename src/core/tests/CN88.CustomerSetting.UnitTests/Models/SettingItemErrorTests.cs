namespace CN88.CustomerSetting.Tests.Models
{
    using CN88.CustomerSetting.AppResources;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Constants;
    using CustomerSetting.Models.Results;
    using NUnit.Framework;

    public class SettingItemErrorTests
    {
        [Test]
        public void ErrorMessage_Always_ReturnSettingTypeErrorMessage()
        {
            var errorItem = new SettingItemError();

            Assert.AreEqual(Resources.SettingTypeError, errorItem.ErrorMessage);
        }

        [Test]
        public void TypeName_Always_ReturnExpectedName()
        {
            var errorItem = new SettingItemError();

            Assert.AreEqual(errorItem.GetType().AssemblyQualifiedName, errorItem.TypeName);
        }

        [Test]
        public void Validate_Always_ReturnFailedSettingResult()
        {
            //// Arrange
            var errorItem = new SettingItemError();
            IResult expected = new Result(ResultValues.Error, Resources.SettingTypeError);

            //// Act
            IResult settingResult = errorItem.Validate();

            //// Assert
            Assert.AreEqual(expected, settingResult);
        }

        [Test]
        public void ToXml_Always_ReturnExpectedString()
        {
            var errorItem = new SettingItemError();
            var expectedXml = $"<Error>{Resources.SettingTypeError}</Error>";

            Assert.AreEqual(expectedXml, errorItem.ToXml());
        }
    }
}