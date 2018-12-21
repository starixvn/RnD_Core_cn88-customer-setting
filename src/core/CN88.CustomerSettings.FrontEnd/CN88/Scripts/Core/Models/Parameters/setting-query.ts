/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />
/// <reference path="abstract-setting-query.ts" />


module CustomerSetting {
    "use strict";

    export class SettingQuery extends AbstractSettingQuery {
        public customerId: number;

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