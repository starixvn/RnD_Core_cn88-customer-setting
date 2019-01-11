/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/models/setting-item.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="commission-group.ts" />
/// <reference path="layout-type.ts" />

module CustomerSetting {
    "use strict";

    export class SettingType2Item extends DefaultSettingItem {
        public commissionGroups: ICommissionGroup[];
        public maxBet: number;
        public minMaxBet: number;
        public maxMaxBet: number;
        public maxPerMatch: number;
        public minMaxPerMatch: number;
        public maxMaxPerMatch: number;

        public mappedCommissionGroups: ICommissionGroup[];
        public memberSpreadGroupIndex: number;

        constructor();
        constructor(settingType2Item: any);
        constructor(settingType2Item: any, backup: boolean)
        constructor(settingType2Item: any, backup: boolean, isNew: boolean)
        constructor(settingType2Item: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(settingType2Item?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            super(settingType2Item, isNew, customerLevel);

            if (settingType2Item) {
                this.commissionGroups = backup ? settingType2Item.commissionGroups : clone(settingType2Item.commissionGroups);
                this.maxBet = settingType2Item.maxBet;
                this.minMaxBet = settingType2Item.minMaxBet;
                this.maxMaxBet = settingType2Item.maxMaxBet;
                this.maxPerMatch = settingType2Item.maxPerMatch;
                this.minMaxPerMatch = settingType2Item.minMaxPerMatch;
                this.maxMaxPerMatch = settingType2Item.maxMaxPerMatch;
            }

            if (this.customerLevel === Level.member) {
                this.memberSpreadGroupIndex = 0;
            }

            if (backup) {
                this.oldSetting = new SettingType2Item(this);
            }

            this.ignoredProperties.push("mappedCommissionGroups");
            this.ignoredProperties.push("memberSpreadGroupIndex");

            this.resetValidationState();
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting: SettingType2Item = <SettingType2Item>this.oldSetting;
            var commisisonGroupChanged: boolean = this.commissionGroups.some((commissionGroup: ICommissionGroup, index: number) => {
                return commissionGroup.value !== oldSetting.commissionGroups[index].value;
            });

            return isChanged
                || commisisonGroupChanged
                || this.maxBet !== oldSetting.maxBet
                || this.maxPerMatch !== oldSetting.maxPerMatch;
        }

        public toJson(): any {
            var json: any = super.toJson();

            if (this.isNew && this.customerLevel === Level.member) {
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

            if (this.maxPerMatch === undefined || this.maxPerMatch === null) {
                this.validationState = new ValidationResult("RequiredField", "maxPerMatch");

                return false;
            }

            if (this.maxBet < this.minMaxBet) {
                this.validationState = new ValidationResult("MaxBetMustBeGreaterThanOrEqualTo", "maxBet", [this.minMaxBet]);

                return false;
            }

            if (this.maxPerMatch < this.minMaxPerMatch) {
                this.validationState = new ValidationResult("MaxPerMatchMustBeGreaterThanOrEqualTo", "maxPerMatch", [this.minMaxPerMatch]);

                return false;
            }

            if (this.customerLevel !== Level.super) {
                if (this.maxBet > this.maxMaxBet) {
                    this.validationState = new ValidationResult("MaxBetMustBeLessThanOrEqualTo", "maxBet", [this.maxMaxBet]);

                    return false;
                }

                if (this.maxPerMatch > this.maxMaxPerMatch) {
                    this.validationState = new ValidationResult("MaxPerMatchMustBeLessThanOrEqualTo", "maxPerMatch", [this.maxMaxPerMatch]);

                    return false;
                }
            }

            return true;
        }
    }
}