/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />

module CustomerSetting {
    "use strict";

    export class MasterAccountDetail extends AccountDetail {
        public superPT: number;
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

                this.superPT = accountDetail.superPT;
                this.minSuperPT = accountDetail.minSuperPT;
                this.maxSuperPT = accountDetail.maxSuperPT;
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting: MasterAccountDetail = <MasterAccountDetail>this.oldSetting;

            return isChanged || this.superPT !== oldSetting.superPT;
        }

        public validate(): boolean {
            if (!super.validate()) {
                return false;
            }

            if (!this.validateSuperPT()) {
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