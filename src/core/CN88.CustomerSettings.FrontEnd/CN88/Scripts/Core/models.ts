/// <reference path="common.ts" />

module CustomerSetting {
    "use strict";

    export interface ISettingItem {
        typeName: string;
        oldSetting: ISettingItem;
        isChanged(): boolean;
        validate(): boolean;
        toJson(): {};
    }

    export enum Level {
        company = 5,
        super = 4,
        master = 3,
        agent = 2,
        member = 1
    }

    export class DefaultSettingItem implements ISettingItem {
        public sportTypeId: number;
        public sportTypeName: string;
        public betTypeId: number;
        public betTypeName: string;
        public typeName: string;
        public isLive: boolean;
        public oldSetting: DefaultSettingItem;
        public validationState: ValidationResult;
        public highlightState: {};

        constructor();
        constructor(settingItem: any);
        constructor(settingItem?: any) {
            if (settingItem) {
                this.sportTypeId = settingItem.sportTypeId;
                this.sportTypeName = settingItem.sportTypeName;
                this.betTypeId = settingItem.betTypeId;
                this.betTypeName = settingItem.betTypeName;
                this.typeName = settingItem.typeName;

                if (settingItem.isLive !== undefined) {
                    this.isLive = settingItem.isLive;
                }
            }

            this.validationState = <ValidationResult>ValidationResult.normal;
            this.highlightState = {};
        }

        public isChanged(): boolean {
            return (this.sportTypeId !== this.oldSetting.sportTypeId)
                || (this.sportTypeName !== this.oldSetting.sportTypeName)
                || (this.betTypeId !== this.oldSetting.betTypeId)
                || (this.betTypeName !== this.oldSetting.betTypeName);
        }

        public copySettingTo(targets: Array<DefaultSettingItem>): { numberOfCopiedItems: number, status: boolean } {
            var numberOfCopiedItems: number = 0;
            var status: boolean = true;

            if (targets) {
                numberOfCopiedItems = targets.filter((item: DefaultSettingItem) => {
                    var validItem: boolean = true;

                    if (this.sportTypeId !== item.sportTypeId || this.betTypeId !== item.betTypeId || this.isLive !== item.isLive) {
                        validItem = item.copySettingFrom(this);
                        status = status ? validItem : status;

                        return validItem ? item.hasHighlightField(NotificationType.success) : validItem;
                    }

                    return false;
                }).length;
            }

            return { numberOfCopiedItems: numberOfCopiedItems, status: status };
        }

        public copySettingFrom(source: DefaultSettingItem): boolean {
            throw Exceptions.notImplemented("copyFrom");
        }

        public validate(): boolean {
            return true;
        }

        public toJson(): {} {
            var settingItem: any = {};
            var reservedProperties: string[] = ["oldSetting", "validationState", "highlightState", "$$hashKey"];

            Object.keys(this).forEach((prop: string) => {
                if (this[prop] !== undefined && this[prop] !== null && reservedProperties.indexOf(prop) < 0) {
                    if (this[prop].toJson) {
                        settingItem[prop] = this[prop].toJson();
                    } else {
                        settingItem[prop] = this[prop];
                    }
                }
            });

            return settingItem;
        }

        public changeFieldHighlightState(fieldName: string, state: NotificationType = NotificationType.none): void {
            if (fieldName) {
                this.highlightState[fieldName] = state;
            }
        }

        public resetValidationState(): void {
            this.validationState = <ValidationResult>Result.normal;
        }

        public highlightCopiedFields(state: {}): void {
            if (state) {
                for (var key in state) {
                    if (this[key] !== state[key]) {
                        this.changeFieldHighlightState(key, NotificationType.success);
                    }
                }
            }
        }

        public highlightUnablyCopiedField(): void {
            this.resetHighlightState();
            this.changeFieldHighlightState(this.validationState.fieldName, NotificationType.error);
            this.resetValidationState();
        }

        public hasHighlightField(type: NotificationType): boolean {
            for (var key in this.highlightState) {
                if (this.highlightState[key] === type) {
                    return true;
                }
            }

            return false;
        }

        public resetHighlightState(): void {
            this.highlightState = {};
        }

