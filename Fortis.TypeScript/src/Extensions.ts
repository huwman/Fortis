"use strict";

interface Array<T> {
    pick<TValueOut>(choice: (item: T) => Option<TValueOut>): Option<TValueOut>
    choose<TValueOut>(choice: (item: T) => Option<TValueOut>): Option<TValueOut>[]
}

function pick<TValue, TValueOut>(items: TValue[], choice: (item: TValue) => Option<TValueOut>): Option<TValueOut> {
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

Array.prototype.pick = function <TValue, TValueOut>(choice: (item: TValue) => Option<TValueOut>): Option<TValueOut> {
    return pick(this, choice);
}

function choose<TValue, TValueOut>(items: TValue[], choice: (item: TValue) => Option<TValueOut>): Option<TValueOut>[] {
    if (choice === undefined && choice === null) {
        throw new Error("'choice' undefined or null!");
    }

    return items.map(choice).filter(item => item.exists);
}

Array.prototype.choose = function <TValue, TValueOut>(choice: (item: TValue) => Option<TValueOut>): Option<TValueOut>[] {
    return choose(this, choice);
}

function tryParseFloat(value: string): Option<number> {
    let res = parseFloat(value);
    return (isNaN(res)) ? Option.none<number>() : Option.ofValue<number>(res);
}

function tryParseInt(value: string, radix?:number): Option<number> {
    let res = parseInt(value, radix);

    return (isNaN(res)) ? Option.none<number>() : Option.ofValue<number>(res);
}