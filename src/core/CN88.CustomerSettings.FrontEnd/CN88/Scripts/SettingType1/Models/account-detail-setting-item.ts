/// <reference path="../../../Libraries/Cryptojs/cryptojs.d.ts" />
/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/models/setting-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class AccountDetailSettingItem extends DefaultSettingItem {
        public username: string;
        public password: string;
        public confirmPassword: string;
        public firstName: string;
        public isAllowedUpdateCredit: boolean;
        public credit: number;
        public minCredit: number;
        public maxCredit: number;
        public uplineUsername: string;

        protected hashKey: string;

        constructor()
        constructor(accountDetail: any)
        constructor(accountDetail: any, isNew: boolean)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            super(accountDetail, isNew, customerLevel);

            this.hashKey = hashKey;

            if (accountDetail) {
                this.username = accountDetail.username;
                this.password = accountDetail.password;
                this.firstName = accountDetail.firstName;
                this.isAllowedUpdateCredit = accountDetail.isAllowedUpdateCredit;
                this.credit = accountDetail.credit;
                this.minCredit = accountDetail.minCredit;
                this.maxCredit = accountDetail.maxCredit;
                this.uplineUsername = accountDetail.uplineUsername;
            }

            this.ignoredProperties = this.ignoredProperties.concat([
                "confirmPassword",
                "isAllowedUpdateCredit",
                "minCredit",
                "maxCredit",
                "hashKey"]);

            if (!this.isNew) {
                this.ignoredProperties = this.ignoredProperties.concat(["username", "password", "uplineUsername"]);
            }

            if (!this.isAllowedUpdateCredit) {
                this.ignoredProperties.push("credit");
            }
        }

        public validate(): boolean {
            if (this.isNew && !this.validateUsername()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        protected validateUsername(): boolean {
            if (!this.username || this.username.length === 0) {
                this.validationState = new ValidationResult("RequiredField", "username");

                return false;
            }

            return true;
        }

        public toJson(): any {
            var accountDetail: any = super.toJson();

            if (this.isNew) {
                var originalPassword: string = accountDetail.password;

                accountDetail.password = originalPassword.encrypt(this.hashKey);
            }

            return accountDetail;
        }
    }
}