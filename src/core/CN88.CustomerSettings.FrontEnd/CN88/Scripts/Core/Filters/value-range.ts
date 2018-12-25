/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../services/value-range-service.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function ValueRangeFilter(valueRangeService: ValueRangeService) {
        return (value: number, min: number, max: number, step: number, roundingFactor: number) => {
            return valueRangeService.generateRanges(
                value,
                min,
                max,
                step,
                roundingFactor);
        }
    }

    ModuleRegister.filter("valueRange", ValueRangeFilter);
}