namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using CN88.CustomerSetting.SettingType1.Models;
    using NUnit.Framework;

    [TestFixture]
    public class SuperAccountDetailTests
    {
        private SuperAccountDetail superAccountDetail;

        [SetUp]
        public void SetUp()
        {
            superAccountDetail = new SuperAccountDetail();
        }

        [Test]
        public void AdjustPTForQuery_CurrentSuperMinPTEqualsSuperMinPT_SetSuperPTToNull()
        {
            superAccountDetail.CurrentSuperMinPT = 20;
            superAccountDetail.SuperMinPT = 20;

            superAccountDetail.AdjustPTForQuery(false);

            Assert.IsFalse(superAccountDetail.SuperMinPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_CurrentSuperMinPTNotEqualsSuperMinPT_SuperMinPTNoChange()
        {
            superAccountDetail.SuperMinPT = 30;

            superAccountDetail.AdjustPTForQuery(false);

            Assert.AreEqual(30, superAccountDetail.SuperMinPT.Value);
        }

        [Test]
        public void AdjustPTForQuery_CurrentSuperMaxPTEqualsSuperMaxPT_SetSuperMaxPTToNull()
        {
            superAccountDetail.CurrentSuperMaxPT = 20;
            superAccountDetail.SuperMaxPT = 20;

            superAccountDetail.AdjustPTForQuery(false);

            Assert.IsFalse(superAccountDetail.SuperMaxPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_CurrentSuperMaxPTNotEqualsSuperMaxPT_SuperMaxPTNoChange()
        {
            superAccountDetail.SuperMaxPT = 30;

            superAccountDetail.AdjustPTForQuery(false);

            Assert.AreEqual(30, superAccountDetail.SuperMaxPT.Value);
        }

        [Test]
        public void IsNoChangePT_SuperMinPTAndSuperMaxPTAreNull_ReturnTrue()
        {
            superAccountDetail.SuperMinPT = null;
            superAccountDetail.SuperMaxPT = null;

            Assert.IsTrue(superAccountDetail.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_SuperMinPTIsNotNull_ReturnFalse()
        {
            superAccountDetail.SuperMinPT = 10;

            Assert.IsFalse(superAccountDetail.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_SuperMaxPTIsNotNull_ReturnFalse()
        {
            superAccountDetail.SuperMaxPT = 10;

            Assert.IsFalse(superAccountDetail.IsNoChangePT());
        }
    }
}