/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../core/module-registrater.ts" />
/// <reference path="../core/directives/settingtypebase/setting-type-base-directive.ts" />
/// <reference path="models/setting-type1-item.ts" />
/// <reference path="resources/settingtype1-resource-service.ts" />

module CustomerSetting {
    "use strict";

    class SettingType1Directive extends SettingTypeBaseDirective {
        public static $inject: string[] = ["settingType1ResourceService"];

        public templateUrl: string = "setting-type1.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISettingTypeDirectiveSharedScope) => {
            scope.settingTypeName = this.settingType1ResourceService.getLabel("AccountDetail");
        };

        private settingType1ResourceService: SettingType1ResourceService;

        constructor(settingType1ResourceService: SettingType1ResourceService) {
            super(SettingType1Item.factory);

            this.settingType1ResourceService = settingType1ResourceService;
        }
    }

    ModuleRegister.directive("settingtype1", SettingType1Directive);
}