/// <reference path="../../../libraries/Hash/hash.d.ts" />
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
        public minCredit: number;
        public maxCredit: number;
        public usernameSuffixes: string[];
        public uplineUsername: string;
        public isPTReadOnly: boolean;
        public ptEffectiveTime: Date;

        protected hashKey: string;

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            super(accountDetail, backup, isNew, customerLevel);

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
                this.minCredit = accountDetail.minCredit;
                this.maxCredit = accountDetail.maxCredit;
                this.usernameSuffixes = accountDetail.usernameSuffixes;
                this.uplineUsername = accountDetail.uplineUsername;
                this.isPTReadOnly = accountDetail.isPTReadOnly;
                this.ptEffectiveTime = accountDetail.ptEffectiveTime;
            }

            this.ignoredProperties = this.ignoredProperties.concat([
                "confirmPassword",
                "usernameSuffixes",
                "minCredit",
                "maxCredit",
                "isPTReadOnly",
                "ptEffectiveTime",
                "hashKey"]);

            if (!this.isNew) {
                this.ignoredProperties.push("uplineUsername");
                this.ignoredProperties.push("username");
            }
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

        protected validatePassword(): boolean {
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

        protected validateCredit(): boolean {
            if (this.credit === undefined || this.credit === null) {
                this.validationState = new ValidationResult("RequiredField", "credit");

                return false;
            }

            if (this.credit < this.minCredit) {
                this.validationState = new ValidationResult(
                    "CreditMustBeGreaterThanOrEqualTo",
                    "credit",
                    [this.minCredit],
                    { precision: 2 });

                return false;
            }

            if (this.credit > this.maxCredit) {
                this.validationState = new ValidationResult(
                    "CreditMustBeLessThanOrEqualTo",
                    "credit",
                    [this.maxCredit],
                    { precision: 2 });

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