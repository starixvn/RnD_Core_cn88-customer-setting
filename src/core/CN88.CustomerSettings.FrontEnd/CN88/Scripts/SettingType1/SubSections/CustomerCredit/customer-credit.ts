/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

module CustomerSetting {
    "use strict";

    interface ICustomerCreditScope extends ISubSectionsSharedScopeScope {
        creditPrecision: number;
    }

    class CustomerCreditDirective extends SubSectionBaseDirective {
        public templateUrl: string = "SubSections/CustomerCredit/customer-credit.htm";

        public get scope(): any {
            return {
                accountDetail: "=",
                editable: "=",
                creditPrecision: "=precision"
            };
        }
    }

    ModuleRegister.directive("customerCredit", CustomerCreditDirective);
}