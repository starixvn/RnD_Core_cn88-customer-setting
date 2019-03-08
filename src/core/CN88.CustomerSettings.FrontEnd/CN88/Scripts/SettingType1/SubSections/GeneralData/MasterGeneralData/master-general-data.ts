/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

namespace CustomerSetting.SettingType1 {
    class MasterGeneralDataDirective implements ng.IDirective {
        public templateUrl: string = "master-general-data.htm";
    }

    ModuleRegister.directive("st1MasterGeneralData", MasterGeneralDataDirective);
}