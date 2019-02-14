namespace CN88.CustomerSetting.WebApi.UnitTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CN88.CustomerSetting.Models;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.WebApi.Models.Binders;
    using CN88.CustomerSetting.WebApi.Models.Requests;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using NUnit.Framework;

    [TestFixture(Category = "Core-WebAPI")]
    public class SettingConverterTests
    {
        private const string SampleDataFilePath = @"\SampleData\settings.json";

        private static Setting ExpectedData
        {
            get
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

                return new Setting(1, Level.Super, 255, null, new List<SettingType> { settingType });
            }
        }

        public static string ReadSampleData()
        {
            string sampleSetting = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + SampleDataFilePath);

            sampleSetting = sampleSetting.Replace("\"", "\\\"");

            return "{\"Setting\":\"" + sampleSetting + "\"}";
        }

        [Test]
        public void SerializeObject_Always_ReturnExpectedData()
        {
            // Act.
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Converters = new List<JsonConverter>
                {
                    new SettingConverter(),
                    new SettingTypeConverter(),
                    new SettingItemConverter(),
                    new LevelConverter()
                }
            };
            var setting = JsonConvert.DeserializeObject<UpdateSettingRequest>(ReadSampleData(), jsonSerializerSettings);

            // Assert.
            string reserializedSettingJson = JsonConvert.SerializeObject(setting.Setting);
            string expectedSettingJson = JsonConvert.SerializeObject(ExpectedData);

            Assert.AreEqual(expectedSettingJson, reserializedSettingJson);
        }
    }
}