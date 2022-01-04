# Warning!

This code is disgusting.  Hideous.  Abhorrent.  Proceed at your own risk!

### [GetLocals](Witchcraft/GetLocals.cs)
> ★★★★★ ☠️

Takes advantage of how the async implementation [generates classes](https://www.simple-talk.com/dotnet/net-tools/c-async-what-is-it-and-how-does-it-work/) which helpfully contain fields representing the variables in the calling closure.  Uses a fair amount of reflection hackery to get ahold of the original context and state machine.  Inspired by Jon Skeet's [Abusing C#](https://vimeo.com/68320506) talk and [sample code](https://github.com/jskeet/DemoCode/blob/master/Abusing%20CSharp/Code/FunWithAwaiters/SaveState.cs).

<sub>May or may not work in your runtime.</sub>

### [Overwrite](Witchcraft/Overwrite.cs)
> ★★★☆☆

Gets a char pointer to a string (from the intern pool!) and writes on top of it.  Since the intern pool is [CLR-scoped](https://stackoverflow.com/a/26544799/382456), some ~~real damage~~ interesting results can be observed when writing here.  Since we can't reallocate the actual string memory (AFAIK), this method is limited to writing strings of equal length or shorter than the string being replaced, otherwise it would write to other random memory which could result in an access violation.  Overwriting strings is also limited in length due to some strange behaviour when dealing with large lengths.

Also serves as a fun way to prove that `const string` is basically meaningless at runtime!

### [AutoClone](Witchcraft/AutoClone.cs)
> ★★☆☆☆

Reflects over fields (including compiler-generated property backing fields) and recursively copies them into a new object.  The new object is allocated using [`GetUninitializedObject`](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatterservices.getuninitializedobject(v=vs.110).aspx), which allocates an object without running its constructors.  I suppose you could argue this is a kind of serialization, so this is alright...

### [GetOffset](Witchcraft/GetOffset.cs)
> ★☆☆☆☆

Simple trick using "undocumented" features to get a pointer to where a managed object is allocated in memory.  Unreliable most of the time, but works quite well with fixed structs.

### [MultiReturn](Witchcraft/MultiReturn.cs)
> ★☆☆☆☆

This began as an exercise to see how many values I could unintuitively hide behind a single variable.  Nothing groundbreaking, but may be interesting to check out the four-line self-awaitable pattern used here.
