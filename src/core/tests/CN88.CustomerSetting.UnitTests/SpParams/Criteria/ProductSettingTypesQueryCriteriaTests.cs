namespace CN88.CustomerSetting.UnitTests.SpParams.Criteria
{
    using CN88.CustomerSetting.SpParams.Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class ProductSettingTypesQueryCriteriaTests
    {
        [TestCase(0)]
        public void IsValid_ProductIdIsZero_ReturnFalse(byte productId)
        {
            //// Arrange
            var criteria = CreateProductSettingTypesQueryCriteriaFakeObject();
            criteria.ProductId = productId;

            //// Act
            var actualResult = criteria.IsValid();

            //// Arrange
            Assert.IsFalse(actualResult);
        }

        [TestCase(1)]
        [TestCase(125)]
        public void IsValid_ProductIdIsGreaterThanZero_ReturnTrue(byte productId)
        {
            //// Arrange
            var criteria = CreateProductSettingTypesQueryCriteriaFakeObject();
            criteria.ProductId = productId;

            //// Act
            var actualResult = criteria.IsValid();

            //// Arrange
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void GetSettingKey_ReturnSettingKey()
        {
            //// Arrange
            var criteria = CreateProductSettingTypesQueryCriteriaFakeObject();

            //// Act
            var actualSettingKey = criteria.GetSettingKey();

            //// Arrange
            Assert.AreEqual("CustSetting_ProductProfileSelect", actualSettingKey);
        }

        private ProductSettingTypesQueryCriteria CreateProductSettingTypesQueryCriteriaFakeObject()
        {
            return new ProductSettingTypesQueryCriteria
            {
                ProductId = 1
            };
        }
    }
}