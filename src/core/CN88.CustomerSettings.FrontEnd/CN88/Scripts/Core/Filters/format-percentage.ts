/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function FormatPercentageFilter($filter: ng.IFilterService): Function {
        var defaultPrecision: number = 0;
        var numberFilter: Function = $filter("number");

        return (value: number, precision: number = defaultPrecision) => {
            return numberFilter(value, precision) + "%";
        };
    }

    FormatPercentageFilter.$inject = ["$filter"];
    ModuleRegister.filter("formatPercentage", FormatPercentageFilter);
}