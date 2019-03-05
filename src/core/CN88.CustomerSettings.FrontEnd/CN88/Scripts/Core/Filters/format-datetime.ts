/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    function FormatDateTime($filter: ng.IFilterService, appConfigService: AppConfigService): Function {
        var dateFilter: Function = $filter("date");

        return (value: Date) => {
            return value ? dateFilter(value, appConfigService.dateTimeFormat) : "";
        };
    }

    FormatDateTime.$inject = ["$filter", "appConfigService"];
    ModuleRegister.filter("formatDateTime", FormatDateTime);
}