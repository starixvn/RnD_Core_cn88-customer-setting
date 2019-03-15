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

        constructor()
        constructor(accountDetail: any)
        constructor(accountDetail: any, isNew: boolean)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, isNew, customerLevel, hashKey);

                this.lastName = accountDetail.lastName;
                this.phone = accountDetail.phone;
                this.mobilePhone = accountDetail.mobilePhone;
                this.fax = accountDetail.fax;
                this.currency = accountDetail.currency;
                this.isPTReadOnly = accountDetail.isPTReadOnly;
                this.ptEffectiveTime = accountDetail.ptEffectiveTime;
                this.ptItems = accountDetail.ptItems.map((ptItem: any) => new SuperPTItem(ptItem, this.isPTReadOnly));
            }

            this.ignoredProperties = this.ignoredProperties.concat(["isPTReadOnly", "ptEffectiveTime", "ptItems"]);

            if (!this.isNew) {
                this.ignoredProperties.push("currency");
            }
        }

        public isChanged(): boolean {
            return super.isChanged() || this.ptItems.some((ptItem: SuperPTItem) => ptItem.isChanged());
        }

        public toJson(): any {
            var superAccountDetail: any = super.toJson();

            superAccountDetail.ptItems = this.ptItems
                .filter((ptItem: SuperPTItem) => ptItem.isChanged())
                .map((ptItem: SuperPTItem) => ptItem.toJson());

            return superAccountDetail;
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