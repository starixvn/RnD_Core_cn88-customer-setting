/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

namespace CustomerSetting.SettingType1 {
    class SuperPositionTakingDirective implements ng.IDirective {
        public templateUrl: string = "super-position-taking.htm";
        public replace: boolean = true;
    }

    ModuleRegister.directive("st1SuperPositionTaking", SuperPositionTakingDirective);
}