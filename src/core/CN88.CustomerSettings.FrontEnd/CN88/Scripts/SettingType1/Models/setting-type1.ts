/// <reference path="../../core/models/setting-type.ts" />

namespace CustomerSetting.SettingType1 {
    export class SettingType1 extends SettingType {
        public creditPrecision: number;

        constructor();
        constructor(settingType: any)
        constructor(settingType?: any) {
            super(settingType);

            this.creditPrecision = 2;
        }
    }
}