/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../core/directives/settingtypebase/setting-type-base-directive.ts" />
/// <reference path="../core/models/enums/level.ts" />
/// <reference path="../core/models/setting-item.ts" />
/// <reference path="../core/models/setting-type.ts" />
/// <reference path="../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../core/module-registrater.ts" />
/// <reference path="models/setting-type1-item.ts" />
/// <reference path="resources/settingtype1-resource-service.ts" />

module CustomerSetting {
    "use strict";

    class SettingType1Directive implements ng.IDirective {
        public static $inject: string[] = [
            "settingType1ResourceService",
            "settingTypesSharedValuesService"
        ];

        public replace: boolean = true;

        public get scope(): any {
            return {
                settingType: "="
            };
        }

        public templateUrl: string = "setting-type1.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISettingTypeDirectiveSharedScope) => {
            scope.settingTypeName = this.settingType1ResourceService.getLabel("AccountDetail");
            scope.actorLevel = this.settingTypesSharedValuesService.actorLevel;
            scope.customerId = this.settingTypesSharedValuesService.customerId;
            scope.uplineId = this.settingTypesSharedValuesService.uplineId;
            scope.customerLevel = this.settingTypesSharedValuesService.customerLevel;
            scope.productId = this.settingTypesSharedValuesService.productId;
            scope.productName = this.settingTypesSharedValuesService.productName;
            scope.isNew = this.settingTypesSharedValuesService.isNew;
            scope.avaiCustomerLevels = Level;

            scope.$watch("settingType", (settingType: SettingType) => {
                if (settingType && angular.isArray(settingType.settingItems)) {
                    if (settingType.isError) {
                        if (settingType.settingItems.length > 0) {
                            settingType.settingItems[0] = new ErrorSettingItem(settingType.settingItems[0]);
                        }
                    } else {
                        settingType.settingItems.forEach((item: any, index: number) => {
                            if (item) {
                                settingType.settingItems[index] = SettingType1Item.factory(
                                    item,
                                    true,
                                    scope.isNew,
                                    scope.customerLevel,
                                    this.settingTypesSharedValuesService.hashKey);
                            }
                        });
                    }
                }
            });
        };

        private settingType1ResourceService: SettingType1ResourceService;
        private settingTypesSharedValuesService: SettingTypesSharedValuesService

        constructor(settingType1ResourceService: SettingType1ResourceService, settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            this.settingType1ResourceService = settingType1ResourceService;
            this.settingTypesSharedValuesService = settingTypesSharedValuesService;
        }
    }

    ModuleRegister.directive("settingtype1", SettingType1Directive);
}