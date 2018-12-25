/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function NAValueFilter(): Function {
        return (value: string) => {
            return value || "---";
        };
    }

    ModuleRegister.filter("naValue", NAValueFilter);
}