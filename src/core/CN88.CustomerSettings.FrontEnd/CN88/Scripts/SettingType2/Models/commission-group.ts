module CustomerSetting {
    "use strict";

    export interface ICommissionGroup {
        groupId: string;
        groupName: string;
        uGroup: string;
        minValue: number;
        maxValue: number;
        value: number;
    }
}