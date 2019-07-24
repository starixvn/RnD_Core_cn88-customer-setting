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
    var commissionAndBetLimitItemFactory = (settingItem: any, isNew: boolean, customerLevel?: Level) => new CommissionAndBetLimitSettingItem(settingItem, isNew, customerLevel);

    interface ICommissionAndBetLimitScope extends IDefaultSettingTypeDirectiveScope {
        availableCommissionGroups: string[];
        availableLayoutTypes: Object;
        eachGroupStyle: {};
        commissionPrecision: number;
        betSettingPrecision: number;
    }

    class CommissionAndBetLimitDirective extends DefaultSettingTypeGroupingDirective {
        public static $inject: string[] = [
            "st2ResourceService",
            "crossingSettingTypesServiceBus",
            "settingTypesSharedValuesService"
        ];

        public link: ng.IDirectiveLinkFn = (scope: ICommissionAndBetLimitScope) => {
            this.linkInternal(scope);

            scope.headerTemplateUrl = "./SettingType2/commission-and-bet-limit-header-template.html";
            scope.valueTemplateUrl = "./SettingType2/commission-and-bet-limit-value-template.html";
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
            }

            if (scope.customerLevel === Level.super) {
                this.crossingSettingTypesServiceBus.register("reloadCurrency", (sharedData: any) => {
                    var newCommissionAndBetLimits: ICommissionAndBetLimitConstraint[] = <ICommissionAndBetLimitConstraint[]>sharedData.newCommissionAndBetLimits;

                    scope.settingType.settingItems.forEach((settingItem: ISettingItem) => {
                        var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;
                        var matchedNewCommissionAndBetLimitItems: ICommissionAndBetLimitConstraint[] = newCommissionAndBetLimits.filter((newCommissionAndBetLimit: ICommissionAndBetLimitConstraint) => {
                            return commissionAndBetLimitItem.sportTypeId === newCommissionAndBetLimit.sportTypeId
                                && commissionAndBetLimitItem.betTypeId === newCommissionAndBetLimit.betTypeId
                                && commissionAndBetLimitItem.isLive === newCommissionAndBetLimit.isLive;
                        });

                        if (matchedNewCommissionAndBetLimitItems.length > 0) {
                            commissionAndBetLimitItem.updateBetLimit(matchedNewCommissionAndBetLimitItems[0]);
                        } else {
                            commissionAndBetLimitItem.resetBetLimit();
                        }
                    });
                });
            }

            if (scope.customerLevel === Level.member) {
                this.crossingSettingTypesServiceBus.register("updateMemberSpreadGroup", (sharedData: any) => {
                    var newMemberGroupName: string = sharedData.newMemberGroup;
                    var memberSelectedSpreadGroupIndex: number = scope.availableCommissionGroups.indexOf(newMemberGroupName);

                    scope.settingType.settingItems.forEach((settingItem: ISettingItem) => {
                        var commissionAndBetLimitItem: CommissionAndBetLimitSettingItem = <CommissionAndBetLimitSettingItem>settingItem;

                        if (commissionAndBetLimitItem.layoutType === LayoutType.multipleGroups) {
                            commissionAndBetLimitItem.memberSpreadGroupIndex = memberSelectedSpreadGroupIndex;
                        }
                    });
                });
            }
        };

        private resourceService: ResourceService;
        private crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus;

        constructor(
            resourceService: ResourceService,
            crossingSettingTypesServiceBus: CrossingSettingTypesServiceBus,
            settingTypesSharedValuesService: SettingTypesSharedValuesService) {
            super(settingTypesSharedValuesService, commissionAndBetLimitItemFactory);

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