/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />

module CustomerSetting {
    "use strict";

    export class CustomerInfo {
        public username: string;
        public password: string;
        public confirmPassword: string;
        public firstName: string;
        public lastName: string;
        public phone: string;
        public mobilePhone: string;
        public fax: string;
        public currencyId: number;
        public currencyName: string;
        public currencies: any[];
        public credit: number;
        public availableCredit: number;
        public uGroup: string;
        public uGroups: string[];
        public usernameSuffix: string[];
        public uplineUsername: string;
        public validationState: ValidationResult;

        private isNew: boolean;
        private customerLevel: Level;
        private oldCustomerInfo: CustomerInfo;

        constructor();
        constructor(customerInfo: any);
        constructor(customerInfo: any, backup: boolean)
        constructor(customerInfo: any, backup: boolean, isNew: boolean)
        constructor(customerInfo: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(customerInfo?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            this.isNew = isNew;
            this.customerLevel = customerLevel;

            if (customerInfo) {
                this.username = customerInfo.username;
                this.password = customerInfo.password;
                this.firstName = customerInfo.firstName;
                this.lastName = customerInfo.lastName;
                this.phone = customerInfo.phone;
                this.mobilePhone = customerInfo.mobilePhone;
                this.fax = customerInfo.fax;
                this.currencyId = customerInfo.currencyId;
                this.currencyName = customerInfo.currencyName;
                this.currencies = customerInfo.currencies;
                this.credit = customerInfo.credit;
                this.availableCredit = customerInfo.availableCredit;
                this.uGroup = customerInfo.uGroup;
                this.usernameSuffix = customerInfo.usernameSuffix;
                this.uGroups = customerInfo.uGroups;
                this.uplineUsername = customerInfo.uplineUsername;
            }

            if (backup) {
                this.oldCustomerInfo = new CustomerInfo(this);
            }

            this.resetValidationState();
        }

        public isChanged(): boolean {
            return this.oldCustomerInfo
                && (this.username !== this.oldCustomerInfo.username
                    || this.password !== this.oldCustomerInfo.password
                    || this.firstName !== this.oldCustomerInfo.firstName
                    || this.lastName !== this.oldCustomerInfo.lastName
                    || this.phone !== this.oldCustomerInfo.phone
                    || this.mobilePhone !== this.oldCustomerInfo.mobilePhone
                    || this.fax !== this.oldCustomerInfo.fax
                    || this.currencyId !== this.oldCustomerInfo.currencyId
                    || this.credit !== this.oldCustomerInfo.credit
                    || this.uGroup !== this.oldCustomerInfo.uGroup);
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

        private validateUsername(): boolean {
            if (this.isNew) {
                if (!this.username || this.username.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "username");

                    return false;
                }
            }

            return true;
        }

        private validatePassword(): boolean {
            if (this.isNew) {
                if (!this.password || this.password.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "password");

                    return false;
                }

                if (this.customerLevel !== Level.super && this.confirmPassword !== this.password) {
                    this.validationState = new ValidationResult("ConfirmPasswordMustMatchPassword", "confirmPassword");

                    return false;
                }

                if (this.customerLevel !== Level.super) {
                    var strongPassword = /^(?=.*\d)(?=.*[a-zA-Z!@#$%^&*])(?!.*\s).{8,40}$/;

                    if (!this.password.match(strongPassword)) {
                        this.validationState = new ValidationResult("PasswordRules", "password");

                        return false;
                    }
                }
            }

            return true;
        }

        private validateCredit(): boolean {
            if (this.customerLevel !== Level.super || this.credit > this.availableCredit) {
                this.validationState = new ValidationResult("CreditMustBeLessThanOrEqualTo", "credit", [this.availableCredit]);

                return false;
            }

            return true;
        }

        public resetValidationState(): void {
            this.validationState = <ValidationResult>Result.normal;
        }

        public toJson(): any {
            var json: any = {
                username: this.username,
                firstName: this.firstName,
                credit: this.credit
            };

            if (this.isNew) {
                // @@TODO: encrypt password
                json.password = this.password;

                if (this.customerLevel === Level.super) {
                    json.currencyId = this.currencyId;
                } else if (this.customerLevel === Level.member) {
                    json.uGroup = this.uGroup;
                }
            }

            if (this.customerLevel === Level.super) {
                json.lastName = this.lastName;
                json.phone = this.phone;
                json.mobilePhone = this.mobilePhone;
                json.fax = this.fax;
            }
        }
    }
}