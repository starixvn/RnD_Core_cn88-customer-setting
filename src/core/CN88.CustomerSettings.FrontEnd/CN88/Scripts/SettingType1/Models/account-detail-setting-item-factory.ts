/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/setting-item.ts" />
/// <reference path="account-detail-setting-item.ts" />
/// <reference path="agent-account-detail-setting-item.ts" />
/// <reference path="master-account-detail-setting-item.ts" />
/// <reference path="super-account-detail-setting-item.ts" />
/// <reference path="member-account-detail-setting-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class AccountDetailSettingItemFactory {
        public static create(settingItem: any, isNew: boolean, customerLevel: Level, hashKey: string): ISettingItem {
            if (settingItem) {
                if (customerLevel === Level.super) {
                    return new SuperAccountDetailSettingItem(settingItem, isNew, customerLevel, hashKey);
                }

                if (customerLevel === Level.master) {
                    return new MasterAccountDetailSettingItem(settingItem, isNew, customerLevel, hashKey);
                }

                if (customerLevel === Level.agent) {
                    return new AgentAccountDetailSettingItem(settingItem, isNew, customerLevel, hashKey);
                }

                if (customerLevel === Level.member) {
                    return new MemberAccountDetailSettingItem(settingItem, isNew, customerLevel, hashKey);
                }

                return new AccountDetailSettingItem(settingItem, isNew, customerLevel, hashKey);
            }
        }
    }
}