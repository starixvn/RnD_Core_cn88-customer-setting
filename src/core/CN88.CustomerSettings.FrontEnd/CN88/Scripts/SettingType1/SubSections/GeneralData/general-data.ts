/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../core/models/enums/level.ts" />
/// <reference path="../../../core/services/crossing-setting-types-service-bus.ts" />
/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../../models/member-account-detail.ts" />
/// <reference path="../sub-section-base.ts" />

module CustomerSetting {
    "use strict";

    interface IGeneralDataScope extends ISubSectionsSharedScopeScope {
        updateSpreadGroup: Function;
    }

    class GeneralDataDirective extends SubSectionBaseDirective {
        public static $inject: string[] = ["settingTypesSharedValuesService", "crossingSettingTypesServiceBus"];

        public templateUrl: string = "general-data.htm";

        public link: ng.IDirectiveLinkFn = (scope: IGeneralDataScope) => {
            this.linkInternal(scope);

            if (scope.isNew && scope.customerLevel === Level.member) {
                scope.updateSpreadGroup = () => {
                    this.crossingSettingTypesServiceBus.execute(
                        "updateMemberSpreadGroup",
                        {
                            newMemberGroup: (<MemberAccountDetail>scope.accountDetail).spreadGroup.groupName
                        });
                };
            }
        };

        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;

        constructor(settingTypesSharedValuesService: SettingTypesSharedValuesService, crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus) {
            super(settingTypesSharedValuesService);

            this.crossingSettingTypesServiceBus = crossingSettingTypesServiceBus;
        }
    }

    ModuleRegister.directive("generalData", GeneralDataDirective);
}