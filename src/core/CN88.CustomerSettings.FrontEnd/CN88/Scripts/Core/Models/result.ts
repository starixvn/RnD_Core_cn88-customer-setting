/// <reference path="enums/message-type.ts" />
/// <reference path="exceptions.ts" />

module CustomerSetting {
    "use strict";

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
        public isFailed: boolean;

        public get isSuccessful(): boolean {
            if (this.subResults) {
                var allSubResultsSucceeded: boolean = this.subResults.every((result: CustomerSettingResult) => {
                    return result.isSuccessful;
                });

                return !this.isFailed && allSubResultsSucceeded;
            }

            return !this.isFailed;
        }

        constructor(result: any) {
            if (result) {
                super(result.code, result.message, result.msgType);

                if (result.isFailed) {
                    this.isFailed = result.isFailed;
                }

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