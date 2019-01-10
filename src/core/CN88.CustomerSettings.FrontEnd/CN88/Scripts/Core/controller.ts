/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="models/parameters/setting-command.ts" />
/// <reference path="models/parameters/setting-query.ts" />
/// <reference path="models/defaults.ts" />
/// <reference path="models/result.ts" />
/// <reference path="models/setting.ts" />
/// <reference path="resources/resource-service.ts" />
/// <reference path="services/app-config-service.ts" />
/// <reference path="services/parameters-service.ts" />
/// <reference path="services/request-service.ts" />
/// <reference path="services/setting-types-shared-values-service.ts" />

module CustomerSetting {
    "use strict";

    interface ISuccessfulUpdatingCallback {
        (isSuccessful: boolean): any;
    }

    class CustomerSettingController {
        public static $inject: string[] = [
            "$window",
            "$timeout",
            "appConfigService",
            "parametersService",
            "resourceService",
            "requestService",
            "settingTypesSharedValuesService"];

        public systemId: number;
        public setting: Setting;
        public warningMessages: Result | UpdatingSettingResult;
        public requestMsgResult: Result | UpdatingSettingResult;

        private $window: ng.IWindowService;
        private $timeout: ng.ITimeoutService;
        private appConfigService: AppConfigService;
        private parametersService: ParametersService;
        private resourceService: ResourceService;
        private requestService: RequestService;
        private settingTypesSharedValuesService: SettingTypesSharedValuesService;
        private successfulUpdatingCallback: ISuccessfulUpdatingCallback;

        constructor(
            $window: ng.IWindowService,
            $timeout: ng.ITimeoutService,
            appConfigService: AppConfigService,
            parametersService: ParametersService,
            resourceService: ResourceService,
            requestService: RequestService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            this.$window = $window;
            this.$timeout = $timeout;
            this.appConfigService = appConfigService;
            this.parametersService = parametersService;
            this.resourceService = resourceService;
            this.requestService = requestService;
            this.settingTypesSharedValuesService = settingTypesSharedValuesService;

            /* tslint:disable:no-string-literal */
            this.successfulUpdatingCallback = this.$window.parent["successfulUpdatingCallback"];
            /* tslint:enable:no-string-literal */

            this.systemId = this.parametersService.systemId;
            this.requestService.registerSharedScope(this);

            angular.element(this.$window.document).ready(() => {
                this.querySetting();
            });
        }

        public update(): void {
            var changedSettings: Setting = this.settingTypesSharedValuesService.isNew ? this.setting.getEditableSettingTypes() : this.setting.getChanges();
            var command: SettingCommand;
            var path: string = this.settingTypesSharedValuesService.isNew
                ? this.appConfigService.getCreatePath()
                : this.appConfigService.getUpdatePath();

            this.requestMsgResult = undefined;

            if (changedSettings) {
                changedSettings.resetValidationState();

                if (changedSettings.validate()) {
                    command = this.parametersService.createSettingCommand(changedSettings);
                    this.updateSetting(path, command);
                }
            } else {
                var error: Result = Result.noChangeToUpdate;

                error.message = this.resourceService.getMessage(error.message);
                this.requestMsgResult = error;
            }
        }

        private updateSetting(path: string, command: SettingCommand): void {
            this.requestService.post(path, command.toJson(), (response: ng.IHttpPromiseCallbackArg<any>) => {
                this.requestMsgResult = new UpdatingSettingResult(response.data);

                this.$timeout(() => {
                    var settingResults = (<UpdatingSettingResult>this.requestMsgResult);

                    if (settingResults && settingResults.removeSuccessResults) {
                        settingResults.removeSuccessResults();
                    }

                    if (settingResults.isSuccessful) {
                        this.requestMsgResult = undefined;

                        if (this.successfulUpdatingCallback) {
                            this.successfulUpdatingCallback(settingResults.isSuccessful);
                        }
                    }
                }, Defaults.displayingMsgTime);
            });
        }

        private querySetting(): void {
            var query: SettingQuery = this.parametersService.getSettingQuery();

            if (query) {
                this.requestService.post(this.appConfigService.getQueryPath(), query.toJson(), (response: ng.IHttpPromiseCallbackArg<any>) => {
                    this.setting = new Setting(response.data);
                    this.warningMessages = this.setting.warningMessage;
                    this.settingTypesSharedValuesService.register(this.setting, this.parametersService.uplineId);
                });
            }
        }
    }

    ModuleRegister.controller("customerSettingController", CustomerSettingController);
}