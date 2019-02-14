namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using CN88.CustomerSetting.SettingType1.Models;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
    public class AgentAccountDetailTests
    {
        private AgentAccountDetail agentAccountDetail;

        [SetUp]
        public void SetUp()
        {
            agentAccountDetail = new AgentAccountDetail();
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentMasterPTEqualsSuperMinPT_SetMasterPTToNull()
        {
            agentAccountDetail.CurrentMasterPT = 20;
            agentAccountDetail.MasterPT = 20;

            agentAccountDetail.AdjustPTForQuery(false);

            Assert.IsFalse(agentAccountDetail.MasterPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentMasterPTNotEqualsSuperMinPT_MasterPTNoChange()
        {
            agentAccountDetail.MasterPT = 30;

            agentAccountDetail.AdjustPTForQuery(false);

            Assert.AreEqual(30, agentAccountDetail.MasterPT.Value);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentAgentPTEqualsAgentPT_SetAgentPTToNull()
        {
            agentAccountDetail.CurrentAgentPT = 20;
            agentAccountDetail.AgentPT = 20;

            agentAccountDetail.AdjustPTForQuery(false);

            Assert.IsFalse(agentAccountDetail.AgentPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentAgentPTNotEqualsSuperMaxPT_AgentPTNoChange()
        {
            agentAccountDetail.AgentPT = 30;

            agentAccountDetail.AdjustPTForQuery(false);

            Assert.AreEqual(30, agentAccountDetail.AgentPT.Value);
        }

        [Test]
        public void IsNoChangePT_MasterPTAndAgentPTAreNull_ReturnTrue()
        {
            agentAccountDetail.AgentPT = null;
            agentAccountDetail.MasterPT = null;

            Assert.IsTrue(agentAccountDetail.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_AgentPTIsNotNull_ReturnFalse()
        {
            agentAccountDetail.AgentPT = 10;

            Assert.IsFalse(agentAccountDetail.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_MasterPTIsNotNull_ReturnFalse()
        {
            agentAccountDetail.MasterPT = 10;

            Assert.IsFalse(agentAccountDetail.IsNoChangePT());
        }
    }
}