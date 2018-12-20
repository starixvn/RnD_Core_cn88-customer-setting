/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/result.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface ISettingResultScope extends ng.IScope {
        result: UpdatingSettingResult;
    }

    class SettingResultDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                result: "="
            };
        }

        public templateUrl: string = "SettingResult/setting-result.htm";

        public controller: any = ["$rootScope", "$scope", "$window", ($rootScope: ng.IRootScopeService, $scope: ISettingResultScope, $window: ng.IWindowService) => {
            $scope.$watch("result", (result: UpdatingSettingResult) => {
                $rootScope.$emit("page-height-changed");

                if (result) {
                    $window.scrollTo(0, 0);
                }
            });
        }];
    }

    ModuleRegister.directive("settingResult", SettingResultDirective);
}