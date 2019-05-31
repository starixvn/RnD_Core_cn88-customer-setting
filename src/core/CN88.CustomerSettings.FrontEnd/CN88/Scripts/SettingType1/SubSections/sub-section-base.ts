/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../Models/account-detail-setting-item.ts" />

namespace CustomerSetting.SettingType1 {
    export interface ISubSectionsSharedScopeScope extends ng.IScope {
        customerLevel: Level;
        accountDetail: AccountDetailSettingItem;
        isNew: boolean;
        isEditable: boolean;
        isMobileView: boolean;
        avaiCustomerLevels: Object;
    }

    export class SubSectionBaseDirective implements ng.IDirective {
        public static $inject: string[] = ["settingTypesSharedValuesService"];

        public replace: boolean = true;

        public get scope(): any {
            return {
                accountDetail: "=",
                isEditable: "="
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: ISubSectionsSharedScopeScope) => {
            this.linkInternal(scope);
        };

        private settingTypesSharedValuesService: SettingTypesSharedValuesService;

        constructor(settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            this.settingTypesSharedValuesService = settingTypesSharedValuesService;
        }

        protected linkInternal(scope: ISubSectionsSharedScopeScope): void {
            scope.customerLevel = this.settingTypesSharedValuesService.customerLevel;
            scope.isNew = this.settingTypesSharedValuesService.isNew;
            scope.isMobileView = this.settingTypesSharedValuesService.isMobileView;
            scope.avaiCustomerLevels = Level;
        }
    }
}