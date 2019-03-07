/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class AgentPTItem extends PTItem {
        public masterPT: number;
        public currentMasterPT: number;
        public minMasterPT: number;
        public maxMasterPT: number;
        public agentPT: number;
        public currentAgentPT: number;
        public minAgentPT: number;
        public maxAgentPT: number;
        public minAllowedPT: number;
        public maxAllowedPT: number;

        constructor(agentPTItem: any, isPTReadOnly: boolean) {
            if (agentPTItem) {
                super(agentPTItem, isPTReadOnly);

                this.currentMasterPT = agentPTItem.currentMasterPT;
                this.minMasterPT = agentPTItem.minMasterPT;
                this.maxMasterPT = agentPTItem.maxMasterPT;
                this.currentAgentPT = agentPTItem.currentAgentPT;
                this.minAgentPT = agentPTItem.minAgentPT;
                this.maxAgentPT = agentPTItem.maxAgentPT;
                this.minAllowedPT = agentPTItem.minAllowedPT;
                this.maxAllowedPT = agentPTItem.maxAllowedPT;

                if (isNumber(agentPTItem.masterPT)) {
                    this.masterPT = agentPTItem.masterPT;
                }

                if (isNumber(agentPTItem.agentPT)) {
                    this.agentPT = agentPTItem.agentPT;
                }
            }

            this.ignoredProperties.push("minMasterPT");
            this.ignoredProperties.push("maxMasterPT");
            this.ignoredProperties.push("minAgentPT");
            this.ignoredProperties.push("maxAgentPT");
            this.ignoredProperties.push("minAllowedPT");
            this.ignoredProperties.push("maxAllowedPT");

            if (this.isPTReadOnly) {
                this.ignoredProperties.push("currentMasterPT");
                this.ignoredProperties.push("masterPT");
                this.ignoredProperties.push("currentAgentPT");
                this.ignoredProperties.push("agentPT");
            }
        }

        public validate(): boolean {
            if (!this.isPTReadOnly) {
                if (isNumber(this.masterPT) && !this.validateMasterPT()) {
                    return false;
                }

                if (isNumber(this.agentPT) && !this.validateAgentPT()) {
                    return false;
                }

                var actualMasterPT = this.masterPT || this.currentMasterPT;
                var actualAgentPT = this.agentPT || this.currentAgentPT;

                if ((actualMasterPT + actualAgentPT > this.maxAllowedPT) || (actualMasterPT + actualAgentPT < this.minAllowedPT)) {
                    this.validationState = new ValidationResult("TotalPTMustBeInRange", "masterPT", [this.minAllowedPT, this.maxAllowedPT]);
                    this.validationState.addAffectedField("agentPT");

                    return false;
                }
            }

            this.resetValidationState();

            return true;
        }

        private validateMasterPT(): boolean {
            if (this.masterPT < this.minMasterPT) {
                this.validationState = new ValidationResult("MasterPTMustBeGreaterThanOrEqualTo", "masterPT", [this.maxMasterPT]);

                return false;
            }

            if (this.masterPT > this.maxMasterPT) {
                this.validationState = new ValidationResult("MasterPTMustBeLessThanOrEqualTo", "masterPT", [this.maxMasterPT]);

                return false;
            }

            return true;
        }

        private validateAgentPT(): boolean {
            if (this.agentPT < this.minAgentPT) {
                this.validationState = new ValidationResult("AgentPTMustBeGreaterThanOrEqualTo", "agentPT", [this.maxAgentPT]);

                return false;
            }

            if (this.agentPT > this.maxAgentPT) {
                this.validationState = new ValidationResult("AgentPTMustBeLessThanOrEqualTo", "agentPT", [this.maxAgentPT]);

                return false;
            }

            return true;
        }
    }
}