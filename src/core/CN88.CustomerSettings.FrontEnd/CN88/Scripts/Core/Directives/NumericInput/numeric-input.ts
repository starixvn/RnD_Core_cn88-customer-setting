/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface INumericInputScope extends ng.IScope {
        precision: number;
    }

    class NumericInputDirective implements ng.IDirective {
        public static $inject: string[] = ["$filter"];
        public static defaultPrecision: number = 0;

        public restrict: string = "A";

        public get scope(): any {
            return {
                precision: "=?numericInput"
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: INumericInputScope, originalElement: ng.IAugmentedJQuery) => {
            var element: ng.IAugmentedJQuery = angular.element(originalElement[0]);
            var modelCtrl: any = element.controller("ngModel");

            scope.precision = scope.precision || NumericInputDirective.defaultPrecision;

            // Chrome has a weird behavior with the mouseup event on textboxes.
            // The position of the cursor is set on this event so that once it is fired, the selection will be removed.
            // We add some lines of code to trace the focus status as a hack for this issue.
            element.on("focus", () => {
                element.data("focused", true);
            });

            element.on("focusout", () => {
                element.data("focused", false);
            });

            element.on("mouseup", () => {
                if (element.data("focused")) {
                    var input: HTMLInputElement = <HTMLInputElement>element[0];

                    input.select();
                }
            });

            element.on("keydown", function (event: JQueryEventObject) {
                var value: string = element.val();
                var dotPistion = value.indexOf(".");
                var numberParts: string[] = value.split(".");
                var input: HTMLInputElement = <HTMLInputElement>element[0];

                if (modelCtrl.$modelValue === 0 && [46, 8].indexOf(event.keyCode) >= 0) {
                    event.preventDefault();

                    return;
                }

                if ([110, 190].indexOf(event.keyCode) >= 0) {
                    if (numberParts.length > 1 || scope.precision === 0) {
                        event.preventDefault();

                        return;
                    } else {
                        return;
                    }
                }

                if ([46, 8, 9, 27, 13].indexOf(event.keyCode) >= 0 // Allow: backspace, delete, tab, escape, enter
                    || (event.keyCode === 65 && event.ctrlKey === true) // Allow: Ctrl+A
                    || (event.keyCode >= 35 && event.keyCode <= 40)) { // Allow: home, end, left, right, down, up
                    // let it happen, don't do anything
                    return;
                }

                if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57))
                    && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();

                    return;
                }

                if (dotPistion >= 0 && dotPistion < input.selectionStart && numberParts[1].length > 1) {
                    event.preventDefault();

                    return;
                }
            });

            modelCtrl.$formatters.unshift(() => {
                return this.$filter("number")(modelCtrl.$modelValue, scope.precision);
            });

            modelCtrl.$parsers.unshift((viewValue: string) => {
                var input: HTMLInputElement = <HTMLInputElement>element[0];
                var value: number;
                var formattedValue: string = viewValue;
                var caretPosition: number = input.selectionStart;
                var numberParts: string[] = viewValue ? viewValue.split(".") : [];

                if (numberParts.length > 1) {
                    var precision = scope.precision;
                    var plainIntegerPart: string = numberParts[0].replace(/[\,]/g, "") || "0";
                    var decimalPart: string = numberParts[1].replace(/[\,\.]/g, "") || "";

                    decimalPart = decimalPart.substring(0, precision);
                    value = parseFloat(plainIntegerPart + "." + decimalPart);
                    formattedValue = this.$filter("number")(plainIntegerPart) + "." + decimalPart;
                } else if (numberParts.length > 0) {
                    var plainNumber: string = viewValue.replace(/[\,]/g, "");

                    value = parseInt(plainNumber, 10) || 0;
                    formattedValue = this.$filter("number")(value);
                }

                element.val(formattedValue);

                if (viewValue.length !== caretPosition) {
                    var offset: number = viewValue.length - formattedValue.length;

                    caretPosition -= offset;

                    if (input.selectionStart === input.selectionEnd) {
                        input.selectionEnd = caretPosition;
                    }

                    input.selectionStart = caretPosition;
                }

                return value;
            });
        };

        private $filter: ng.IFilterService;

        constructor($filter: ng.IFilterService) {
            this.$filter = $filter;
        }
    }

    ModuleRegister.directive("numericInput", NumericInputDirective);
}