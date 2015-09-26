"use strict";

abstract class Option<TValue>{
    abstract getTag(): Option.Tag;

    public get exists(): boolean {
        return Option.isSome<TValue>(this);
    }

    public andThen<TValueOut>(callback: (value: TValue) => Option<TValueOut>) {
        if (callback === undefined && callback === null) {
            throw new Error("'callback' undefined or null!");
        }

        let self = this;
        if (Option.isNone(self)) {
            return Option.none<TValueOut>();
        }
        else if (Option.isSome(self)) {
            return callback(self.value);
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

    public map<TValueOut>(mapper: (success: TValue) => TValueOut): Option<TValueOut> {
        if (mapper === undefined && mapper === null) {
            throw new Error("'mapper' undefined or null!");
        }

        let self = this;
        if (Option.isNone(self)) {
            return Option.none<TValueOut>();
        }
        else if (Option.isSome(self)) {
            return Option.ofValue(mapper(self.value));
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

    public substitute<TOutcome>(forNone: () => TOutcome, forSome: (value: TValue) => TOutcome) {
        if (forNone === undefined || forNone === null) {
            throw new Error("'forNone' undefined or null!");
        }

        if (forSome === undefined || forSome === null) {
            throw new Error("'forSome' undefined or null!");
        }

        let self = this;
        if (Option.isNone(self)) {
            return forNone();
        }
        else if (Option.isSome(self)) {
            return forSome(self.value);
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

    public withDefault(value: TValue): TValue {
        let self = this;
        if (Option.isSome<TValue>(self)) {
            return self.value;
        }
        else {
            return value;
        }
    }

    public toResult(): Result<Unit, TValue> {
        return this.substitute(
            () => Result.error<Unit, TValue>(unit()),
            value => Result.success<Unit, TValue>(value));
    }
}

module Option {
    export const enum Tag {
        None,
        Some
    }

    export class None<TValue> extends Option<TValue> {
        private __CC108EF9_AAE0_4AF5_A4C9_40EA6C2C8CA3__ = {}; // make type distinct

        constructor() {
            super();
        }

        public getTag() {
            return Tag.None;
        }
    }

    export class Some<TValue> extends Option<TValue> {
        private __F2492BFC_A6B2_484B_9195_399DBA086729__ = {}; // make type distinct

        constructor(private someValue: TValue) {
            super();
        }

        public getTag() {
            return Tag.Some;
        }

        public get value() {
            return this.someValue;
        }
    }

    export function none<TValue>(): Option<TValue> {
        return new None<TValue>();
    }

    export function isNone<TValue>(instance: Option<TValue>): instance is None<TValue> {
        return instance.getTag() === Tag.None;
    }

    export function some<TValue>(someValue: TValue): Option<TValue> {
        return new Some<TValue>(someValue);
    }

    export function isSome<TValue>(instance: Option<TValue>): instance is Some<TValue> {
        return instance.getTag() === Tag.Some;
    }

    export function ofBool(value: boolean): Option<Unit> {
        return (value === undefined || value === null || value === false)
            ? none<Unit>()
            : some<Unit>(unit());
    }

    export function ofValue<TValue>(value: TValue): Option<TValue> {
        return (value === undefined || value === null)
            ? none<TValue>()
            : some<TValue>(value);
    }

    export function guard<TValue>(callback: () => TValue): Option<TValue> {
        if (callback === undefined && callback === null) {
            throw new Error("'callback' undefined or null!");
        }

        try {
            return ofValue(callback());
        }
        catch (e) {
            return none<TValue>();
        }
    }
}