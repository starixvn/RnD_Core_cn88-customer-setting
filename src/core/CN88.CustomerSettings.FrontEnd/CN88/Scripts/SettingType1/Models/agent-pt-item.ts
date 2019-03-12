/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class AgentPTItem extends PTItem {
        public currentMasterPT: number;
        public masterPT: number;
        public minMasterPT: number;
        public maxMasterPT: number;
        public currentAgentPT: number;
        public agentPT: number;
        public minAgentPT: number;
        public maxAgentPT: number;
        public minAllowedPT: number;
        public maxAllowedPT: number;

        constructor(agentPTItem: any, isPTReadOnly: boolean) {
            if (agentPTItem) {
                super(agentPTItem);

                this.currentMasterPT = agentPTItem.currentMasterPT;
                this.masterPT = agentPTItem.masterPT;
                this.minMasterPT = agentPTItem.minMasterPT;
                this.maxMasterPT = agentPTItem.maxMasterPT;
                this.currentAgentPT = agentPTItem.currentAgentPT;
                this.agentPT = agentPTItem.agentPT;
                this.minAgentPT = agentPTItem.minAgentPT;
                this.maxAgentPT = agentPTItem.maxAgentPT;
                this.minAllowedPT = agentPTItem.minAllowedPT;
                this.maxAllowedPT = agentPTItem.maxAllowedPT;
            }

            this.ignoredProperties = this.ignoredProperties.concat([
                "minMasterPT",
                "maxMasterPT",
                "minAgentPT",
                "maxAgentPT",
                "minAllowedPT",
                "maxAllowedPT"]);

            if (isPTReadOnly) {
                this.ignoredProperties = this.ignoredProperties.concat([
                    "currentMasterPT",
                    "masterPT",
                    "currentAgentPT",
                    "agentPT"]);
            }
        }

        public validate(): boolean {
            if (!isNumber(this.masterPT) || this.masterPT < this.minMasterPT || this.masterPT > this.maxMasterPT) {
                this.validationState = new ValidationResult("MasterPTIsOutOfRange", "masterPT");

                return false;
            }

            if (!isNumber(this.agentPT) || this.agentPT < this.minAgentPT || this.agentPT > this.maxAgentPT) {
                this.validationState = new ValidationResult("AgentPTIsOutOfRange", "agentPT");

                return false;
            }

            if ((this.masterPT + this.agentPT > this.maxAllowedPT) || (this.masterPT + this.agentPT < this.minAllowedPT)) {
                this.validationState = new ValidationResult("TotalPTMustBeInRange", "masterPT", [this.minAllowedPT, this.maxAllowedPT]);
                this.validationState.addAffectedField("agentPT");

                return false;
            }

            this.resetValidationState();

            return true;
        }
    }
}