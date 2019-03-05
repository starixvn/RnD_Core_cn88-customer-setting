/// <reference path="enums/display-mode.ts" />
/// <reference path="key-value-pair.ts" />
/// <reference path="setting-item.ts" />

namespace CustomerSetting {
    export class SettingType {
        public id: number;
        public name: string;
        public settingLevel: number;
        public settingItems: ISettingItem[];
        public typeName: string;
        public displayOrder: number;
        public displayMode: number;
        public isError: boolean;

        constructor();
        constructor(settingType: any)
        constructor(settingType?: any) {
            if (settingType) {
                this.id = settingType.id;
                this.name = settingType.name;
                this.settingLevel = settingType.settingLevel;
                this.settingItems = settingType.settingItems;
                this.typeName = settingType.typeName;
                this.displayOrder = settingType.displayOrder;
                this.displayMode = settingType.displayMode;
                this.isError = settingType.isError;
            }
        }

        public getChanges(): SettingType {
            var updatedSettingType: SettingType;
            var changedSettingItems: ISettingItem[] = this.settingItems.filter((item: ISettingItem) => {
                if (item.isChanged) {
                    return item.isChanged();
                } else {
                    return false;
                }
            });

            if (changedSettingItems && changedSettingItems.length > 0) {
                updatedSettingType = new SettingType();

                updatedSettingType.id = this.id;
                updatedSettingType.name = this.name;
                updatedSettingType.settingLevel = this.settingLevel;
                updatedSettingType.settingItems = changedSettingItems;
                updatedSettingType.typeName = this.typeName;
                updatedSettingType.displayOrder = this.displayOrder;
                updatedSettingType.displayMode = this.displayMode;
                updatedSettingType.isError = this.isError;
            }

            return updatedSettingType;
        }

        public validate(): boolean {
            var validSettingItems: boolean = this.settingItems.every((settingItem: ISettingItem) => {
                return !settingItem.validate || settingItem.validate();
            });

            return this.id > 0
                && this.settingLevel > 0
                && this.typeName !== undefined
                && validSettingItems;
        }

        public resetValidationState(): void {
            this.settingItems.forEach((settingItem: ISettingItem) => {
                settingItem.resetValidationState();
            });
        }

        public toJson(): {} {
            var settingType: any = {
                id: this.id,
                settingLevel: this.settingLevel,
                settingItems: [],
                typeName: this.typeName,
                displayOrder: this.displayOrder,
                displayMode: this.displayMode,
                isError: this.isError
            };

            this.settingItems.forEach((settingItem: ISettingItem) => {
                settingType.settingItems.push(settingItem.toJson());
            });

            return settingType;
        }
    }
}