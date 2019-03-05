/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    function ValidSettingTypeFilter(): Function {
        return (settingType: { isError: boolean }) => {
            return !settingType.isError;
        };
    }

    ModuleRegister.filter("validSettingType", ValidSettingTypeFilter);
}