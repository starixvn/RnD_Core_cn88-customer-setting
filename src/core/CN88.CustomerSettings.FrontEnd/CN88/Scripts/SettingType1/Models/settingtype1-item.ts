/// <reference path="../../core/models/result.ts" />
/// <reference path="../../core/models/setting-item.ts" />
/// <reference path="customer-info.ts" />
/// <reference path="pt-item.ts" />
/// <reference path="super-pt-item.ts" />
/// <reference path="master-pt-item.ts" />
/// <reference path="agent-pt-item.ts" />

module CustomerSetting {
    "use strict";

    export class SettingType1Item extends DefaultSettingItem {
        public customerInfo: CustomerInfo;
        public ptItem: PTItem;

        constructor();
        constructor(settingItem: any);
        constructor(settingItem: any, backup: boolean)
        constructor(settingItem: any, backup: boolean, isNew: boolean)
        constructor(settingItem: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(settingItem?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            super(settingItem);

            if (settingItem) {
                this.customerInfo = new CustomerInfo(settingItem.customerInfo, backup, isNew, customerLevel);

                if (customerLevel === Level.super) {
                    this.ptItem = new SuperPTItem(settingItem.ptItem, backup);
                } else if (customerLevel === Level.master) {
                    this.ptItem = new MasterPTItem(settingItem.ptItem, backup);
                } else if (customerLevel === Level.agent) {
                    this.ptItem = new AgentPTItem(settingItem.ptItem, backup);
                }
            }

            if (backup) {
                this.oldSetting = new SettingType1Item(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();

            return isChanged || this.customerInfo.isChanged() || this.ptItem.isChanged();
        }

        public validate(): boolean {
            if (!this.customerInfo.validate()) {
                return false;
            }

            if (!this.ptItem.validate()) {
                return false;
            }

            return true;
        }
    }
}