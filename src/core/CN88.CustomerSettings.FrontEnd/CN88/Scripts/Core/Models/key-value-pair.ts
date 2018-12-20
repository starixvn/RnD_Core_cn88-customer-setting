module CustomerSetting {
    "use strict";

    export class KeyValuePair<T, K> {
        public key: T;
        public value: K;

        constructor(key: T, value: K) {
            this.key = key;
            this.value = value;
        }
    }
}