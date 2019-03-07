/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/models/setting-item.ts" />
/// <reference path="commission-group.ts" />
/// <reference path="layout-type.ts" />

namespace CustomerSetting.SettingType2 {
    export class CommissionAndBetLimitSettingItem extends DefaultSettingItem {
        public commissionGroups: ICommissionGroup[];
        public maxBet: number;
        public minMaxBet: number;
        public maxMaxBet: number;
        public maxPerEvent: number;
        public minMaxPerEvent: number;
        public maxMaxPerEvent: number;

        public mappedCommissionGroups: ICommissionGroup[];
        public memberSpreadGroupIndex: number;

        constructor();
        constructor(settingType2Item: any);
        constructor(settingType2Item: any, backup: boolean)
        constructor(settingType2Item: any, backup: boolean, isNew: boolean)
        constructor(settingType2Item: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(settingType2Item?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            super(settingType2Item, backup, isNew, customerLevel);

            if (settingType2Item) {
                this.commissionGroups = settingType2Item.commissionGroups;
                this.maxBet = settingType2Item.maxBet;
                this.minMaxBet = settingType2Item.minMaxBet;
                this.maxMaxBet = settingType2Item.maxMaxBet;
                this.maxPerEvent = settingType2Item.maxPerEvent;
                this.minMaxPerEvent = settingType2Item.minMaxPerEvent;
                this.maxMaxPerEvent = settingType2Item.maxMaxPerEvent;
            }

            if (this.customerLevel === Level.member) {
                this.memberSpreadGroupIndex = 0;
            }

            this.ignoredProperties.push("mappedCommissionGroups");
            this.ignoredProperties.push("memberSpreadGroupIndex");

            this.resetValidationState();
        }

        public toJson(): any {
            var json: any = super.toJson();

            if (this.isNew && this.customerLevel === Level.member && this.mappedCommissionGroups.length > 0) {
                if (this.layoutType === LayoutType.multipleGroups) {
                    json.commissionGroups = [this.mappedCommissionGroups[this.memberSpreadGroupIndex]];
                } else {
                    json.commissionGroups = [this.mappedCommissionGroups[0]];
                }
            }

            return json;
        }

        public mapCommissionGroups(allAvailableGroups: string[]): void {
            this.mappedCommissionGroups = [];

            if (!this.commissionGroups || this.commissionGroups.length === 0) {
                return;
            }

            if (this.layoutType === LayoutType.singleGroup) {
                this.mappedCommissionGroups.push(this.commissionGroups[0]);

                return;
            }

            allAvailableGroups.forEach((groupName: string) => {
                var matchedCommissionGroups: ICommissionGroup[] = this.commissionGroups.filter((commissionGroup: ICommissionGroup) => {
                    return groupName === commissionGroup.name;
                });

                if (matchedCommissionGroups && matchedCommissionGroups.length > 0) {
                    this.mappedCommissionGroups.push(matchedCommissionGroups[0]);
                } else {
                    this.mappedCommissionGroups.push(null);
                }
            });
        }

        public validate(): boolean {
            if (!this.validateCommission()) {
                return false;
            }

            if (!this.validateBetLimit()) {
                return false;
            }

            this.resetValidationState();

            return true;
        }

        private validateCommission(): boolean {
            this.mappedCommissionGroups.forEach((commissionGroup: ICommissionGroup, index: number) => {
                if (commissionGroup.value > commissionGroup.maxValue || commissionGroup.value < commissionGroup.minValue) {
                    this.validationState = new ValidationResult("CommissionIsOutOfRange", "commissionGroups-" + index);

                    return false;
                }
            });

            return true;
        }

        private validateBetLimit(): boolean {
            if (this.maxBet === undefined || this.maxBet === null) {
                this.validationState = new ValidationResult("RequiredField", "maxBet");

                return false;
            }

            if (this.maxPerEvent === undefined || this.maxPerEvent === null) {
                this.validationState = new ValidationResult("RequiredField", "maxPerEvent");

                return false;
            }

            if (this.maxBet < this.minMaxBet) {
                this.validationState = new ValidationResult("MaxBetMustBeGreaterThanOrEqualTo", "maxBet", [this.minMaxBet]);

                return false;
            }

            if (this.maxPerEvent < this.minMaxPerEvent) {
                this.validationState = new ValidationResult("MaxPerEventMustBeGreaterThanOrEqualTo", "maxPerEvent", [this.minMaxPerEvent]);

                return false;
            }

            if (this.customerLevel !== Level.super) {
                if (this.maxBet > this.maxMaxBet) {
                    this.validationState = new ValidationResult("MaxBetMustBeLessThanOrEqualTo", "maxBet", [this.maxMaxBet]);

                    return false;
                }

                if (this.maxPerEvent > this.maxMaxPerEvent) {
                    this.validationState = new ValidationResult("MaxPerEventMustBeLessThanOrEqualTo", "maxPerEvent", [this.maxMaxPerEvent]);

                    return false;
                }
            }

            if (this.maxBet > this.maxPerEvent) {
                this.validationState = new ValidationResult("MaxBetMustBeLessThanOrEqualToMaxPerEvent", "maxBet");

                return false;
            }

            return true;
        }
    }
}