/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../core/models/enums/level.ts" />
/// <reference path="../../../core/services/app-config-service.ts" />
/// <reference path="../../../core/services/crossing-setting-types-service-bus.ts" />
/// <reference path="../../../core/services/request-service.ts" />
/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../../models/currency.ts" />
/// <reference path="../../models/member-account-detail.ts" />
/// <reference path="../../models/super-account-detail.ts" />
/// <reference path="../sub-section-base.ts" />

module CustomerSetting {
    "use strict";

    interface IGeneralDataScope extends ISubSectionsSharedScopeScope {
        updateSpreadGroup: Function;
        validateAndLoadHeadlineCurrency: Function;
    }

    class GeneralDataDirective extends SubSectionBaseDirective {
        public static $inject: string[] = ["settingTypesSharedValuesService", "appConfigService", "crossingSettingTypesServiceBus", "requestService"];

        private static validatingAndLoadingHeadlineCurrencyPath: string = "CreateCN88Customer/GetHeadlineCurrency";

        public templateUrl: string = "general-data.htm";

        public link: ng.IDirectiveLinkFn = (scope: IGeneralDataScope) => {
            this.linkInternal(scope);

            if (scope.isNew) {
                if (scope.customerLevel === Level.super) {
                    var requestPath: string = this.appConfigService.buildPath(GeneralDataDirective.validatingAndLoadingHeadlineCurrencyPath);
                    var superAccountDetail: SuperAccountDetail = <SuperAccountDetail>scope.accountDetail;
                    var lastValidatedHeadline: string;

                    scope.validateAndLoadHeadlineCurrency = ($event: JQueryEventObject) => {
                        if (superAccountDetail.uplineUsername
                            && lastValidatedHeadline !== superAccountDetail.uplineUsername
                            && !($event.relatedTarget && $event.relatedTarget.hasAttribute("data-headline-suggestion-item"))) {
                            lastValidatedHeadline = superAccountDetail.uplineUsername;

                            this.requestService.get(
                                requestPath,
                                { username: superAccountDetail.uplineUsername },
                                (response: ng.IHttpPromiseCallbackArg<ICurrency>) => {
                                    if (response.data) {
                                        superAccountDetail.currency = response.data;
                                    } else {
                                        superAccountDetail.validationState = new ValidationResult("InvalidHeadline", "uplineUsername");
                                    }
                                },
                                false);
                        }
                    };
                }

                if (scope.customerLevel === Level.member) {
                    scope.updateSpreadGroup = () => {
                        this.crossingSettingTypesServiceBus.execute(
                            "updateMemberSpreadGroup",
                            {
                                newMemberGroup: (<MemberAccountDetail>scope.accountDetail).spreadGroup.groupName
                            });
                    };
                }
            }
        };

        private appConfigService: AppConfigService;
        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;
        private requestService: RequestService;

        constructor(
            settingTypesSharedValuesService: SettingTypesSharedValuesService,
            appConfigService: AppConfigService,
            crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus,
            requestService: RequestService) {
            super(settingTypesSharedValuesService);

            this.requestService = requestService;
            this.appConfigService = appConfigService;
            this.crossingSettingTypesServiceBus = crossingSettingTypesServiceBus;
        }
    }

    ModuleRegister.directive("generalData", GeneralDataDirective);
}