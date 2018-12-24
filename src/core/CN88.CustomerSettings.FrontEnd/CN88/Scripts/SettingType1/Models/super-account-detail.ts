/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />
/// <reference path="currency.ts" />

module CustomerSetting {
    "use strict";

    export class SuperAccountDetail extends AccountDetail {
        public superMinPT: number;
        public superMaxPT: number;
        public currency: ICurrency;
        public currencies: ICurrency[];

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel);

                this.superMinPT = accountDetail.superMinPT;
                this.superMaxPT = accountDetail.superMaxPT;
                this.currency = accountDetail.currency;
                this.currencies = accountDetail.currencies;
            }

            if (backup) {
                this.oldSetting = new SuperAccountDetail(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting: SuperAccountDetail = <SuperAccountDetail>this.oldSetting;

            return isChanged
                || this.superMinPT !== oldSetting.superMinPT
                || this.superMaxPT !== oldSetting.superMaxPT
                || this.currency.id !== oldSetting.currency.id;
        }

        public validate(): boolean {
            if (this.isNew) {
                if (!this.username || this.username.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "username");

                    return false;
                }

                if (!this.password || this.password.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "password");

                    return false;
                }

                if (!this.currency) {
                    this.validationState = new ValidationResult("RequiredField", "currency");

                    return false;
                }
            }

            if (!this.validateSuperMinPT()) {
                return false;
            }

            if (!this.validateSuperMaxPT()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        public validateSuperMinPT(): boolean {
            if (this.superMinPT < this.minPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeGreaterThanOrEqualTo", "superMinPT", [this.minPT]);

                return false;
            }

            if (this.superMinPT > this.maxPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualTo", "superMinPT", [this.maxPT]);

                return false;
            }

            return true;
        }

        public validateSuperMaxPT(): boolean {
            if (this.superMaxPT < this.minPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustBeGreaterThanOrEqualTo", "superMaxPT", [this.minPT]);

                return false;
            }

            if (this.superMaxPT > this.maxPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustLessThanOrEqualTo", "superMaxPT", [this.minPT]);

                return false;
            }

            return true;
        }
    }
}