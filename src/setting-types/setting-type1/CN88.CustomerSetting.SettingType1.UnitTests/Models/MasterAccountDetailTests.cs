namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CN88.CustomerSetting.SettingType1.Models;
    using NUnit.Framework;

    [TestFixture]
    public class MasterAccountDetailTests
    {
        private MasterAccountDetail masterAccountDetail;

        [SetUp]
        public void SetUp()
        {
            masterAccountDetail = new MasterAccountDetail();
        }

        [Test]
        public void AdjustPTForQuery_CurrentSuperPTEqualsSuperMinPT_SetSuperPTToNull()
        {
            masterAccountDetail.CurrentSuperPT = 20;
            masterAccountDetail.SuperPT = 20;

            masterAccountDetail.AdjustPTForQuery(false);

            Assert.IsFalse(masterAccountDetail.SuperPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_CurrentSuperPTNotEqualsSuperMinPT_SuperPTNoChange()
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
