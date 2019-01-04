/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface ISettingResultScope extends ng.IScope {
        result: UpdatingSettingResult;
    }

    class SettingResultDirective implements ng.IDirective {
        public static $inject: string[] = ["$rootScope", "$window"];

        public replace: boolean = true;

        public get scope(): any {
            return {
                result: "="
            };
        }

        public templateUrl: string = "Directives/SettingResult/setting-result.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISettingResultScope) => {
            scope.$watch("result", (result: UpdatingSettingResult) => {
                this.$rootScope.$emit("page-height-changed");

                if (result) {
                    this.$window.scrollTo(0, 0);
                }
            });
        };

        private $rootScope: ng.IRootScopeService;
        private $window: ng.IWindowService;

        constructor($rootScope: ng.IRootScopeService, $window: ng.IWindowService) {
            this.$rootScope = $rootScope;
            this.$window = $window;
        }
    }

    ModuleRegister.directive("settingResult", SettingResultDirective);
}