/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../core/directives/settingtypebase/setting-type-base-directive.ts" />
/// <reference path="../core/models/enums/level.ts" />
/// <reference path="../core/models/setting-item.ts" />
/// <reference path="../core/models/setting-type.ts" />
/// <reference path="../core/services/app-config-service.ts" />
/// <reference path="../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../core/module-registrater.ts" />
/// <reference path="models/setting-type1-item.ts" />
/// <reference path="resources/settingtype1-resource-service.ts" />

module CustomerSetting {
    "use strict";

    class SettingType1Directive extends SettingTypeBaseDirective {
        public static $inject: string[] = [
            "settingType1ResourceService",
            "appConfigService",
            "settingTypesSharedValuesService"
        ];

        public templateUrl: string = "setting-type1.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISettingTypeDirectiveSharedScope) => {
            this.linkInternal(scope);

            scope.settingTypeName = this.settingType1ResourceService.getLabel("AccountDetail");

            if (scope.settingType && angular.isArray(scope.settingType.settingItems)) {
                if (scope.settingType.isError) {
                    if (scope.settingType.settingItems.length > 0) {
                        scope.settingType.settingItems[0] = new ErrorSettingItem(scope.settingType.settingItems[0]);
                    }
                } else {
                    scope.settingType.settingItems.forEach((item: any, index: number) => {
                        if (item) {
                            scope.settingType.settingItems[index] = SettingType1Item.factory(
                                item,
                                true,
                                scope.isNew,
                                scope.customerLevel,
                                this.settingTypesSharedValuesService.hashKey);
                        }
                    });
                }
            }
        };

        private settingType1ResourceService: SettingType1ResourceService;

        constructor(
            settingType1ResourceService: SettingType1ResourceService,
            appConfigService: AppConfigService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            super(appConfigService, settingTypesSharedValuesService, null);

            this.settingType1ResourceService = settingType1ResourceService;
        }
    }

    ModuleRegister.directive("settingtype1", SettingType1Directive);
}