/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../../Core/Resources/resource-service.ts" />
/// <reference path="../../core/services/parameters-service.ts" />

namespace CustomerSetting.SettingType2 {
    export class ResourceService extends CustomerSetting.ResourceService {
        public static $inject: string[] = ["st2Resources", "parametersService"];

        constructor(resources: any, parametersService: ParametersService) {
            super(resources, parametersService);
        }
    }

    ModuleRegister.service("st2ResourceService", ResourceService);

    function LabelFilter(resourceService: ResourceService): Function {
        return (name: string) => {
            return resourceService.getLabel(name);
        };
    }

    LabelFilter.$inject = ["st2ResourceService"];
    ModuleRegister.filter("st2Label", LabelFilter);

    function MessageFilter(resourceService: ResourceService): Function {
        return (name: string, ...replacements: any[]) => {
            replacements.unshift(name);

            return resourceService.getFormattedMessage.apply(resourceService, replacements);
        };
    }

    MessageFilter.$inject = ["st2ResourceService"];
    ModuleRegister.filter("st2Message", MessageFilter);
}