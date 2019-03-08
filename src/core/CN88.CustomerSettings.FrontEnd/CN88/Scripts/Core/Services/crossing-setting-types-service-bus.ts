/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/key-value-pair.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    interface ICrossingSettingTypesListener {
        (sharedData: any): void;
    }

    export class CrossingSettingTypesServiceBus {
        public static $inject: string[] = ["$q"];

        private listeners: KeyValuePair<string, ICrossingSettingTypesListener>[];

        private $q: ng.IQService;

        constructor($q: ng.IQService) {
            this.$q = $q;
            this.listeners = [];
        }

        public reset(): void {
            this.listeners = [];
        }

        public register(channel: string, listener: ICrossingSettingTypesListener): void {
            var isNew: boolean = this.listeners.every((registeredListener: KeyValuePair<string, ICrossingSettingTypesListener>) => {
                return channel !== registeredListener.key;
            });

            if (isNew) {
                this.listeners.push(new KeyValuePair<string, ICrossingSettingTypesListener>(channel, listener));
            }
        }

        public execute(trigger: string, sharedData: any): ng.IPromise<void> {
            var deferred: ng.IDeferred<void> = this.$q.defer<void>();
            var promise: ng.IPromise<void> = deferred.promise;

            deferred.resolve();

            if (sharedData) {
                this.listeners.forEach((registeredPromise: KeyValuePair<string, ICrossingSettingTypesListener>) => {
                    if (registeredPromise.key === trigger) {
                        promise = promise.then<void>(() => registeredPromise.value(sharedData));
                    }
                });
            }

            return promise;
        }
    }

    ModuleRegister.service("crossingSettingTypesServiceBus", CrossingSettingTypesServiceBus);
}