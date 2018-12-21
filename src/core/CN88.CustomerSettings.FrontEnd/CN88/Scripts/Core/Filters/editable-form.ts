/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />
/// <reference path="../services/app-config-service.ts" />

module CustomerSetting {
    "use strict";

    function EditableFormFilter(appConfigService: AppConfigService): Function {
        return (setting: { settingTypes: Array<{ displayMode: number }> }) => {
            var hasEditableSettingType: boolean = false;

            if (setting && setting.settingTypes) {
                hasEditableSettingType = setting.settingTypes.some((item: { displayMode: number }) => {
                    return item.displayMode === DisplayMode.editable;
                });
            }

            return appConfigService.canUpdate() && hasEditableSettingType;
        };
    }

    EditableFormFilter.$inject = ["appConfigService"];
    ModuleRegister.filter("editableForm", EditableFormFilter);
}