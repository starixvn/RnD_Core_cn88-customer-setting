/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/enums/display-mode.ts" />
/// <reference path="../services/app-config-service.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    function EditableFormFilter(parametersService: ParametersService): Function {
        return (setting: { settingTypes: Array<{ displayMode: number }> }) => {
            var hasEditableSettingType: boolean = false;

            if (setting && setting.settingTypes) {
                hasEditableSettingType = setting.settingTypes.some((item: { displayMode: number }) => {
                    return item.displayMode === DisplayMode.editable;
                });
            }

            return parametersService.isEditable && hasEditableSettingType;
        };
    }

    EditableFormFilter.$inject = ["parametersService"];
    ModuleRegister.filter("editableForm", EditableFormFilter);
}