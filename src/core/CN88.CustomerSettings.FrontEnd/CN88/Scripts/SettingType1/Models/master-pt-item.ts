/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class MasterPTItem extends PTItem {
        public superPT: number;
        public currentSuperPT: number;
        public minSuperPT: number;
        public maxSuperPT: number;

        constructor(masterPTItem: any, isPTReadOnly: boolean) {
            if (masterPTItem) {
                super(masterPTItem, isPTReadOnly);

                this.currentSuperPT = masterPTItem.currentSuperPT;
                this.minSuperPT = masterPTItem.minSuperPT;
                this.maxSuperPT = masterPTItem.maxSuperPT;

                if (isNumber(masterPTItem.superPT)) {
                    this.superPT = masterPTItem.superPT;
                }
            }

            this.ignoredProperties.push("minSuperPT");
            this.ignoredProperties.push("maxSuperPT");

            if (this.isPTReadOnly) {
                this.ignoredProperties.push("currentSuperPT");
                this.ignoredProperties.push("superPT");
            }
        }

        public validate(): boolean {
            if (!this.isPTReadOnly && isNumber(this.superPT)) {
                if (this.superPT < this.minSuperPT) {
                    this.validationState = new ValidationResult("SuperPTMustBeGreaterThanOrEqualTo", "superPT", [this.minSuperPT]);

                    return false;
                }

                if (this.superPT > this.maxSuperPT) {
                    this.validationState = new ValidationResult("SuperPTMustBeLessThanOrEqualTo", "superPT", [this.maxSuperPT]);

                    return false;
                }
            }

            this.resetValidationState();

            return true;
        }
    }
}