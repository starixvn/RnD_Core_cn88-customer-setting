/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function FormatShortDate($filter: ng.IFilterService, appConfigService: AppConfigService): Function {
        var dateFilter: Function = $filter("date");

        return (value: Date) => {
            return value ? dateFilter(value, appConfigService.shortDateFormat) : "";
        };
    }

    FormatShortDate.$inject = ["$filter", "appConfigService"];
    ModuleRegister.filter("formatShortDate", FormatShortDate);
}