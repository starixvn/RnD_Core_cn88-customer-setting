/// <reference path="../../helpers.ts" />
/// <reference path="../setting.ts" />

namespace CustomerSetting {
    export class SettingCommand {
        public uplineId: number;
        public setting: Setting;

        constructor(uplineId: number, setting: Setting) {
            this.uplineId = uplineId;
            this.setting = setting;
        }

        public toJson(): {} {
            return {
                uplineId: this.uplineId,
                setting: stringifyJson(this.setting.toJson())
            };
        }
    }
}