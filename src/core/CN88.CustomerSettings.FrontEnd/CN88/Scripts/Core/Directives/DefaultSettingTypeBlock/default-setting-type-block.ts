/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/setting-level.ts" />
/// <reference path="../../models/setting-item.ts" />
/// <reference path="../settingtypebase/setting-type-base-directive.ts" />

module CustomerSetting {
    "use strict";

    export interface IDefaultSettingTypeDirectiveScope extends ISettingTypeDirectiveSharedScope {
        avaiSettingLevels: Object;
        sportGroups: DefaultSettingItem[][];
        headerTemplateUrl: string;
        valueTemplateUrl: string;
    }

    export class DefaultSettingTypeGroupingDirective implements ng.IDirective {
        public replace: boolean = true;
        public templateUrl: string = "Directives/DefaultSettingTypeBlock/default-setting-type-block.htm";

        public controller: any = ["$scope", ($scope: IDefaultSettingTypeDirectiveScope) => {
            $scope.avaiSettingLevels = SettingLevel;

            $scope.$watch("settingType", (settingType: SettingType) => {
                if (settingType && angular.isArray(settingType.settingItems)) {
                    if (settingType.settingLevel === SettingLevel.sportTypeBetType) {
                        $scope.sportGroups = this.groupSettingItemsBySportType(settingType.settingItems);
                    } else {
                        $scope.sportGroups = <DefaultSettingItem[][]>[settingType.settingItems];
                    }

                    this.reorderBetTypesOfSports($scope.sportGroups);
                }
            });
        }];

        protected groupSettingItemsBySportType(settingItems: ISettingItem[]): DefaultSettingItem[][] {
            var currentSportTypeId: number = -1;
            var result: Array<Array<DefaultSettingItem>> = new Array<Array<DefaultSettingItem>>();
            var group: Array<DefaultSettingItem>;

            settingItems.forEach((item: ISettingItem) => {
                var settingItem = <DefaultSettingItem>item;

                if (currentSportTypeId !== settingItem.sportTypeId) {
                    if (group) {
                        result.push(group);
                    }

                    currentSportTypeId = settingItem.sportTypeId;
                    group = new Array<DefaultSettingItem>();
                }

                group.push(settingItem);
            });

            if (group) {
                result.push(group);
            }

            return result;
        }

        protected reorderBetTypesOfSports(originalSportGroups: DefaultSettingItem[][]): void {
            originalSportGroups.forEach((group: DefaultSettingItem[]) => {
                group.sort((firstSettingItem: DefaultSettingItem, secondSettingItem: DefaultSettingItem) => {
                    if (firstSettingItem.displayOrder > secondSettingItem.displayOrder) {
                        return 1;
                    }

                    if (firstSettingItem.displayOrder < secondSettingItem.displayOrder) {
                        return -1;
                    }

                    return 0;
                });
            });
        }
    }

    ModuleRegister.directive("defaultSettingTypeGrouping", DefaultSettingTypeGroupingDirective);
}