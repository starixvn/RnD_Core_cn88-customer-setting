/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="account-detail-setting-item.ts" />
/// <reference path="spread-group.ts" />

namespace CustomerSetting.SettingType1 {
    export class MemberAccountDetailSettingItem extends AccountDetailSettingItem {
        public usernameSuffixes: string[];
        public spreadGroups: ISpreadGroup[];
        public spreadGroup: ISpreadGroup;

        constructor()
        constructor(accountDetail: any)
        constructor(accountDetail: any, isNew: boolean)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, isNew, customerLevel, hashKey);

                this.usernameSuffixes = accountDetail.usernameSuffixes;
                this.spreadGroups = accountDetail.spreadGroups;
                this.spreadGroup = accountDetail.spreadGroup;
            }

            this.ignoredProperties = this.ignoredProperties.concat(["usernameSuffixes", "spreadGroups"]);

            if (!this.isNew) {
                this.ignoredProperties.push("spreadGroup");
            }
        }

        public validate(): boolean {
            if (!this.validateUsername()) {
                return false;
            }

            if (this.isNew && !this.validatePassword()) {
                return false;
            }

            if (!this.validateCredit()) {
                return false;
            }

            if (this.isNew && !this.validateOddsSpread()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        private validatePassword(): boolean {
            if (!this.password || this.password.length === 0) {
                this.validationState = new ValidationResult("RequiredField", "password");

                return false;
            }

            var memberPasswordPattern: RegExp = /^((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*[\s])).{6,10}$/;

            if (!memberPasswordPattern.test(this.password)) {
                this.validationState = new ValidationResult("MemberPasswordRules", "password");

                return false;
            }

            if (this.confirmPassword !== this.password) {
                this.validationState = new ValidationResult("ConfirmPasswordMustMatchPassword", "confirmPassword");

                return false;
            }

            return true;
        }

        private validateCredit(): boolean {
            if (!isNumber(this.credit)) {
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

            if (this.credit > this.maxCredit) {
                this.validationState = new ValidationResult(
                    "CreditsMustBeLessThanOrEqualTo",
                    "credit",
                    [this.maxCredit],
                    { precision: 2 });

                return false;
            }

            return true;
        }

        private validateOddsSpread(): boolean {
            if (!this.spreadGroup) {
                this.validationState = new ValidationResult("RequiredField", "spreadGroup");

                return false;
            }

            return true;
        }
    }
}