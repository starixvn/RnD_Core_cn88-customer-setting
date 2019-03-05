/// <reference path="../typings/angularjs/angular.d.ts" />

namespace CustomerSetting {
    export class StringFormater {
        public static format(originalString: string, ...replacements: any[]): string {
            return originalString.replace(/{(\d+)}/g, (match: string, index: number) => {
                return replacements[index] === undefined || replacements[index] === null
                    ? match
                    : replacements[index];
            });
        }
    }

    export class ArrayHelpers {
        public static join2DimensionalArray<T>(source: Array<Array<T> | T>): Array<T> {
            var result: Array<T> = new Array<T>();

            if (source && source.length > 0) {
                source.forEach((item: Array<T> | T) => {
                    if (item instanceof Array) {
                        result.push.apply(result, item);
                    } else {
                        result.push.apply(result, [item]);
                    }
                });
            }

            return result;
        }
    }

    export function debounce(func: Function, waitTime: number): any {
        var timeout: number;
        var timestamp: number;
        var context: any;
        var args: IArguments;

        var runLater = () => {
            var lastRun: number = new Date().getTime() - timestamp;

            if (lastRun > 0 && lastRun < waitTime) {
                timeout = setTimeout(runLater, waitTime - lastRun);
            } else {
                timeout = null;
                func.apply(context, args);
                context = null;
                args = null;
            }
        };

        context = this;
        args = arguments;

        return () => {
            timestamp = new Date().getTime();

            if (!timeout) {
                timeout = setTimeout(runLater, waitTime);
            }
        };
    }

    export function clone(source: any): any {
        return angular.copy(source);
    }

    export function stringifyJson(json: any): any {
        return angular.toJson(json);
    }

    export function equals(first: any, second: any): boolean {
        return angular.equals(first, second);
    }

    export function isNumber(value: any): boolean {
        return typeof value === "number";
    }
}