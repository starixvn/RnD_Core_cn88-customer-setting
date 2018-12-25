/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/key-value-pair.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class ValueRangeService {
        public generateRanges(value: number, min: number, max: number, step: number, roundingFactor: number): number[] {
            var ranges: number[] = [];

            min = Math.max(0, min);
            max = Math.max(0, max);

            if (value > max) {
                ranges.push(value);
            }

            while (max >= min) {
                ranges.push(max);
                max -= step;
                max = Math.round(max * roundingFactor) / roundingFactor;
            }

            if (value < min) {
                ranges.push(value);
            }

            return ranges;
        }
    }

    ModuleRegister.service("valueRangeService", ValueRangeService);
}