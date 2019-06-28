/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/services/app-config-service.ts" />
/// <reference path="../../../../core/services/crossing-setting-types-service-bus.ts" />
/// <reference path="../../../../core/services/request-service.ts" />
/// <reference path="../../../../core/module-registrater.ts" />
/// <reference path="../../../models/currency.ts" />
/// <reference path="../../../models/super-account-detail-setting-item.ts" />
/// <reference path="../../sub-section-base.ts" />

namespace CustomerSetting.SettingType1 {
    interface ISuperGeneralDataScope extends ISubSectionsSharedScopeScope {
        validateAndLoadHeadlineCurrency: Function;
    }

    class SuperGeneralDataDirective implements ng.IDirective {
        public static $inject: string[] = ["appConfigService", "crossingSettingTypesServiceBus", "requestService"];

        private static validatingAndLoadingHeadlineCurrencyPath: string = "CreateCN88Customer/GetHeadlineCurrency";

        public templateUrl: string = "super-general-data.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISuperGeneralDataScope) => {
            if (scope.isNew) {
                var requestPath: string = this.appConfigService.buildPath(SuperGeneralDataDirective.validatingAndLoadingHeadlineCurrencyPath);
                var superAccountDetail: SuperAccountDetailSettingItem = <SuperAccountDetailSettingItem>scope.accountDetail;
                var lastValidatedHeadline: string;

                scope.validateAndLoadHeadlineCurrency = ($event: JQueryEventObject) => {
                    if (superAccountDetail.uplineUsername
                        && lastValidatedHeadline !== superAccountDetail.uplineUsername
                        && !($event.relatedTarget && $event.relatedTarget.hasAttribute("data-headline-suggestion-item"))) {
                        lastValidatedHeadline = superAccountDetail.uplineUsername;

                        this.requestService.get(
                            requestPath,
                            { username: superAccountDetail.uplineUsername },
                            (response: ng.IHttpPromiseCallbackArg<any>) => {
                                var responseData = response.data;

                                superAccountDetail.currency = responseData ? <ICurrency>responseData : null;

                                this.crossingSettingTypesServiceBus.execute(
                                    "reloadCurrency",
                                    {
                                        newCommissionAndBetLimits: responseData.betSettings
                                    });
                            },
                            false);
                    }
                };
            }
        };

        private appConfigService: AppConfigService;
        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;
        private requestService: RequestService;

        constructor(
            appConfigService: AppConfigService,
            crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus,
            requestService: RequestService) {
            this.appConfigService = appConfigService;
            this.crossingSettingTypesServiceBus = crossingSettingTypesServiceBus;
            this.requestService = requestService;
        }
    }

    ModuleRegister.directive("st1SuperGeneralData", SuperGeneralDataDirective);
}