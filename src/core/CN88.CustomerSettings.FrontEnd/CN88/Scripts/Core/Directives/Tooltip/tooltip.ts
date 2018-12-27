/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/position-model.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../resources/resource-service.ts" />

module CustomerSetting {
    "use strict";

    interface ITooltipScope extends ng.IScope {
        tooltipMessage: string;
        tooltipType: string;
        tooltipPositionModel: PositionModel,
        onHover: boolean;
        fieldName: string;
        showed: boolean;
    }

    class TooltipDirective implements ng.IDirective {
        public static $inject: string[] = ["$window", "$compile", "$timeout", "$templateCache", "$rootScope"];

        public restrict: string = "A";
        public priority: number = 100;

        public get scope(): any {
            return {
                tooltipMessage: "@",
                tooltipType: "@",
                tooltipPositionModel: "@",
                onHover: "@",
                fieldName: "@"
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: ITooltipScope, wrappedElement: ng.IAugmentedJQuery) => {
            var element: ng.IAugmentedJQuery = angular.element(wrappedElement);
            var target: ng.IAugmentedJQuery = angular.element(element.children()[0]);

            var template: {} = this.$templateCache.get("Directives/Tooltip/tooltip.htm");
            var tooltip: ng.IAugmentedJQuery = angular.element(template);

            var debounceResize: any = debounce(() => {
                if (scope.tooltipMessage) {
                    this.calculateTooltipPosition(wrappedElement, scope.tooltipPositionModel);
                }
            }, 100);

            scope.tooltipType = scope.tooltipType || "error";

            if (scope.onHover) {
                element.on("mouseenter", () => {
                    scope.showed = true;
                    scope.$apply();
                    this.calculateTooltipPosition(wrappedElement, scope.tooltipPositionModel);
                });

                element.on("mouseleave", () => {
                    scope.showed = false;
                    scope.$apply();
                });
            } else {
                scope.showed = true;
            }

            this.$compile(tooltip)(scope);
            wrappedElement.prepend(tooltip);

            scope.$watch("tooltipMessage", () => {
                if (scope.tooltipMessage) {
                    this.calculateTooltipPosition(wrappedElement, scope.tooltipPositionModel);
                    target.addClass("setting-field--" + scope.tooltipType);
                } else {
                    target.removeClass("setting-field--" + scope.tooltipType);
                }
            });

            angular.element(this.$window).on("resize", debounceResize);
            this.$rootScope.$on("page-height-changed", () => {
                if (scope.tooltipMessage) {
                    this.calculateTooltipPosition(wrappedElement, scope.tooltipPositionModel);
                }
            });
        };

        private $window: ng.IWindowService;
        private $compile: ng.ICompileService;
        private $timeout: ng.ITimeoutService;
        private $templateCache: ng.ITemplateCacheService;
        private $rootScope: ng.IRootScopeService;

        constructor(
            $window: ng.IWindowService,
            $compile: ng.ICompileService,
            $timeout: ng.ITimeoutService,
            $templateCache: ng.ITemplateCacheService,
            $rootScope: ng.IRootScopeService) {
            this.$window = $window;
            this.$compile = $compile;
            this.$timeout = $timeout;
            this.$templateCache = $templateCache;
            this.$rootScope = $rootScope;
        }

        private calculateTooltipPosition(wrappedElement: ng.IAugmentedJQuery, positionModel: PositionModel): void {
            var children = wrappedElement.children();

            if (!children || children.length !== 2) {
                return;
            }

            this.$timeout(() => {
                var tooltip: ng.IAugmentedJQuery = angular.element(children[0]);
                var target: ng.IAugmentedJQuery = angular.element(children[1]);

                tooltip.removeClass("tooltip--right tooltip--left");
                tooltip.css({ "left": "0px", "visibility": "hidden" });

                var elementRect: ClientRect = target[0].getBoundingClientRect();
                var tooltipRect: ClientRect = tooltip[0].getBoundingClientRect();

                var topOffset: number = 0;
                var leftOffset: number = 0;

                var arrowWidth: number = 5;

                if ((+positionModel || PositionModel.relative) === PositionModel.relative) {
                    var bodyRect: ClientRect = this.$window.document.body.getBoundingClientRect();

                    topOffset = bodyRect.top;
                    leftOffset = bodyRect.left;
                }

                var top: number = elementRect.top - topOffset - tooltipRect.height + arrowWidth;
                var left: number = elementRect.left + (elementRect.width - tooltipRect.width) / 2 - leftOffset;

                if (this.$window.innerWidth < left + tooltipRect.width) {
                    top = elementRect.top - topOffset - (tooltipRect.height - elementRect.height) / 2 + arrowWidth;
                    left = elementRect.left - tooltipRect.width - arrowWidth;
                    tooltip.addClass("tooltip--right");
                }

                if (left < 0) {
                    top = elementRect.top - topOffset - (tooltipRect.height - elementRect.height) / 2 + arrowWidth;
                    left = elementRect.left + elementRect.width;
                    tooltip.addClass("tooltip--left");
                }

                if (top < 0) {
                    top = elementRect.bottom - topOffset - arrowWidth;
                    tooltip.addClass("tooltip--bottom");
                }

                tooltip.css({
                    "top": top + "px",
                    "left": left + "px",
                    "visibility": "visible"
                });
            });
        }
    }

    ModuleRegister.directive("tooltip", TooltipDirective);

    function TooltipMessage($injector: ng.auto.IInjectorService, $filter: ng.IFilterService): Function {
        return (result: ValidationResult, resourceServiceName: string, fieldName: string, customFilterName: string = null) => {
            var message: string = null;

            if (result
                && result.message
                && (!fieldName || (fieldName && fieldName === result.fieldName))) {
                var resourceService: ResourceService = $injector.get<ResourceService>(resourceServiceName);
                var params: string[] = (result.msgParams || []).map((param: any) => {
                    if (typeof param === "number") {
                        var value = $filter("number")(param);

                        if (customFilterName) {
                            value = $filter(customFilterName)(value);
                        }

                        return value;
                    } else {
                        return param;
                    }
                });

                if (resourceService) {
                    params.unshift(result.message)
                    message = resourceService.getFormattedMessage.apply(resourceService, params);
                }
            }

            return message;
        };
    }

    TooltipMessage.$inject = ["$injector", "$filter"];
    ModuleRegister.filter("tooltipMessage", TooltipMessage);
}