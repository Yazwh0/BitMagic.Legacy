using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Via;

[TestClass]
public class Register_A
{
    [TestMethod]
    public async Task RegisterA_Defaults()
    {
        var emulator = new Emulator();

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0xff, emulator.Via.Register_A_InValue);
    }


    [TestMethod]
    public async Task RegisterA()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xab;
        emulator.Via.Register_A_InValue = 0x89;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stp",
                emulator);

        Assert.AreEqual(0xab, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xab, emulator.Memory[0x9f0f]);
        Assert.AreEqual(0xab, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0x89, emulator.Via.Register_A_InValue);
    }

    [TestMethod]
    public async Task DataDirectionRegister_Set_NothingOut()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0x00;
        emulator.Via.Register_A_InValue = 0xff;
        emulator.Memory[0x9f03] = 0x00;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_DDRA
                stp",
                emulator);

        Assert.AreEqual(0xff, emulator.Memory[0x9f03]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f01]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f0f]);
    }

    [TestMethod]
    public async Task DataDirectionRegister_Set_DataOut()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xaa;
        emulator.Via.Register_A_InValue = 0xff;
        emulator.Memory[0x9f03] = 0x00;
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_DDRA
                stp",
                emulator);

        Assert.AreEqual(0xff, emulator.Memory[0x9f03]);
        Assert.AreEqual(0xaa, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xaa, emulator.Memory[0x9f0f]);
    }


    [TestMethod]
    public async Task DataDirectionRegister_SetPartial_DataOut_InNonFF()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xaa;
        emulator.Via.Register_A_InValue = 0x55;
        emulator.Memory[0x9f03] = 0x00;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_DDRA
                stp",
                emulator);

        Assert.AreEqual(0x0f, emulator.Memory[0x9f03]);
        Assert.AreEqual(0x5a, emulator.Memory[0x9f01]);
        Assert.AreEqual(0x5a, emulator.Memory[0x9f0f]);
    }

    [TestMethod]
    public async Task DataDirectionRegister_Set_DataOut_Partial()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xaa;
        emulator.Via.Register_A_InValue = 0xff;
        emulator.Memory[0x9f03] = 0x00;
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_DDRA
                stp",
                emulator);

        Assert.AreEqual(0x0f, emulator.Memory[0x9f03]);
        Assert.AreEqual(0xfa, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xfa, emulator.Memory[0x9f0f]);
    }

    [TestMethod]
    public async Task DataDirectionRegister_Clear()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0x00;
        emulator.Via.Register_A_InValue = 0xff;
        emulator.Memory[0x9f03] = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz V_DDRA
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9f03]);
        Assert.AreEqual(0xff, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xff, emulator.Memory[0x9f0f]);
    }

    [TestMethod]
    public async Task RegisterA_Set()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0x00;
        emulator.Via.Register_A_InValue = 0xff;
        emulator.Memory[0x9f03] = 0xff; // all output
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_PRA
                stp",
                emulator);

        Assert.AreEqual(0xff, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xff, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0xff, emulator.Via.Register_A_OutValue);
    }

    [TestMethod]
    public async Task RegisterA_Clear()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xff;
        emulator.Via.Register_A_InValue = 0xff;
        emulator.Memory[0x9f03] = 0xff; // all output

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz V_PRA
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9f01]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f0f]);
        Assert.AreEqual(0x00, emulator.Via.Register_A_OutValue);
    }

    [TestMethod]
    public async Task RegisterA_Set_PartialOutput_NothingIn()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0x00;
        emulator.Via.Register_A_InValue = 0x00;
        emulator.Memory[0x9f03] = 0x0f; // half output, 1s here should be 1 in the output
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_PRA
                stp",
                emulator);

        Assert.AreEqual(0x0f, emulator.Memory[0x9f01]);
        Assert.AreEqual(0x0f, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0xff, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0x00, emulator.Via.Register_A_InValue);
    }

    [TestMethod]
    public async Task RegisterA_Clear_PartialOutput_NothingIn()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xff;
        emulator.Via.Register_A_InValue = 0x00;
        emulator.Memory[0x9f03] = 0x0f; 

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz V_PRA
                stp",
                emulator);

        Assert.AreEqual(0x00, emulator.Memory[0x9f01]);
        Assert.AreEqual(0x00, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0x00, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0x00, emulator.Via.Register_A_InValue);
    }

    [TestMethod]
    public async Task RegisterA_Set_PartialOutput_DataIn()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0x00;
        emulator.Via.Register_A_InValue = 0xaa;
        emulator.Memory[0x9f03] = 0x0f; // half output, 1s here should be 1 in the output
        emulator.A = 0xff;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_PRA
                stp",
                emulator);

        Assert.AreEqual(0xaf, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xaf, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0xff, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0xaa, emulator.Via.Register_A_InValue);
    }

    [TestMethod]
    public async Task RegisterA_SetPartial_PartialOutput_DataIn()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0x00;
        emulator.Via.Register_A_InValue = 0xaa;
        emulator.Memory[0x9f03] = 0x0f; // half output, 1s here should be 1 in the output
        emulator.A = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                sta V_PRA
                stp",
                emulator);

        Assert.AreEqual(0xaf, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xaf, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0x0f, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0xaa, emulator.Via.Register_A_InValue);
    }

    [TestMethod]
    public async Task RegisterA_Clear_PartialOutput_DataIn()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xff;
        emulator.Via.Register_A_InValue = 0xaa;
        emulator.Memory[0x9f03] = 0x0f;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                stz V_PRA
                stp",
                emulator);

        Assert.AreEqual(0xa0, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xa0, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0x00, emulator.Via.Register_A_OutValue);
        Assert.AreEqual(0xaa, emulator.Via.Register_A_InValue);
    }

    [TestMethod]
    public async Task RegisterA_RW_Set()
    {
        var emulator = new Emulator();
        emulator.Via.Register_A_OutValue = 0xff;
        emulator.Via.Register_A_InValue = 0x00;
        emulator.Memory[0x9f03] = 0xff; // all output
        emulator.A = 0x01;

        await X16TestHelper.Emulate(@"
                .machine CommanderX16R40
                .org $810
                trb V_PRA
                stp",
                emulator);

        Assert.AreEqual(0xfe, emulator.Memory[0x9f01]);
        Assert.AreEqual(0xfe, emulator.Memory[0x9f0f]); // ORA should mirror DRA
        Assert.AreEqual(0xfe, emulator.Via.Register_A_OutValue);
    }
}