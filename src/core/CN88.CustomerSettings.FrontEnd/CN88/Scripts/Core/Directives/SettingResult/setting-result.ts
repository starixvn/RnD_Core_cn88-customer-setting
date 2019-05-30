/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../services/scroll-service.ts" />
/// <reference path="../../module-registrater.ts" />

namespace CustomerSetting {
    interface ISettingResultScope extends ng.IScope {
        result: UpdatingSettingResult;
    }

    class SettingResultDirective implements ng.IDirective {
        public static $inject: string[] = ["$rootScope", "scrollService"];

        public replace: boolean = true;

        public get scope(): any {
            return {
                result: "="
            };
        }

        public templateUrl: string = "setting-result.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISettingResultScope) => {
            scope.$watch("result", (result: UpdatingSettingResult) => {
                this.$rootScope.$emit("page-height-changed");

                if (result) {
                    this.scrollService.scrollTo(0, 0);
                }
            });
        };

        private $rootScope: ng.IRootScopeService;
        private scrollService: ng.IWindowService;

        constructor($rootScope: ng.IRootScopeService, scrollService: ng.IWindowService) {
            this.$rootScope = $rootScope;
            this.scrollService = scrollService;
        }
    }

    ModuleRegister.directive("settingResult", SettingResultDirective);
}