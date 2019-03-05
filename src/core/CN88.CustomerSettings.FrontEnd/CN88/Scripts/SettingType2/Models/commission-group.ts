namespace CustomerSetting {
    export interface ICommissionGroup {
        id: string;
        name: string;
        minValue: number;
        maxValue: number;
        value: number;
    }
}