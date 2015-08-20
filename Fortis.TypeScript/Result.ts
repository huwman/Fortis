"use strict";

import { Unit } from "Unit";

export class Result<TError, TValue> {
    public static Error<TError, TValue>(value: TError): Result<TError, TValue> {
        return new Error<TError, TValue>(value);
    }

    public static Success<TError, TValue>(value: TValue): Result<TError, TValue> {
        return new Success<TError, TValue>(value);
    }
}

export class Error<TError, TValue> extends Result<TError, TValue> {
    private __A70636B6_B7FD_44C1_B686_E8F57620F9F6__: {}; // make type distinct

    constructor(private value: TError) {
        super();

        this.__A70636B6_B7FD_44C1_B686_E8F57620F9F6__ = {};
    }

    public get Value() {
        return this.value;
    }
}

export class Success<TError, TValue> extends Result<TError, TValue> {
    private __88EBD69A_CCD1_4D41_96CC_E8E312417DB4__: {}; // make type distinct

    constructor(private value: TValue) {
        super();

        this.__88EBD69A_CCD1_4D41_96CC_E8E312417DB4__ = {};
    }

    public get Value() {
        return this.value;
    }
}