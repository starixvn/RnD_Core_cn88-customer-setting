namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using CN88.CustomerSetting.SettingType1.Models;
    using CN88.CustomerSetting.SettingType1.Models.PTItems;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
    public class MasterPTItemTests
    {
        private MasterPTItem masterPTItem;

        [SetUp]
        public void SetUp()
        {
            masterPTItem = new MasterPTItem();
        }

        [Test]
        public void AdjustPTForQuery_WhenCreateNew_SetSuperPTToZero()
        {
            masterPTItem.AdjustPTForQuery(true);

            Assert.AreEqual(0, masterPTItem.SuperPT.Value);
        }


        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperPTEqualsSuperMinPT_SetSuperPTToNull()
        {
            masterPTItem.CurrentSuperPT = 20;
            masterPTItem.SuperPT = 20;

            masterPTItem.AdjustPTForQuery(false);

            Assert.IsFalse(masterPTItem.SuperPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperPTNotEqualsSuperMinPT_SuperPTNoChange()
        {
            masterPTItem.SuperPT = 30;

            masterPTItem.AdjustPTForQuery(false);

            Assert.AreEqual(30, masterPTItem.SuperPT.Value);
        }

        [Test]
        public void IsNoChangePT_SuperPTIsNull_ReturnTrue()
        {
            masterPTItem.SuperPT = null;

            Assert.IsTrue(masterPTItem.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_SuperPTIsNotNull_ReturnFalse()
        {
            masterPTItem.SuperPT = 10;

            Assert.IsFalse(masterPTItem.IsNoChangePT());
        }
    }
}