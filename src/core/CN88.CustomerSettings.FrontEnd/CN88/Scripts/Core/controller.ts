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

namespace CustomerSetting {
    interface ISuccessfulUpdatingCallback {
        (isNew: boolean, customerLevel: Level, customerId: number): any;
    }

    class CustomerSettingController {
        public static $inject: string[] = [
            "$window",
            "$timeout",
            "$locale",
            "appConfigService",
            "parametersService",
            "resourceService",
            "requestService",
            "settingTypesSharedValuesService"];

        public title: string;
        public systemId: number;
        public isMobileView: boolean;
        public setting: Setting;
        public warningMessages: Result | UpdatingSettingResult;
        public requestMsgResult: Result | UpdatingSettingResult;
        public cancelButtonRequired: boolean;
        public cancelCallback: Function;
        public saveAndNewRequired: boolean;
        public saveAndNewCallback: ISuccessfulUpdatingCallback;
        public saveRequired: boolean;
        public saveCallback: ISuccessfulUpdatingCallback;

        private $window: ng.IWindowService;
        private $timeout: ng.ITimeoutService;
        private appConfigService: AppConfigService;
        private parametersService: ParametersService;
        private resourceService: ResourceService;
        private requestService: RequestService;
        private settingTypesSharedValuesService: SettingTypesSharedValuesService;

        constructor(
            $window: ng.IWindowService,
            $timeout: ng.ITimeoutService,
            $locale: ng.ILocaleService,
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

            $locale.NUMBER_FORMATS.GROUP_SEP = this.appConfigService.thousandSeparator;
            $locale.NUMBER_FORMATS.DECIMAL_SEP = this.appConfigService.decimalSeparator;

        /* tslint:disable:no-string-literal */
            this.saveRequired = this.$window.parent["showCN88SaveButton"];
            this.saveCallback = this.$window.parent["cn88SuccessfulUpdatingCallback"];

            if (this.saveRequired === undefined || this.saveRequired === null) {
                this.saveRequired = true
            }

            this.cancelButtonRequired = this.$window.parent["showCN88CancelButton"];

            if (this.cancelButtonRequired) {
                this.cancelCallback = this.$window.parent["cn88CancelCallback"];
            }

            if (this.parametersService.isNew) {
                this.saveAndNewRequired = this.$window.parent["showCN88SaveAndNewButton"];

                if (this.saveAndNewRequired) {
                    this.saveAndNewCallback = this.$window.parent["cn88SaveAndNewCallback"];
                }
            }
            /* tslint:enable:no-string-literal */

            this.systemId = this.parametersService.systemId;
            this.isMobileView = this.parametersService.isMobileView;
            this.title = this.detectTitle();
            this.requestService.registerSharedScope(this);

            angular.element(this.$window.document).ready(() => {
                this.querySetting();
            });
        }

        public saveAndNew(): void {
            this.update(this.saveAndNewCallback);
        }

        public update(callback: ISuccessfulUpdatingCallback = this.saveCallback): void {
            var changedSettings: Setting = this.parametersService.isNew ? this.setting.getEditableSettings() : this.setting.getChanges();
            var command: SettingCommand;
            var path: string = this.parametersService.isNew
                ? this.appConfigService.getCreatePath()
                : this.appConfigService.getUpdatePath();

            this.requestMsgResult = undefined;

            if (changedSettings) {
                changedSettings.resetValidationState();

                if (changedSettings.validate()) {
                    command = this.parametersService.createSettingCommand(changedSettings);
                    this.updateSetting(path, command, callback);
                }
            } else {
                var error: Result = Result.noChangeToUpdate;

                error.message = this.resourceService.getMessage(error.message);
                this.requestMsgResult = error;
            }
        }

        private updateSetting(path: string, command: SettingCommand, callback: ISuccessfulUpdatingCallback): void {
            this.requestService.post(path, command.toJson(), (response: ng.IHttpPromiseCallbackArg<any>) => {
                this.requestMsgResult = new UpdatingSettingResult(response.data);

                this.$timeout(() => {
                    var settingResults = (<UpdatingSettingResult>this.requestMsgResult);

                    if (settingResults && settingResults.removeSuccessResults) {
                        settingResults.removeSuccessResults();
                    }

                    if (settingResults.isSuccessful) {
                        this.requestMsgResult = undefined;

                        if (callback) {
                            var customerId = this.settingTypesSharedValuesService.isNew
                                ? settingResults.customerId
                                : this.settingTypesSharedValuesService.customerId;

                            callback(
                                this.settingTypesSharedValuesService.isNew,
                                this.settingTypesSharedValuesService.customerLevel,
                                customerId);
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

        private detectTitle(): string {
            var customerLevel = this.parametersService.getSettingQuery().customerLevel;
            var isCreatingCustomer = this.parametersService.isNew;
            var isEditingCustomer = !this.parametersService.isNew && this.parametersService.isEditable;

            if (isCreatingCustomer) {
                switch (customerLevel) {
                    case Level.super:
                        return this.resourceService.getLabel("NewSuper");
                    case Level.master:
                        return this.resourceService.getLabel("NewMaster");
                    case Level.agent:
                        return this.resourceService.getLabel("NewAgent");
                    case Level.member:
                        return this.resourceService.getLabel("NewMember");
                }
            }

            if (isEditingCustomer) {
                switch (customerLevel) {
                    case Level.super:
                        return this.resourceService.getLabel("EditSuper");
                    case Level.master:
                        return this.resourceService.getLabel("EditMaster");
                    case Level.agent:
                        return this.resourceService.getLabel("EditAgent");
                    case Level.member:
                        return this.resourceService.getLabel("EditMember");
                }
            }

            return this.resourceService.getLabel("MySettings");
        }
    }

    ModuleRegister.controller("customerSettingController", CustomerSettingController);
}