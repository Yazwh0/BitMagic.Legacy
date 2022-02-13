# BitMagic

The aim of the BitMagic was to create a Template Engine, Compiler, Emulator and Debug Adaptor to provide a rich development experience for the X16. However with the state of the [X16](https://www.commanderx16.com/) project, its more prudent to spend time making the Template Engine and Compiler to be able to target other platforms, such as the [Mega 65](https://mega65.org/) or even non 6502 based systems like the Amiga.

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

If you use Visual Studio Code its worth building and installing the [extension](BitMagic.VscGrammar/README.md)).

Open the VSC workspace in `Examples\DisplayImage` for a practical example of how a project can be put together. If you want to see this example you can do so [here](https://www.commanderx16.com/forum/index.php?/files/file/228-bitmagic-example/).
