/// <reference path="enums/message-type.ts" />
/// <reference path="exceptions.ts" />

namespace CustomerSetting {
    export class Result {
        private static _normal: Result = new Result(0, "", MessageType.success);
        private static _systemError: Result = new Result(-1, "SystemError", MessageType.error);
        private static _notAuthorizedError: Result = new Result(-2, "YouAreNotAuthorized", MessageType.error);
        private static _noChangeToUpdate: Result = new Result(-3, "NoChangeToUpdate", MessageType.error);
        private static _invalidParameters: Result = new Result(-3, "NoChangeToUpdate", MessageType.error);

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
            return this._normal;
        }

        public static get systemError(): Result {
            return this._systemError;
        }

        public static get notAuthorizedError(): Result {
            return this._notAuthorizedError;
        }

        public static get noChangeToUpdate(): Result {
            return this._noChangeToUpdate;
        }

        public static get invalidParameters(): Result {
            return this._invalidParameters;
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

        public static detechMsgType(result: any): MessageType {
            if (result.isSuccessful) {
                return MessageType.success;
            } else if (result.msgType !== 0) {
                return MessageType.error;
            } else {
                return MessageType.warning;
            }
        }
    }

    export class SettingResult extends Result {
        public settingTypeId: number;
        public settingTypeName: string;
        public messages: string[];

        constructor(
            code: number,
            message: string,
            msgType: MessageType,
            settingTypeId: number,
            settingTypeName: string,
            messages: string[]) {
            super(code, message, msgType);
            this.settingTypeId = settingTypeId;
            this.settingTypeName = settingTypeName;
            this.messages = messages;
        }
    }

    export class CustomerSettingResult {
        public customerId: number;
        public customerName: string;
        public settingResults: SettingResult[];

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
                this.settingResults = [];
                this.settingResults.push(new SettingResult(
                    settingResult.code,
                    settingResult.message,
                    Result.detechMsgType(settingResult),
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
                    Result.detechMsgType(settingResult),
                    settingResult.settingTypeId,
                    settingResult.settingTypeName,
                    settingResult.subMessages));
            }
        }

        public removeSuccessResults(): SettingResult[] {
            if (this.settingResults) {
                this.settingResults = this.settingResults.filter((result: SettingResult) => {
                    return !result.isSuccessful;
                });
            }

            return this.settingResults || [];
        }

        public removeDuplicatedMessages(): void {
            var uniquedResults: SettingResult[] = [];

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
    }

    export class UpdatingSettingResult extends Result {
        public subResults: CustomerSettingResult[];
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
                super(result.code, result.message, Result.detechMsgType(result));

                if (result.isFailed) {
                    this.isFailed = result.isFailed;
                }

                if (result.subResults) {
                    this.subResults = [];

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

        public removeSuccessResults(): CustomerSettingResult[] {
            if (this.subResults) {
                this.subResults = this.subResults.filter((result: CustomerSettingResult) => {
                    return !result.isSuccessful && result.removeSuccessResults().length > 0;
                });
            }

            return this.subResults || [];
        }
    }

    export class ValidationResult extends Result {
        public triggeringFieldName: string;
        public msgParams: any[];
        public extraData: any;
        public affectedFields: string[];

        constructor(message: string, triggeringFieldName?: string, msgParams?: any[], extraData?: any) {
            super(-8000, message, MessageType.error);
            this.triggeringFieldName = triggeringFieldName;
            this.msgParams = msgParams;
            this.extraData = extraData || {};
            this.affectedFields = [];

            if (triggeringFieldName) {
                this.affectedFields.push(triggeringFieldName);
            }
        }

        public addAffectedField(fieldName: string): void {
            if (this.affectedFields.indexOf(fieldName) < 0) {
                this.affectedFields.push(fieldName);
            }
        }
    }
}