namespace CN88.CustomerSetting.Tests.Models
{
    using CN88.CustomerSetting.AppResources;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Constants;
    using CustomerSetting.Models.Results;
    using Xunit;

    public class SettingItemErrorTests
    {
        [Fact]
        public void ErrorMessage_Always_ReturnSettingTypeErrorMessage()
        {
            var errorItem = new SettingItemError();

            Assert.Equal(Resources.SettingTypeError, errorItem.ErrorMessage);
        }

        [Fact]
        public void TypeName_Always_ReturnExpectedName()
        {
            var errorItem = new SettingItemError();

            Assert.Equal(errorItem.GetType().AssemblyQualifiedName, errorItem.TypeName);
        }

        [Fact]
        public void Validate_Always_ReturnFailedSettingResult()
        {
            //// Arrange
            var errorItem = new SettingItemError();
            IResult expected = new Result(ResultValues.Error, Resources.SettingTypeError);

            //// Act
            IResult settingResult = errorItem.Validate();

            //// Assert
            Assert.Equal(expected, settingResult);
        }

        [Fact]
        public void ToXml_Always_ReturnExpectedString()
        {
            var errorItem = new SettingItemError();
            var expectedXml = $"<Error>{Resources.SettingTypeError}</Error>";

            Assert.Equal(expectedXml, errorItem.ToXml());
        }
    }
}