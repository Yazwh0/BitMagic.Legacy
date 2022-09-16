using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class PHP
{
    [TestMethod]
    public async Task Php_NoFlags()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // compilation
        Assert.AreEqual(0x08, emulator.Memory[0x810]);

        // emulation
        Assert.AreEqual(0x30, emulator.Memory[0x1fd]); 
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(false, false, false, false);
    }

    [TestMethod]
    public async Task Php_Full()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                ldx #$ff
                txs
                lda #$ff
            .loop:
                php
                dec
                bne loop
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x32, emulator.Memory[0x100]);

        for (var i = 0x101; i < 0x180; i++)
            Assert.AreEqual(0x30, emulator.Memory[i]);

        for (var i = 0x180; i < 0x200; i++)
            Assert.AreEqual(0xb0, emulator.Memory[i]);

        emulator.AssertState(0x00, 0xff, 0x00, stackPointer: 0x1ff);
        emulator.AssertFlags(true, false, false, false, false, false);
    }

    [TestMethod]
    public async Task Php_Carry()
    {
        var emulator = new Emulator();

        emulator.Carry = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x31, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(false, false, false, true, false, false);
    }

    [TestMethod]
    public async Task Php_Negative()
    {
        var emulator = new Emulator();

        emulator.Negative = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0xb0, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(false, true, false, false, false, false);
    }

    [TestMethod]
    public async Task Php_Zero()
    {
        var emulator = new Emulator();

        emulator.Zero = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x32, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(true, false, false, false, false, false);
    }

    [TestMethod]
    public async Task Php_InterruptDisable()
    {
        var emulator = new Emulator();

        emulator.InterruptDisable = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x34, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(false, false, false, false, true, false);
    }

    [TestMethod]
    public async Task Php_Decimal()
    {
        var emulator = new Emulator();

        emulator.Decimal = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x38, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(false, false, false, false, false, true);
    }

    [TestMethod]
    public async Task Php_Overflow()
    {
        var emulator = new Emulator();

        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0x70, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(false, false, true, false, false, false);
    }

    [TestMethod]
    public async Task Php_All()
    {
        var emulator = new Emulator();

        emulator.Zero = true;
        emulator.Negative = true;
        emulator.Carry= true;
        emulator.Decimal = true;
        emulator.InterruptDisable = true;
        emulator.Overflow = true;

        await X16TestHelper.Emulate(@"                
                .machine CommanderX16R40
                .org $810
                php
                stp",
                emulator);

        // emulation
        Assert.AreEqual(0xff, emulator.Memory[0x1fd]);
        emulator.AssertState(0x00, 0x00, 0x00, 0x812, 3);
        emulator.AssertFlags(true, true, true, true, true, true);
    }
}

