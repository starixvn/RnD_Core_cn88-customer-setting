/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../sub-section-base.ts" />

module CustomerSetting {
    "use strict";

    class CustomerCreditDirective extends SubSectionBaseDirective {
        public templateUrl: string = "SubSections/CustomerCredit/customer-credit.htm";
    }

    ModuleRegister.directive("customerCredit", CustomerCreditDirective);
}