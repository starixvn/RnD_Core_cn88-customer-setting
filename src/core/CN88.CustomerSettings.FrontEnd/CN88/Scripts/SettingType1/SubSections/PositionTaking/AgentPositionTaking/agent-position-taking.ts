/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

namespace CustomerSetting.SettingType1 {
    class AgentPositionTakingDirective implements ng.IDirective {
        public templateUrl: string = "agent-position-taking.htm";
        public replace: boolean = true;
    }

    ModuleRegister.directive("st1AgentPositionTaking", AgentPositionTakingDirective);
}