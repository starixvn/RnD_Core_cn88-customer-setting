/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />

module CustomerSetting {
    "use strict";

    export class AgentAccountDetail extends AccountDetail {
        public masterPT: number;
        public minMasterPT: number;
        public maxMasterPT: number;
        public agentPT: number;
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

                this.masterPT = accountDetail.masterPT;
                this.minMasterPT = accountDetail.minMasterPT;
                this.maxMasterPT = accountDetail.maxMasterPT;
                this.agentPT = accountDetail.agentPT;
                this.minAgentPT = accountDetail.minAgentPT;
                this.maxAgentPT = accountDetail.maxAgentPT;
                this.minAllowedPT = accountDetail.minAllowedPT;
                this.maxAllowedPT = accountDetail.maxAllowedPT;
            }

            if (backup) {
                this.oldSetting = new AgentAccountDetail(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting: AgentAccountDetail = <AgentAccountDetail>this.oldSetting;

            return isChanged
                || this.masterPT !== oldSetting.masterPT
                || this.agentPT !== oldSetting.agentPT;
        }

        public validate(): boolean {
            if (!super.validate()) {
                return false;
            }

            if (!this.validatePT()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        private validatePT(): boolean {
            if (this.masterPT > this.maxMasterPT) {
                this.validationState = new ValidationResult("MasterPTMustBeLessThanOrEqualTo", "masterPT", [this.maxMasterPT]);

                return false;
            }

            if (this.agentPT > this.maxAgentPT) {
                this.validationState = new ValidationResult("AgentPTMustBeLessThanOrEqualTo", "agentPT", [this.maxAgentPT]);

                return false;
            }

            if ((this.masterPT + this.agentPT > this.maxAllowedPT) || (this.masterPT + this.agentPT < this.minAllowedPT)) {
                this.validationState = new ValidationResult("TotalPTMustBeInRange", "masterPT", [this.minAllowedPT, this.maxAllowedPT]);
                this.validationState.addAffectedField("agentPT");

                return false;
            }

            return true;
        }
    }
}