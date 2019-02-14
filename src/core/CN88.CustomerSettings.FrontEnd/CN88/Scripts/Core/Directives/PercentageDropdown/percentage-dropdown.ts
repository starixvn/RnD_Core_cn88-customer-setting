/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/value-range-service.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface IPercentageDropdownScope extends ng.IScope {
        value: number;
        min: number;
        max: number;
        step: number;
        factor: number;
        precision: number;
        placeholder: string;
        placeholderVisible: boolean;
        autoRefresh: boolean;
        range: number[];
    }

    class PercentageDropdownDirective implements ng.IDirective {
        public static $inject: string[] = ["valueRangeService"];

        public replace: boolean = true;
        public templateUrl: string = "percentage-dropdown.htm";

        public restrict: string = "E";

        public get scope(): any {
            return {
                value: "=ngModel",
                min: "=",
                max: "=",
                step: "=?",
                precision: "=?",
                placeholder: "@",
                placeholderVisible:"=?",
                autoRefresh: "=?"
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: IPercentageDropdownScope) => {
            if (scope.autoRefresh) {
                scope.$watchGroup(["value", "min", "max", "step", "factor"], () => {
                    this.initializeRange(scope);
                });
            } else {
                this.initializeRange(scope);
            }
        };

        private valueRangeService: ValueRangeService;

        constructor(valueRangeService: ValueRangeService) {
            this.valueRangeService = valueRangeService;
        }

        private initializeRange(scope: IPercentageDropdownScope): void {
            scope.precision = scope.precision || 0;
            scope.factor = Math.pow(10, scope.precision);
            scope.step = scope.step || 1;
            scope.range = this.valueRangeService.generateRanges(
                scope.value,
                scope.min,
                scope.max,
                scope.step,
                scope.factor);
        }
    }

    ModuleRegister.directive("percentageDropdown", PercentageDropdownDirective);
}