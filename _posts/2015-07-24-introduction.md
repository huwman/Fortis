---
layout:     post
title:      "Introduction to Fortis C#"
subtitle:   "Make your code more robust by using types more effectively"
date:       2015-07-24 12:00:00
author:     "Huw Simpson"
---

> My inspiration for this library and the series of blog posts that will accompany it, is this: *I have a love-hate relationship with C#.* There are many things I love about it, but there are also a couple things that I think can do with some love. Fortunately we can design around some of the *'bad'* features and *increase the love*. This blog will show you how!

### Prerequisites
* Install the [Code Contracts](https://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970) extension for Visual Studio.
* Create a fresh C# .Net 4.5 project
* Add a Nuget package reference to [Fortis.CSharp](https://www.nuget.org/packages/Fortis.CSharp)

## Lets begin

### Avoiding the billion dollar mistake
In 1969 [Tony Hoare](https://en.wikipedia.org/wiki/Tony_Hoare) was designing the type system for the Algol W Object Oriented language. He recalls: 
> "My goal was to ensure that all use of references should be absolutely safe, with checking performed automatically by the compiler. But I couldn't resist the temptation to put in a null reference, simply because it was so easy to implement. This has led to innumerable errors, vulnerabilities, and system crashes, which have probably caused a billion dollars of pain and damage in the last forty years."

Nearly every object oriented language to follow has copied this design flaw, and whether we are used to dealing with it or not, nulls effect us in a negative way. When attempting to write a robust and error free program, the C# compiler does not aide us in finding references to variables, which at runtime, may evaluate to null! So what should we do? __Avoid using nulls!!!__

You can model your application is such a manner that the absence of data is represented by a specific type rather than the *typeless* null.

Consider this code:

```csharp

Person GetPersonByIdNumber(string idNumber)
{
    // Implementation ommitted!
}

void Main(object[] args)
{
    // Print the persons name to the console
    var p = GetPersonByIdNumber("7910156517081");
    Console.WriteLine(String.Format("FirstName: {0}, Surname:{1}", p.FirstName, p.Surname);
}
```

Should we be checking whether the result of `GetPersonByIdNumber` is `null` before using it? Without knowledge of the implementation of the function we're calling, we do not know how it handles an error condition, it could return a `null` or it could throw an exception. The signature of the function offers no guidance in this case!

What about this code?

```csharp

Nullable<int> ConvertToInt(string s)
{
	// Implementation ommitted!
}

void Main(object[] args)
{
    var p = ConvertToInt("bla");
    
    // Print the result to the console
    Console.WriteLine(p.Value);
}
```

Does the call to `Console.WriteLine(p.Value)` cause an exception? Yes it does, it suffers from a poor design. Exceptions are meant to be used for the *Truly Exceptional* conditions that arise, like when memory can't be allocated! There would be little point in catching a `OutOfMemoryException`, what could you do to correct the situation? But here accessing `p.Value` throws an exception, a situation that could be avoided entirely with the correct design!

Let's create our own types to model the absence and presence of values:

```csharp

abstract class Option<T>;
{
	public sealed class None // Represents an absent value.
    {
    }
    
    public sealed class Some // Wraps a value when it is present.
    {
    	public T Value
        {
        	get;
            private set;
        }
        
        public Some(T value)
        {
        	this.Value = value;
        }
    }
}
```

Now lets see how we could use this in an improved design:

```csharp

Option<int> ConvertToInt(string s)
{
	// Implementation omitted!
}

void Main(object[] args)
{
    var p = ConvertToInt("bla");
  	var someP = p as Option<int>.Some;
    if (someP == null)
    {
    	Console.WriteLine("Couldn't convert to int");
    }
    else 
    {
    	Console.WriteLine(someP.Value);
    }
}
```

Now you may say that we could have added a similar check earlier when dealing with `Nullable<int>` to prevent an exception, the difference here is, access to `Value` can only be achieved by casting to `Option<int>.Some`. We now have the assistance of the type system and unlike `Nullable<int>`, `Option<int>` can contain any value, not just value types like `int`!

If you've been watching carefully though, you may note that we have simply deferred the null issue! Nothing prevents us from constructing an instance of `Option<string>.Some` with a null value for instance. This is where *Code Contracts* come in handy.

If you have already installed *Microsoft Code Contracts*, you can make the following simple change to the constructor for `Option<T>.Some`:

```csharp

public sealed class Some // Wraps a value when it is present.
{
	public T Value
    {
    	get;
        private set;
    }
        
    public Some(T value)
    {
      	// Tell code contracts to ensure that the value will never be null!
       	Contract.Requires(value != null); 
       	this.Value = value;
    }
}
```

*Code Contracts* will now ensure that callers never create an instance of `Option<T>.Some` with a `null` value! You've seen how we can avoid the *Billion Dollar Mistake*, but it may seem like a lot of effort, how can it be made easier?

### The Fortis.CSharp library
The `Option<T>` class as introduced earlier along with *Code Contracts* solves the *Billion Dollar Mistake*, but there are many other desirable features that could be added. What about:
    * Use as a dictionary key or in a hash table?
    * Composing functions that use `Option<T>`?
    * Converting from `Nullable<T>` to `Option<T>` and visa-versa?

The `Option<T>` type available in the [![Nuget package](https://img.shields.io/badge/nuget-Fortis%20C%23-blue.svg)](https://www.nuget.org/packages/Fortis.CSharp) package has the above mentioned features as well as others which will be discussed in future posts.

Please give it a try, and lets make programming in C# more fun and less error prone.


