/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />

module CustomerSetting {
    "use strict";

    export class AgentAccountDetail extends AccountDetail {
        public masterPT: number;
        public agentPT: number;

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel);

                this.masterPT = accountDetail.masterPT;
                this.agentPT = accountDetail.agentPT;
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
            if (this.masterPT > this.maxPT) {
                this.validationState = new ValidationResult("MasterPTMustBeLessThanOrEqualTo", "masterPT", [this.maxPT]);

                return false;
            }

            if (this.agentPT > this.maxPT) {
                this.validationState = new ValidationResult("AgentPTMustBeLessThanOrEqualTo", "agentPT", [this.maxPT]);

                return false;
            }

            if ((this.masterPT + this.agentPT > this.maxPT) || (this.masterPT + this.agentPT < this.minPT)) {
                this.validationState = new ValidationResult("TotalPTMustBeInRange", "masterPT", [this.minPT, this.maxPT]);

                return false;
            }

            return true;
        }
    }
}