/// <reference path="../enums/level.ts" />
/// <reference path="../exceptions.ts" />
/// <reference path="../result.ts" />
/// <reference path="../setting.ts" />
/// <reference path="abstract-setting-query.ts" />

module CustomerSetting {
    "use strict";

    export class SettingCommand {
        public uplineLevel: Level;
        public setting: Setting;

        constructor(uplineLevel: Level, setting: Setting) {
            this.uplineLevel = uplineLevel;
            this.setting = setting;
        }

        public validate(): Result {
            if ((this.uplineLevel && (this.uplineLevel <= Level.member || this.uplineLevel >= Level.company)) || !this.setting) {
                return Result.invalidParameters;
            }

            return Result.normal;
        }

        public toJson(): {} {
            return {
                uplineLevel: this.uplineLevel,
                setting: JSON.stringify(this.setting.toJson())
            };
        }
    }
}