/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/enums/setting-level.ts" />
/// <reference path="../models/setting-item.ts" />

module CustomerSetting {
    "use strict";

    function ShowingName(): Function {
        return (settingItem: DefaultSettingItem, settingLevel: SettingLevel, productName: string) => {
            return settingLevel === SettingLevel.product
                ? productName
                : (settingLevel === SettingLevel.sportType
                    ? settingItem.sportTypeName
                    : settingItem.betTypeName);
        };
    }

    ModuleRegister.filter("showingName", ShowingName);
}