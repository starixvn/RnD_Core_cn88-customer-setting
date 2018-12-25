/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../models/account-detail.ts" />

module CustomerSetting {
    "use strict";

    export interface ICustomerCreditScope extends ng.IScope {
        customerLevel: Level;
        accountDetail: AccountDetail;
        isNew: boolean;
        avaiCustomerLevels: Object;
    }

    class CustomerCreditDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                accountDetail: "="
            };
        }

        public templateUrl: string = "CustomerCredit/customer-credit.htm";

        public controller: any = [
            "$scope",
            "settingTypesSharedValuesService",
            ($scope: ICustomerCreditScope, settingTypesSharedValuesService: SettingTypesSharedValuesService) => {
                $scope.customerLevel = settingTypesSharedValuesService.customerLevel;
                $scope.isNew = settingTypesSharedValuesService.isNew;
                $scope.avaiCustomerLevels = Level;
            }];
    }

    ModuleRegister.directive("customerCredit", CustomerCreditDirective);
}