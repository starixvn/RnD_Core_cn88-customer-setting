/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/display-mode.ts" />
/// <reference path="../../models/enums/level.ts" />
/// <reference path="../../models/key-value-pair.ts" />
/// <reference path="../../models/setting-item.ts" />
/// <reference path="../../models/setting-type.ts" />
/// <reference path="../../services/app-config-service.ts" />
/// <reference path="../../services/setting-types-shared-values-service.ts" />

module CustomerSetting {
    "use strict";

    interface INewSettingItem {
        (settingItem: any, backup?: boolean, isNew?: boolean, customerLevel?: Level, uplineId?: number): ISettingItem;
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
        editable: boolean;
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

        protected appConfigService: AppConfigService;
        protected settingTypesSharedValuesService: SettingTypesSharedValuesService;
        protected instantiateSettingItem: INewSettingItem;

        constructor(
            appConfigService: AppConfigService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService,
            instantiateSettingItem: INewSettingItem) {
            this.appConfigService = appConfigService;
            this.settingTypesSharedValuesService = settingTypesSharedValuesService;
            this.instantiateSettingItem = instantiateSettingItem;
        }

        protected linkInternal(scope: ISettingTypeDirectiveSharedScope): void {
            scope.actorLevel = this.settingTypesSharedValuesService.actorLevel;
            scope.customerId = this.settingTypesSharedValuesService.customerId;
            scope.uplineId = this.settingTypesSharedValuesService.uplineId;
            scope.customerLevel = this.settingTypesSharedValuesService.customerLevel;
            scope.productId = this.settingTypesSharedValuesService.productId;
            scope.productName = this.settingTypesSharedValuesService.productName;
            scope.isNew = this.settingTypesSharedValuesService.isNew;
            scope.editable = this.appConfigService.canUpdate() && scope.settingType.displayMode === DisplayMode.editable;
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
                                    scope.uplineId);
                            }
                        });
                    }
                }
            }
        }
    }
}