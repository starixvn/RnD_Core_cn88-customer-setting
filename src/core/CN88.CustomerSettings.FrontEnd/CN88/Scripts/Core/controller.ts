/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="models/result.ts" />
/// <reference path="models/setting.ts" />
/// <reference path="models/parameters/setting-command.ts" />
/// <reference path="models/parameters/setting-query.ts" />
/// <reference path="resources/resource-service.ts" />
/// <reference path="services/app-config-service.ts" />
/// <reference path="services/parameters-service.ts" />
/// <reference path="services/request-service.ts" />
/// <reference path="services/settingtypes-shared-values-service.ts" />

module CustomerSetting {
    "use strict";

    interface ISuccessfulUpdatingCallback {
        (): any;
    }

    class CustomerSettingController {
        public static $inject: string[] = [
            "$window",
            "$rootScope",
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
        private $rootScope: ng.IRootScopeService;
        private $timeout: ng.ITimeoutService;
        private appConfigService: AppConfigService;
        private parametersService: ParametersService;
        private resourceService: ResourceService;
        private requestService: RequestService;
        private settingTypesSharedValuesService: SettingTypesSharedValuesService;
        private successfulUpdatingCallback: ISuccessfulUpdatingCallback;

        constructor(
            $window: ng.IWindowService,
            $rootScope: ng.IRootScopeService,
            $timeout: ng.ITimeoutService,
            appConfigService: AppConfigService,
            parametersService: ParametersService,
            resourceService: ResourceService,
            requestService: RequestService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            this.$window = $window;
            this.$rootScope = $rootScope;
            this.$timeout = $timeout;
            this.appConfigService = appConfigService;
            this.parametersService = parametersService;
            this.resourceService = resourceService;
            this.requestService = requestService;
            this.settingTypesSharedValuesService = settingTypesSharedValuesService;
            this.successfulUpdatingCallback = this.$window.parent["successfulUpdatingCallback"];

            this.systemId = this.parametersService.systemId;
            this.requestService.registerSharedScope(this);

            angular.element(this.$window.document).ready(() => {
                this.querySetting();
            });
        }

        public update(settingTypeId?: number): void {
            var path: string = this.appConfigService.getUpdatePath();
            var editableSettings: Setting = this.setting.getEditableSettingTypes(settingTypeId);
            var setting: Setting = this.setting.getChanges(settingTypeId);
            var validationResult: Result;
            var command: SettingCommand;

            this.$rootScope.$emit("updateSetting", settingTypeId);
            this.requestMsgResult = undefined;

            if (setting) {
                if (editableSettings.validate()) {
                    command = this.parametersService.createSettingCommand(setting);
                    validationResult = command.validate();

                    if (validationResult.isSuccessful) {
                        this.updateSetting(path, command);
                    } else {
                        this.requestMsgResult = validationResult;
                    }
                }
            } else {
                var error: Result = Result.noChangeToUpdate;

                error.message = this.resourceService.getMessage(error.message);
                this.requestMsgResult = error;
            }
        }

        private updateSetting(path: string, command: SettingCommand): void {
            this.requestService.request(path, command.toJson(), (response: ng.IHttpPromiseCallbackArg<any>) => {
                this.requestMsgResult = new UpdatingSettingResult(response.data);
                this.querySetting();

                this.$timeout(() => {
                    var settingResults = (<UpdatingSettingResult>this.requestMsgResult);

                    if (settingResults && settingResults.removeSuccessResults && settingResults.removeSuccessResults().length === 0) {
                        this.requestMsgResult = undefined;

                        if (this.successfulUpdatingCallback) {
                            this.successfulUpdatingCallback();
                        }
                    }
                }, Defaults.displayingMsgTime);
            });
        }

        private querySetting(): void {
            var query: SettingQuery = this.parametersService.getSettingQuery();

            if (query) {
                this.requestService.request(this.appConfigService.getQueryPath(), query.toJson(), (response: ng.IHttpPromiseCallbackArg<any>) => {
                    this.setting = new Setting(response.data);
                    this.warningMessages = this.setting.warningMessage;
                    this.settingTypesSharedValuesService.register(this.setting, this.parametersService.uplineLevel);
                    this.$rootScope.$emit("queriedSetting");
                });
            }
        }
    }

    ModuleRegister.controller("customerSettingController", CustomerSettingController);
}