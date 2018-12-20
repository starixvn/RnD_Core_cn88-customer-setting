/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function Unsafe($sce: ng.ISCEService): Function {
        return (value: string) => {
            return $sce.trustAsHtml(value);
        };
    }

    Unsafe.$inject = ["$sce"];
    ModuleRegister.filter("unsafe", Unsafe);
}