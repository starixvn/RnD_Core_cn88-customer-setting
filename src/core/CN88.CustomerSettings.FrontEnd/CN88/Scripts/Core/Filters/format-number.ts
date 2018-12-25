/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function FormatNumberFilter($filter: ng.IFilterService): Function {
        var numberFilter = $filter("number");

        return (value: number, factor?: number) => {
            factor = factor || value.toString().indexOf(".") > 0 ? 2 : 0;

            return numberFilter(value, factor);
        };
    }

    FormatNumberFilter.$inject = ["$filter"];
    ModuleRegister.filter("formatNumber", FormatNumberFilter);
}