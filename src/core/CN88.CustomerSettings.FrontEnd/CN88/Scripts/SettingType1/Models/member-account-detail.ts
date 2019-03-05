/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />
/// <reference path="spread-group.ts" />

namespace CustomerSetting.SettingType1 {
    export class MemberAccountDetail extends AccountDetail {
        public spreadGroups: ISpreadGroup[];
        public spreadGroup: ISpreadGroup;

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel, hashKey);

                this.spreadGroups = accountDetail.spreadGroups;
                this.spreadGroup = accountDetail.spreadGroup;
            }

            this.ignoredProperties.push("spreadGroups");

            if (!this.isNew) {
                this.ignoredProperties.push("spreadGroup");
            }
        }

        public validate(): boolean {
            if (!this.validateUsername()) {
                return false;
            }

            if (this.isNew && !this.validateMemberPassword()) {
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

        protected validateMemberPassword(): boolean {
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

        private validateOddsSpread(): boolean {
            if (!this.spreadGroup) {
                this.validationState = new ValidationResult("RequiredField", "spreadGroup");

                return false;
            }

            return true;
        }
    }
}