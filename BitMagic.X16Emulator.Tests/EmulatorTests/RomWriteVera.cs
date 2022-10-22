using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Core;

[TestClass]
public class RomWriteVera
{
    [TestMethod]
    public async Task Write_L0BaseAddress()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x8d;
        emulator.RomBank[0x0001] = 0x2e;
        emulator.RomBank[0x0002] = 0x9f;
        emulator.RomBank[0x0003] = 0xdb;

        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000
                ",
                emulator);

        Assert.AreEqual((uint)0x1e00, emulator.Vera.Layer0_MapAddress);
    }

    [TestMethod]
    public async Task Write_L1BaseAddress()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x8d;
        emulator.RomBank[0x0001] = 0x35;
        emulator.RomBank[0x0002] = 0x9f;
        emulator.RomBank[0x0003] = 0xdb;

        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000
                ",
                emulator);

        Assert.AreEqual((uint)0x1e00, emulator.Vera.Layer1_MapAddress);
    }

    [TestMethod]
    public async Task ReadWrite_AddrL()
    {
        var emulator = new Emulator();

        emulator.RomBank[0x0000] = 0x8d; // sta
        emulator.RomBank[0x0001] = 0x20;
        emulator.RomBank[0x0002] = 0x9f;
        emulator.RomBank[0x0003] = 0xad; // lda 
        emulator.RomBank[0x0004] = 0x20;
        emulator.RomBank[0x0005] = 0x9f;
        emulator.RomBank[0x0006] = 0xdb;

        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                jmp $c000
                ",
                emulator);

        Assert.AreEqual(0x0f, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x0f, emulator.Memory[0x9f20]);
        Assert.AreEqual(0x0f, emulator.A);
    }
}