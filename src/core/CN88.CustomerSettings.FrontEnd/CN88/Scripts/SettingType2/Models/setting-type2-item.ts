/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/models/setting-item.ts" />
/// <reference path="commission-group.ts" />

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

        protected isNew: boolean;
        protected customerLevel: Level;

        constructor();
        constructor(settingType2Item: any);
        constructor(settingType2Item: any, backup: boolean)
        constructor(settingType2Item: any, backup: boolean, isNew: boolean)
        constructor(settingType2Item: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(settingType2Item?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            super(settingType2Item);

            this.isNew = isNew;
            this.customerLevel = customerLevel;

            if (settingType2Item) {
                this.commissionGroups = settingType2Item.commissionGroups;
                this.maxBet = settingType2Item.maxBet;
                this.minMaxBet = settingType2Item.minMaxBet;
                this.maxMaxBet = settingType2Item.maxMaxBet;
                this.maxPerMatch = settingType2Item.maxPerMatch;
                this.minMaxPerMatch = settingType2Item.minMaxPerMatch;
                this.maxMaxPerMatch = settingType2Item.maxMaxPerMatch;
            }

            if (backup) {
                this.oldSetting = new SettingType2Item(this);
            }

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

        public validate(): boolean {
            this.resetValidationState();

            return true;
        }
    }
}