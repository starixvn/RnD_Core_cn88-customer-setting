/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

module CustomerSetting {
    "use strict";

    class PositionTakingDirective extends SubSectionBaseDirective {
        public templateUrl: string = "position-taking.htm";
    }

    ModuleRegister.directive("positionTaking", PositionTakingDirective);
}