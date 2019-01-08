namespace CN88.CustomerSetting.Tests.Models
{
    using System;
    using CustomerSetting.Models;
    using CustomerSetting.Models.Enumerations;
    using Xunit;

    public class UserTests
    {
        private static readonly object[] isMethodAlwaysReturnRightValueCases =
        {
            new object[] { Level.Company, Level.Company, true },
            new object[] { Level.Company, Level.Super, false },
            new object[] { Level.Super, Level.Super, true },
            new object[] { Level.Super, Level.Master, false },
            new object[] { Level.Master, Level.Master, true },
            new object[] { Level.Master, Level.Agent, false },
            new object[] { Level.Agent, Level.Agent, true }
        };

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
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

        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
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

        [Fact]
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

        [Fact]
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

        [Theory]
        [InlineData(-10)]
        [InlineData(-1)]
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