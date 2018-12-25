/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function UnsafeFilter($sce: ng.ISCEService): Function {
        return (value: string) => {
            return $sce.trustAsHtml(value);
        };
    }

    UnsafeFilter.$inject = ["$sce"];
    ModuleRegister.filter("unsafe", UnsafeFilter);
}