/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />

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
}