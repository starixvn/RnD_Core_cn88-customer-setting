/// <reference path="../enums/level.ts" />
/// <reference path="../result.ts" />
/// <reference path="../setting.ts" />

module CustomerSetting {
    "use strict";

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
                setting: JSON.stringify(this.setting.toJson())
            };
        }
    }
}