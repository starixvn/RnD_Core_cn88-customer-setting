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

        public controller: any = [
            "$scope",
            "settingTypesSharedValuesService",
            ($scope: ISettingTypeDirectiveSharedScope, settingTypesSharedValuesService: SettingTypesSharedValuesService) => {
                $scope.actorLevel = settingTypesSharedValuesService.actorLevel;
                $scope.customerId = settingTypesSharedValuesService.customerId;
                $scope.uplineId = settingTypesSharedValuesService.uplineId;
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
                                    settingType.settingItems[index] = this.instantiateSettingItem(
                                        item,
                                        true,
                                        $scope.isNew,
                                        $scope.customerLevel,
                                        $scope.uplineId,
                                        settingType.extraData);
                                }
                            });
                        }
                    }
                });
            }];

        private instantiateSettingItem: INewSettingItem;

        constructor(instantiateSettingItem: INewSettingItem) {
            this.instantiateSettingItem = instantiateSettingItem;
        }
    }
}