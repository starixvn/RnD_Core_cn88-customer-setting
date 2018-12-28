module CustomerSetting {
    "use strict";

    export interface ICommissionGroup {
        id: string;
        name: string;
        uGroup: string;
        minValue: number;
        maxValue: number;
        value: number;
    }
}