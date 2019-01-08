namespace CN88.CustomerSetting.Tests.Models
{
    using CustomerSetting.Models;
    using Xunit;

    public class SettingItemDefaultTests
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(1, 0)]
        public void Validate_SportTypeOrBetTypeIsNegativeOrZero_ReturnErrorResult(
            short sportTypeId,
            short betTypeId)
        {
            //// Arrange
            var settingItem = new SettingItemDefault
            {
                SportTypeId = sportTypeId,
                BetTypeId = betTypeId
            };

            //// Act
            var actualResult = settingItem.Validate();

            //// Assert
            Assert.False(actualResult.IsSuccessful);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(10, 1)]
        public void Validate_DataIsValid_ReturnSuccessResult(
            short sportTypeId,
            short betTypeId)
        {
            //// Arrange
            var settingItem = new SettingItemDefault
            {
                SportTypeId = sportTypeId,
                BetTypeId = betTypeId
            };

            //// Act
            var actualResult = settingItem.Validate();

            //// Assert
            Assert.True(actualResult.IsSuccessful);
        }

        [Theory]
        [InlineData(1, 1, "SportType=\"1\" BetType=\"1\"")]
        [InlineData(10, 1, "SportType=\"10\" BetType=\"1\"")]
        [InlineData(1, 10, "SportType=\"1\" BetType=\"10\"")]
        public void ToXml_ReturnFormattedXml(
            short sportTypeId,
            short betTypeId,
            string expectedXml)
        {
            //// Arrange
            var settingItem = new SettingItemDefault
            {
                SportTypeId = sportTypeId,
                BetTypeId = betTypeId
            };

            //// Act
            var actualXml = settingItem.ToXml();

            //// Assert
            Assert.Equal(expectedXml, actualXml);
        }

        [Fact]
        public void TypeName_Always_ReturnsExpectedValue()
        {
            //// Arrange
            var settingItem = new SettingItemDefault();

            //// Assert
            Assert.Equal(settingItem.GetType().AssemblyQualifiedName, settingItem.TypeName);
        }
    }
}