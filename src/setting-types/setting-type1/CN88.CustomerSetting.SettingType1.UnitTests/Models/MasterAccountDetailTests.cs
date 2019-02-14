namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using CN88.CustomerSetting.SettingType1.Models;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
    public class MasterAccountDetailTests
    {
        private MasterAccountDetail masterAccountDetail;

        [SetUp]
        public void SetUp()
        {
            masterAccountDetail = new MasterAccountDetail();
        }

        [Test]
        public void AdjustPTForQuery_WhenCreateNew_SetSuperPTToZero()
        {
            masterAccountDetail.AdjustPTForQuery(true);

            Assert.AreEqual(0, masterAccountDetail.SuperPT.Value);
        }


        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperPTEqualsSuperMinPT_SetSuperPTToNull()
        {
            masterAccountDetail.CurrentSuperPT = 20;
            masterAccountDetail.SuperPT = 20;

            masterAccountDetail.AdjustPTForQuery(false);

            Assert.IsFalse(masterAccountDetail.SuperPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentSuperPTNotEqualsSuperMinPT_SuperPTNoChange()
        {
            masterAccountDetail.SuperPT = 30;

            masterAccountDetail.AdjustPTForQuery(false);

            Assert.AreEqual(30, masterAccountDetail.SuperPT.Value);
        }

        [Test]
        public void IsNoChangePT_SuperPTIsNull_ReturnTrue()
        {
            masterAccountDetail.SuperPT = null;

            Assert.IsTrue(masterAccountDetail.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_SuperPTIsNotNull_ReturnFalse()
        {
            masterAccountDetail.SuperPT = 10;

            Assert.IsFalse(masterAccountDetail.IsNoChangePT());
        }
    }
}