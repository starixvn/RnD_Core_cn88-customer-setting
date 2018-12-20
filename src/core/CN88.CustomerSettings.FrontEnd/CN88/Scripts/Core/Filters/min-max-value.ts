/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function MinValue(): Function {
        return (numbers: number[]) => {
            return Math.min.apply(Math, numbers);
        };
    }

    ModuleRegister.filter("minValue", MinValue);

    function MaxValue(): Function {
        return (numbers: number[]) => {
            return Math.max.apply(Math, numbers);
        };
    }

    ModuleRegister.filter("maxValue", MaxValue);
}