namespace CN88.CustomerSetting.WebApi.UnitTests.Models
{
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.WebApi.Models.Requests;
    using NUnit.Framework;

    [TestFixture(Category = "Core-WebAPI")]
    public class UpdateSettingRequestTests
    {
        private static Setting sampleSetting;

        protected static Setting SampleSetting
        {
            get
            {
                if (sampleSetting == null)
                {
                    var settingItem = new SettingItemDefault
                    {
                        SportTypeId = 121,
                        SportTypeName = "Sport Type 121",
                        BetTypeId = 30001,
                        BetTypeName = "Bet Type 30001"
                    };
                    var settingType = new SettingType
                    {
                        Id = 1,
                        Name = "Setting Type 1",
                        SettingLevel = 1,
                        DisplayOrder = 1,
                        DisplayMode = 1,
                        SettingItems = new List<ISettingItem>
                    {
                        settingItem
                    }
                    };

                    sampleSetting = new Setting(1, Level.Member, 255, null, new List<SettingType> { settingType });
                }

                return sampleSetting;
            }
        }

        protected virtual UpdateSettingRequest Request { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            Request = new UpdateSettingRequest
            {
                Setting = SampleSetting
            };
        }

        [Test]
        public void GetUpdateCustomerSettingCommand_Always_ReturnObject()
        {
            // Act.
            var command = this.Request.GetUpdateCustomerSettingCommand();

            // Assert.
            Assert.NotNull(command);
        }

        [Test]
        public void GetUpdateCustomerSettingCommand_Always_ReturnTypeOfUpdateCustomerSettingCommand()
        {
            // Act.
            var command = this.Request.GetUpdateCustomerSettingCommand();

            // Assert.
            Assert.IsInstanceOf<UpdateCustomerSettingCommand>(command);
        }

        [Test]
        public void GetUpdateCustomerSettingCommand_Always_ReturnedObjectContainsExpectedData()
        {
            // Act.
            var command = this.Request.GetUpdateCustomerSettingCommand();

            // Assert.
            Assert.AreSame(SampleSetting, command.Setting);
        }
    }
}