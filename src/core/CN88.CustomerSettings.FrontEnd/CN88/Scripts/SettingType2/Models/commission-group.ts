module CustomerSetting {
    "use strict";

    export interface ICommissionGroup {
        groupId: string;
        groupName: string;
        uGroup: string;
        maxValue: number;
        value: number;
    }
}