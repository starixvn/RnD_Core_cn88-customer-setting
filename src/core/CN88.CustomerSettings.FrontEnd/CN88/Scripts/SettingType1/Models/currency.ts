module CustomerSetting {
    "use strict";

    export interface ICurrency {
        id: number;
        name: string;
        code?: string;
    }
}