/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

module CustomerSetting {
    "use strict";

    export class MasterPTItem extends PTItem {
        public superPT: number;

        constructor(masterPTItem: any, backup: boolean = false) {
            if (masterPTItem) {
                super(masterPTItem);

                this.superPT = masterPTItem.superPT;
            }

            if (backup) {
                this.oldPTItem = new MasterPTItem(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldMasterPTItem: MasterPTItem = <MasterPTItem>this.oldPTItem;

            return isChanged || this.superPT !== oldMasterPTItem.superPT;
        }

        public validate(): boolean {
            if (this.superPT < this.minPT) {
                this.validationState = new ValidationResult("SuperPTMustBeGreaterThanOrEqualTo", "superPT", [this.minPT]);

                return false;
            }

            if (this.superPT > this.maxPT) {
                this.validationState = new ValidationResult("SuperPTMustBeLessThanOrEqualTo", "superPT", [this.maxPT]);

                return false;
            }

            this.resetValidationState();

            return true;
        }

        public toJson(): {} {
            var json: any = super.toJson();

            json.superPT = this.superPT;

            return json;
        }
    }
}