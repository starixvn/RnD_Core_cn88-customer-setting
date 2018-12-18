/// <reference path="helpers.ts" />

module CustomerSetting {
    "use strict";

    export enum System {
        nap = 1,
        alpha = 2,
        internalAlpha = 3
    }

    export enum SettingLevel {
        product = 1,
        sportType = 2,
        betType = 3,
        sportTypeBetType = 4
    }

    export enum MessageType {
        success = 0,
        warning = 1,
        error = 2
    }

    export enum DisplayMode {
        editable = 1,
        readOnly = 2,
        hidden = 3
    }

    export enum ActionType {
        query = 0,
        update = 1,
        multipleUpdate = 2
    }

    export enum NotificationType {
        none = 0,
        success = 1,
        warning = 2,
        error = 3
    }

    export enum PositionModel {
        relative = 1,
        absolute = 2,
        fixed = 3
    }

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

    export class Defaults {
        public static get system(): number {
            return 1;
        }

        public static get language(): string {
            return "en-US";
        }

        public static get percentageDivider(): number {
            return 100;
        }

        public static get displayingMsgTime(): number {
            return 3000;
        }
    }

    export class KeyValuePair<T, K> {
        public key: T;
        public value: K;

        constructor(key: T, value: K) {
            this.key = key;
            this.value = value;
        }
    }
}