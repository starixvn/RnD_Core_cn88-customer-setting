/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/app-config-service.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface IOriginalAndFormattedValuePair {
        formattedValue: string;
        value: number;
    }

    interface IWindowService extends ng.IWindowService {
        clipboardData: DataTransfer;
    }

    interface INumericInputScope extends ng.IScope {
        precision: number;
        strictFormat: boolean;
    }

    class NumericInputDirective implements ng.IDirective {
        public static $inject: string[] = ["$filter", "$window", "appConfigService"];
        public static defaultPrecision: number = 0;

        public restrict: string = "A";

        public get scope(): any {
            return {
                precision: "=?numericInput",
                strictFormat: "@"
            };
        }

        public link: ng.IDirectiveLinkFn = (scope: INumericInputScope, originalElement: ng.IAugmentedJQuery) => {
            var element: ng.IAugmentedJQuery = angular.element(originalElement[0]);
            var maxLength: number = parseInt(element.attr("maxlength"), 10);
            var modelCtrl: any = element.controller("ngModel");

            scope.precision = scope.precision || NumericInputDirective.defaultPrecision;

            element.on("focus", () => {
                element.data("focused", true);
            });

            element.on("focusout", () => {
                element.data("focused", false);

                if (scope.strictFormat && scope.precision > 0) {
                    element.val(this.numberFilter(modelCtrl.$modelValue, scope.precision));
                }
            });

            element.on("mouseup", () => {
                if (element.data("focused")) {
                    var input: HTMLInputElement = <HTMLInputElement>element[0];

                    input.select();
                    element.data("focused", false);
                }
            });

            element.on("keydown", (event: JQueryEventObject) => {
                var value: string = element.val();
                var decimalSeparatorPosition = value.indexOf(this.decimalSeparator);
                var numberParts: string[] = value.split(this.decimalSeparator);
                var input: HTMLInputElement = <HTMLInputElement>element[0];
                var cursorPosition: number = input.selectionStart;

                if ([110, 190].indexOf(event.keyCode) >= 0) {
                    // Prevent more than one decimal character or do not allow decimal character in integer field
                    if (numberParts.length > 1 || scope.precision === 0) {
                        event.preventDefault();

                        return;
                    } else if (numberParts[0].length === 0) { // Integer part is required in decimal field
                        event.preventDefault();

                        return;
                    } else {
                        return;
                    }
                }

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

                if ([9, 27, 13].indexOf(event.keyCode) >= 0 // Allow: tab, escape, enter
                    || (event.keyCode === 65 && event.ctrlKey) // Allow: Ctrl+A
                    || (event.keyCode === 67 && event.ctrlKey) // Allow: Ctrl+C
                    || (event.keyCode === 86 && event.ctrlKey) // Allow: Ctrl+V
                    || (event.keyCode >= 35 && event.keyCode <= 40)) { // Allow: home, end, left, right, down, up
                    // let it happen, don't do anything
                    return;
                }

                // Only numeric characters are allowed
                if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57))
                    && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();

                    return;
                }

                // Do not allow number of numeric characters in decimal part exceeds the precision
                if (decimalSeparatorPosition >= 0
                    && decimalSeparatorPosition < cursorPosition
                    && numberParts[1].length >= scope.precision) {
                    event.preventDefault();

                    return;
                }

                // In decimal fields, (precision + 1) characters are booked for decimal part
                if (decimalSeparatorPosition < 0 && scope.precision > 0) {
                    var allowedLength: number = maxLength - scope.precision - 1;

                    if (value.length >= allowedLength) {
                        event.preventDefault();
                    }
                }
            });

            element.on("paste", (event: JQueryEventObject) => {
                var clipboardData: DataTransfer =
                    (<ClipboardEvent><Event>event).clipboardData
                    || this.$window.clipboardData
                    || (<ClipboardEvent>event.originalEvent).clipboardData;
                var clipboard: string = clipboardData.getData("Text");
                var { formattedValue }: IOriginalAndFormattedValuePair = this.parseAndFormatViewValue(clipboard, scope.precision, true);
                var input: HTMLInputElement = <HTMLInputElement>element[0];
                var selection: number = input.selectionEnd - input.selectionStart;
                var currentValue: string = element.val();

                event.preventDefault();

                if ((!currentValue || selection === currentValue.length) && formattedValue && formattedValue.length <= maxLength) {
                    modelCtrl.$setViewValue(formattedValue);
                    modelCtrl.$render();
                    input.selectionStart = formattedValue.length;
                }
            });

            modelCtrl.$formatters.unshift(() => {
                return this.numberFilter(modelCtrl.$modelValue, scope.precision);
            });

            modelCtrl.$parsers.unshift((viewValue: string) => {
                var input: HTMLInputElement = <HTMLInputElement>element[0];
                var caretPosition: number = input.selectionStart;
                var { formattedValue, value }: IOriginalAndFormattedValuePair = this.parseAndFormatViewValue(viewValue, scope.precision, false);

                element.val(formattedValue);

                if (viewValue.length !== caretPosition) {
                    var offset: number = viewValue.length - formattedValue.length;

                    caretPosition = Math.max(0, caretPosition - offset);

                    if (input.selectionStart === input.selectionEnd) {
                        input.selectionEnd = caretPosition;
                    }

                    input.selectionStart = caretPosition;
                }

                return value;
            });
        };

        private $window: IWindowService;
        private numberFilter: Function;
        private thousandSeparator: string;
        private decimalSeparator: string;

        constructor($filter: ng.IFilterService, $window: IWindowService, appConfigService: AppConfigService) {
            this.$window = $window;
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
                formattedValue = this.numberFilter(value);
            }

            return {
                formattedValue: formattedValue,
                value: value
            };
        }
    }

    ModuleRegister.directive("numericInput", NumericInputDirective);
}