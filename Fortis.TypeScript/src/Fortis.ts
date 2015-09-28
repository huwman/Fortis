"use strict";

/**
 * The unit type is a type that indicates the absence of a specific value;
 * the unit type has only a single value, which acts as a placeholder when no other value exists or is needed.
 */
export class Unit {
    private static __80DD04AC_87D8_4891_8AF1_870B4532A226__ = {}; // Makes type distinct
}

/**
 * Creates a Unit value.
 */
export function unit() {
    return new Unit();
}

/**
 * The option type is a type that represents optional values.
 * Use as a replacement for null or undefined, to represent the presence or absence of a value.
 */
export abstract class Option<TValue>{
    abstract getTag(): Option.Tag;

    /**
     * Checks whether the specified option is Some.
     * @returns A boolean value.
     */
    public get exists(): boolean {
        return Option.isSome<TValue>(this);
    }

    /**
     * Applies a mapping function to the specified option.
     */
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

    /**
     * Defaults to the supplied value when the specified Option is None.
     */
    public withDefault(value: TValue): TValue {
        let self = this;
        if (Option.isSome<TValue>(self)) {
            return self.value;
        }
        else {
            return value;
        }
    }

    /**
     * Evaluates the callback when the option is Some.
     * Use for chaining multiple options together.
     */
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

    /**
     * Substitutes for the option value using the substitution functions.
     */
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

    /**
     * Filter the Option
     * @returns The option if {predicate} evaluates to true, otherwise None.
     */
    public filter(predicate: (value: TValue) => boolean) {
        let self = this;
        if (Option.isSome<TValue>(self) && predicate(self.value)) {
            return self;
        }
        else {
            return Option.none<TValue>();
        }
    }

    /**
     * Convert to a Result.
     */
    public toResult(): Result<Unit, TValue> {
        return this.substitute(
            () => Result.error<Unit, TValue>(unit()),
            value => Result.success<Unit, TValue>(value));
    }
}

export module Option {
    export const enum Tag {
        None,
        Some
    }

    /**
     * The representation of "No value".
     */
    export class None<TValue> extends Option<TValue> {
        private __CC108EF9_AAE0_4AF5_A4C9_40EA6C2C8CA3__ = {}; // make type distinct

        constructor() {
            super();
        }

        public getTag() {
            return Tag.None;
        }
    }

    /**
     * The representation of "Value of type TValue".
     */
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

    /**
     * Creates a "None" Option. 
     */
    export function none<TValue>(): Option<TValue> {
        return new None<TValue>();
    }

    /**
     * Checks if an Option is "None".
     * @param instance The Option to check.
     */
    export function isNone<TValue>(instance: Option<TValue>): instance is None<TValue> {
        return instance.getTag() === Tag.None;
    }

    /**
     * Creates a "Some of TValue" Option.
     * @param value The value to wrap.
     */
    export function some<TValue>(value: TValue): Option<TValue> {
        if (value === undefined && value === null) {
            throw new Error("'value' undefined or null!");
        }

        return new Some<TValue>(value);
    }

    /**
     * Checks if an Option is "Some".
     * @param instance The Option to check.
     */
    export function isSome<TValue>(instance: Option<TValue>): instance is Some<TValue> {
        return instance.getTag() === Tag.Some;
    }

    /**
     * Creates an Option from a boolean value.
     * @param value A boolean value.
     * @returns Some of Unit when value is true, otherwise None.
     */
    export function ofBool(value: boolean): Option<Unit> {
        return (value === undefined || value === null || value === false)
            ? none<Unit>()
            : some<Unit>(unit());
    }

    /**
     * Creates an Option from value TValue.
     * @param value The value to evaluate.
     * @returns None when the value is null, otherwise Some of TValue.
     */
    export function ofValue<TValue>(value: TValue): Option<TValue> {
        return (value === undefined || value === null)
            ? none<TValue>()
            : some<TValue>(value);
    }

    /**
     * Guards the specified callback.
     * @param callback The callback to guard.
     * @returns Some of TValue when no exception occurs, otherwise None.
     */
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

/**
 * The result type is a type that represent error or success values.
 * Use rather than throwing exceptions or returning nulls.
 */
export abstract class Result<TError, TValue>{
    abstract getTag(): Result.Tag;

