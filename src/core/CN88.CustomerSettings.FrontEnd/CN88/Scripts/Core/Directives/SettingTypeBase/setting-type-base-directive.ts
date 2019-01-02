/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/level.ts" />
/// <reference path="../../models/setting-item.ts" />
/// <reference path="../../models/setting-type.ts" />
/// <reference path="../../services/setting-types-shared-values-service.ts" />

module CustomerSetting {
    "use strict";

    interface INewSettingItem {
        (settingItem: any, backup?: boolean, isNew?: boolean, customerLevel?: Level, uplineId?: number, extraData?: Array<KeyValuePair<string, any>>): ISettingItem;
    }

    export interface ISettingTypeDirectiveSharedScope extends ng.IScope {
        actorLevel: Level;
        customerId: number;
        uplineId: number;
        customerLevel: Level;
        productId: number;
        productName: string;
        settingType: SettingType;
        settingTypeName: string;
        isNew: boolean;
        avaiCustomerLevels: Object;
    }

    export class SettingTypeBaseDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                settingType: "="
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: ISettingTypeDirectiveSharedScope) => {
            this.linkInternal(scope);
        };

        protected linkInternal(scope: ISettingTypeDirectiveSharedScope): void {
            scope.actorLevel = this.settingTypesSharedValuesService.actorLevel;
            scope.customerId = this.settingTypesSharedValuesService.customerId;
            scope.uplineId = this.settingTypesSharedValuesService.uplineId;
            scope.customerLevel = this.settingTypesSharedValuesService.customerLevel;
            scope.productId = this.settingTypesSharedValuesService.productId;
            scope.productName = this.settingTypesSharedValuesService.productName;
            scope.isNew = this.settingTypesSharedValuesService.isNew;
            scope.avaiCustomerLevels = Level;

            if (this.instantiateSettingItem) {
                if (scope.settingType && angular.isArray(scope.settingType.settingItems)) {
                    if (scope.settingType.isError) {
                        if (scope.settingType.settingItems.length > 0) {
                            scope.settingType.settingItems[0] = new ErrorSettingItem(scope.settingType.settingItems[0]);
                        }
                    } else {
                        scope.settingType.settingItems.forEach((item: any, index: number) => {
                            if (item) {
                                scope.settingType.settingItems[index] = this.instantiateSettingItem(
                                    item,
                                    true,
                                    scope.isNew,
                                    scope.customerLevel,
                                    scope.uplineId,
                                    scope.settingType.extraData);
                            }
                        });
                    }
                }
            }
        }

        protected instantiateSettingItem: INewSettingItem;
        protected settingTypesSharedValuesService: SettingTypesSharedValuesService;

        constructor(settingTypesSharedValuesService: SettingTypesSharedValuesService, instantiateSettingItem: INewSettingItem) {
            this.instantiateSettingItem = instantiateSettingItem;
            this.settingTypesSharedValuesService = settingTypesSharedValuesService;
        }
    }
}