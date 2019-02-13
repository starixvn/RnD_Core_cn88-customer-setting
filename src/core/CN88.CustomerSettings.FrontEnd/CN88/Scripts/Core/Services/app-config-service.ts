/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class AppConfigService {
        public static $inject: string[] = ["$window"];

        public get shortDateFormat(): string {
            return "yyyy-MM-dd";
        }

        public get dateTimeFormat(): string {
            return "yyyy-MM-dd hh:mm:ss";
        }

        public get thousandSeparator(): string {
            return ",";
        }

        public get decimalSeparator(): string {
            return ".";
        }

        public get decimalKeyCodes(): number[] {
            return [110, 190];
        }

        public get negativePrefix(): string {
            return "-";
        }

        public get negativeKeyCodes(): number[] {
            return [109, 189];
        }

        private applicationPath: string;
        private antiForgeryToken: string;
        private editable: boolean;

        private $window: ng.IWindowService;

        constructor($window: ng.IWindowService) {
            var appConfig: any;

            this.$window = $window;

            /* tslint:disable:no-string-literal */
            appConfig = this.$window["appConfig"] || {};
            /* tslint:enable:no-string-literal */

            if (appConfig) {
                this.applicationPath = appConfig.applicationPath;
                this.antiForgeryToken = appConfig.antiForgeryToken;
                this.editable = appConfig.editable;

                if (this.applicationPath && this.applicationPath[this.applicationPath.length - 1] !== "/") {
                    this.applicationPath += "/";
                }
            }
        }

        public getQueryPath(): string {
            return this.applicationPath + "ViewCN88CustomerSetting/QuerySetting";
        }

        public getCreatePath(): string {
            return this.applicationPath + "CreateCN88Customer/Create";
        }

        public getUpdatePath(): string {
            return this.applicationPath + "UpdateCN88CustomerSetting/Update";
        }

        public buildPath(relativePath: string): string {
            return this.applicationPath + relativePath;
        }

        public requestVerificationToken(): string {
            return this.antiForgeryToken;
        }

        public canUpdate(): boolean {
            return this.editable;
        }
    }

    ModuleRegister.service("appConfigService", AppConfigService);

    app.run(["$http", "appConfigService", ($http: ng.IHttpService, appConfigService: AppConfigService) => {
        /* tslint:disable:no-string-literal */
        $http.defaults.headers.common["__RequestVerificationToken"] = appConfigService.requestVerificationToken();
        /* tslint:enable:no-string-literal */
    }]);
}