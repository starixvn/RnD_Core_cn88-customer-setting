/// <reference path="../enums/level.ts" />
/// <reference path="abstract-setting-query.ts" />

module CustomerSetting {
    "use strict";

    export class SettingQuery extends AbstractSettingQuery {
        public customerId: number;
        public isNew: boolean;

        constructor(productId: number, uplineId: number, customerLevel: Level, customerId: number, isNew: boolean) {
            super(productId, uplineId, customerLevel);
            this.customerId = customerId;
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