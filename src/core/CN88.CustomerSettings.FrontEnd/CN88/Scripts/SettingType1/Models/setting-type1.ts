/// <reference path="../../core/models/setting-type.ts" />

namespace CustomerSetting {
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