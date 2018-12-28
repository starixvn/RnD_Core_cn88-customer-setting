/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../services/app-config-service.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    function EditableSettingTypeFilter(appConfigService: AppConfigService): Function {
        return (item: { displayMode: number }) => {
            return appConfigService.canUpdate()
                && item
                && item.displayMode === DisplayMode.editable;
        };
    }

    EditableSettingTypeFilter.$inject = ["appConfigService"];
    ModuleRegister.filter("editableSettingType", EditableSettingTypeFilter);
}