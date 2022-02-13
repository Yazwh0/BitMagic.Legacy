# BitMagic the Template Engine

BitMagic's Template Engine is the first stage of the build process. It takes the `csasm` file written by the developer and turns it into `bmasm` file that can be compiled.

## The Basics

The Template Engine works in a similar way to the Razor engine that ASP.net uses. It allows us to write C# mixed with assembler code to produce code that can then be compiled. C# provides the structure, while any code is the output.

It is not a language in its own right. Just like all Macro Assemblers you will still need to code in the underlying assembler. As the code produced will be compiled by the BitMagic compiler, its worth taking time to understand the syntax that it understands.

Currently it only supports 65c02, but eventually this will be switchable to a set of supported CPUs.

### Example Application

Consider the following which demonstrates how the Template Engine works with mixing C# and asm.

```csharp
BM.X16Header();

void SetColour();

// define a function that can be reused, akin to a 'macro'. 
// It takes a color as defined in System.Drawing, changes it into 4 bit colour and writes it to the VERA.
void SetColour(Color colour)
{
    lda #@((colour.G & 0xf0) + (colour.B >> 4))
    sta DATA0
    lda #@(colour.R >> 4)
    sta DATA0
}

SetColour(Color.DarkOrchid); // $9932CC, or $93c on the X16, and so $3c, $09 when loading into VERA
SetColour(Color.White);

.loop:          // infinite loop
jmp loop
```

`BM` is a static class that holds several useful functions. The `X16Header()` above outputs the byte code required to start an application.

`SetColour` is a C# method that takes a `Color` as an input, converts it into 4bit colour and writes it to whatever the DATA0 Vera register is pointing to. This is then called twice, followed by a loop.

When compiled this will create a .prg file with the following:

```txt
    $0801:  $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
    $0810:  $4C, $10, $08         JMP       loop
    $0813:  $A9, $3C              LDA       #60
    $0815:  $8D, $23, $9F         STA       DATA0
    $0818:  $A9, $09              LDA       #9
    $081A:  $8D, $23, $9F         STA       DATA0
    $081D:  $A9, $FF              LDA       #255
    $081F:  $8D, $23, $9F         STA       DATA0
    $0822:  $A9, $0F              LDA       #15
    $0824:  $8D, $23, $9F         STA       DATA0
```

The values being loaded into DATA0 match what we expect. (*Note, they're displayed in decimal rather than hex.*)

## Libraries

Libraries can be created using with the standard c# class library, once you've made a class library edit the `.csproj` file so it looks like the following (adjust the paths accordingly):

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\BitMagic.AsmTemplate\BitMagic.AsmTemplate.csproj" />
    <Using Include="BitMagic.AsmTemplate.BitMagicHelper" Alias="BM" />
  </ItemGroup>

  <Target Name="csasm" BeforeTargets="BeforeBuild">
    <Exec Command="..\..\Bitmagic.SdkPreProcessor\bin\Debug\net6.0\Bitmagic.SdkPreProcessor --recursive=true --base-folder=$(MSBuildProjectDirectory) *.csasm"/>
  </Target>

</Project>
```

This will have to be done until there is a SDK set up and a proper installer.

The project reference and including ensure the static `BM` class is referenced, as well as a helper class to run the template once the project is compiled.

The `BeforeBuild` directive runs the preprocessor which converts the `.csmasm` file into a `.cs` file, which his then build using the normal dotnet process. Because this is all controlled in a the `.csproj` file the libraries can be written in any editor that supports .net, such as Visual Studio Code. **If you use VSC there is a syntax extension [here](../BitMagic.VscGrammar/README.md)**.

From here just create `.csasm` files for the code than generates asm as normal. There are some example libraries in the `Libraries` folder.

Once a library is complete it can be referenced into the applications Template Engine with a special keyword `assembly` along with the path to the `.dll` at the start of the `.csasm` file. See the Special Syntax section below.

## Reference

Reference for BitMagic the Template Engine.

### Special Syntax

The non-library `.csasm` files have a few special keywords for references as there is no project file.

| Name | Purpose |
| --- | --- |
| assembly | Load an assembly that is in a `.dll` into the Template Engine. |
| reference | Reference an assembly that is already loaded. Typically System references. |

### BM Static Class

| Name | Description |
| --- | --- |
| `BM.X16header()` | Will output the basic commands to jump to the code at $810. |
| `BM.Bytes(IEnumerable<byte> bytes, int width = 16)` | Outputs the bytes as '.byte' code. |
| `BM.Words(IEnumerable<short> words, int width = 16)` | Outputs the words as '.word' code. |

## Todo

- Change the pre-processor so it is CPU agnostic.
- Add some beautifier to the `bmasm` that the process creates to make it easier to read.
