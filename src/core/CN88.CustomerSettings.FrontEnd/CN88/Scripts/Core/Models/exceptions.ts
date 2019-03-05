/// <reference path="../helpers.ts" />

namespace CustomerSetting {
    export class Exceptions {
        public static notImplemented(funcName: string): string {
            return StringFormater.format("{0} is not implemented", funcName);
        }

        public static negativeParam(paramName: string): string {
            return StringFormater.format("{0} must be greater than zero.", paramName);
        }

        public static invalidParams(): string {
            return "The request parameters have get some invalid values.";
        }

        public static invalidParam(paramName: string): string {
            return StringFormater.format("{0} has taken an invalid value.", paramName);
        }

        public static wrongCustomerLevel(): string {
            return "Wrong customer level.";
        }

        public static customerIdsNotMatchCustomerNames(): string {
            return "The number of customer IDs does not match with the number of customer names.";
        }
    }
}