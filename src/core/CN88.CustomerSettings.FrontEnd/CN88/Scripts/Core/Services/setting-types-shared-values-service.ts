/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/enums/level.ts" />
/// <reference path="../models/setting.ts" />
/// <reference path="../module-registrater.ts" />
/// <reference path="parameters-service.ts" />

module CustomerSetting {
    "use strict";

    export class SettingTypesSharedValuesService {
        public static $inject: string[] = ["parametersService"];

        public actorLevel: Level;
        public customerId: number;
        public uplineId: number;
        public customerLevel: Level;
        public productId: number;
        public productName: string;
        public isNew: boolean;
        public hashKey: string;

        private parametersService: ParametersService;

        constructor(parametersService: ParametersService) {
            this.parametersService = parametersService;
        }

        public register(setting: Setting, uplineId: number): void {
            this.actorLevel = this.parametersService.actorLevel;
            this.uplineId = uplineId;
            this.isNew = this.parametersService.isNew;
            this.hashKey = this.parametersService.hashKey;

            if (setting) {
                this.customerId = setting.customerId;
                this.customerLevel = setting.customerLevel;
                this.productId = setting.productId;
                this.productName = setting.productName;
            }
        }
    }

    ModuleRegister.service("settingTypesSharedValuesService", SettingTypesSharedValuesService);
}