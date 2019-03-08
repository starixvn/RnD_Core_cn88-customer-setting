/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

namespace CustomerSetting.SettingType1 {
    class MasterPositionTakingDirective implements ng.IDirective {
        public templateUrl: string = "master-position-taking.htm";
        public replace: boolean = true;
    }

    ModuleRegister.directive("st1MasterPositionTaking", MasterPositionTakingDirective);
}