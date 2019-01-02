/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../models/key-value-pair.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class ValueRangeService {
        public generateRanges(value: number, min: number, max: number, step: number, roundingFactor: number): number[] {
            var ranges: number[] = [];
            var finalMin = Math.max(0, min);
            var finalMax = Math.max(0, max);
            var currentValue: number;

            if (value > finalMax) {
                ranges.push(value);
            }

            while (finalMax >= finalMin) {
                currentValue = Math.round(finalMax * roundingFactor) / roundingFactor;
                ranges.push(currentValue);
                finalMax -= step;
            }

            if (value < finalMin) {
                ranges.push(value);
            }

            return ranges;
        }
    }

    ModuleRegister.service("valueRangeService", ValueRangeService);
}