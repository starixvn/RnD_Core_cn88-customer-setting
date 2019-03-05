/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../../Core/Resources/resource-service.ts" />
/// <reference path="../../core/services/parameters-service.ts" />

namespace CustomerSetting {
    export class SettingType2ResourceService extends ResourceService {
        public static $inject: string[] = ["settingType2Resources", "parametersService"];

        constructor(settingType2Resources: any, parametersService: ParametersService) {
            super(settingType2Resources, parametersService);
        }
    }

    ModuleRegister.service("settingType2ResourceService", SettingType2ResourceService);

    function SettingType2LabelFilter(settingType2ResourceService: SettingType2ResourceService): Function {
        return (name: string) => {
            return settingType2ResourceService.getLabel(name);
        };
    }

    SettingType2LabelFilter.$inject = ["settingType2ResourceService"];
    ModuleRegister.filter("settingType2Label", SettingType2LabelFilter);

    function SettingType2MessageFilter(settingType2ResourceService: SettingType2ResourceService): Function {
        return (name: string, ...replacements: any[]) => {
            replacements.unshift(name);

            return settingType2ResourceService.getFormattedMessage.apply(settingType2ResourceService, replacements);
        };
    }

    SettingType2MessageFilter.$inject = ["settingType2ResourceService"];
    ModuleRegister.filter("settingType2Message", SettingType2MessageFilter);
}