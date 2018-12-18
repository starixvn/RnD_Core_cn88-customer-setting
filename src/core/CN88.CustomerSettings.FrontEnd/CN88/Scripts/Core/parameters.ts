/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="common.ts" />
/// <reference path="helpers.ts" />
/// <reference path="models.ts" />
/// <reference path="module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class AbstractSettingQuery {
        private prodId: number;
        private uplineLv: Level;
        private custLv: Level;

        public get productId(): number {
            return this.prodId;
        }

        public set productId(productId: number) {
            if (productId <= 0) {
                throw Exceptions.negativeParam("productId");
            }

            this.prodId = productId;
        }

        public get uplineLevel(): Level {
            return this.uplineLv;
        }

        public set uplineLevel(level: Level) {
            if (level < Level.agent) {
                throw Exceptions.invalidParam("uplineLevel");
            }

            if (level === Level.company) {
                level = null;
            }

            this.uplineLv = level;
        }

        public get customerLevel(): Level {
            return this.custLv;
        }

        public set customerLevel(level: Level) {
            if (level < Level.member || level > Level.super) {
                throw Exceptions.wrongCustomerLevel;
            }

            this.custLv = level;
        }

        constructor(productId: number, uplineLevel: Level, customerLevel: Level) {
            this.productId = productId;
            this.uplineLevel = uplineLevel;
            this.customerLevel = customerLevel;
        }
    }

    export class SettingQuery extends AbstractSettingQuery {
        private custId: number;

        public get customerId(): number {
            return this.custId;
        }

        public set customerId(custId: number) {
            if (custId <= 0) {
                throw Exceptions.negativeParam("customerId");
            }

            this.custId = custId;
        }

        constructor(productId: number, uplineLevel: Level, customerLevel: Level, customerId: number) {
            super(productId, uplineLevel, customerLevel);
            this.customerId = customerId;
        }

        public toJson(): {} {
            var params: any = {
                customerId: this.customerId,
                customerLevel: this.customerLevel,
                productId: this.productId
            };

            if (this.uplineLevel) {
                params.uplineLevel = this.uplineLevel;
            }

            return params;
        }
    }

    export class SettingCommand {
        public uplineLevel: Level;
        public setting: Setting;

        constructor(uplineLevel: Level, setting: Setting) {
            this.uplineLevel = uplineLevel;
            this.setting = setting;
        }

        public validate(): Result {
            if ((this.uplineLevel && (this.uplineLevel <= Level.member || this.uplineLevel >= Level.company)) || !this.setting) {
                return Result.invalidParameters;
            }

            return Result.normal;
        }

        public toJson(): {} {
            return {
                uplineLevel: this.uplineLevel,
                setting: JSON.stringify(this.setting.toJson())
            };
        }
    }
}