        public resetFieldHighlightState(fieldName: string): void {
            this.changeFieldHighlightState(fieldName);
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

    export class Setting {
        public customerId: number;
        public customerLevel: Level;
        public productId: number;
        public productName: string;
        public settingTypes: Array<SettingType>;
        public warningMessage: UpdatingSettingResult;
        public isNew: boolean;

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

        public getEditableSettingTypes(settingTypeId?: number): Setting {
            var newSetting: Setting;
            var editableSettingTypes: SettingType[];

            if (settingTypeId) {
                editableSettingTypes = this.settingTypes.filter((item: SettingType) => {
                    return item.id === settingTypeId && item.displayMode === DisplayMode.editable;
                });
            } else {
                editableSettingTypes = this.getListOfEditableSettingTypes();
            }

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

        public getChanges(settingTypeId?: number): Setting {
            var newSetting: Setting;
            var matchedSettingTypes: Array<SettingType>;
            var changedSettingTypes: Array<SettingType> = new Array<SettingType>();

            if (settingTypeId) {
                matchedSettingTypes = this.settingTypes.filter((item: SettingType) => {
                    return item.id === settingTypeId;
                });
            } else {
                matchedSettingTypes = this.getListOfEditableSettingTypes();
            }

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

    export class Result {
        public code: number;
        public message: string;
        public msgType: MessageType;

        public get isSuccessful(): boolean {
            return this.code === 0;
        }

        public get msgTypeName(): string {
            return MessageType[this.msgType.toString()];
        }

        public static get normal(): Result {
            return new Result(0, "", MessageType.success);
        }

        public static get systemError(): Result {
            return new Result(-1, "SystemError", MessageType.error);
        }

        public static get notAuthorizedError(): Result {
            return new Result(-2, "YouAreNotAuthorized", MessageType.error);
        }

        public static get noChangeToUpdate(): Result {
            return new Result(-3, "NoChangeToUpdate", MessageType.error);
        }

        public static get invalidParameters(): Result {
            return new Result(-4, Exceptions.invalidParams(), MessageType.error);
        }

        constructor(code: number, message: string, msgType: MessageType) {
            this.code = code;
            this.message = message;

            if (msgType) {
                this.msgType = msgType;
            } else if (this.code !== 0) {
                this.msgType = MessageType.error;
            } else {
                this.msgType = MessageType.success;
            }
        }
    }

    export class SettingResult extends Result {
        public settingTypeId: number;
        public settingTypeName: string;
        public messages: Array<string>;

        constructor(
            code: number,
            message: string,
            msgType: MessageType,
            settingTypeId: number,
            settingTypeName: string,
            messages: Array<string>) {
            super(code, message, msgType);
            this.settingTypeId = settingTypeId;
            this.settingTypeName = settingTypeName;
            this.messages = messages;
        }
    }

    export class CustomerSettingResult {
        public customerId: number;
        public customerName: string;
        public settingResults: Array<SettingResult>;

        public get isSuccessful(): boolean {
            return !this.settingResults
                || this.settingResults.every((result: SettingResult) => {
                    return result.isSuccessful;
                });
        }

        constructor(settingResult: any) {
            if (settingResult) {
                this.customerId = settingResult.customerId;
                this.customerName = settingResult.customerName;
                this.settingResults = new Array<SettingResult>();
                this.settingResults.push(new SettingResult(
                    settingResult.code,
                    settingResult.message,
                    this.detechMsgType(settingResult),
                    settingResult.settingTypeId,
                    settingResult.settingTypeName,
                    settingResult.subMessages));
            }
        }

        public pushSettingResult(settingResult: any) {
            if (settingResult) {
                this.settingResults.push(new SettingResult(
                    settingResult.code,
                    settingResult.message,
                    this.detechMsgType(settingResult),
                    settingResult.settingTypeId,
                    settingResult.settingTypeName,
                    settingResult.subMessages));
            }
        }

        public removeSuccessResults(): Array<SettingResult> {
            if (this.settingResults) {
                this.settingResults = this.settingResults.filter((result: SettingResult) => {
                    return !result.isSuccessful;
                });
            }

            return this.settingResults || new Array<SettingResult>();
        }

        public removeDuplicatedMessages(): void {
            var uniquedResults: Array<SettingResult> = new Array<SettingResult>();

            this.settingResults.forEach((item: SettingResult) => {
                if (!item.messages && uniquedResults.length > 0) {
                    var isDuplicated = uniquedResults.some((anotherItem: SettingResult) => {
                        return !anotherItem.messages
                            && item.code === anotherItem.code
                            && item.message === anotherItem.message;
                    });

                    if (!isDuplicated) {
                        uniquedResults.push(item);
                    }
                } else {
                    uniquedResults.push(item);
                }
            });

            this.settingResults = uniquedResults;
        }

        private detechMsgType(settingResult: any): MessageType {
            if (settingResult.isSuccessful) {
                return MessageType.success;
            } else if (settingResult.msgType !== 0) {
                return MessageType.error;
            } else {
                return MessageType.warning;
            }
        }
    }

    export class UpdatingSettingResult extends Result {
        public subResults: Array<CustomerSettingResult>;

        public get isSuccessful(): boolean {
            return !this.subResults
                || this.subResults.every((result: CustomerSettingResult) => {
                    return result.isSuccessful;
                });
        }

        constructor(result: any) {
            if (result) {
                super(result.code, result.message, result.msgType);

                if (result.subResults) {
                    this.subResults = new Array<CustomerSettingResult>();

                    result.subResults.forEach((subResult: any) => {
                        var existingCustomerResult: CustomerSettingResult = this.subResults.filter((item: CustomerSettingResult) => {
                            return item.customerId === subResult.customerId;
                        })[0];

                        if (existingCustomerResult) {
                            existingCustomerResult.pushSettingResult(subResult);
                        } else {
                            this.subResults.push(new CustomerSettingResult(subResult));
                        }
                    });

                    this.subResults.forEach((subResult: CustomerSettingResult) => {
                        subResult.removeDuplicatedMessages();
                    });
                }
            }
        }

        public removeSuccessResults(): Array<CustomerSettingResult> {
            if (this.subResults) {
                this.subResults = this.subResults.filter((result: CustomerSettingResult) => {
                    return !result.isSuccessful && result.removeSuccessResults().length > 0;
                });
            }

            return this.subResults || new Array<CustomerSettingResult>();
        }
    }

    export class ValidationResult extends Result {
        public fieldName: string;
        public msgParams: any[];

        constructor(message: string, fieldName?: string, msgParams?: any[]) {
            super(-8000, message, MessageType.error);
            this.fieldName = fieldName;
            this.msgParams = msgParams;
        }
    }
}