/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="common.ts" />
/// <reference path="module-registrater.ts" />
/// <reference path="parameters.ts" />

module CustomerSetting {
    "use strict";

    export class AppConfigService {
        public static $inject: string[] = ["$window"];

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
            return this.applicationPath + "viewCustomerSetting/querySetting?";
        }

        public getUpdatePath(): string {
            return this.applicationPath + "updateCustomerSetting/updateSetting";
        }

        public requestVerificationToken(): string {
            return this.antiForgeryToken;
        }
    }

    ModuleRegister.service("appConfigService", AppConfigService);

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

        private query: SettingQuery;
        private lang: string;
        private system: number;

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

                this.query = new SettingQuery(
                    params.productId,
                    uplineLevel,
                    customerLevel,
                    params.customerId);
            }
        }
    }

    ModuleRegister.service("parametersService", ParametersService);

    export class SettingTypesSharedValuesService {
        public static $inject: string[] = ["parametersService"];

        public customerId: number
        public uplineLevel: Level;
        public customerLevel: Level;
        public productId: number;
        public productName: string;

        private parametersService: ParametersService;

        constructor(parametersService: ParametersService) {
            this.parametersService = parametersService;
        }

        public register(setting: Setting, uplineLevel: Level): void {
            this.uplineLevel = uplineLevel;

            if (setting) {
                this.customerId = setting.customerId;
                this.customerLevel = setting.customerLevel;
                this.productId = setting.productId;
                this.productName = setting.productName;
            }
        }
    }

    ModuleRegister.service("settingTypesSharedValuesService", SettingTypesSharedValuesService);

    export class ValueRangeService {
        public generateRanges(value: number, min: number, max: number, step: number, roundingFactor: number): Array<KeyValuePair<number, number>> {
            var ranges: Array<KeyValuePair<number, number>> = new Array<KeyValuePair<number, number>>();

            min = Math.max(0, min);
            max = Math.max(0, max);

            if (value > max) {
                ranges.push(new KeyValuePair<number, number>(value, value));
            }

            while (max >= min) {
                ranges.push(new KeyValuePair<number, number>(max, max));
                max -= step;
                max = Math.round(max * roundingFactor) / roundingFactor;
            }

            if (value < min) {
                ranges.push(new KeyValuePair<number, number>(value, value));
            }

            return ranges;
        }
    }

    ModuleRegister.service("valueRangeService", ValueRangeService);

    function FormatNumber($filter: ng.IFilterService): Function {
        var numberFilter = $filter("number");

        return (value: number, factor?: number) => {
            factor = factor || value.toString().indexOf(".") > 0 ? 2 : 0;

            return numberFilter(value, factor);
        };
    }

    FormatNumber.$inject = ["$filter"];
    ModuleRegister.filter("formatNumber", FormatNumber);

    function NAValue(): Function {
        return (value: string) => {
            return value || "---";
        };
    }

    ModuleRegister.filter("naValue", NAValue);

    function MinValue(): Function {
        return (numbers: number[]) => {
            return Math.min.apply(Math, numbers);
        };
    }

    ModuleRegister.filter("minValue", MinValue);

    function MaxValue(): Function {
        return (numbers: number[]) => {
            return Math.max.apply(Math, numbers);
        };
    }

    ModuleRegister.filter("maxValue", MaxValue);

    function ValidSettingTypeFilter(): Function {
        return (settingType: { isError: boolean }) => {
            return !settingType.isError;
        };
    }

    ModuleRegister.filter("validSettingType", ValidSettingTypeFilter);

    function Unsafe($sce: ng.ISCEService): Function {
        return (value: string) => {
            return $sce.trustAsHtml(value);
        };
    }

    Unsafe.$inject = ["$sce"];
    ModuleRegister.filter("unsafe", Unsafe);
}