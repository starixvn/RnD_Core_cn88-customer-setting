namespace CN88.CustomerSetting.Tests.Models
{
    using CustomerSetting.Models;
    using CustomerSetting.Models.Enumerations;
    using NUnit.Framework;

    public class SettingTests
    {
        [Test]
        public void CustomerId_SetValidValueThenGet_ReturnSetValue()
        {
            //// Arrange
            Setting setting = InstanceFactory.CreateCustomerSetting();
            var customerId = 100;

            //// Act
            setting.CustomerId = customerId;

            //// Assert
            Assert.AreEqual(customerId, setting.CustomerId);
        }

        ////[Test]
        ////public void CustomerLevel_SetNullValue_ThrowArgumentNullException()
        ////{
        ////    //// Arrange
        ////    Setting setting = InstanceFactory.CreateCustomerSetting();

        ////    //// Act and Assert
        ////    var ex = Assert.Throws<ArgumentNullException>(() => { setting.CustomerLevel = null; });
        ////    Assert.tha(ex.ParamName, Is.EqualTo("value"));
        ////}

        [Test]
        public void SettingTypes_SetValueToNullThenGet_ReturnEmptySettingTypes()
        {
            //// Arrange
            var setting = new Setting(1, Level.Super, 1, null);

            //// Assert
            Assert.IsEmpty(setting.SettingTypes);
        }
    }
}