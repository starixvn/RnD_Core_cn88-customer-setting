/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../models/enums/position-model.ts" />
/// <reference path="../../models/result.ts" />
/// <reference path="../../resources/resource-service.ts" />
/// <reference path="../../services/scroll-service.ts" />
/// <reference path="../../module-registrater.ts" />

namespace CustomerSetting {
    interface ITooltipScope extends ng.IScope {
        tooltipMessage: string;
        tooltipType: string;
        tooltipPositionModel: PositionModel;
        target: ng.IAugmentedJQuery;
        innerTooltip: boolean;
        manual: boolean;
        beingShowed: boolean;
        relocate: Function;
    }

    class TooltipDirective implements ng.IDirective {
        public static $inject: string[] = ["$window", "$compile", "$timeout", "$templateCache", "$rootScope", "scrollService"];

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
                $scope.target = target;

                if ($scope.tooltipMessage === tooltipMessage) {
                    $scope.relocate();
                } else {
                    $scope.tooltipMessage = tooltipMessage;
                }
            };
            this.hideTooltip = () => {
                $scope.tooltipMessage = null;
            };
        }];

        public link: ng.IDirectiveLinkFn = (scope: ITooltipScope, element: ng.IAugmentedJQuery) => {
            var template: {} = this.$templateCache.get("./tooltip.html");
            var tooltip: ng.IAugmentedJQuery = angular.element(template);

            var relocate = () => {
                if (scope.tooltipMessage) {
                    this.calculateTooltipPosition(
                        scope.target,
                        tooltip,
                        scope.tooltipPositionModel,
                        scope.manual);
                }
            };
            var debounceResize: any = debounce(relocate, 100);

            scope.tooltipType = scope.tooltipType || "info";
            scope.target = element;

            if (scope.manual) {
                scope.beingShowed = true;
            } else {
                tooltip.addClass("tooltip--on-hover");

                scope.target.on("mouseenter", () => {
                    if (!scope.beingShowed) {
                        tooltip.css({ "left": "0px", "visibility": "hidden" });

                        scope.beingShowed = true;
                        scope.$apply();
                        relocate();
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

            scope.$watch("tooltipMessage", relocate);
            scope.relocate = relocate;

            angular.element(this.$window).on("resize", debounceResize);
            this.$rootScope.$on("page-height-changed", debounceResize);
        };

        private $window: ng.IWindowService;
        private $compile: ng.ICompileService;
        private $timeout: ng.ITimeoutService;
        private $templateCache: ng.ITemplateCacheService;
        private $rootScope: ng.IRootScopeService;
        private scrollService: ScrollService;

        constructor(
            $window: ng.IWindowService,
            $compile: ng.ICompileService,
            $timeout: ng.ITimeoutService,
            $templateCache: ng.ITemplateCacheService,
            $rootScope: ng.IRootScopeService,
            scrollService: ScrollService) {
            this.$window = $window;
            this.$compile = $compile;
            this.$timeout = $timeout;
            this.$templateCache = $templateCache;
            this.$rootScope = $rootScope;
            this.scrollService = scrollService;
        }

        private calculateTooltipPosition(
            target: ng.IAugmentedJQuery,
            tooltip: ng.IAugmentedJQuery,
            positionModel: PositionModel,
            autoScrollTo: boolean): void {
            if (!target || target.length === 0 || !tooltip || tooltip.length === 0) {
                return;
            }

            this.$timeout(() => {
                tooltip.removeClass("tooltip--right tooltip--left tooltip--bottom");
                tooltip.css({ "left": "0px", "visibility": "hidden" });

                var elementRect: ClientRect = target[0].getBoundingClientRect();
                var tooltipRect: ClientRect = tooltip.children()[0].getBoundingClientRect();

                var topOffset: number = 0;
                var leftOffset: number = 0;

                var arrowWidth: number = 7;
                var scrollbarOffset: number = 17;

                if ((+positionModel || PositionModel.relative) === PositionModel.relative) {
                    var bodyRect: ClientRect = this.$window.document.body.getBoundingClientRect();

                    topOffset = bodyRect.top;
                    leftOffset = bodyRect.left;
                }

                var top: number = elementRect.top - topOffset - tooltipRect.height - arrowWidth;
                var left: number = elementRect.left + (elementRect.width - tooltipRect.width) / 2 - leftOffset;

                if (this.$window.innerWidth - scrollbarOffset < left + tooltipRect.width) {
                    top = elementRect.top - topOffset - (tooltipRect.height - elementRect.height) / 2;
                    left = elementRect.left - tooltipRect.width - arrowWidth;
                    tooltip.addClass("tooltip--right");
                }

                if (left < 0) {
                    top = elementRect.top - topOffset - (tooltipRect.height - elementRect.height) / 2;
                    left = elementRect.left + elementRect.width;
                    tooltip.addClass("tooltip--left");
                }

                if (top < 0) {
                    top = elementRect.bottom - topOffset + arrowWidth;
                    tooltip.addClass("tooltip--bottom");
                }

                tooltip.css({
                    "top": top + "px",
                    "left": left + "px",
                    "visibility": "visible"
                });

                if (autoScrollTo && (top < this.$window.pageYOffset || left < this.$window.pageXOffset)) {
                    this.scrollService.scrollTo(left, top);
                }
            });
        }
    }

    ModuleRegister.directive("tooltip", TooltipDirective);
}