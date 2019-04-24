/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

namespace CustomerSetting.SettingType1 {
    class CustomerCreditDirective extends SubSectionBaseDirective {
        public templateUrl: string = "customer-credit.htm";

        public get scope(): any {
            return {
                accountDetail: "=",
                isEditable: "=",
                creditPrecision: "=precision"
            };
        }
    }

    ModuleRegister.directive("st1CustomerCredit", CustomerCreditDirective);
}