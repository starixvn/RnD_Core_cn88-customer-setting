module CustomerSetting {
    "use strict";

    export interface ICommissionGroup {
        id: string;
        name: string;
        minValue: number;
        maxValue: number;
        value: number;
    }
}