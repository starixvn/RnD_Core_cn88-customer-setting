/// <reference path="models/enums/level.ts" />
/// <reference path="models/key-value-pair.ts" />
/// <reference path="models/setting-type.ts" />

namespace CustomerSetting {
    export interface ISettingTypeConstructor {
        (settingType: any, customerLevel?: Level): SettingType;
    }

    export class SettingTypeRegister {
        private static registeredSettingTypes: any = {};
        private static defaultSettingTypeInitializer: ISettingTypeConstructor = (settingType: any) => new SettingType(settingType);

        public static register(settingTypeId: number, settingTypeConstructor: ISettingTypeConstructor) {
            this.registeredSettingTypes[settingTypeId] = settingTypeConstructor;
        }

        public static getInitializer(settingTypeId: number): ISettingTypeConstructor {
            return this.registeredSettingTypes[settingTypeId] || this.defaultSettingTypeInitializer;
        }
    }
}