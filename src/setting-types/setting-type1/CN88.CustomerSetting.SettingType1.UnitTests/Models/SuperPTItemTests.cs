namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.SettingType1.Models.PTItems;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
    public class SuperPTItemTests
    {
        private SuperPTItem superPTItem;

        [SetUp]
        public void SetUp()
        {
            superPTItem = new SuperPTItem();
        }

        [Test]
        public void AdjustPTForQuery_WhenCreateNew_SetSuperMinPTAndSuperMaxPTToZero()
        {
            superPTItem.AdjustPTForQuery(true);

            Assert.AreEqual(0, superPTItem.SuperMinPT.Value);
            Assert.AreEqual(0, superPTItem.SuperMaxPT.Value);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperMinPTEqualsSuperMinPT_SetSuperPTToNull()
        {
            superPTItem.CurrentSuperMinPT = 20;
            superPTItem.SuperMinPT = 20;

            superPTItem.AdjustPTForQuery(false);

            Assert.IsFalse(superPTItem.SuperMinPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperMinPTNotEqualsSuperMinPT_SuperMinPTNoChange()
        {
            superPTItem.SuperMinPT = 30;

            superPTItem.AdjustPTForQuery(false);

            Assert.AreEqual(30, superPTItem.SuperMinPT.Value);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperMaxPTEqualsSuperMaxPT_SetSuperMaxPTToNull()
        {
            superPTItem.CurrentSuperMaxPT = 20;
            superPTItem.SuperMaxPT = 20;

            superPTItem.AdjustPTForQuery(false);

            Assert.IsFalse(superPTItem.SuperMaxPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperMaxPTNotEqualsSuperMaxPT_SuperMaxPTNoChange()
        {
            superPTItem.SuperMaxPT = 30;

            superPTItem.AdjustPTForQuery(false);

            Assert.AreEqual(30, superPTItem.SuperMaxPT.Value);
        }

        [Test]
        public void IsNoChangePT_SuperMinPTAndSuperMaxPTAreNull_ReturnTrue()
        {
            superPTItem.SuperMinPT = null;
            superPTItem.SuperMaxPT = null;

            Assert.IsTrue(superPTItem.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_SuperMinPTIsNotNull_ReturnFalse()
        {
            superPTItem.SuperMinPT = 10;

            Assert.IsFalse(superPTItem.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_SuperMaxPTIsNotNull_ReturnFalse()
        {
            superPTItem.SuperMaxPT = 10;

            Assert.IsFalse(superPTItem.IsNoChangePT());
        }
    }
}