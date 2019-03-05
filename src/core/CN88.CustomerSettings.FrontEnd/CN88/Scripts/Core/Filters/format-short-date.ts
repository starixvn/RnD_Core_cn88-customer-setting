/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    function FormatShortDate($filter: ng.IFilterService, appConfigService: AppConfigService): Function {
        var dateFilter: Function = $filter("date");

        return (value: Date) => {
            return value ? dateFilter(value, appConfigService.shortDateFormat) : "";
        };
    }

    FormatShortDate.$inject = ["$filter", "appConfigService"];
    ModuleRegister.filter("formatShortDate", FormatShortDate);
}