/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class SuperPTItem extends PTItem {
        public superMinPT: number;
        public currentSuperMinPT: number;
        public minSuperMinPT: number;
        public maxSuperMinPT: number;
        public superMaxPT: number;
        public currentSuperMaxPT: number;
        public minSuperMaxPT: number;
        public maxSuperMaxPT: number;

        constructor(superPTItem: any, isPTReadOnly: boolean) {
            if (superPTItem) {
                super(superPTItem, isPTReadOnly);

                this.currentSuperMinPT = superPTItem.currentSuperMinPT;
                this.minSuperMinPT = superPTItem.minSuperMinPT;
                this.maxSuperMinPT = superPTItem.maxSuperMinPT;
                this.currentSuperMaxPT = superPTItem.currentSuperMaxPT;
                this.minSuperMaxPT = superPTItem.minSuperMaxPT;
                this.maxSuperMaxPT = superPTItem.maxSuperMaxPT;

                if (isNumber(superPTItem.superMinPT)) {
                    this.superMinPT = superPTItem.superMinPT;
                }

                if (isNumber(superPTItem.superMaxPT)) {
                    this.superMaxPT = superPTItem.superMaxPT;
                }
            }

            this.ignoredProperties.push("minSuperMinPT");
            this.ignoredProperties.push("maxSuperMinPT");
            this.ignoredProperties.push("minSuperMaxPT");
            this.ignoredProperties.push("maxSuperMaxPT");

            if (this.isPTReadOnly) {
                this.ignoredProperties.push("currentSuperMinPT");
                this.ignoredProperties.push("superMinPT");
                this.ignoredProperties.push("currentSuperMaxPT");
                this.ignoredProperties.push("superMaxPT");
            }
        }

        public validate(): boolean {
            if (!this.isPTReadOnly) {
                if (isNumber(this.superMinPT) && !this.validateSuperMinPT()) {
                    return false;
                }

                if (isNumber(this.superMaxPT) && !this.validateSuperMaxPT()) {
                    return false;
                }

                var actualSuperMinPT = this.superMinPT || this.currentSuperMinPT;
                var actualSuperMaxPT = this.superMaxPT || this.currentSuperMaxPT;

                if (actualSuperMinPT > actualSuperMaxPT) {
                    this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualToSuperMaxPT", "superMinPT");

                    return false;
                }
            }

            this.resetValidationState();

            return true;
        }

        private validateSuperMinPT(): boolean {
            if (this.superMinPT < this.minSuperMinPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeGreaterThanOrEqualTo", "superMinPT", [this.minSuperMinPT]);

                return false;
            }

            if (this.superMinPT > this.maxSuperMinPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualTo", "superMinPT", [this.maxSuperMinPT]);

                return false;
            }

            return true;
        }

        private validateSuperMaxPT(): boolean {
            if (this.superMaxPT < this.minSuperMaxPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustBeGreaterThanOrEqualTo", "superMaxPT", [this.minSuperMaxPT]);

                return false;
            }

            if (this.superMaxPT > this.maxSuperMaxPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustBeLessThanOrEqualTo", "superMaxPT", [this.maxSuperMaxPT]);

                return false;
            }

            return true;
        }
    }
}