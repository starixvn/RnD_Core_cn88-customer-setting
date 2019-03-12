/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class MasterPTItem extends PTItem {
        public currentSuperPT: number;
        public superPT: number;
        public minSuperPT: number;
        public maxSuperPT: number;

        constructor(masterPTItem: any, isPTReadOnly: boolean) {
            if (masterPTItem) {
                super(masterPTItem);

                this.currentSuperPT = masterPTItem.currentSuperPT;
                this.superPT = masterPTItem.superPT;
                this.minSuperPT = masterPTItem.minSuperPT;
                this.maxSuperPT = masterPTItem.maxSuperPT;
            }

            this.ignoredProperties = this.ignoredProperties.concat(["minSuperPT", "maxSuperPT"]);

            if (isPTReadOnly) {
                this.ignoredProperties = this.ignoredProperties.concat(["currentSuperPT", "superPT"]);
            }
        }

        public validate(): boolean {
            if (!isNumber(this.superPT)) {
                this.validationState = new ValidationResult("SuperPTIsOutOfRange", "superPT");

                return false;
            }

            if (this.superPT < this.minSuperPT) {
                this.validationState = new ValidationResult("SuperPTMustBeGreaterThanOrEqualTo", "superPT", [this.minSuperPT]);

                return false;
            }

            if (this.superPT > this.maxSuperPT) {
                this.validationState = new ValidationResult("SuperPTMustBeLessThanOrEqualTo", "superPT", [this.maxSuperPT]);

                return false;
            }

            this.resetValidationState();

            return true;
        }
    }
}