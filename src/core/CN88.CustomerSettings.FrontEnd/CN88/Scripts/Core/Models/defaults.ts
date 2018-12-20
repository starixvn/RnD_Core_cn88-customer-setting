module CustomerSetting {
    "use strict";

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
}