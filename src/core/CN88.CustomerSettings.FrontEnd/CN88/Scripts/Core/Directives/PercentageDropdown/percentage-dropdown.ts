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
        range: number[];
    }

    class PercentageDropdownDirective implements ng.IDirective {
        public replace: boolean = true;
        public templateUrl: string = "Directives/PercentageDropdown/percentage-dropdown.htm";

        public restrict: string = "E";

        public get scope(): any {
            return {
                value: "=ngModel",
                min: "=",
                max: "=",
                step: "=",
                factor: "=",
                precision: "="
            };
        }

        public controller: any = ["$scope", "valueRangeService", ($scope: IPercentageDropdownScope, valueRangeService: ValueRangeService) => {
            $scope.$watchGroup(["value", "min", "max", "step", "factor"], () => {

                $scope.factor = $scope.factor || 100;
                $scope.step = $scope.step || 1;
                $scope.range = valueRangeService.generateRanges(
                    $scope.value,
                    $scope.min,
                    $scope.max,
                    $scope.step,
                    $scope.factor);
            });
        }];
    }

    ModuleRegister.directive("percentageDropdown", PercentageDropdownDirective);
}