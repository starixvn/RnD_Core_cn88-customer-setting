/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />
/// <reference path="abstract-setting-query.ts" />


module CustomerSetting {
    "use strict";

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
}