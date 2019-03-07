/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="account-detail-setting-item.ts" />
/// <reference path="currency.ts" />
/// <reference path="super-pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class SuperAccountDetailSettingItem extends AccountDetailSettingItem {
        public lastName: string;
        public phone: string;
        public mobilePhone: string;
        public fax: string;
        public currency: ICurrency;
        public isPTReadOnly: boolean;
        public ptEffectiveTime: Date;
        public ptItems: SuperPTItem[];

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel, hashKey);

                this.lastName = accountDetail.lastName;
                this.phone = accountDetail.phone;
                this.mobilePhone = accountDetail.mobilePhone;
                this.fax = accountDetail.fax;
                this.currency = accountDetail.currency;
                this.isPTReadOnly = accountDetail.isPTReadOnly;
                this.ptEffectiveTime = accountDetail.ptEffectiveTime;
                this.ptItems = accountDetail.ptItems.map((ptItem: any) => new SuperPTItem(ptItem, accountDetail.isPTReadOnly));
            }

            this.ignoredProperties = this.ignoredProperties.concat([
                "isPTReadOnly",
                "ptEffectiveTime"]);

            if (!this.isNew) {
                this.ignoredProperties.push("currency");
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

            if (!this.isPTReadOnly) {
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
    }
}