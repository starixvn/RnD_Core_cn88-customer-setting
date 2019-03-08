/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

namespace CustomerSetting.SettingType1 {
    class GeneralDataDirective extends SubSectionBaseDirective {
        public templateUrl: string = "general-data.htm";
    }

    ModuleRegister.directive("st1GeneralData", GeneralDataDirective);
}