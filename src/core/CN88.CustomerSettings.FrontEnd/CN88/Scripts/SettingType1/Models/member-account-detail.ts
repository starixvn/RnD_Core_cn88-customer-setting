/// <reference path="../../core/models/enums/level.ts" />
/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />
/// <reference path="spread-group.ts" />

module CustomerSetting {
    "use strict";

    export class MemberAccountDetail extends AccountDetail {
        public spreadGroup: ISpreadGroup;
        public spreadGroups: ISpreadGroup[];

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level, hashKey: string)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null, hashKey: string = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel, hashKey);

                this.spreadGroup = accountDetail.spreadGroup;
                this.spreadGroups = accountDetail.spreadGroups;
            }

            if (backup) {
                this.oldSetting = new MemberAccountDetail(this);
            }

            this.ignoredProperties.push("spreadGroups");

            if (!this.isNew) {
                this.ignoredProperties.push("spreadGroup");
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting: MemberAccountDetail = <MemberAccountDetail>this.oldSetting;

            return isChanged || this.spreadGroup.uGroup !== oldSetting.spreadGroup.uGroup;
        }

        public validate(): boolean {
            if (!super.validate()) {
                return false;
            }

            if (this.isNew) {
                if (!this.spreadGroup) {
                    this.validationState = new ValidationResult("RequiredField", "spreadGroup");

                    return false;
                }
            }

            this.resetValidationState();

            return true;
        }
    }
}