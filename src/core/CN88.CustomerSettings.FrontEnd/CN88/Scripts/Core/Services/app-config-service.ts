/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    export class AppConfigService {
        public static $inject: string[] = ["$window"];

        public get shortDateFormat(): string {
            return "yyyy-MM-dd";
        }

        public get timeFormat(): string {
            return "HH:mm:ss";
        }

        public get dateTimeFormat(): string {
            return "yyyy-MM-dd HH:mm:ss";
        }

        public get thousandSeparator(): string {
            return ",";
        }

        public get decimalSeparator(): string {
            return ".";
        }

        private applicationPath: string;
        private antiForgeryToken: string;

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
    }

    ModuleRegister.service("appConfigService", AppConfigService);

    app.run(["$http", "appConfigService", ($http: ng.IHttpService, appConfigService: AppConfigService) => {
        /* tslint:disable:no-string-literal */
        $http.defaults.headers.common["__RequestVerificationToken"] = appConfigService.requestVerificationToken();
        /* tslint:enable:no-string-literal */
    }]);
}