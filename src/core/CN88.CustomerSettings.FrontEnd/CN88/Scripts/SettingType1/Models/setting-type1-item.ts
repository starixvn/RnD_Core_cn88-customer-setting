/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/setting-item.ts" />
/// <reference path="agent-account-detail.ts" />
/// <reference path="master-account-detail.ts" />
/// <reference path="super-account-detail.ts" />
/// <reference path="member-account-detail.ts" />

namespace CustomerSetting.SettingType1 {
    export class SettingType1Item {
        public static factory(settingItem: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string): ISettingItem {
            if (settingItem) {
                if (customerLevel === Level.super) {
                    return new SuperAccountDetail(settingItem, backup, isNew, customerLevel, hashKey);
                }

                if (customerLevel === Level.master) {
                    return new MasterAccountDetail(settingItem, backup, isNew, customerLevel, hashKey);
                }

                if (customerLevel === Level.agent) {
                    return new AgentAccountDetail(settingItem, backup, isNew, customerLevel, hashKey);
                }

                if (customerLevel === Level.member) {
                    return new MemberAccountDetail(settingItem, backup, isNew, customerLevel, hashKey);
                }

                return new AccountDetail(settingItem, backup, isNew, customerLevel, hashKey);
            }
        }
    }
}