/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/setting.ts" />
/// <reference path="../module-registrater.ts" />
/// <reference path="parameters-service.ts" />

module CustomerSetting {
    "use strict";

    export class SettingTypesSharedValuesService {
        public static $inject: string[] = ["parametersService"];

        public sharedSettings: Setting;
        public customerId: number
        public uplineLevel: Level;
        public customerLevel: Level;
        public productId: number;
        public productName: string;
        public isNew: boolean;

        private parametersService: ParametersService;

        constructor(parametersService: ParametersService) {
            this.parametersService = parametersService;
        }

        public register(setting: Setting, uplineLevel: Level): void {
            this.uplineLevel = uplineLevel;
            this.isNew = this.parametersService.isNew;

            if (setting) {
                this.sharedSettings = setting;
                this.customerId = setting.customerId;
                this.customerLevel = setting.customerLevel;
                this.productId = setting.productId;
                this.productName = setting.productName;
            }
        }
    }

    ModuleRegister.service("settingTypesSharedValuesService", SettingTypesSharedValuesService);
}