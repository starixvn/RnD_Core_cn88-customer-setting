/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="account-detail.ts" />

namespace CustomerSetting {
    export class MasterAccountDetail extends AccountDetail {
        public superPT: number;
        public currentSuperPT: number;
        public minSuperPT: number;
        public maxSuperPT: number;

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel, hashKey);

                this.currentSuperPT = accountDetail.currentSuperPT;
                this.minSuperPT = accountDetail.minSuperPT;
                this.maxSuperPT = accountDetail.maxSuperPT;

                if (isNumber(accountDetail.superPT)) {
                    this.superPT = accountDetail.superPT;
                }
            }

            this.ignoredProperties.push("minSuperPT");
            this.ignoredProperties.push("maxSuperPT");

            if (this.isPTReadOnly) {
                this.ignoredProperties.push("currentSuperPT");
                this.ignoredProperties.push("superPT");
            }
        }

        public validate(): boolean {
            if (!super.validate()) {
                return false;
            }

            if (!this.isPTReadOnly && isNumber(this.superPT) && !this.validateSuperPT()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        private validateSuperPT(): boolean {
            if (this.superPT < this.minSuperPT) {
                this.validationState = new ValidationResult("SuperPTMustBeGreaterThanOrEqualTo", "superPT", [this.minSuperPT]);

                return false;
            }

            if (this.superPT > this.maxSuperPT) {
                this.validationState = new ValidationResult("SuperPTMustBeLessThanOrEqualTo", "superPT", [this.maxSuperPT]);

                return false;
            }

            return true;
        }
    }
}