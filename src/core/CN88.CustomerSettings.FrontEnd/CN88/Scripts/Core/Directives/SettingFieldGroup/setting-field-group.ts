/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/key-value-pair.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../models/setting-item.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export interface ISettingFieldGroupScopeController {
        register: (fieldName: string, element: ng.IAugmentedJQuery) => void;
    }

    interface ISettingFieldHTMLControl {
        element: ng.IAugmentedJQuery;
        dirty?: boolean;
    }

    interface ISettingFieldGroupScope extends ng.IScope {
        settingItem: DefaultSettingItem;
        resourceServiceName: string;
        customFilterName: string;
        registeredFields: KeyValuePair<string, ISettingFieldHTMLControl>[];
        errorMessage: string;
        dirtyField: ng.IAugmentedJQuery;
        reset: () => void;
    }

    class SettingFieldGroupDirective implements ng.IDirective {
        public static $inject: string[] = ["$injector", "$filter"];

        public restrict: string = "A";
        public priority: number = 100;

        public get scope(): any {
            return {
                settingItem: "=settingFieldGroup",
                resourceServiceName: "@",
                customFilterName: "@"
            };
        }

        public compile: ng.IDirectiveCompileFn = (element: ng.IAugmentedJQuery) => {
            var errorTooltipContainer: ng.IAugmentedJQuery = angular.element("<span></span>");

            errorTooltipContainer.addClass("error-tooltip-container");
            errorTooltipContainer.attr("data-tooltip", "{{errorMessage}}");
            errorTooltipContainer.attr("data-manual", "true");
            errorTooltipContainer.attr("data-tooltip-type", "error");
            errorTooltipContainer.attr("data-target", "dirtyField");

            element.prepend(errorTooltipContainer);

            return {
                post: this.link
            };
        };

        public controller: any = ["$scope", function ($scope: ISettingFieldGroupScope) {
            $scope.registeredFields = [];
            $scope.errorMessage = null;

            this.register = (fieldName: string, element: ng.IAugmentedJQuery) => {
                if (!fieldName || !element) {
                    return;
                }

                var existing: boolean = $scope.registeredFields.some((registeredField: KeyValuePair<string, ISettingFieldHTMLControl>) => {
                    return registeredField.key === fieldName;
                });

                if (!existing) {
                    $scope.registeredFields.push(new KeyValuePair<string, ISettingFieldHTMLControl>(fieldName, { element: element }));
                }
            };
        }];

        public link: ng.IDirectiveLinkFn = (scope: ISettingFieldGroupScope, element: ng.IAugmentedJQuery) => {
            var errorContainer: ng.IAugmentedJQuery = angular.element(element[0].getElementsByClassName("error-tooltip-container")[0]);
            var tooltipController: any = errorContainer.controller("tooltip");

            scope.reset = () => {
                scope.$apply(() => {
                    scope.settingItem.resetValidationState();
                });
            };

            scope.$watch("settingItem.validationState", (validationState: ValidationResult, oldValidationState: ValidationResult) => {
                if (!oldValidationState || oldValidationState === validationState) {
                    return;
                }

                if (scope.registeredFields.length > 0) {
                    if (validationState === Result.normal) {
                        if (tooltipController && scope.dirtyField) {
                            tooltipController.hideTooltip();
                            scope.dirtyField = null;
                        }

                        this.handleValidationStateChange(scope);
                    } else {
                        var errorMessage: string = this.getErrorMessage(validationState, scope.resourceServiceName, scope.customFilterName);

                        scope.dirtyField = this.handleValidationStateChange(scope, validationState.fieldName);

                        if (tooltipController && errorMessage && scope.dirtyField) {
                            tooltipController.showTooltipAt(errorMessage, scope.dirtyField);
                        }
                    }
                }
            });
        };

        private $injector: ng.auto.IInjectorService;
        private $filter: ng.IFilterService;

        constructor($injector: ng.auto.IInjectorService, $filter: ng.IFilterService) {
            this.$injector = $injector;
            this.$filter = $filter;
        }

        private handleValidationStateChange(scope: ISettingFieldGroupScope, excludedFieldName: string = null): ng.IAugmentedJQuery {
            var dirtyField: ng.IAugmentedJQuery;

            scope.registeredFields.forEach((registeredField: KeyValuePair<string, ISettingFieldHTMLControl>) => {
                if (registeredField.key === excludedFieldName) {
                    registeredField.value.element.addClass("setting-field--error");
                    registeredField.value.element[0].focus();
                    registeredField.value.element.on("change", scope.reset);
                    registeredField.value.dirty = true;

                    dirtyField = registeredField.value.element;
                } else if (registeredField.value.dirty) {
                    registeredField.value.element.removeClass("setting-field--error");
                    registeredField.value.element.off("change", scope.reset);
                    registeredField.value.dirty = false;
                }
            });

            return dirtyField;
        }

        private getErrorMessage(result: ValidationResult, resourceServiceName: string, customFilterName: string = null): string {
            var message: string = null;

            if (result.message) {
                var resourceService: ResourceService = this.$injector.get<ResourceService>(resourceServiceName);
                var params: string[] = (result.msgParams || []).map((param: any) => {
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
                    params.unshift(result.message);
                    message = resourceService.getFormattedMessage.apply(resourceService, params);
                }
            }

            return message;
        }
    }

    ModuleRegister.directive("settingFieldGroup", SettingFieldGroupDirective);
}