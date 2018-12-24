/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />
/// <reference path="abstract-setting-query.ts" />


module CustomerSetting {
    "use strict";

    export class SettingQuery extends AbstractSettingQuery {
        public customerId: number;
        public isNew: boolean;

        constructor(productId: number, uplineLevel: Level, customerLevel: Level, customerId: number, isNew: boolean) {
            super(productId, uplineLevel, customerLevel);
            this.customerId = customerId;
            this.isNew = isNew;
        }

        public toJson(): {} {
            var params: any = {
                customerId: this.customerId,
                customerLevel: this.customerLevel,
                productId: this.productId,
                isCreateNew: this.isNew
            };

            if (this.uplineLevel) {
                params.uplineLevel = this.uplineLevel;
            }

            return params;
        }
    }
}