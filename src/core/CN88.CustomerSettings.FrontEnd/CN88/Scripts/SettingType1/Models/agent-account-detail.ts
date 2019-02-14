/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="account-detail.ts" />

module CustomerSetting {
    "use strict";

    export class AgentAccountDetail extends AccountDetail {
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

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel, hashKey);

                this.currentMasterPT = accountDetail.currentMasterPT;
                this.minMasterPT = accountDetail.minMasterPT;
                this.maxMasterPT = accountDetail.maxMasterPT;
                this.currentAgentPT = accountDetail.currentAgentPT;
                this.minAgentPT = accountDetail.minAgentPT;
                this.maxAgentPT = accountDetail.maxAgentPT;
                this.minAllowedPT = accountDetail.minAllowedPT;
                this.maxAllowedPT = accountDetail.maxAllowedPT;

                if (isNumber(accountDetail.masterPT)) {
                    this.masterPT = accountDetail.masterPT;
                }

                if (isNumber(accountDetail.agentPT)) {
                    this.agentPT = accountDetail.agentPT;
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
            if (!super.validate()) {
                return false;
            }

            if (!this.isPTReadOnly && !this.validatePT()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        private validatePT(): boolean {
            if (isNumber(this.masterPT) && (this.masterPT > this.maxMasterPT)) {
                this.validationState = new ValidationResult("MasterPTMustBeLessThanOrEqualTo", "masterPT", [this.maxMasterPT]);

                return false;
            }

            if (isNumber(this.agentPT) && (this.agentPT > this.maxAgentPT)) {
                this.validationState = new ValidationResult("AgentPTMustBeLessThanOrEqualTo", "agentPT", [this.maxAgentPT]);

                return false;
            }

            var actualMasterPT = this.masterPT || this.currentMasterPT;
            var actualAgentPT = this.agentPT || this.currentAgentPT;

            if ((actualMasterPT + actualAgentPT > this.maxAllowedPT) || (actualMasterPT + actualAgentPT < this.minAllowedPT)) {
                this.validationState = new ValidationResult("TotalPTMustBeInRange", "masterPT", [this.minAllowedPT, this.maxAllowedPT]);
                this.validationState.addAffectedField("agentPT");

                return false;
            }

            return true;
        }
    }
}