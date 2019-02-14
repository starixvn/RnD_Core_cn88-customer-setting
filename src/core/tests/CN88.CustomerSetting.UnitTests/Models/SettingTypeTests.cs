namespace CN88.CustomerSetting.Tests.Models
{
    using System.Collections.Generic;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Constants;
    using CustomerSetting.Models.Enumerations;
    using CustomerSetting.Models.Results;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture(Category = "Core")]
    public class SettingTypeTests
    {
        [Test]
        public void SettingItemsProperty_SetNullValueThenGet_ReturnEmptySettingItem()
        {
            //// Arrange
            var settingType = new SettingType();

            //// Act
            settingType.SettingItems = null;

            //// Assert
            Assert.IsEmpty(settingType.SettingItems);
        }

        [Test]
        public void TypeName_Always_ReturnExpectedValue()
        {
            var settingType = new SettingType();

            Assert.AreEqual(settingType.GetType().AssemblyQualifiedName, settingType.TypeName);
        }

        [Test]
        public void ToXml_Always_ReturnFormattedXml()
        {
            //// Arrange
            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem>
                {
                    new SettingItemStub()
                }
            };

            //// Act
            var actualXmlFormatted = settingType.ToXml();

            //// Assert
            Assert.AreEqual("<ROOT>SettingItemStub</ROOT>", actualXmlFormatted);
        }

        [Test]
        public void Validate_SettingTypeHasNoSettingItem_ReturnNoChangeResult()
        {
            //// Arrange
            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem>()
            };

            //// Act
            var actualResult = settingType.Validate();

            //// Assert
            Assert.AreEqual(ResultValues.NoChange, actualResult.Code);
        }

        [Test]
        public void Validate_SettingItemIsNotValid_ReturnErrorResult()
        {
            //// Arrange
            var invalidSettingItem = Substitute.For<ISettingItem>();
            invalidSettingItem.Validate().Returns(Result.DataError);
            var validSettingItem = Substitute.For<ISettingItem>();
            validSettingItem.Validate().Returns(Result.Success);
            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem>
                {
                    invalidSettingItem,
                    validSettingItem
                }
            };

            //// Act
            var actualResult = settingType.Validate();

            //// Assert
            Assert.False(actualResult.IsSuccessful);
        }

        [Test]
        public void Validate_AllSettingItemsAreValid_ReturnSuccessResult()
        {
            //// Arrange
            var validSettingItem = Substitute.For<ISettingItem>();
            validSettingItem.Validate().Returns(Result.Success);
            var settingType = new SettingType
            {
                SettingItems = new List<ISettingItem>
                {
                    validSettingItem
                }
            };

            //// Act
            var actualResult = settingType.Validate();

            //// Assert
            Assert.True(actualResult.IsSuccessful);
        }

        [Test]
        public void ToStringMethod_Always_ReturnCombinationOfIdAndTemplate()
        {
            //// Arrange
            short settingTypeId = 1;
            var settingType = new SettingType
            {
                Id = settingTypeId
            };

            //// Act
            var actual = settingType.ToString();

            //// Assert
            Assert.AreEqual(string.Join(typeof(SettingType).ToString(), settingTypeId), actual);
        }

        [Test]
        public void SatisfyCustomerLevelConstraint_ActorIsAdmin_ReturnTrue()
        {
            // Arrange.
            var settingType = new SettingType();
            var actorLevel = Level.Company;
            var anyCustomerLevel = Level.Agent;

            // Act.
            var actualResult = settingType.SatisfyCustomerLevelConstraint(actorLevel, anyCustomerLevel);

            // Assert.
            Assert.True(actualResult);
        }
    }
}