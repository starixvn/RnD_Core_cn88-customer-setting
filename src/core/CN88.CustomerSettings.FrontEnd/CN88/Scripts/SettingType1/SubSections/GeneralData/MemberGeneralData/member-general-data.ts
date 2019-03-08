/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/services/crossing-setting-types-service-bus.ts" />
/// <reference path="../../../../core/module-registrater.ts" />
/// <reference path="../../../models/member-account-detail-setting-item.ts" />
/// <reference path="../../sub-section-base.ts" />

namespace CustomerSetting.SettingType1 {
    interface IMemberGeneralDataScope extends ISubSectionsSharedScopeScope {
        updateSpreadGroup: Function;
    }

    class MemberGeneralDataDirective implements ng.IDirective {
        public static $inject: string[] = ["crossingSettingTypesServiceBus"];

        public templateUrl: string = "member-general-data.htm";

        public link: ng.IDirectiveLinkFn = (scope: IMemberGeneralDataScope) => {
            if (scope.isNew) {
                scope.updateSpreadGroup = () => {
                    this.crossingSettingTypesServiceBus.execute(
                        "updateMemberSpreadGroup",
                        {
                            newMemberGroup: (<MemberAccountDetailSettingItem>scope.accountDetail).spreadGroup.groupName
                        });
                };
            }
        };

        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;

        constructor(crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus) {
            this.crossingSettingTypesServiceBus = crossingSettingTypesServiceBus;
        }
    }

    ModuleRegister.directive("st1MemberGeneralData", MemberGeneralDataDirective);
}