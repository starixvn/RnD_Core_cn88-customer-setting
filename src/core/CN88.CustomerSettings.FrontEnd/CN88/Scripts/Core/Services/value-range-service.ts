/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

namespace CustomerSetting {
    export class ValueRangeService {
        public generateRanges(value: number, min: number, max: number, step: number, roundingFactor: number): number[] {
            var ranges: number[] = [];
            var finalMin = Math.max(0, min);
            var finalMax = Math.max(0, max);

            if (value > finalMax) {
                ranges.push(value);
            }

            while (finalMax >= finalMin) {
                ranges.push(finalMax);
                finalMax -= step;
                finalMax = Math.round(finalMax * roundingFactor) / roundingFactor;
            }

            if (value < finalMin) {
                ranges.push(value);
            }

            return ranges;
        }
    }

    ModuleRegister.service("valueRangeService", ValueRangeService);
}