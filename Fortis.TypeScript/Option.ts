"use strict";

import { Unit } from "Unit";

export class Option<TValue> {
    private __80DD04AC_87D8_4891_8AF1_870B4532A226__; // makes type distinct

    constructor() {
        this.__80DD04AC_87D8_4891_8AF1_870B4532A226__ = {};
    }

    public static None<TValue>(): Option<TValue> {
        return new None<TValue>();
    }

    public static Some<TValue>(value: TValue): Option<TValue> {
        return new Some<TValue>(value);
    }

    public static OfValue<TValue>(value: TValue): Option<TValue> {
        if (value === undefined || value === null) {
            return this.None<TValue>();
        } else {
            return this.Some(value);
        }
    }

    public static OfBoolean(value: boolean): Option<Unit> {
        if (value) {
            return this.Some(new Unit());
        } else {
            return this.None<Unit>();
        }
    }

    public static Guard<TValue>(callback: () => TValue): Option<TValue> {
        try {
            return this.Some(callback());
        } catch (e) {
            return this.None<TValue>();
        }
    }

    public get Exists() {
        return this instanceof Some;
    }

    public Map<TValueOut>(mapper: (source: TValue) => TValueOut): Option<TValueOut> {
        if (this.Exists) {
            let value = (<Some<TValue>>this).Value;
            return Option.OfValue(mapper(value));
        } else {
            return Option.None<TValueOut>();
        }
    }

    public WithDefault(defaultValue: TValue): TValue {
        if (this.Exists) {
            return (<Some<TValue>>this).Value;
        } else {
            return defaultValue;
        }
    }

    public AndThen<TValueOut>(callback: (source: TValue) => Option<TValueOut>): Option<TValueOut> {
        if (this.Exists) {
            let value = (<Some<TValue>>this).Value;
            return callback(value);
        } else {
            return Option.None<TValueOut>();
        }
    }

    public Substitute<TValueOut>(forNone: (value: Unit) => TValueOut, forSome: (value: TValue) => TValueOut) {
        if (this.Exists) {
            let value = (<Some<TValue>>this).Value;
            return forSome(value);
        } else {
            return forNone(new Unit());
        }
    }
}

export class None<TValue> extends Option<TValue> {
    constructor() {
        super();
    }
}

export class Some<TValue> extends Option<TValue> {
    constructor(private value: TValue) {
        super();
    }

    public get Value() {
        return this.value;
    }
}