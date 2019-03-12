/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class SuperPTItem extends PTItem {
        public currentSuperMinPT: number;
        public superMinPT: number;
        public minSuperMinPT: number;
        public maxSuperMinPT: number;
        public currentSuperMaxPT: number;
        public superMaxPT: number;
        public minSuperMaxPT: number;
        public maxSuperMaxPT: number;

        constructor(superPTItem: any, isPTReadOnly: boolean) {
            if (superPTItem) {
                super(superPTItem);

                this.currentSuperMinPT = superPTItem.currentSuperMinPT;
                this.superMinPT = superPTItem.superMinPT;
                this.minSuperMinPT = superPTItem.minSuperMinPT;
                this.maxSuperMinPT = superPTItem.maxSuperMinPT;
                this.currentSuperMaxPT = superPTItem.currentSuperMaxPT;
                this.superMaxPT = superPTItem.superMaxPT;
                this.minSuperMaxPT = superPTItem.minSuperMaxPT;
                this.maxSuperMaxPT = superPTItem.maxSuperMaxPT;
            }

            this.ignoredProperties = this.ignoredProperties.concat([
                "minSuperMinPT",
                "maxSuperMinPT",
                "minSuperMaxPT",
                "maxSuperMaxPT"]);

            if (isPTReadOnly) {
                this.ignoredProperties = this.ignoredProperties.concat([
                    "currentSuperMinPT",
                    "superMinPT",
                    "currentSuperMaxPT",
                    "superMaxPT"]);
            }
        }

        public validate(): boolean {
            if (!this.validateSuperMinPT()) {
                return false;
            }

            if (!this.validateSuperMaxPT()) {
                return false;
            }

            if (this.superMinPT > this.superMaxPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualToSuperMaxPT", "superMinPT");

                return false;
            }

            this.resetValidationState();

            return true;
        }

        private validateSuperMinPT(): boolean {
            if (!isNumber(this.superMinPT)) {
                this.validationState = new ValidationResult("SuperMinPTIsOutOfRange", "superMinPT");

                return false;
            }

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
            if (!isNumber(this.superMaxPT)) {
                this.validationState = new ValidationResult("SuperMaxPTIsOutOfRange", "superMaxPT");

                return false;
            }

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