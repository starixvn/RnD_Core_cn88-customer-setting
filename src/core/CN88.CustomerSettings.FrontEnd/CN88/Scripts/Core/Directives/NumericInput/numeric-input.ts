/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/app-config-service.ts" />
/// <reference path="../../module-registrater.ts" />

namespace CustomerSetting {
    interface IOriginalAndFormattedValuePair {
        formattedValue: string;
        value: number;
    }

    interface INumericInputScope extends ng.IScope {
        precision: number;
        strictFormat: boolean;
        negativeAllowed: boolean;
    }

    class NumericInputDirective implements ng.IDirective {
        public static $inject: string[] = ["$filter", "appConfigService"];
        public static defaultPrecision: number = 0;

        public restrict: string = "A";

        public get scope(): any {
            return {
                precision: "=?numericInput",
                strictFormat: "@",
                negativeAllowed: "@"
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: INumericInputScope, originalElement: ng.IAugmentedJQuery) => {
            var element: ng.IAugmentedJQuery = angular.element(originalElement[0]);
            var input: HTMLInputElement = <HTMLInputElement>element[0];
            var maxLength: number = parseInt(element.attr("maxlength"), 10);
            var modelCtrl: any = element.controller("ngModel");
            var latestValidValue: number;
            var latestValidFormattedValue: string;

            scope.precision = scope.precision || NumericInputDirective.defaultPrecision;

            element.attr("inputmode", "numeric");

            element.on("focus", () => {
                element.data("focused", true);
            });

            element.on("focusout", () => {
                element.data("focused", false);

                if (scope.strictFormat && scope.precision > 0) {
                    latestValidValue = modelCtrl.$modelValue;
                    latestValidFormattedValue = this.numberFilter(modelCtrl.$modelValue, scope.precision);

                    element.val(latestValidFormattedValue);
                }
            });

            element.on("mouseup", () => {
                if (element.data("focused")) {
                    input.select();
                    element.data("focused", false);
                }
            });

            element.on("keydown", (event: JQueryKeyEventObject) => {
                var value: string = element.val();
                var cursorPosition: number = input.selectionStart;

                // Special check on backspace keydown
                if (event.keyCode === 8) {
                    // Move cursor to the previous position if it is right after a thousand separator
                    if ((input.selectionStart === input.selectionEnd)
                        && (cursorPosition > 0)
                        && (value[cursorPosition - 1] === this.thousandSeparator)) {
                        input.selectionStart = cursorPosition - 1;
                        input.selectionEnd = cursorPosition - 1;
                    }

                    return;
                }

                // Special check on delete keydown
                if (event.keyCode === 46) {
                    // Move cursor to the next position if it is right before a thousand separator
                    if ((input.selectionStart === input.selectionEnd)
                        && (cursorPosition > 0)
                        && (value[cursorPosition] === this.thousandSeparator)) {
                        input.selectionStart = cursorPosition + 1;
                        input.selectionEnd = cursorPosition + 1;
                    }

                    return;
                }

                // Special case to prevent the user from inputting key 'E' which will result in exponent in float.
                if (event.keyCode === 69) {
                    event.preventDefault();

                    return;
                }
            });

            modelCtrl.$formatters.unshift(() => {
                latestValidValue = modelCtrl.$modelValue;
                latestValidFormattedValue = this.numberFilter(modelCtrl.$modelValue, scope.precision);

                return latestValidFormattedValue;
            });

            modelCtrl.$parsers.unshift((viewValue: string) => {
                if (modelCtrl.$isEmpty(viewValue)) {
                    latestValidValue = null;
                    latestValidFormattedValue = viewValue;

                    return latestValidValue;
                }

                if (latestValidFormattedValue === viewValue) {
                    return latestValidValue;
                }

                var caretPosition: number = input.selectionStart;
                var { formattedValue, value }: IOriginalAndFormattedValuePair = this.parseAndFormatViewValue(viewValue, scope.precision, false);

                if (!isNaN(value) && (scope.negativeAllowed || value >= 0)) {
                    var toBeValidValue: number = value;
                    var toBeValidFormattedValue: string = formattedValue;
                    var allowedLength: number = maxLength;

                    if (scope.precision > 0 && toBeValidFormattedValue.indexOf(this.decimalSeparator) < 0) {
                        allowedLength = maxLength - scope.precision - 1;
                    }

                    if (toBeValidFormattedValue.length > allowedLength) {
                        toBeValidValue = latestValidValue;
                        toBeValidFormattedValue = latestValidFormattedValue;
                    }

                    latestValidValue = toBeValidValue;
                    latestValidFormattedValue = toBeValidFormattedValue;
                }

                modelCtrl.$setViewValue(latestValidFormattedValue);
                modelCtrl.$render();
                this.setCaretPosition(
                    input,
                    caretPosition,
                    viewValue,
                    latestValidFormattedValue);

                return latestValidValue;
            });
        };

        private numberFilter: Function;
        private thousandSeparator: string;
        private decimalSeparator: string;

        constructor($filter: ng.IFilterService, appConfigService: AppConfigService) {
            this.numberFilter = $filter("number");
            this.thousandSeparator = appConfigService.thousandSeparator;
            this.decimalSeparator = appConfigService.decimalSeparator;
        }

        private parseAndFormatViewValue(viewValue: string, precision: number, rounding: boolean): IOriginalAndFormattedValuePair {
            var value: number;
            var formattedValue: string = "";
            var numberParts: string[] = viewValue ? viewValue.split(this.decimalSeparator) : [];

            if (numberParts.length > 1) {
                if (precision > 0 && !/[\,\.]/.test(numberParts[1])) {
                    var plainIntegerPart: string = numberParts[0].replace(/[\,]/g, "");
                    var decimalPart: string = rounding ? numberParts[1] : numberParts[1].substring(0, precision);

                    value = +(plainIntegerPart + this.decimalSeparator + decimalPart);

                    if (!isNaN(value)) {
                        formattedValue = rounding
                            ? this.numberFilter(value, precision)
                            : this.numberFilter(plainIntegerPart) + this.decimalSeparator + decimalPart;
                    }
                }
            } else if (numberParts.length > 0) {
                var plainNumber: string = numberParts[0].replace(/[\,]/g, "");

                value = +plainNumber;

                if (!isNaN(value)) {
                    formattedValue = this.numberFilter(value);
                }
            }

            return {
                formattedValue: formattedValue,
                value: value
            };
        }

        private setCaretPosition(
            input: HTMLInputElement,
            originalCaretPosition: number,
            originalValue: string,
            formattedValue: string): void {
            var caretPosition: number = originalCaretPosition;

            if (originalValue.length !== caretPosition) {
                var offset: number = originalValue.length - formattedValue.length;

                caretPosition = Math.max(0, caretPosition - offset);

                if (input.selectionStart === input.selectionEnd) {
                    input.selectionEnd = caretPosition;
                }

                input.selectionStart = caretPosition;
            }
        }
    }

    ModuleRegister.directive("numericInput", NumericInputDirective);
}