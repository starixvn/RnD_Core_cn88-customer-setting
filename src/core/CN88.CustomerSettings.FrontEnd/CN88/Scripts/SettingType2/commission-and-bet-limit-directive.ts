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
/// <reference path="models/commission-and-bet-limit-setting-item.ts" />
/// <reference path="models/commission-and-bet-limit-setting-type.ts" />
/// <reference path="resources/resource-service.ts" />

namespace CustomerSetting.SettingType2 {
    var commissionAndBetLimitItemFactory = (
        settingItem: any,
        backup?: boolean,
        isNew?: boolean,
        customerLevel?: Level) => new CommissionAndBetLimitSettingItem(settingItem, backup, isNew, customerLevel);

    interface ICommissionAndBetLimitScope extends IDefaultSettingTypeDirectiveScope {
        availableCommissionGroups: string[];
        availableLayoutTypes: Object;
        eachGroupStyle: {};
        hasMaxBetConstraint: boolean;
        hasMaxPerEventConstraint: boolean;
        commissionPrecision: number;
        betSettingPrecision: number;
    }

    class CommissionAndBetLimitDirective extends DefaultSettingTypeGroupingDirective {
        public static $inject: string[] = [
            "$q",
            "st2ResourceService",
            "crossingSettingTypesServiceBus",
            "appConfigService",
            "settingTypesSharedValuesService"
        ];

        public link: ng.IDirectiveLinkFn = (scope: ICommissionAndBetLimitScope) => {
            this.linkInternal(scope);

            scope.headerTemplateUrl = "./SettingType2/commission-and-bet-limit-header-template.html";
            scope.valueTemplateUrl = "./SettingType2/commission-and-bet-limit-value-template.html";
            scope.cssClass = "full-width";
            scope.settingTypeName = this.resourceService.getLabel("CommissionAndBetLimit");
            scope.availableLayoutTypes = LayoutType;
            scope.commissionPrecision = (<CommissionAndBetLimitSettingType>scope.settingType).commissionPrecision;
            scope.betSettingPrecision = (<CommissionAndBetLimitSettingType>scope.settingType).betSettingPrecision;

            if (scope.settingType.settingItems && scope.settingType.settingItems.length > 0) {
                scope.availableCommissionGroups = this.getAllAvailableGroups(scope.settingType.settingItems);

                if (scope.availableCommissionGroups.length > 0) {
                    scope.settingType.settingItems.forEach((settingItem: ISettingItem) => {
                        var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;

                        commissionAndBetLimitItem.mapCommissionGroups(scope.availableCommissionGroups);
                    });
                    scope.eachGroupStyle = {
                        "min-width": (100 / scope.availableCommissionGroups.length) + "%"
                    };
                }

                if (scope.customerLevel === Level.super) {
                    scope.hasMaxBetConstraint = scope.settingType.settingItems.some((settingItem: ISettingItem) => {
                        var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;

                        return commissionAndBetLimitItem.minMaxBet > 0 || commissionAndBetLimitItem.maxMaxBet > 0;
                    });

                    scope.hasMaxPerEventConstraint = scope.settingType.settingItems.some((settingItem: ISettingItem) => {
                        var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;

                        return commissionAndBetLimitItem.minMaxPerEvent > 0 || commissionAndBetLimitItem.maxMaxPerEvent > 0;
                    });
                } else {
                    scope.hasMaxBetConstraint = true;
                    scope.hasMaxPerEventConstraint = true;
                }
            }

            if (scope.customerLevel === Level.member) {
                this.crossingSettingTypesServiceBus.register("updateMemberSpreadGroup", (sharedData: any) => {
                    var deferred: ng.IDeferred<void> = this.$q.defer<void>();
                    var newMemberGroupName: string = sharedData.newMemberGroup;
                    var memberSelectedSpreadGroupIndex: number = scope.availableCommissionGroups.indexOf(newMemberGroupName);

                    scope.settingType.settingItems.forEach((settingItem: ISettingItem) => {
                        var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;

                        commissionAndBetLimitItem.memberSpreadGroupIndex = memberSelectedSpreadGroupIndex;
                    });

                    deferred.resolve();

                    return deferred.promise;
                });
            }
        };

        private $q: ng.IQService;
        private resourceService: ResourceService;
        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;

        constructor(
            $q: ng.IQService,
            resourceService: ResourceService,
            crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus,
            appConfigService: AppConfigService,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            super(appConfigService, settingTypesSharedValuesService, commissionAndBetLimitItemFactory);

            this.$q = $q;
            this.resourceService = resourceService;
            this.crossingSettingTypesServiceBus = crossingSettingTypesServiceBus;
        }

        private getAllAvailableGroups(settingItems: ISettingItem[]): string[] {
            var allAvailableGroups: string[] = [];

            settingItems.forEach((settingItem: ISettingItem) => {
                var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;

                if (commissionAndBetLimitItem
                    && commissionAndBetLimitItem.commissionGroups
                    && commissionAndBetLimitItem.commissionGroups.length > 0) {
                    commissionAndBetLimitItem.commissionGroups.forEach((commissionGroup: ICommissionGroup) => {
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

    ModuleRegister.directive("st2", CommissionAndBetLimitDirective);

    SettingTypeRegister.register(2, (settingType: any) => new CommissionAndBetLimitSettingType(settingType));
}