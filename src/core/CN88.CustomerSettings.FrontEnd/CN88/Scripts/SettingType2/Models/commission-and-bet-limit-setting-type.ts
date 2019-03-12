/// <reference path="../../core/models/setting-type.ts" />

namespace CustomerSetting.SettingType2 {
    export class CommissionAndBetLimitSettingType extends SettingType {
        public commissionPrecision: number;
        public betSettingPrecision: number;

        constructor()
        constructor(settingType: any)
        constructor(settingType?: any) {
            super(settingType);

            this.commissionPrecision = 2;
            this.betSettingPrecision = 0;
        }
    }
}