/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="account-detail-setting-item.ts" />
/// <reference path="master-pt-item.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class MasterAccountDetailSettingItem extends AccountDetailSettingItem {
        public usernameSuffixes: string[];
        public isPTReadOnly: boolean;
        public ptEffectiveTime: Date;
        public ptItems: PTItem[];

        constructor()
        constructor(accountDetail: any)
        constructor(accountDetail: any, isNew: boolean)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, isNew, customerLevel, hashKey);

                this.usernameSuffixes = accountDetail.usernameSuffixes;
                this.isPTReadOnly = accountDetail.isPTReadOnly;
                this.ptEffectiveTime = accountDetail.ptEffectiveTime;
                this.ptItems = accountDetail.ptItems.map((ptItem: any) => new MasterPTItem(ptItem, accountDetail.isPTReadOnly));
            }

            this.ignoredProperties = this.ignoredProperties.concat([
                "usernameSuffixes",
                "isPTReadOnly",
                "ptEffectiveTime",
                "ptItems"]);
        }

        public isChanged(): boolean {
            return super.isChanged() || this.ptItems.some((ptItem: PTItem) => ptItem.isChanged());
        }

        public toJson(): any {
            var accountDetail: any = super.toJson();

            accountDetail.ptItems = this.ptItems.map((ptItem: PTItem) => ptItem.toJson());

            return accountDetail;
        }

        public validate(): boolean {
            if (this.isNew && !this.validateUsername()) {
                return false;
            }

            if (this.isNew && !this.validatePassword()) {
                return false;
            }

            if (!this.validateCredit()) {
                return false;
            }

            if (this.isNew || !this.isPTReadOnly) {
                var { isValid, validationState } = PTItem.validatePTItems(this.ptItems);

                if (!isValid) {
                    this.validationState = validationState;

                    return false;
                }
            }

            this.resetValidationState();

            return true;
        }

        private validatePassword(): boolean {
            if (!this.password || this.password.length === 0) {
                this.validationState = new ValidationResult("RequiredField", "password");

                return false;
            }

            var passwordPattern: RegExp
                = /^(((?=.*\d)(?=.*[a-z])(?=.*[A-Z])|(?=.*\d)(?=.*[a-zA-Z])(?=.*[\W_])|(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]))(?!.*[\s])).{8,16}$/;

            if (!passwordPattern.test(this.password)) {
                this.validationState = new ValidationResult("PasswordRules", "password");

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
    }
}