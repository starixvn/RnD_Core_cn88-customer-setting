/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

namespace CustomerSetting.SettingType1 {
    class AgentGeneralDataDirective implements ng.IDirective {
        public templateUrl: string = "agent-general-data.htm";
    }

    ModuleRegister.directive("st1AgentGeneralData", AgentGeneralDataDirective);
}