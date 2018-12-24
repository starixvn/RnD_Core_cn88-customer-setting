/// <reference path="enums/display-mode.ts" />
/// <reference path="enums/level.ts" />
/// <reference path="enums/message-type.ts" />
/// <reference path="setting-type.ts" />
/// <reference path="result.ts" />

module CustomerSetting {
    "use strict";

    export class Setting {
        public customerId: number;
        public customerLevel: Level;
        public productId: number;
        public productName: string;
        public settingTypes: Array<SettingType>;
        public warningMessage: UpdatingSettingResult;

        constructor();
        constructor(setting: any)
        constructor(setting?: any) {
            if (setting) {
                this.customerId = setting.customerId;
                this.customerLevel = setting.customerLevel.value;
                this.productId = setting.productId;
                this.productName = setting.productName;

                if (setting.warningMessage) {
                    this.initializeWarningMessage(setting.warningMessage);
                }

                if (setting.settingTypes) {
                    this.settingTypes = setting.settingTypes.map((settingType: any) => new SettingType(settingType));
                } else {
                    this.settingTypes = new Array<SettingType>();
                }
            }
        }

        public getEditableSettingTypes(): Setting {
            var newSetting: Setting;
            var editableSettingTypes: SettingType[] = this.getListOfEditableSettingTypes();

            if (editableSettingTypes && editableSettingTypes.length > 0) {
                newSetting = new Setting();

                newSetting.customerId = this.customerId;
                newSetting.customerLevel = this.customerLevel;
                newSetting.productId = this.productId;
                newSetting.productName = this.productName;
                newSetting.settingTypes = editableSettingTypes;
            }

            return newSetting;
        }

        public getChanges(): Setting {
            var newSetting: Setting;
            var matchedSettingTypes: Array<SettingType> = this.getListOfEditableSettingTypes();
            var changedSettingTypes: Array<SettingType> = new Array<SettingType>();

            if (matchedSettingTypes && matchedSettingTypes.length > 0) {
                matchedSettingTypes.forEach((settingType: SettingType) => {
                    var changedSettingType: SettingType = settingType.getChanges();

                    if (changedSettingType) {
                        changedSettingTypes.push(changedSettingType);
                    }
                });
            }

            if (changedSettingTypes.length > 0) {
                newSetting = new Setting();

                newSetting.customerId = this.customerId;
                newSetting.customerLevel = this.customerLevel;
                newSetting.productId = this.productId;
                newSetting.productName = this.productName;
                newSetting.settingTypes = changedSettingTypes;
            }

            return newSetting;
        }

        public validate(): boolean {
            var editableSettingTypes = this.getListOfEditableSettingTypes();
            var validSettingTypes: boolean = editableSettingTypes.every((item: SettingType) => {
                return item.validate();
            });

            return this.customerLevel < Level.company
                && this.customerId > 0
                && this.productId > 0
                && validSettingTypes;
        }

        public toJson(): {} {
            var setting: any = {
                customerId: this.customerId,
                customerLevel: {
                    value: this.customerLevel
                },
                productId: this.productId,
                productName: this.productName,
                settingTypes: []
            };

            this.settingTypes.forEach((settingType: SettingType) => {
                setting.settingTypes.push(settingType.toJson());
            });

            return setting;
        }

        private getListOfEditableSettingTypes(): SettingType[] {
            return this.settingTypes.filter((settingType: SettingType) => {
                return settingType.displayMode === DisplayMode.editable;
            });
        }

        private initializeWarningMessage(warningMessage: any): void {
            this.warningMessage = new UpdatingSettingResult(warningMessage);
            this.warningMessage.msgType = MessageType.warning;

            if (this.warningMessage.subResults) {
                this.warningMessage.subResults.forEach((subResult: CustomerSettingResult) => {
                    subResult.settingResults.forEach((settingResult: SettingResult) => {
                        settingResult.msgType = MessageType.warning;
                    });
                });
            }
        }
    }
}