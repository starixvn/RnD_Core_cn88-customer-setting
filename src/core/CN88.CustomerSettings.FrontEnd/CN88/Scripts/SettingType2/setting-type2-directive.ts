/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../core/directives/defaultsettingtypeblock/default-setting-type-block.ts" />
/// <reference path="../core/models/enums/level.ts" />
/// <reference path="../core/models/setting-item.ts" />
/// <reference path="../core/models/setting-type.ts" />
/// <reference path="../core/services/app-config-service.ts" />
/// <reference path="../core/services/crossing-setting-types-service-bus.ts" />
/// <reference path="../core/services/setting-types-shared-values-service.ts" />
/// <reference path="../core/module-registrater.ts" />
/// <reference path="../core/setting-type-registrater.ts" />
/// <reference path="models/layout-type.ts" />
/// <reference path="models/setting-type2.ts" />
/// <reference path="models/setting-type2-item.ts" />
/// <reference path="resources/settingtype2-resource-service.ts" />

module CustomerSetting {
    "use strict";

    interface ISettingType2Scope extends IDefaultSettingTypeDirectiveScope {
        availableCommissionGroups: string[];
        availableLayoutTypes: Object;
        eachGroupStyle: {};
        hasMaxBetConstraint: boolean;
        hasMaxPerMatchConstraint: boolean;
        commissionPrecision: number;
        betSettingPrecision: number;
    }

    class SettingType2Directive extends DefaultSettingTypeGroupingDirective {
        public static $inject: string[] = [
            "$q",
            "settingType2ResourceService",
            "crossingSettingTypesServiceBus",
            "appConfigService",
            "settingTypesSharedValuesService"
        ];

        public link: ng.IDirectiveLinkFn = (scope: ISettingType2Scope) => {
            this.linkInternal(scope);

            scope.headerTemplateUrl = "setting-type2-header.html";
            scope.valueTemplateUrl = "setting-type2-value.html";
            scope.cssClass = "full-width";
            scope.settingTypeName = this.settingType2ResourceService.getLabel("CommissionAndBetLimit");
            scope.availableLayoutTypes = LayoutType;
            scope.commissionPrecision = (<SettingType2>scope.settingType).commissionPrecision;
            scope.betSettingPrecision = (<SettingType2>scope.settingType).betSettingPrecision;

            if (scope.settingType.settingItems && scope.settingType.settingItems.length > 0) {
                scope.availableCommissionGroups = this.getAllAvailableGroups(scope.settingType.settingItems);

                if (scope.availableCommissionGroups.length > 0) {
                    scope.settingType.settingItems.forEach((settingItem: ISettingItem) => {
                        var settingType2Item: SettingType2Item = <SettingType2Item>settingItem;

                        settingType2Item.mapCommissionGroups(scope.availableCommissionGroups);
                    });
                    scope.eachGroupStyle = {
                        "min-width": (100 / scope.availableCommissionGroups.length) + "%"
                    };
                }

                if (scope.customerLevel === Level.super) {
                    scope.hasMaxBetConstraint = scope.settingType.settingItems.some((settingItem: ISettingItem) => {
                        var settingType2Item: SettingType2Item = <SettingType2Item>settingItem;

                        return settingType2Item.minMaxBet > 0 || settingType2Item.maxMaxBet > 0;
                    });

                    scope.hasMaxPerMatchConstraint = scope.settingType.settingItems.some((settingItem: ISettingItem) => {
                        var settingType2Item: SettingType2Item = <SettingType2Item>settingItem;

                        return settingType2Item.minMaxPerMatch > 0 || settingType2Item.maxMaxPerMatch > 0;
                    });
                } else {
                    scope.hasMaxBetConstraint = true;
                    scope.hasMaxPerMatchConstraint = true;
                }
            }

            if (scope.customerLevel === Level.member) {
                this.crossingSettingTypesServiceBus.register("updateMemberSpreadGroup", (sharedData: any) => {
                    var deferred: ng.IDeferred<void> = this.$q.defer<void>();
                    var newMemberGroupName: string = sharedData.newMemberGroup;
                    var memberSelectedSpreadGroupIndex: number = scope.availableCommissionGroups.indexOf(newMemberGroupName);

                    scope.settingType.settingItems.forEach((settingItem: ISettingItem) => {
                        var settingType2Item: SettingType2Item = <SettingType2Item>settingItem;

                        settingType2Item.memberSpreadGroupIndex = memberSelectedSpreadGroupIndex;
                    });

                    deferred.resolve();

                    return deferred.promise;
                });
            }
        };

        private $q: ng.IQService;
        private settingType2ResourceService: SettingType2ResourceService;
        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;

        constructor(
            $q: ng.IQService,
            settingType2ResourceService: SettingType2ResourceService,
            crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus,
            appConfigService: AppConfigService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            super(
                appConfigService,
                settingTypesSharedValuesService,
                (settingItem: any, backup?: boolean, isNew?: boolean, customerLevel?: Level) => new SettingType2Item(settingItem, backup, isNew, customerLevel));

            this.$q = $q;
            this.settingType2ResourceService = settingType2ResourceService;
            this.crossingSettingTypesServiceBus = crossingSettingTypesServiceBus;
        }

        private getAllAvailableGroups(settingItems: ISettingItem[]): string[] {
            var allAvailableGroups: string[] = [];

            settingItems.forEach((settingItem: ISettingItem) => {
                var settingType2Item: SettingType2Item = <SettingType2Item>settingItem;

                if (settingType2Item
                    && settingType2Item.commissionGroups
                    && settingType2Item.commissionGroups.length > 0) {
                    settingType2Item.commissionGroups.forEach((commissionGroup: ICommissionGroup) => {
                        if (commissionGroup.id && allAvailableGroups.indexOf(commissionGroup.name) < 0) {
                            allAvailableGroups.push(commissionGroup.name);
                        }
                    });
                }
            });

            if (allAvailableGroups.length > 0) {
                allAvailableGroups.sort((firstGroupName: string, secondGroupName: string) => {
                    if (firstGroupName > secondGroupName) {
                        return 1;
                    }

                    if (firstGroupName < secondGroupName) {
                        return -1;
                    }

                    return 0;
                });
            }

            return allAvailableGroups;
        }
    }

    ModuleRegister.directive("settingtype2", SettingType2Directive);

    SettingTypeRegister.register(2, (settingType: any) => new SettingType2(settingType));
}