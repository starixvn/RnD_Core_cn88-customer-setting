/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/parameters/setting-command.ts" />
/// <reference path="../models/parameters/setting-query.ts" />
/// <reference path="../models/defaults.ts" />
/// <reference path="../models/setting.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class ParametersService {
        public static $inject: string[] = ["$window"];

        public get systemId(): number {
            return this.system;
        }

        public get language(): string {
            return this.lang;
        }

        public get uplineLevel(): Level {
            return this.query.uplineLevel;
        }

        public get isNew(): boolean {
            return this.isCreateNew;
        }

        private query: SettingQuery;
        private lang: string;
        private system: number;
        private isCreateNew: boolean;

        private $window: ng.IWindowService;

        constructor($window: ng.IWindowService) {
            this.$window = $window;
            this.fetchParams();
        }

        public getSettingQuery(): SettingQuery {
            return this.query;
        }

        public createSettingCommand(setting: Setting): SettingCommand {
            return new SettingCommand(this.query.uplineLevel, setting);
        }

        private fetchParams(): void {
            /* tslint:disable:no-string-literal */
            var params: any = this.$window["params"];
            /* tslint:enable:no-string-literal */

            if (params) {
                var uplineLevel = params.uplineLevel ? params.uplineLevel : undefined;
                var customerLevel = params.customerLevel ? params.customerLevel : undefined;

                this.lang = params.language || Defaults.language;
                this.system = params.system || Defaults.system;
                this.isCreateNew = params.isCreateNew || false;

                this.query = new SettingQuery(
                    params.productId,
                    uplineLevel,
                    customerLevel,
                    params.customerId);
            }
        }
    }

    ModuleRegister.service("parametersService", ParametersService);
}