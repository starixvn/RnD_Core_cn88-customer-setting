/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

namespace CustomerSetting {
    class PositionTakingDirective extends SubSectionBaseDirective {
        public templateUrl: string = "position-taking.htm";
    }

    ModuleRegister.directive("positionTaking", PositionTakingDirective);
}