/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../../Core/Resources/resource-service.ts" />
/// <reference path="../../core/services/parameters-service.ts" />

module CustomerSetting {
    "use strict";

    export class SettingType1ResourceService extends ResourceService {
        public static $inject: string[] = ["settingType1Resources", "parametersService"];

        constructor(settingType1Resources: any, parametersService: ParametersService) {
            super(settingType1Resources, parametersService);
        }
    }

    ModuleRegister.service("settingType1ResourceService", SettingType1ResourceService);

    function SettingType1LabelFilter(settingType1ResourceService: SettingType1ResourceService): Function {
        return (name: string) => {
            return settingType1ResourceService.getLabel(name);
        };
    }

    SettingType1LabelFilter.$inject = ["settingType1ResourceService"];
    ModuleRegister.filter("settingType1Label", SettingType1LabelFilter);

    function SettingType1MessageFilter(settingType1ResourceService: SettingType1ResourceService): Function {
        return (name: string, ...replacements: any[]) => {
            replacements.unshift(name);

            return settingType1ResourceService.getFormattedMessage.apply(settingType1ResourceService, replacements);
        };
    }

    SettingType1MessageFilter.$inject = ["settingType1ResourceService"];
    ModuleRegister.filter("settingType1Message", SettingType1MessageFilter);
}