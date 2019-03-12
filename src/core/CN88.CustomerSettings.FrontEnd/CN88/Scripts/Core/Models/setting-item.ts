/// <reference path="../helpers.ts" />
/// <reference path="enums/level.ts" />
/// <reference path="result.ts" />

namespace CustomerSetting {
    export interface ISettingItem {
        typeName: string;
        oldSetting: ISettingItem;
        isChanged(): boolean;
        validate(): boolean;
        resetValidationState(): void;
        toJson(): {};
    }

    export class DefaultSettingItem implements ISettingItem {
        public sportTypeId: number;
        public sportTypeName: string;
        public betTypeId: number;
        public betTypeName: string;
        public typeName: string;
        public isLive: boolean;
        public displayOrder: number;
        public layoutType: number;
        public intervalValue: number;
        public oldSetting: DefaultSettingItem;
        public validationState: ValidationResult;

        protected isNew: boolean;
        protected customerLevel: Level;
        protected ignoredProperties: string[];

        constructor()
        constructor(settingItem: any)
        constructor(settingItem: any, isNew: boolean)
        constructor(settingItem: any, isNew: boolean, customerLevel: Level)
        constructor(settingItem?: any, isNew: boolean = false, customerLevel: Level = null) {
            this.isNew = isNew;
            this.customerLevel = customerLevel;

            if (settingItem) {
                this.sportTypeId = settingItem.sportTypeId;
                this.sportTypeName = settingItem.sportTypeName;
                this.betTypeId = settingItem.betTypeId;
                this.betTypeName = settingItem.betTypeName;
                this.typeName = settingItem.typeName;
                this.displayOrder = settingItem.childOrder;
                this.layoutType = settingItem.layoutType;
                this.intervalValue = settingItem.intervalValue;

                if (settingItem.isLive !== undefined) {
                    this.isLive = settingItem.isLive;
                }

                this.oldSetting = clone(settingItem);
            }

            this.resetValidationState();
            this.ignoredProperties = [
                "sportTypeName",
                "betTypeName",
                "displayOrder",
                "layoutType",
                "intervalValue",
                "oldSetting",
                "validationState",
                "ignoredProperties",
                "isNew",
                "customerLevel"];
        }

        public isChanged(): boolean {
            return Object.keys(this).some((prop: string) => {
                if ((this.ignoredProperties.indexOf(prop) < 0)
                    && (prop.charAt(0) !== "$")
                    && !equals(this[prop], this.oldSetting[prop])) {
                    return true;
                }
            });
        }

        public validate(): boolean {
            return true;
        }

        public resetValidationState(): void {
            this.validationState = <ValidationResult>Result.normal;
        }

        public toJson(): any {
            var settingItem: any = {};

            Object.keys(this).forEach((prop: string) => {
                if (this[prop] !== undefined && this[prop] !== null && this.ignoredProperties.indexOf(prop) < 0) {
                    if (this[prop].toJson) {
                        settingItem[prop] = this[prop].toJson();
                    } else {
                        settingItem[prop] = this[prop];
                    }
                }
            });

            return settingItem;
        }
    }

    export class ErrorSettingItem implements ISettingItem {
        public errorMessage: string;
        public typeName: string;
        public oldSetting: DefaultSettingItem;

        constructor()
        constructor(errorItem: any)
        constructor(errorItem?: any) {
            if (errorItem && errorItem.errorMessage) {
                this.errorMessage = errorItem.errorMessage;
                this.typeName = errorItem.typeName;
            }
        }

        public isChanged(): boolean {
            return false;
        }

        public validate(): boolean {
            return true;
        }

        public resetValidationState(): void {
            return;
        }

        public toJson(): {} {
            return {
                errorMessage: this.errorMessage,
                typeName: this.typeName
            };
        };
    }
}