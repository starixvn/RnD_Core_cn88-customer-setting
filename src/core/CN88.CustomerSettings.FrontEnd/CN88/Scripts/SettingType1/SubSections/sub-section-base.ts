/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../models/setting-type1-item.ts" />

module CustomerSetting {
    "use strict";

    export interface ISubSectionsSharedScopeScope extends ng.IScope {
        customerLevel: Level;
        accountDetail: AccountDetail;
        isNew: boolean;
        avaiCustomerLevels: Object;
    }

    export class SubSectionBaseDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                accountDetail: "="
            };
        }

        public controller: any = [
            "$scope",
            "settingTypesSharedValuesService",
            ($scope: ISubSectionsSharedScopeScope, settingTypesSharedValuesService: SettingTypesSharedValuesService) => {
                $scope.customerLevel = settingTypesSharedValuesService.customerLevel;
                $scope.isNew = settingTypesSharedValuesService.isNew;
                $scope.avaiCustomerLevels = Level;
            }];
    }
}