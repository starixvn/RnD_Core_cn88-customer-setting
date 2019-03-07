namespace CN88.CustomerSetting.SettingType1.UnitTests.Models
{
    using CN88.CustomerSetting.SettingType1.Models.PTItems;
    using NUnit.Framework;

    [TestFixture(Category = "SettingType1-AccountDetail")]
    public class AgentPTItemTests
    {
        private AgentPTItem agentPTItem;

        [SetUp]
        public void SetUp()
        {
            agentPTItem = new AgentPTItem();
        }

        [Test]
        public void AdjustPTForQuery_WhenCreateNew_SetMasterPTAndAgentPTToZero()
        {
            agentPTItem.AdjustPTForQuery(true);

            Assert.AreEqual(0, agentPTItem.MasterPT.Value);
            Assert.AreEqual(0, agentPTItem.AgentPT.Value);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentMasterPTEqualsSuperMinPT_SetMasterPTToNull()
        {
            agentPTItem.CurrentMasterPT = 20;
            agentPTItem.MasterPT = 20;

            agentPTItem.AdjustPTForQuery(false);

            Assert.IsFalse(agentPTItem.MasterPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentMasterPTNotEqualsSuperMinPT_MasterPTNoChange()
        {
            agentPTItem.MasterPT = 30;

            agentPTItem.AdjustPTForQuery(false);

            Assert.AreEqual(30, agentPTItem.MasterPT.Value);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentAgentPTEqualsAgentPT_SetAgentPTToNull()
        {
            agentPTItem.CurrentAgentPT = 20;
            agentPTItem.AgentPT = 20;

            agentPTItem.AdjustPTForQuery(false);

            Assert.IsFalse(agentPTItem.AgentPT.HasValue);
        }

        [Test]
        public void AdjustPTForQuery_WhenEditSetting_CurrentAgentPTNotEqualsSuperMaxPT_AgentPTNoChange()
        {
            agentPTItem.AgentPT = 30;

            agentPTItem.AdjustPTForQuery(false);

            Assert.AreEqual(30, agentPTItem.AgentPT.Value);
        }

        [Test]
        public void IsNoChangePT_MasterPTAndAgentPTAreNull_ReturnTrue()
        {
            agentPTItem.AgentPT = null;
            agentPTItem.MasterPT = null;

            Assert.IsTrue(agentPTItem.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_AgentPTIsNotNull_ReturnFalse()
        {
            agentPTItem.AgentPT = 10;

            Assert.IsFalse(agentPTItem.IsNoChangePT());
        }

        [Test]
        public void IsNoChangePT_MasterPTIsNotNull_ReturnFalse()
        {
            agentPTItem.MasterPT = 10;

            Assert.IsFalse(agentPTItem.IsNoChangePT());
        }
    }
}