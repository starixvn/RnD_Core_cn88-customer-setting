/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="account-detail.ts" />
/// <reference path="currency.ts" />

namespace CustomerSetting.SettingType1 {
    export class SuperAccountDetail extends AccountDetail {
        public superMinPT: number;
        public currentSuperMinPT: number;
        public minSuperMinPT: number;
        public maxSuperMinPT: number;
        public superMaxPT: number;
        public currentSuperMaxPT: number;
        public minSuperMaxPT: number;
        public maxSuperMaxPT: number;
        public currency: ICurrency;

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel, hashKey);

                this.currentSuperMinPT = accountDetail.currentSuperMinPT;
                this.minSuperMinPT = accountDetail.minSuperMinPT;
                this.maxSuperMinPT = accountDetail.maxSuperMinPT;
                this.currentSuperMaxPT = accountDetail.currentSuperMaxPT;
                this.minSuperMaxPT = accountDetail.minSuperMaxPT;
                this.maxSuperMaxPT = accountDetail.maxSuperMaxPT;
                this.currency = accountDetail.currency;

                if (isNumber(accountDetail.superMinPT)) {
                    this.superMinPT = accountDetail.superMinPT;
                }

                if (isNumber(accountDetail.superMaxPT)) {
                    this.superMaxPT = accountDetail.superMaxPT;
                }
            }

            this.ignoredProperties.push("minSuperMinPT");
            this.ignoredProperties.push("maxSuperMinPT");
            this.ignoredProperties.push("minSuperMaxPT");
            this.ignoredProperties.push("maxSuperMaxPT");

            if (!this.isNew) {
                this.ignoredProperties.push("currency");
            }

            if (this.isPTReadOnly) {
                this.ignoredProperties.push("currentSuperMinPT");
                this.ignoredProperties.push("superMinPT");
                this.ignoredProperties.push("currentSuperMaxPT");
                this.ignoredProperties.push("superMaxPT");
            }
        }

        public validate(): boolean {
            if (this.isNew && !this.validateUplineUsername()) {
                return false;
            }

            if (this.isNew && !super.validateUsername()) {
                return false;
            }

            if (this.isNew && !this.validatePassword()) {
                return false;
            }

            if (!this.validateCredit()) {
                return false;
            }

            if (!this.isPTReadOnly && !this.validatePT()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        protected validatePassword(): boolean {
            if (!this.password || this.password.length === 0) {
                this.validationState = new ValidationResult("RequiredField", "password");

                return false;
            }

            return true;
        }

        protected validateCredit(): boolean {
            if (this.credit === undefined || this.credit === null) {
                this.validationState = new ValidationResult("RequiredField", "credit");

                return false;
            }

            if (this.credit < this.minCredit) {
                this.validationState = new ValidationResult(
                    "CreditsMustBeGreaterThanOrEqualTo",
                    "credit",
                    [this.minCredit],
                    { precision: 2 });

                return false;
            }

            return true;
        }

        private validateUplineUsername(): boolean {
            if (!this.uplineUsername || this.uplineUsername.length === 0) {
                this.validationState = new ValidationResult("RequiredField", "uplineUsername");

                return false;
            }

            return true;
        }

        private validatePT(): boolean {
            if (isNumber(this.superMinPT) && !this.validateSuperMinPT()) {
                return false;
            }

            if (isNumber(this.superMaxPT) && !this.validateSuperMaxPT()) {
                return false;
            }

            var actualSuperMinPT = this.superMinPT || this.currentSuperMinPT;
            var actualSuperMaxPT = this.superMaxPT || this.currentSuperMaxPT;

            if (actualSuperMinPT > actualSuperMaxPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualToSuperMaxPT", "superMinPT");

                return false;
            }

            return true;
        }

        private validateSuperMinPT(): boolean {
            if (this.superMinPT < this.minSuperMinPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeGreaterThanOrEqualTo", "superMinPT", [this.minSuperMinPT]);

                return false;
            }

            if (this.superMinPT > this.maxSuperMinPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualTo", "superMinPT", [this.maxSuperMinPT]);

                return false;
            }

            return true;
        }

        private validateSuperMaxPT(): boolean {
            if (this.superMaxPT < this.minSuperMaxPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustBeGreaterThanOrEqualTo", "superMaxPT", [this.minSuperMaxPT]);

                return false;
            }

            if (this.superMaxPT > this.maxSuperMaxPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustBeLessThanOrEqualTo", "superMaxPT", [this.maxSuperMaxPT]);

                return false;
            }

            return true;
        }
    }
}