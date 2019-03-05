/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/enums/level.ts" />
/// <reference path="../models/parameters/setting-command.ts" />
/// <reference path="../models/parameters/setting-query.ts" />
/// <reference path="../models/defaults.ts" />
/// <reference path="../models/setting.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    export class ParametersService {
        public static $inject: string[] = ["$window"];

        public hashKey: string;

        public get systemId(): number {
            return this.system;
        }

        public get actorLevel(): Level {
            return this.actorLv;
        }

        public get language(): string {
            return this.lang;
        }

        public get uplineId(): number {
            return this.query.uplineId;
        }

        public get isNew(): boolean {
            return this.isCreateNew;
        }

        private system: number;
        private actorLv: Level;
        private lang: string;
        private isCreateNew: boolean;
        private query: SettingQuery;

        private $window: ng.IWindowService;

        constructor($window: ng.IWindowService) {
            this.$window = $window;
            this.fetchParams();
        }

        public getSettingQuery(): SettingQuery {
            return this.query;
        }

        public createSettingCommand(setting: Setting): SettingCommand {
            return new SettingCommand(this.query.uplineId, setting);
        }

        private fetchParams(): void {
            /* tslint:disable:no-string-literal */
            var params: any = this.$window["params"];
            /* tslint:enable:no-string-literal */

            if (params) {
                var uplineId = params.uplineId ? params.uplineId : undefined;
                var customerLevel = params.customerLevel ? params.customerLevel : undefined;

                this.lang = params.language || Defaults.language;
                this.system = params.system || Defaults.system;
                this.isCreateNew = params.isCreateNew || false;
                this.hashKey = params.hashKey;
                this.actorLv = params.actorLevel ? params.actorLevel : undefined;

                this.query = new SettingQuery(
                    params.productId,
                    uplineId,
                    customerLevel,
                    params.customerId,
                    this.isNew);
            }
        }
    }

    ModuleRegister.service("parametersService", ParametersService);
}