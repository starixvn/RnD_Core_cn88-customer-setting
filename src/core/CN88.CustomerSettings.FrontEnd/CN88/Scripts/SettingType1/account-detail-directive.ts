/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../core/directives/settingtypebase/setting-type-base-directive.ts" />
/// <reference path="../core/models/enums/level.ts" />
/// <reference path="../core/models/setting-item.ts" />
/// <reference path="../core/models/setting-type.ts" />
/// <reference path="../core/services/app-config-service.ts" />
/// <reference path="../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../core/module-registrater.ts" />
/// <reference path="models/account-detail-setting-item-factory.ts" />
/// <reference path="models/account-detail-setting-type.ts" />
/// <reference path="resources/resource-service.ts" />

namespace CustomerSetting.SettingType1 {
    class AccountDetailDirective extends SettingTypeBaseDirective {
        public static $inject: string[] = [
            "st1ResourceService",
            "appConfigService",
            "settingTypesSharedValuesService"
        ];

        public templateUrl: string = "account-detail-template.htm";

        public link: ng.IDirectiveLinkFn = (scope: ISettingTypeDirectiveSharedScope) => {
            this.linkInternal(scope);

            scope.settingTypeName = this.resourceService.getLabel("AccountDetail");

            if (scope.settingType && angular.isArray(scope.settingType.settingItems)) {
                if (scope.settingType.isError) {
                    if (scope.settingType.settingItems.length > 0) {
                        scope.settingType.settingItems[0] = new ErrorSettingItem(scope.settingType.settingItems[0]);
                    }
                } else {
                    scope.settingType.settingItems.forEach((item: any, index: number) => {
                        if (item) {
                            scope.settingType.settingItems[index] = AccountDetailSettingItemFactory.create(
                                item,
                                true,
                                scope.isNew,
                                scope.customerLevel,
                                this.settingTypesSharedValuesService.hashKey);
                        }
                    });
                }
            }
        };

        private resourceService: ResourceService;

        constructor(
            resourceService: ResourceService,
            appConfigService: AppConfigService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            super(appConfigService, settingTypesSharedValuesService, null);

            this.resourceService = resourceService;
        }
    }

    ModuleRegister.directive("st1", AccountDetailDirective);

    SettingTypeRegister.register(1, (settingType: any) => new AccountDetailSettingType(settingType));
}