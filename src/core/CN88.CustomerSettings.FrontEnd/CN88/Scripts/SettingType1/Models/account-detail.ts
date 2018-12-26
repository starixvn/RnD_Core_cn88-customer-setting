/// <reference path="../../../libraries/hash.d.ts" />
/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/models/setting-item.ts" />

module CustomerSetting {
    "use strict";

    export class AccountDetail extends DefaultSettingItem {
        public username: string;
        public password: string;
        public confirmPassword: string;
        public firstName: string;
        public lastName: string;
        public phone: string;
        public mobilePhone: string;
        public fax: string;
        public credit: number;
        public availableCredit: number;
        public usernameSuffixes: string[];
        public uplineUsername: string;
        public interval: number;

        protected isNew: boolean;
        protected customerLevel: Level;
        protected hashKey: string;

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            super(accountDetail);

            this.isNew = isNew;
            this.customerLevel = customerLevel;
            this.hashKey = hashKey;

            if (accountDetail) {
                this.username = accountDetail.username;
                this.password = accountDetail.password;
                this.firstName = accountDetail.firstName;
                this.lastName = accountDetail.lastName;
                this.phone = accountDetail.phone;
                this.mobilePhone = accountDetail.mobilePhone;
                this.fax = accountDetail.fax;
                this.credit = accountDetail.credit;
                this.availableCredit = accountDetail.availableCredit;
                this.usernameSuffixes = accountDetail.usernameSuffixes;
                this.uplineUsername = accountDetail.uplineUsername;
                this.interval = accountDetail.intervalValue;
            }

            if (backup) {
                this.oldSetting = new AccountDetail(this);
            }

            this.resetValidationState();
            this.ignoredProperties = this.ignoredProperties.concat([
                "usernameSuffixes",
                "uplineUsername",
                "interval",
                "isNew",
                "customerLevel",
                "hashKey"]);

            if (!this.isNew) {
                this.ignoredProperties.push("username");
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting = <AccountDetail>this.oldSetting;

            return isChanged
                || this.username !== oldSetting.username
                || this.password !== oldSetting.password
                || this.firstName !== oldSetting.firstName
                || this.lastName !== oldSetting.lastName
                || this.phone !== oldSetting.phone
                || this.mobilePhone !== oldSetting.mobilePhone
                || this.fax !== oldSetting.fax
                || this.credit !== oldSetting.credit;
        }

        public validate(): boolean {
            if (!this.validateUsername()) {
                return false;
            }

            if (!this.validatePassword()) {
                return false;
            }

            if (!this.validateCredit()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        protected validateUsername(): boolean {
            if (this.isNew) {
                if (!this.username || this.username.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "username");

                    return false;
                }
            }

            return true;
        }

        protected validatePassword(): boolean {
            if (this.isNew) {
                if (!this.password || this.password.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "password");

                    return false;
                }

                var strongPassword = /^(?=.*\d)(?=.*[a-zA-Z!@#$%^&*])(?!.*\s).{8,40}$/;

                if (!this.password.match(strongPassword)) {
                    this.validationState = new ValidationResult("InvalidPassword", "password");

                    return false;
                }

                if (this.confirmPassword !== this.password) {
                    this.validationState = new ValidationResult("ConfirmPasswordMustMatchPassword", "confirmPassword");

                    return false;
                }
            }

            return true;
        }

        protected validateCredit(): boolean {
            if (this.credit > this.availableCredit) {
                this.validationState = new ValidationResult("CreditMustBeLessThanOrEqualTo", "credit", [this.availableCredit]);

                return false;
            }

            return true;
        }

        public toJson(): any {
            var accountDetail: any = super.toJson();

            if (this.isNew) {
                var originalPassword: string = accountDetail.password;

                accountDetail.password = originalPassword.encrypt(this.hashKey).hash();
            }

            return accountDetail;
        }
    }
}