/// <reference path="result.ts" />

module CustomerSetting {
    "use strict";

    export interface ISettingItem {
        typeName: string;
        oldSetting: ISettingItem;
        isChanged(): boolean;
        validate(): boolean;
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
        public oldSetting: DefaultSettingItem;
        public validationState: ValidationResult;

        protected ignoredProperties: string[];

        constructor();
        constructor(settingItem: any);
        constructor(settingItem?: any) {
            if (settingItem) {
                this.sportTypeId = settingItem.sportTypeId;
                this.sportTypeName = settingItem.sportTypeName;
                this.betTypeId = settingItem.betTypeId;
                this.betTypeName = settingItem.betTypeName;
                this.typeName = settingItem.typeName;
                this.displayOrder = settingItem.childOrder;
                this.layoutType = settingItem.layoutType;

                if (settingItem.isLive !== undefined) {
                    this.isLive = settingItem.isLive;
                }
            }

            this.validationState = <ValidationResult>ValidationResult.normal;
            this.ignoredProperties = [
                "sportTypeName",
                "betTypeName",
                "oldSetting",
                "validationState",
                "ignoredProperties",
                "$$hashKey"];
        }

        public isChanged(): boolean {
            return (this.sportTypeId !== this.oldSetting.sportTypeId)
                || (this.sportTypeName !== this.oldSetting.sportTypeName)
                || (this.betTypeId !== this.oldSetting.betTypeId)
                || (this.betTypeName !== this.oldSetting.betTypeName);
        }

        public validate(): boolean {
            return true;
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

        public resetValidationState(): void {
            this.validationState = <ValidationResult>Result.normal;
        }
    }

    export class ErrorSettingItem implements ISettingItem {
        public errorMessage: string;
        public typeName: string;
        public oldSetting: DefaultSettingItem;

        constructor();
        constructor(errorItem: any);
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

        public toJson(): {} {
            return {
                errorMessage: this.errorMessage,
                typeName: this.typeName
            };
        };
    }
}