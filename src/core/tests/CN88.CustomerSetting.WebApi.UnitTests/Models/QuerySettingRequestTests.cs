namespace CN88.CustomerSetting.WebApi.UnitTests.Models
{
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Requests;
    using CN88.CustomerSetting.WebApi.Models.Requests;
    using NUnit.Framework;

    [TestFixture(Category = "Core-WebAPI")]
    public class QuerySettingRequestTests
    {
        private QuerySettingRequest request;

        [SetUp]
        public void SetUp()
        {
            request = new QuerySettingRequest
            {
                ProductId = 1,
                CustomerId = 1,
                CustomerLevel = Level.Member.Value
            };
        }

        [Test]
        public void GetCustomerSettingQuery_Always_ReturnObject()
        {
            // Act.
            var query = request.GetCustomerSettingQuery();

            // Assert.
            Assert.NotNull(query);
        }

        [Test]
        public void GetCustomerSettingQuery_Always_ReturnTypeOfCustomerSettingQuery()
        {
            // Act.
            var query = request.GetCustomerSettingQuery();

            // Assert.
            Assert.IsAssignableFrom<CustomerSettingQuery>(query);
        }

        [Test]
        public void GetCustomerSettingQuery_Always_ReturnedObjectContainsExpectedData()
        {
            // Act.
            var query = request.GetCustomerSettingQuery();

            // Assert.
            Assert.AreEqual(1, query.ProductId);
            Assert.AreEqual(1, query.CustomerId);
            Assert.AreEqual(Level.Member, query.CustomerLevel);
        }
    }
}