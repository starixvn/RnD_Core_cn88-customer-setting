/// <reference path="../../core/module-registrater.ts" />
/// <reference path="../../Core/Resources/resource-service.ts" />
/// <reference path="../../core/services/parameters-service.ts" />

namespace CustomerSetting.SettingType1 {
    export class ResourceService extends CustomerSetting.ResourceService {
        public static $inject: string[] = ["st1Resources", "parametersService"];

        constructor(resources: any, parametersService: ParametersService) {
            super(resources, parametersService);
        }
    }

    ModuleRegister.service("st1ResourceService", ResourceService);

    function LabelFilter(resourceService: ResourceService): Function {
        return (name: string) => {
            return resourceService.getLabel(name);
        };
    }

    LabelFilter.$inject = ["st1ResourceService"];
    ModuleRegister.filter("st1Label", LabelFilter);

    function MessageFilter(resourceService: ResourceService): Function {
        return (name: string, ...replacements: any[]) => {
            replacements.unshift(name);

            return resourceService.getFormattedMessage.apply(resourceService, replacements);
        };
    }

    MessageFilter.$inject = ["st1ResourceService"];
    ModuleRegister.filter("st1Message", MessageFilter);
}