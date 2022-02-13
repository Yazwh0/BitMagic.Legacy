# BitMagic

The aim of the BitMagic was to create a Template Engine, Compiler, Emulator and Debug Adaptor to provide a rich development experience for the X16. However with the state of the [X16](https://www.commanderx16.com/) project, its more prudent to spend time making the Template Engine and Compiler to be able to target other platforms, such as the [Mega 65](https://mega65.org/) or even non 6502 based systems like the Amiga.

## Why BitMagic?

Why BitMagic over the other 'Macro Assemblers' like ca65 or kasm?

After writing a couple of [intros](https://www.commanderx16.com/forum/index.php?/profile/1576-yazwho/content/&type=downloads_file), as well as a basic '[tracker](https://github.com/Yazwh0/BitPlayer)' type application, I started to find coding using ca65 to be quite restrictive.

I don't think its just ca65, I think all Macro Assemblers have a similar problem in that they are restricted to what they can do by the time of the team behind them. Nowadays we expect so much functionality out of the box it will be hard for small teams to create the feature rich environment that we want. This is why I use C# as the Template Engine language, by allowing us to write in C# and use the dotnet eco system, this will give devs access to anything we could ever need.

This keeps the build environment in one project, including as much of the maintenance of assets as possible. For example image conversion, tile creation and file compression can be put inside the project. No more delicate scripts to create assets.

As the libraries are also written in C# it offers the advantage of integration into asset creation apps. A tracker for example can include a dotnet library that generates the code required to play the tune, so it would only include the features needed, and it could do so from within the application itself using intuitive syntax rather than writing to its own file.

With a basic emulator these libraries could even be unit testable. Not to test the text of the code, but actually what it does. Something that makes life so much easier when writing libraries.

The C# Template Engine really is a 'Macro Assembler' on steroids.

## Documentation

You can read more about the various stages using these links:

- [BitMagic the Template Engine](Documentation/TemplateEngine.md)
- [BitMagic the Compiler](Documentation/Compiler.md)
- [BitMagic the Emulator](Documentation/Emulator.md)
- [BitMagic the Debugger](Documentation/Debugger.md)
- [BitMagic the Visual Studio Code Extension](BitMagic.VscGrammar/README.md)

## Installation

Clone the repo.

Open `BitMagic.sln` in your favourite c# editor. I use Visual Studio 2022. Set the build configuration to Debug and build everything. We need to do this as there is not yet a installer, so even if you just want to write code for the X16 you still need to create the applications.

If you use Visual Studio Code its worth building and installing the [extension](BitMagic.VscGrammar/README.md).

Open the VSC workspace in `Examples\DisplayImage` for a practical example of how a project can be put together. If you want to see this example you can do so [here](https://www.commanderx16.com/forum/index.php?/files/file/228-bitmagic-example/).

## License

Unless otherwise stated (generally code from others that falls under a difference license) all the code is under GLP-3.0. Applications written using BitMagic however are license free, unless a library is used with a specific license. (The Compression library for example.)

That said a friendly nod would be appreciated!
