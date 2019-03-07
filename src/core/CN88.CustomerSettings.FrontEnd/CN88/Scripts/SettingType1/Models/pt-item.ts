/// <reference path="../../core/models/enums/level.ts" />

namespace CustomerSetting.SettingType1 {
    export class PTItem extends DefaultSettingItem {
        public isPTReadOnly: boolean;

        constructor(ptItem: any, isPTReadOnly: boolean) {
            if (ptItem) {
                super(ptItem, true);

                this.isPTReadOnly = isPTReadOnly;
            }
        }

        public static validatePTItems(ptItems: PTItem[]): { isValid: boolean, validationState: ValidationResult } {
            var isValid: boolean = ptItems.every((ptItem: PTItem) => ptItem.validate());
            var validationState: ValidationResult;

            if (!isValid) {
                var invalidPtItems: PTItem[] = ptItems.filter((ptItem: PTItem) => ptItem.validationState !== Result.normal);
                var firstInvalidPtItemValidationState: ValidationResult = invalidPtItems[0].validationState;
                var firstInvalidPtItemIndex: number = ptItems.indexOf(invalidPtItems[0]);

                if (firstInvalidPtItemValidationState) {
                    validationState = new ValidationResult(
                        firstInvalidPtItemValidationState.message,
                        "ptItems-" + firstInvalidPtItemIndex + "-" + firstInvalidPtItemValidationState.triggeringFieldName,
                        firstInvalidPtItemValidationState.msgParams,
                        firstInvalidPtItemValidationState.extraData);
                }
            }

            return {
                isValid: isValid,
                validationState: validationState
            };
        }
    }
}