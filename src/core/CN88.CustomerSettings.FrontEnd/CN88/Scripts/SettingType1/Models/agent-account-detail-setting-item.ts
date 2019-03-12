/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/helpers.ts" />
/// <reference path="agent-pt-item.ts" />
/// <reference path="master-account-detail-setting-item.ts" />
/// <reference path="pt-item.ts" />

namespace CustomerSetting.SettingType1 {
    export class AgentAccountDetailSettingItem extends MasterAccountDetailSettingItem {
        constructor()
        constructor(accountDetail: any)
        constructor(accountDetail: any, isNew: boolean)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, isNew, customerLevel, hashKey);

                this.ptItems = accountDetail.ptItems.map((ptItem: any) => new AgentPTItem(ptItem, accountDetail.isPTReadOnly));
            }
        }
    }
}