    /**
     * Applies a mapping function to the specified Result.
     */
    public map<TValueOut>(mapper: (success: TValue) => TValueOut): Result<TError, TValueOut> {
        if (mapper === undefined && mapper === null) {
            throw new Error("'mapper' undefined or null!");
        }

        let self = this;
        if (Result.isError(self)) {
            return Result.error<TError, TValueOut>(self.value);
        }
        else if (Result.isSuccess(self)) {
            return Result.success<TError, TValueOut>(mapper(self.value));
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

    /**
     * Evaluates the callback when the Result is Success.
     * Use for chaining multiple results together.
     */
    public andThen<TValueOut>(callback: (value: TValue) => Result<TError, TValueOut>) {
        if (callback === undefined && callback === null) {
            throw new Error("'callback' undefined or null!");
        }

        let self = this;
        if (Result.isError(self)) {
            return Result.error<TError, TValueOut>(self.value);
        }
        else if (Result.isSuccess(self)) {
            return callback(self.value);
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

    /**
     * Formats the error, when an instance of Error.
     */
    public formatError<TErrorOut>(formatter: (error: TError) => TErrorOut): Result<TErrorOut, TValue> {
        if (formatter === undefined && formatter === null) {
            throw new Error("'formatter' undefined or null!");
        }

        let self = this;
        if (Result.isError(self)) {
            return Result.error<TErrorOut, TValue>(formatter(self.value));
        }
        else if (Result.isSuccess(self)) {
            return Result.success<TErrorOut, TValue>(self.value);
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

     /**
      * Substitutes for the Result using the substitution functions.
      */
    public substitute<TOutcome>(forError: (error: TError) => TOutcome, forSuccess: (value: TValue) => TOutcome) {
        if (forError === undefined || forError === null) {
            throw new Error("'forError' undefined or null!");
        }

        if (forSuccess === undefined || forSuccess === null) {
            throw new Error("'forSuccess' undefined or null!");
        }

        let self = this;
        if (Result.isError(self)) {
            return forError(self.value);
        }
        else if (Result.isSuccess(self)) {
            return forSuccess(self.value);
        }
        else {
            throw new Error("Unexpected case!");
        }
    }

    /**
     * Convert to an Option
     */
    public toOption(): Option<TValue> {
        return this.substitute(
            (error) => Option.none<TValue>(),
            (value) => Option.some<TValue>(value));
    }
}

export module Result {
    export const enum Tag {
        Error,
        Success
    }

    /**
     * The representation of "Value of type TError"
     */
    export class Error<TError, TValue> extends Result<TError, TValue> {
        private __A70636B6_B7FD_44C1_B686_E8F57620F9F6__ = {}; // make type distinct

        constructor(private error: TError) {
            super();
        }

        public getTag() {
            return Tag.Error;
        }

        public get value() {
            return this.error;
        }
    }

    /**
     * The representation of "Value of type TValue"
     */
    export class Success<TError, TValue> extends Result<TError, TValue> {
        private __88EBD69A_CCD1_4D41_96CC_E8E312417DB4__ = {}; // make type distinct

        constructor(private success: TValue) {
            super();
        }

        public getTag() {
            return Tag.Success;
        }

        public get value() {
            return this.success;
        }
    }

    /**
     * Creates an "Error" Result.
     * @param error The error value.
     */
    export function error<TError, TValue>(error: TError): Result<TError, TValue> {
        return new Error<TError, TValue>(error);
    }

    /**
     * Checks if a Result is "Error".
     * @param instance The Result to check.
     */
    export function isError<TError, TValue>(instance: Result<TError, TValue>): instance is Error<TError, TValue> {
        return instance.getTag() === Tag.Error;
    }

    /**
     * Creates a "Success" Result.
     * @param success the success value.
     */
    export function success<TError, TValue>(success: TValue): Result<TError, TValue> {
        return new Success<TError, TValue>(success);
    }

    /**
     * Checks if a Result is "Success".
     * @param instance The Result to check.
     */
    export function isSuccess<TError, TValue>(instance: Result<TError, TValue>): instance is Success<TError, TValue> {
        return instance.getTag() === Tag.Success;
    }

    /**
     * Creates a Result from a boolean value.
     * @param value A boolean value.
     * @returns Success of Unit when value is true, otherwise Error of Unit.
     */
    export function ofBool(value: boolean): Result<Unit, Unit> {
        return (value === undefined || value === null || value === false)
            ? new Error<Unit, Unit>(unit())
            : new Success<Unit, Unit>(unit());
    }

    /**
     * Creates a Result from value TValue.
     * @param value The value to evaluate.
     * @returns Error of Unit when the value is null, otherwise Success of TValue.
     */
    export function ofValue<TValue>(value: TValue): Result<Unit, TValue> {
        return (value === undefined || value === null)
            ? new Error<Unit, TValue>(unit())
            : new Success<Unit, TValue>(value);
    }
}

/**
 * Pick the first item from items where the choice function returns Some of TValueOut.
 * @param items An array of items.
 * @param choice The choice function.
 */
export function pick<TValue, TValueOut>(items: TValue[], choice: (item: TValue) => Option<TValueOut>): Option<TValueOut> {
    if (choice === undefined && choice === null) {
        throw new Error("'choice' undefined or null!");
    }

    for (let item of items) {
        let res = choice(item);
        if (res.exists) {
            return res;
        }
    }

    return Option.none<TValueOut>();
}

/**
 * Choose from items where the choice function returns Some of TValueOut.
 * @param items An array of items.
 * @param choice The choice function.
 */
export function choose<TValue, TValueOut>(items: TValue[], choice: (item: TValue) => Option<TValueOut>): Option<TValueOut>[] {
    if (choice === undefined && choice === null) {
        throw new Error("'choice' undefined or null!");
    }

    return items.map(choice).filter(item => item.exists);
}