/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />

module CustomerSetting {
    "use strict";

    export class AbstractSettingQuery {
        public uplineId: number;

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

        constructor(productId: number, uplineId: number, customerLevel: Level) {
            this.productId = productId;
            this.uplineId = uplineId;
            this.customerLevel = customerLevel;
        }
    }
}