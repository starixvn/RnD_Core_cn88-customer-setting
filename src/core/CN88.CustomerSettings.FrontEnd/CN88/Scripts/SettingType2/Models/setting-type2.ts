/// <reference path="../../core/models/setting-type.ts" />

namespace CustomerSetting {
    export class SettingType2 extends SettingType {
        public commissionPrecision: number;
        public betSettingPrecision: number;

        constructor();
        constructor(settingType: any)
        constructor(settingType?: any) {
            super(settingType);

            this.commissionPrecision = 2;
            this.betSettingPrecision = 0;
        }
    }
}