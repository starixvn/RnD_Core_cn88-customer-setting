/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

module CustomerSetting {
    "use strict";

    export class SuperPTItem extends PTItem {
        public superMinPT: number;
        public superMaxPT: number;

        constructor(superPTItem: any, backup: boolean = false) {
            if (superPTItem) {
                super(superPTItem);

                this.superMinPT = superPTItem.superMinPT;
                this.superMaxPT = superPTItem.superMaxPT;
            }

            if (backup) {
                this.oldPTItem = new SuperPTItem(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSuperPTItem: SuperPTItem = <SuperPTItem>this.oldPTItem;

            return isChanged
                || this.superMinPT !== oldSuperPTItem.superMinPT
                || this.superMaxPT !== oldSuperPTItem.superMaxPT;
        }

        public validate(): boolean {
            if (!this.validateSuperMinPT()) {
                return false;
            }

            if (!this.validateSuperMaxPT()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        public validateSuperMinPT(): boolean {
            if (this.superMinPT < this.minPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeGreaterThanOrEqualTo", "superMinPT", [this.minPT]);

                return false;
            }

            if (this.superMinPT > this.maxPT) {
                this.validationState = new ValidationResult("SuperMinPTMustBeLessThanOrEqualTo", "superMinPT", [this.maxPT]);

                return false;
            }

            return true;
        }

        public validateSuperMaxPT(): boolean {
            if (this.superMaxPT < this.minPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustBeGreaterThanOrEqualTo", "superMaxPT", [this.minPT]);

                return false;
            }

            if (this.superMaxPT > this.maxPT) {
                this.validationState = new ValidationResult("SuperMaxPTMustLessThanOrEqualTo", "superMaxPT", [this.minPT]);

                return false;
            }

            return true;
        }

        public toJson(): {} {
            var json: any = super.toJson();

            json.superMinPT = this.superMinPT;
            json.superMaxPT = this.superMaxPT;

            return json;
        }
    }
}