/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

namespace CustomerSetting {
    interface ICustomerCreditScope extends ISubSectionsSharedScopeScope {
        creditPrecision: number;
    }

    class CustomerCreditDirective extends SubSectionBaseDirective {
        public templateUrl: string = "customer-credit.htm";

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