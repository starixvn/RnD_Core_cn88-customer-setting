/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../module-registrater.ts" />
/// <reference path="setting-field-group.ts" />

namespace CustomerSetting {
    class SettingFieldDirective implements ng.IDirective {
        public require: string = "^settingFieldGroup";
        public restrict: string = "A";

        public link: ng.IDirectiveLinkFn = (
            scope: ng.IScope,
            element: ng.IAugmentedJQuery,
            attributes: ng.IAttributes,
            settingFieldGroupController: ISettingFieldGroupScopeController) => {
            attributes.$observe("settingField", (settingField: string) => {
                settingFieldGroupController.register(settingField, element);
            });
        };
    }

    ModuleRegister.directive("settingField", SettingFieldDirective);
}