/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/key-value-pair.ts" />
/// <reference path="../models/setting-type.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export interface ICrossingSettingTypesPromise {
        (sharedData: any): ng.IPromise<void>;
    }

    export class CrossingSettingTypesServiceBus {
        public static $inject: string[] = ["$q"];

        private promises: KeyValuePair<string, ICrossingSettingTypesPromise>[];

        private $q: ng.IQService;

        constructor($q: ng.IQService) {
            this.$q = $q;
            this.promises = [];
        }

        public reset(): void {
            this.promises = [];
        }

        public register(channel: string, promise: ICrossingSettingTypesPromise): void {
            var isNew: boolean = this.promises.every((promise: KeyValuePair<string, ICrossingSettingTypesPromise>) => {
                return channel !== promise.key;
            });

            if (isNew) {
                this.promises.push(new KeyValuePair<string, ICrossingSettingTypesPromise>(channel, promise));
            }
        }

        public execute(trigger: string, sharedData: any): ng.IPromise<void> {
            var deferred: ng.IDeferred<void> = this.$q.defer<void>();

            if (sharedData) {
                var current: ng.IPromise<void>;

                this.promises.forEach((promise: KeyValuePair<string, ICrossingSettingTypesPromise>) => {
                    if (promise.key === trigger) {
                        if (!current) {
                            current = promise.value(sharedData);
                        } else {
                            current = current.then<void>(() => promise.value(sharedData));
                        }
                    }
                });

                if (current) {
                    return current;
                }
            }

            deferred.resolve();

            return deferred.promise;
        }
    }

    ModuleRegister.service("crossingSettingTypesServiceBus", CrossingSettingTypesServiceBus);
}