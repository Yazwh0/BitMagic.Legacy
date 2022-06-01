# BitMagic the Emulator

## X16 Emulator

The BitMagic X16 emulator is currently on hiatus.

It currently emulates most 65c02 opcodes, the Vera's 4 and 8 bits per pixel tile modes and sprites.

Additionally there is no timing code, so it runs as fast as it can.

## Unit Tests

The compiler and emulator can be used to construct unit tests for 65c02\Basic X16 code. There is an example text project with a helper class to make writing the tests easier.

Below is an example of such a test.

```c#
[TestMethod]
public async Task Lda_Immediate()
{
    var result = await CommanderX16Test.UntilStp(@"
        .machine CommanderX16R40
        .org $810
        lda #$ff
        stp
        ");

    // compilation
    Assert.AreEqual(0xa9, result.Cpu.Memory.PeekByte(0x810));
    Assert.AreEqual(0xff, result.Cpu.Memory.PeekByte(0x811));

    // emulation
    Assert.AreEqual(0xff, result.Cpu.Registers.A);
    Assert.AreEqual(0x813, result.Cpu.Registers.PC);
}
```
### Todo

- Create a dedicated test ROM so code can be tested without the 'basic' header.
- Be able to snapshot the whole machine, so we can test for side effects.
- Add extensions to help with VERA unit tests, such as image comparison.
