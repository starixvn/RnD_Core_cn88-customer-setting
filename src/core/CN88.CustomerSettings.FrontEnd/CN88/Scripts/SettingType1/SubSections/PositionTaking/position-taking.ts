/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

namespace CustomerSetting.SettingType1 {
    class PositionTakingDirective extends SubSectionBaseDirective {
        public templateUrl: string = "position-taking.htm";
    }

    ModuleRegister.directive("st1PositionTaking", PositionTakingDirective);
}