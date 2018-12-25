/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../models/account-detail.ts" />

module CustomerSetting {
    "use strict";

    export interface IGeneralDataScope extends ng.IScope {
        customerLevel: Level;
        accountDetail: AccountDetail;
        isNew: boolean;
        avaiCustomerLevels: Object;
    }

    class GeneralDataDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                accountDetail: "="
            };
        }

        public templateUrl: string = "GeneralData/general-data.htm";

        public controller: any = [
            "$scope",
            "settingTypesSharedValuesService",
            ($scope: IGeneralDataScope, settingTypesSharedValuesService: SettingTypesSharedValuesService) => {
                $scope.customerLevel = settingTypesSharedValuesService.customerLevel;
                $scope.isNew = settingTypesSharedValuesService.isNew;
                $scope.avaiCustomerLevels = Level;
            }];
    }

    ModuleRegister.directive("generalData", GeneralDataDirective);
}