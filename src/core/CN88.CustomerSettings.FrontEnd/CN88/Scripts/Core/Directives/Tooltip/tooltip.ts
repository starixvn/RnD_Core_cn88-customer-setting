/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/position-model.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../resources/resource-service.ts" />

module CustomerSetting {
    "use strict";

    interface ITooltipScope extends ng.IScope {
        tooltipMessage: string;
        tooltipType: string;
        tooltipPositionModel: PositionModel;
        target: ng.IAugmentedJQuery;
        innerTooltip: boolean;
        manual: boolean;
        beingShowed: boolean;
    }

    class TooltipDirective implements ng.IDirective {
        public static $inject: string[] = ["$window", "$compile", "$timeout", "$templateCache", "$rootScope"];

        public restrict: string = "A";
        public priority: number = 100;

        public get scope(): any {
            return {
                tooltipMessage: "@tooltip",
                tooltipType: "@",
                tooltipPositionModel: "@",
                innerTooltip: "@",
                manual: "@"
            };
        }

        public controller: any = ["$scope", function ($scope: ITooltipScope) {
            this.showTooltipAt = (tooltipMessage: string, target: ng.IAugmentedJQuery) => {
                $scope.tooltipMessage = tooltipMessage;
                $scope.target = target;
            };
        }];

        public link: ng.IDirectiveLinkFn = (scope: ITooltipScope, element: ng.IAugmentedJQuery) => {
            var template: {} = this.$templateCache.get("Directives/Tooltip/tooltip.htm");
            var tooltip: ng.IAugmentedJQuery = angular.element(template);

            var debounceResize: any = debounce(() => {
                if (scope.tooltipMessage) {
                    this.calculateTooltipPosition(scope.target, tooltip, scope.tooltipPositionModel);
                }
            }, 100);

            scope.tooltipType = scope.tooltipType || "info";
            scope.target = element;

            if (scope.manual) {
                scope.beingShowed = true;
            } else {
                tooltip.addClass("tooltip--on-hover");

                scope.target.on("mouseenter", () => {
                    if(!scope.beingShowed) {
                        scope.beingShowed = true;
                        tooltip.css({ "left": "0px", "visibility": "hidden" });
                        scope.$apply();
                        this.calculateTooltipPosition(scope.target, tooltip, scope.tooltipPositionModel);
                    }
                });

                scope.target.on("mouseleave", () => {
                    if (scope.beingShowed) {
                        scope.beingShowed = false;
                        scope.$apply();
                    }
                });
            }

            this.$compile(tooltip)(scope);

            if (scope.innerTooltip) {
                element.prepend(tooltip);
            } else {
                element.parent().prepend(tooltip);
            }

            scope.$watch("tooltipMessage", () => {
                if (scope.tooltipMessage) {
                    this.calculateTooltipPosition(scope.target, tooltip, scope.tooltipPositionModel);
                }
            });

            angular.element(this.$window).on("resize", debounceResize);
            this.$rootScope.$on("page-height-changed", debounceResize);
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

        private calculateTooltipPosition(
            target: ng.IAugmentedJQuery,
            tooltip: ng.IAugmentedJQuery,
            positionModel: PositionModel): void {
            if (!target || target.length === 0 || !tooltip || tooltip.length === 0) {
                return;
            }

            this.$timeout(() => {
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
}