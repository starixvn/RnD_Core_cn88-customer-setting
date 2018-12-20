/// <reference path="../../core/models/result.ts" />
/// <reference path="pt-item.ts" />

module CustomerSetting {
    "use strict";

    export class AgentPTItem extends PTItem {
        public masterPT: number;
        public agentPT: number;

        constructor(agentPTItem: any, backup: boolean = false) {
            if (agentPTItem) {
                super(agentPTItem);

                this.masterPT = agentPTItem.masterPT;
                this.agentPT = agentPTItem.agentPT;
            }

            if (backup) {
                this.oldPTItem = new AgentPTItem(this);
            }
        }

        public isChanged(): boolean {
            var isChanged: boolean = super.isChanged();
            var oldAgentPTItem: AgentPTItem = <AgentPTItem>this.oldPTItem;

            return isChanged
                || this.masterPT !== oldAgentPTItem.masterPT
                || this.agentPT !== oldAgentPTItem.agentPT;
        }

        public validate(): boolean {
            if (this.masterPT > this.maxPT) {
                this.validationState = new ValidationResult("MasterPTMustBeLessThanOrEqualTo", "masterPT", [this.maxPT]);

                return false;
            }

            if (this.agentPT > this.maxPT) {
                this.validationState = new ValidationResult("AgentPTMustBeLessThanOrEqualTo", "agentPT", [this.maxPT]);

                return false;
            }

            if ((this.masterPT + this.agentPT > this.maxPT) || (this.masterPT + this.agentPT < this.minPT)) {
                this.validationState = new ValidationResult("TotalPTMustBeInRange", "masterPT", [this.minPT, this.maxPT]);

                return false;
            }

            this.resetValidationState();

            return true;
        }

        public toJson(): {} {
            var json: any = super.toJson();

            json.superMinPT = this.masterPT;
            json.superMaxPT = this.agentPT;

            return json;
        }
    }
}