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
        cssClass: string;
    }

    export class DefaultSettingTypeGroupingDirective extends SettingTypeBaseDirective {
        public templateUrl: string = "Directives/DefaultSettingTypeBlock/default-setting-type-block.htm";

        public link: ng.IDirectiveLinkFn = (scope: IDefaultSettingTypeDirectiveScope) => {
            this.linkInternal(scope);
        };

        protected linkInternal(scope: IDefaultSettingTypeDirectiveScope): void {
            super.linkInternal(scope);

            scope.avaiSettingLevels = SettingLevel;

            if (scope.settingType && angular.isArray(scope.settingType.settingItems)) {
                if (scope.settingType.settingLevel === SettingLevel.sportTypeBetType) {
                    scope.sportGroups = this.groupSettingItemsBySportType(scope.settingType.settingItems);
                } else {
                    scope.sportGroups = <DefaultSettingItem[][]>[scope.settingType.settingItems];
                }
            }
        }

        protected groupSettingItemsBySportType(settingItems: ISettingItem[]): DefaultSettingItem[][] {
            var currentSportTypeId: number = -1;
            var result: DefaultSettingItem[][] = [];
            var group: DefaultSettingItem[];

            settingItems.forEach((item: ISettingItem) => {
                var settingItem = <DefaultSettingItem>item;

                if (currentSportTypeId !== settingItem.sportTypeId) {
                    if (group) {
                        result.push(group);
                    }

                    currentSportTypeId = settingItem.sportTypeId;
                    group = [];
                }

                group.push(settingItem);
            });

            if (group) {
                result.push(group);
            }

            return result;
        }
    }
}