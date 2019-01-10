/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />

module CustomerSetting {
    "use strict";

    export class SettingQuery {
        public customerId: number;
        public uplineId: number;
        public isNew: boolean;

        public get productId(): number {
            return this.prodId;
        }

        public set productId(productId: number) {
            if (productId <= 0) {
                throw Exceptions.negativeParam("productId");
            }

            this.prodId = productId;
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

        private prodId: number;
        private custLv: Level;

        constructor(productId: number, uplineId: number, customerLevel: Level, customerId: number, isNew: boolean) {
            this.customerId = customerId;
            this.productId = productId;
            this.uplineId = uplineId;
            this.customerLevel = customerLevel;
            this.isNew = isNew;
        }

        public toJson(): {} {
            var params: any = {
                customerId: this.customerId,
                customerLevel: this.customerLevel,
                productId: this.productId,
                isCreateNew: this.isNew,
                uplineId: this.uplineId
            };

            return params;
        }
    }
}