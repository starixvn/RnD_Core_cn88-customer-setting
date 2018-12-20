/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/enums/level.ts" />
/// <reference path="../models/setting-item.ts" />
/// <reference path="../models/setting-type.ts" />
/// <reference path="../services/settingtypes-shared-values-service.ts" />

module CustomerSetting {
    "use strict";

    interface INewSettingItem {
        (settingItem: any, backup?: boolean, isNew?: boolean, customerLevel?: Level, uplineLevel?: Level, extraData?: Array<KeyValuePair<string, any>>): ISettingItem;
    }

    export interface ISettingTypeDirectiveSharedScope extends ng.IScope {
        customerId: number;
        uplineLevel: Level;
        customerLevel: Level;
        productId: number;
        productName: string;
        settingType: SettingType;
        settingTypeName: string;
        isNew: boolean;
        avaiCustomerLevels: Object;
    }

    export class SettingTypeDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                settingType: "="
            };
        }

        public controller: any = [
            "$scope",
            "settingTypesSharedValuesService",
            ($scope: ISettingTypeDirectiveSharedScope, settingTypesSharedValuesService: SettingTypesSharedValuesService) => {
                $scope.customerId = settingTypesSharedValuesService.customerId;
                $scope.uplineLevel = settingTypesSharedValuesService.uplineLevel;
                $scope.customerLevel = settingTypesSharedValuesService.customerLevel;
                $scope.productId = settingTypesSharedValuesService.productId;
                $scope.productName = settingTypesSharedValuesService.productName;
                $scope.isNew = settingTypesSharedValuesService.isNew;
                $scope.avaiCustomerLevels = Level;

                $scope.$watch("settingType", (settingType: SettingType) => {
                    if (settingType && angular.isArray(settingType.settingItems)) {
                        if (settingType.isError) {
                            if (settingType.settingItems.length > 0) {
                                settingType.settingItems[0] = new ErrorSettingItem(settingType.settingItems[0]);
                            }
                        } else {
                            settingType.settingItems.forEach((item: any, index: number) => {
                                if (item) {
                                    settingType.settingItems[index] = this.settingItemType(
                                        item,
                                        true,
                                        $scope.isNew,
                                        $scope.customerLevel,
                                        $scope.uplineLevel,
                                        settingType.extraData);
                                }
                            });
                        }
                    }
                });
            }];

        private settingItemType: INewSettingItem;

        constructor(settingItemType: INewSettingItem) {
            this.settingItemType = settingItemType;
        }
    }
}