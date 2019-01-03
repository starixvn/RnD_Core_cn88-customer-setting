/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/position-model.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../models/setting-item.ts" />
/// <reference path="../../resources/resource-service.ts" />

module CustomerSetting {
    "use strict";

    interface ISettingItemValidatorConfiguration {
        settingItem: DefaultSettingItem;
        resourceServiceName: string;
        customFilterName: string;
        fieldName: string;
    }

    interface ISettingItemValidatorScope extends ng.IScope {
        configuration: ISettingItemValidatorConfiguration;
        errorMessage: string;
        resetValidationState: Function;
    }

    class SettingItemValidatorDirective implements ng.IDirective {
        public static $inject: string[] = ["$compile", "$injector", "$filter"];

        public restrict: string = "A";
        public priority: number = 100;

        public get scope(): any {
            return {
                configuration: "=settingItemValidator"
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: ISettingItemValidatorScope, element: ng.IAugmentedJQuery) => {
            var settingField: ng.IAugmentedJQuery = angular.element(element[0].getElementsByClassName("setting-field")[0]);
            var ngChangeOnElement: string = settingField.attr("data-ng-change") || settingField.attr("ng-change");

            if (!ngChangeOnElement) {
                settingField.attr("data-ng-change", "resetValidationState()");
            } else {
                settingField.attr("data-ng-change", ngChangeOnElement + ";resetValidationState();");
            }

            scope.$watch("configuration.settingItem.validationState", (validationState: ValidationResult) => {
                scope.errorMessage = this.getErrorMessage(
                    scope.configuration.settingItem.validationState,
                    scope.configuration.resourceServiceName,
                    scope.configuration.customFilterName,
                    scope.configuration.fieldName);
            });

            scope.resetValidationState = () => {
                scope.configuration.settingItem.resetValidationState();
            };

            element.attr("data-tooltip", "errorMessage");

            this.$compile(element)(scope);
        };

        private $compile: ng.ICompileService;
        private $injector: ng.auto.IInjectorService;
        private $filter: ng.IFilterService;

        constructor($compile: ng.ICompileService, $injector: ng.auto.IInjectorService, $filter: ng.IFilterService) {
            this.$compile = $compile;
            this.$injector = $injector;
            this.$filter = $filter;
        }

        private getErrorMessage(validationResult: ValidationResult, resourceServiceName: string, customFilterName: string, fieldName: string): string {
            var message: string = null;

            if (validationResult
                && validationResult.message
                && (!fieldName || (fieldName && fieldName === validationResult.fieldName))) {
                var resourceService: ResourceService = this.$injector.get<ResourceService>(resourceServiceName);
                var params: string[] = (validationResult.msgParams || []).map((param: any) => {
                    if (typeof param === "number") {
                        var value = this.$filter("number")(param);

                        if (customFilterName) {
                            value = this.$filter(customFilterName)(value);
                        }

                        return value;
                    } else {
                        return param;
                    }
                });

                if (resourceService) {
                    params.unshift(validationResult.message);
                    message = resourceService.getFormattedMessage.apply(resourceService, params);
                }
            }

            return message;
        }
    }

    ModuleRegister.directive("settingItemValidator", SettingItemValidatorDirective);
}