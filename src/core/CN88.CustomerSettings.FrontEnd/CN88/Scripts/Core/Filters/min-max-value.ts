/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function MinValueFilter(): Function {
        return (numbers: number[]) => {
            return Math.min.apply(Math, numbers);
        };
    }

    ModuleRegister.filter("minValue", MinValueFilter);

    function MaxValueFilter(): Function {
        return (numbers: number[]) => {
            return Math.max.apply(Math, numbers);
        };
    }

    ModuleRegister.filter("maxValue", MaxValueFilter);
}