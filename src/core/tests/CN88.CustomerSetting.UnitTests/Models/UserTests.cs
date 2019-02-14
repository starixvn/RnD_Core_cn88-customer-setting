namespace CN88.CustomerSetting.Tests.Models
{
    using System;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Enumerations;
    using NUnit.Framework;

    [TestFixture(Category = "Core")]
    public class UserTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void SetId_InputValueIsNegativeOrZero_ThrowArgumentOutOfRangeException(int userId)
        {
            //// Arrange
            User user = InstanceFactory.CreateUser();

            //// Act
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    user.Id = userId;
                });
        }

        
        [TestCase("    ")]
        [TestCase("")]
        [TestCase(null)]
        public void SetName_InputValueIsNullOrWhiteSpace_ThrowArgumentOutOfRangeException(string userName)
        {
            //// Arrange
            User user = InstanceFactory.CreateUser();

            //// Act
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                    {
                user.Name = userName;
            });
        }

        [Test]
        public void SetLevel_InputValuesIsNull_ThrowArgumentNullException()
        {
            //// Arrange
            User user = InstanceFactory.CreateUser();

            //// Act
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    user.Level = null;
                });
        }

        [Test]
        public void SetLevel_InputValuesIsMember_ThrowArgumentOutOfRangeException()
        {
            //// Arrange
            User user = InstanceFactory.CreateUser();

            //// Act
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    user.Level = Level.Member;
                });
        }

        
        [TestCase(-10)]
        [TestCase(-1)]
        public void SetSubAccountId_InputValueIsNegative_ThrowArgumentOutOfRangeException(int subAccountId)
        {
            //// Arrange
            User user = InstanceFactory.CreateUser();

            //// Act
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    user.SubAccountId = subAccountId;
                });
        }
    }
}