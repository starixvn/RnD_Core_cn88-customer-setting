/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/setting.ts" />
/// <reference path="../../models/setting-type.ts" />
/// <reference path="../../helpers.ts" />
/// <reference path="../../module-registrater.ts" />

namespace CustomerSetting {
    interface ISettingTypeLoaderScope extends ng.IScope {
        setting: Setting;
    }

    class SettingTypeDirectiveLoader implements ng.IDirective {
        public static $inject: string[] = ["$compile"];
        public replace: boolean = true;

        public get scope(): any {
            return {
                setting: "="
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: ISettingTypeLoaderScope, element: ng.IAugmentedJQuery) => {
            scope.$watch("setting.settingTypes", () => {
                this.generateNodes(scope, element);
            });
        };

        private $compile: ng.ICompileService;
        private loadedSettingTypes: number[];

        constructor($compile: ng.ICompileService) {
            this.$compile = $compile;
            this.loadedSettingTypes = [];
        }

        private generateNodes(scope: ISettingTypeLoaderScope, element: ng.IAugmentedJQuery): void {
            if (scope.setting && scope.setting.settingTypes && angular.isArray(scope.setting.settingTypes)) {
                scope.setting.settingTypes.forEach((settingType: SettingType, index: number) => {
                    var loaded = this.loadedSettingTypes.some((item: number) => {
                        return item === settingType.id;
                    });

                    if (!loaded) {
                        var settingTypeNodeTemplate: string = StringFormater.format(
                            "<st{0} data-setting-type=\"setting.settingTypes[{1}]\"></st{0}>",
                            settingType.id,
                            index);
                        var settingTypeElement: ng.IAugmentedJQuery = angular.element(settingTypeNodeTemplate);

                        if (index === scope.setting.settingTypes.length - 1) {
                            settingTypeElement.addClass("last-setting-type");
                        }

                        element.append(settingTypeElement);
                        this.$compile(settingTypeElement)(scope);
                        this.loadedSettingTypes.push(settingType.id);
                    }
                });
            }
        }
    }

    ModuleRegister.directive("settingTypeLoader", SettingTypeDirectiveLoader);
}