/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

module CustomerSetting {
    "use strict";

    interface ICustomerCreditScope extends ISubSectionsSharedScopeScope {
        creditPrecision: number;
    }

    class CustomerCreditDirective extends SubSectionBaseDirective {
        public templateUrl: string = "SubSections/CustomerCredit/customer-credit.htm";

        public link: ng.IDirectiveLinkFn = (scope: ICustomerCreditScope) => {
            this.linkInternal(scope);

            scope.creditPrecision = 2;
        };
    }

    ModuleRegister.directive("customerCredit", CustomerCreditDirective);
}