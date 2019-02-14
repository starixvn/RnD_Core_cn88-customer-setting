namespace CN88.CustomerSetting.Tests.Models
{
    using CustomerSetting.Models;
    using NUnit.Framework;

    [TestFixture(Category = "Core")]
    public class SettingItemDefaultTests
    {
        
        [TestCase(0, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(1, 0)]
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

        
        [TestCase(1, 10)]
        [TestCase(10, 1)]
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

        
        [TestCase(1, 1, false, "SportType=\"1\" BetType=\"1\" IsLive=\"0\"")]
        [TestCase(10, 1, false, "SportType=\"10\" BetType=\"1\" IsLive=\"0\"")]
        [TestCase(1, 10, false, "SportType=\"1\" BetType=\"10\" IsLive=\"0\"")]
        [TestCase(1, 1, true, "SportType=\"1\" BetType=\"1\" IsLive=\"1\"")]
        [TestCase(10, 1, true, "SportType=\"10\" BetType=\"1\" IsLive=\"1\"")]
        [TestCase(1, 10, true, "SportType=\"1\" BetType=\"10\" IsLive=\"1\"")]
        public void ToXml_ReturnFormattedXml(
            short sportTypeId,
            short betTypeId,
            bool isLive,
            string expectedXml)
        {
            //// Arrange
            var settingItem = new SettingItemDefault
            {
                SportTypeId = sportTypeId,
                BetTypeId = betTypeId,
                IsLive = isLive
            };

            //// Act
            var actualXml = settingItem.ToXml();

            //// Assert
            Assert.AreEqual(expectedXml, actualXml);
        }

        [Test]
        public void TypeName_Always_ReturnsExpectedValue()
        {
            //// Arrange
            var settingItem = new SettingItemDefault();

            //// Assert
            Assert.AreEqual(settingItem.GetType().AssemblyQualifiedName, settingItem.TypeName);
        }
    }
}