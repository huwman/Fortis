"use strict";

abstract class Result<TError, TValue>{
    abstract getTag(): Result.Tag;

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

    public toOption(): Option<TValue> {
        return this.substitute(
            (error) => Option.none<TValue>(),
            (value) => Option.some<TValue>(value));
    }
}

module Result {
    export const enum Tag {
        Error,
        Success
    }

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

    export function error<TError, TValue>(error: TError): Result<TError, TValue> {
        return new Error<TError, TValue>(error);
    }

    export function isError<TError, TValue>(instance: Result<TError, TValue>): instance is Error<TError, TValue> {
        return instance.getTag() === Tag.Error;
    }

    export function success<TError, TValue>(success: TValue): Result<TError, TValue> {
        return new Success<TError, TValue>(success);
    }

    export function isSuccess<TError, TValue>(instance: Result<TError, TValue>): instance is Success<TError, TValue> {
        return instance.getTag() === Tag.Success;
    }

    export function ofBool(value: boolean): Result<Unit, Unit> {
        return (value === undefined || value === null || value === false)
            ? new Error<Unit, Unit>(unit())
            : new Success<Unit, Unit>(unit());
    }

    export function ofValue<TValue>(value: TValue): Result<Unit, TValue> {
        return (value === undefined || value === null)
            ? new Error<Unit, TValue>(unit())
            : new Success<Unit, TValue>(value);
    }
}