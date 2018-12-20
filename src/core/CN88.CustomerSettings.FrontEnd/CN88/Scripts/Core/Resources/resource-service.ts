/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/defaults.ts" />
/// <reference path="../services/parameters-service.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class ResourceService {
        public static $inject: string[] = ["customerSettingResources", "parametersService"];

        private resources: any;
        private defaultLabels: any;
        private defaultMessages: any;
        private labels: any;
        private messages: any;

        constructor(customerSettingResources: any, parametersService: ParametersService) {
            this.resources = customerSettingResources;
            this.setLanguage(parametersService.language || Defaults.language);
        }

        public getLabel(key: string): string {
            var value: string = this.labels[key];

            if (angular.isUndefined(value)) {
                value = this.defaultLabels[key] || key;
            }

            return value;
        }

        public getFormattedLabel(key: string, ...args: any[]): string {
            var label: string = this.getLabel(key);

            if (label) {
                label = label.replace(/{(\d+)}/g, (match: string, index: number) => {
                    return args[index] === undefined || args[index] === null
                        ? match
                        : args[index];
                });
            }

            return label;
        }

        public getMessage(key: string): string {
            var msg: string = this.messages[key];

            if (angular.isUndefined(msg)) {
                msg = this.defaultMessages[key] || key;
            }

            return msg;
        }

        public getFormattedMessage(key: string, ...args: any[]): string {
            var msg: string = this.getMessage(key);

            if (msg) {
                msg = msg.replace(/{(\d+)}/g, (match: string, index: number) => {
                    return args[index] === undefined || args[index] === null
                        ? match
                        : args[index];
                });
            }

            return msg;
        }

        /* tslint:disable:no-string-literal */
        private setLanguage(language: string): void {
            var defaultLanguage: string = Defaults.language.toLowerCase();
            var labels = this.resources["labels"] || {};
            var messages = this.resources["messages"] || {};

            if (language) {
                language = language.toLowerCase();
            } else {
                language = defaultLanguage;
            }

            this.defaultLabels = labels[defaultLanguage] || {};
            this.defaultMessages = messages[defaultLanguage] || {};

            this.labels = labels[language] || this.defaultLabels;
            this.messages = messages[language] || this.defaultMessages;
        }
        /* tslint:enable:no-string-literal */
    }

    ModuleRegister.service("resourceService", ResourceService);

    function LabelFilter(resourceService: ResourceService): Function {
        return (name: string) => {
            return resourceService.getLabel(name);
        };
    }

    LabelFilter.$inject = ["resourceService"];
    ModuleRegister.filter("label", LabelFilter);

    function MessageFilter(resourceService: ResourceService): Function {
        return (name: string, ...replacements: any[]) => {
            replacements.unshift(name)

            return resourceService.getFormattedMessage.apply(resourceService, replacements);
        };
    }

    MessageFilter.$inject = ["resourceService"];
    ModuleRegister.filter("message", MessageFilter);
}