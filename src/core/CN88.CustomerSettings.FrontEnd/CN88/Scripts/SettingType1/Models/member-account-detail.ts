/// <reference path="../../core/models/result.ts" />
/// <reference path="account-detail.ts" />

module CustomerSetting {
    "use strict";

    export class MemberAccountDetail extends AccountDetail {
        public uGroup: string;
        public uGroups: string[];

        constructor();
        constructor(accountDetail: any);
        constructor(accountDetail: any, backup: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean)
        constructor(accountDetail: any, backup: boolean, isNew: boolean, customerLevel: Level)
        constructor(accountDetail?: any, backup: boolean = false, isNew: boolean = false, customerLevel: Level = null) {
            if (accountDetail) {
                super(accountDetail, backup, isNew, customerLevel);

                this.uGroup = accountDetail.uGroup;
                this.uGroups = accountDetail.uGroups;
            }

            if (backup) {
                this.oldSetting = new MemberAccountDetail(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldSetting: MemberAccountDetail = <MemberAccountDetail>this.oldSetting;

            return isChanged || this.uGroup !== oldSetting.uGroup;
        }

        public validate(): boolean {
            if (!super.validate()) {
                return false;
            }

            if (this.isNew) {
                if (!this.uGroup || this.uGroup.length === 0) {
                    this.validationState = new ValidationResult("RequiredField", "uGroup");

                    return false;
                }
            }

            this.resetValidationState();

            return true;
        }
    }
}