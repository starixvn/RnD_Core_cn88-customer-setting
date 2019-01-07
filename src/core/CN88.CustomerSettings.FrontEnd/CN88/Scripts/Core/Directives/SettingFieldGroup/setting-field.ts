/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/key-value-pair.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../models/setting-item.ts" />
/// <reference path="../../module-registrater.ts" />
/// <reference path="setting-field-group.ts" />

module CustomerSetting {
    "use strict";

    interface ISettingFieldAttributes extends ng.IAttributes {
        settingField: string;
    }

    class SettingFieldDirective implements ng.IDirective {
        public require: string = "^settingFieldGroup";
        public restrict: string = "A";

        public link: ng.IDirectiveLinkFn = (
            scope: ng.IScope,
            element: ng.IAugmentedJQuery,
            attributes: ISettingFieldAttributes,
            settingFieldGroupController: ISettingFieldGroupScopeController) => {
            var fieldName: string = scope.$eval(attributes.settingField) || attributes.settingField;

            settingFieldGroupController.register(fieldName, element);
        };
    }

    ModuleRegister.directive("settingField", SettingFieldDirective);
}