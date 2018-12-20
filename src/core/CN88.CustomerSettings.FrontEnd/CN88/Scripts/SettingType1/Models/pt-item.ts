/// <reference path="../../core/models/result.ts" />

module CustomerSetting {
    "use strict";

    export class PTItem {
        public minPT: number;
        public maxPT: number;
        public interval: number;
        public oldPTItem: PTItem;
        public validationState: ValidationResult;

        constructor(ptItem: any, backup: boolean = false) {
            if (ptItem) {
                this.minPT = ptItem.minPT;
                this.maxPT = ptItem.maxPT;
                this.interval = ptItem.interval;
            }

            if (backup) {
                this.oldPTItem = new PTItem(this);
            }

            this.resetValidationState();
        }

        public isChanged(): boolean {
            return this.oldPTItem
                && (this.minPT !== this.oldPTItem.minPT
                    || this.maxPT !== this.oldPTItem.maxPT
                    || this.interval !== this.oldPTItem.interval);
        }

        public validate(): boolean {
            return true;
        }

        public resetValidationState(): void {
            this.validationState = <ValidationResult>Result.normal;
        }

        public toJson(): any {
            return {
                minPT: this.minPT,
                maxPT: this.maxPT,
                interval: this.interval
            };
        }
    }
}