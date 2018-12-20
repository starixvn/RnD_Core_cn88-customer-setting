/// <reference path="key-value-pair.ts" />

module CustomerSetting {
    "use strict";

    export class SettingType {
        public id: number;
        public name: string;
        public settingLevel: number;
        public settingItems: Array<ISettingItem>;
        public typeName: string;
        public displayOrder: number;
        public displayMode: number;
        public isError: boolean;
        public extraData: Array<KeyValuePair<string, any>>;

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
                this.extraData = this.getExtraData(settingType);
            }
        }

        public getChanges(): SettingType {
            var updatedSettingType: SettingType;
            var changedSettingItems: Array<ISettingItem> = this.settingItems.filter((item: ISettingItem) => {
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
                updatedSettingType.extraData = this.extraData;
            }

            return updatedSettingType;
        }

        public validate(): boolean {
            var validSettingItems: boolean = this.settingItems.every((item: ISettingItem) => {
                return !item.validate || item.validate();
            });

            return this.id > 0
                && this.settingLevel > 0
                && this.typeName !== undefined
                && validSettingItems;
        }

        public toJson(): {} {
            var settingType: any = {
                id: this.id,
                name: this.name,
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

            if (this.extraData && this.extraData.length > 0) {
                this.extraData.forEach((extraItem: KeyValuePair<string, any>) => {
                    settingType[extraItem.key] = extraItem.value;
                });
            }

            return settingType;
        }

        private getExtraData(rawData: any): Array<KeyValuePair<string, any>> {
            var extraData: Array<KeyValuePair<string, any>> = new Array<KeyValuePair<string, any>>();
            var addedProperties: string[] = Object.keys(this).filter((prop: string) => {
                return prop !== "extraData";
            });

            extraData = Object.keys(rawData).filter((propName: string) => {
                return addedProperties.indexOf(propName) < 0;
            }).map((prop: string) => {
                return new KeyValuePair<string, any>(prop, rawData[prop]);
            });

            return extraData;
        }
    }
}