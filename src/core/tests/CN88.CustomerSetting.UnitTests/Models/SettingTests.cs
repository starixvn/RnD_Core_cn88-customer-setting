namespace CN88.CustomerSetting.Tests.Models
{
    using System;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Enumerations;
    using Xunit;

    public class SettingTests
    {
        //[Fact]
        //public void SetProductId_ValueIsZero_ThrowArgumentOutOfRangeException()
        //{
        //    //// Arrange
        //    Setting setting = InstanceFactory.CreateCustomerSetting();

        //    //// Act and Assert
        //    Assert.Throws<ArgumentOutOfRangeException>(() => { setting.ProductId = 0; }, "Product Id cannot be negative or zero.");
        //}

        //[InlineData(0)]
        //[InlineData(-1)]
        //public void CustomerId_SetValueIsNegativeOrZero_ThrowArgumentOutOfRangeException(int customerId)
        //{
        //    //// Arrange
        //    Setting setting = InstanceFactory.CreateCustomerSetting();

        //    //// Act and Assert
        //    Assert.Throws<ArgumentOutOfRangeException>(() => { setting.CustomerId = customerId; }, "Customer Id cannot be negative or zero.");
        //}

        [Fact]
        public void CustomerId_SetValidValueThenGet_ReturnSetValue()
        {
            //// Arrange
            Setting setting = InstanceFactory.CreateCustomerSetting();
            var customerId = 100;

            //// Act
            setting.CustomerId = customerId;

            //// Assert
            Assert.Equal(customerId, setting.CustomerId);
        }

        ////[Fact]
        ////public void CustomerLevel_SetNullValue_ThrowArgumentNullException()
        ////{
        ////    //// Arrange
        ////    Setting setting = InstanceFactory.CreateCustomerSetting();

        ////    //// Act and Assert
        ////    var ex = Assert.Throws<ArgumentNullException>(() => { setting.CustomerLevel = null; });
        ////    Assert.tha(ex.ParamName, Is.EqualTo("value"));
        ////}

        [Fact]
        public void SettingTypes_SetValueToNullThenGet_ReturnEmptySettingTypes()
        {
            //// Arrange
            var setting = new Setting(1, Level.Super, 1, null);

            //// Assert
            Assert.Empty(setting.SettingTypes);
        }
    }
}