using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests;

[TestClass]
public class Addr
{
    [TestMethod]
    public async Task AddrL_Set_Data0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_L
                stp",
                emulator);

        Assert.AreEqual(0x000ff, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(0xff, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrL_Set_Data1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.A = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_L
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x000ff, emulator.Vera.Data1_Address);

        Assert.AreEqual(0xff, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrM_Set_Data0()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_M
                stp",
                emulator);

        Assert.AreEqual(0x0ff00, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0xff, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrM_Set_Data1()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 2;
        emulator.Vera.Data1_Address = 0x0000;
        emulator.A = 0xff;
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_M
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x0ff00, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0xff, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x20, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_Data0_HighBit()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x2345;
        emulator.A = 0x01; // for 0x1235, 

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x12345, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_Data1_HighBit()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x2345;
        emulator.A = 0x01; // for 0x1235, 
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x12345, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x01, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_Data0_StepChange()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x2345;
        emulator.A = 0x50; // for 0x1235, 

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x02345, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(16, emulator.Vera.Data0_Step);
        Assert.AreEqual(0, emulator.Vera.Data1_Step);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x50, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_Data1_StepChange()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x2345;
        emulator.A = 0x50; // for 0x1235, 
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x02345, emulator.Vera.Data1_Address);

        Assert.AreEqual(0, emulator.Vera.Data0_Step);
        Assert.AreEqual(16, emulator.Vera.Data1_Step);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x50, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_Data0_StepChange_Negative()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 0;
        emulator.Vera.Data0_Address = 0x2345;
        emulator.A = 0x58; 

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x02345, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(-16, emulator.Vera.Data0_Step);
        Assert.AreEqual(0, emulator.Vera.Data1_Step);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x58, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_Data1_StepChange_Negative()
    {
        var emulator = new Emulator();

        emulator.Vera.Data1_Step = 0;
        emulator.Vera.Data1_Address = 0x2345;
        emulator.A = 0x58; 
        emulator.Vera.AddrSel = true;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x00000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x02345, emulator.Vera.Data1_Address);

        Assert.AreEqual(0, emulator.Vera.Data0_Step);
        Assert.AreEqual(-16, emulator.Vera.Data1_Step);

        Assert.AreEqual(0x45, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x23, emulator.Memory[0x9F21]);
        Assert.AreEqual(0x58, emulator.Memory[0x9F22]);
    }

    [TestMethod]
    public async Task AddrH_Set_CorrectMask()
    {
        var emulator = new Emulator();

        emulator.Vera.Data0_Step = 2;
        emulator.Vera.Data0_Address = 0x0000;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta ADDRx_H
                stp",
                emulator);

        Assert.AreEqual(0x10000, emulator.Vera.Data0_Address);
        Assert.AreEqual(0x00000, emulator.Vera.Data1_Address);

        Assert.AreEqual(0x00, emulator.Memory[0x9F20]);
        Assert.AreEqual(0x00, emulator.Memory[0x9F21]);
        Assert.AreEqual(0xf9, emulator.Memory[0x9F22]);
    }
}