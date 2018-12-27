/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../models/account-detail.ts" />

module CustomerSetting {
    "use strict";

    interface IPositionTakingScope extends ng.IScope {
        customerLevel: Level;
        accountDetail: AccountDetail;
        isNew: boolean;
        avaiCustomerLevels: Object;
    }

    class PositionTakingDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                accountDetail: "="
            };
        }

        public templateUrl: string = "PositionTaking/position-taking.htm";

        public controller: any = [
            "$scope",
            "settingTypesSharedValuesService",
            ($scope: IPositionTakingScope, settingTypesSharedValuesService: SettingTypesSharedValuesService) => {
                $scope.customerLevel = settingTypesSharedValuesService.customerLevel;
                $scope.isNew = settingTypesSharedValuesService.isNew;
                $scope.avaiCustomerLevels = Level;
            }];
    }

    ModuleRegister.directive("positionTaking", PositionTakingDirective);
}