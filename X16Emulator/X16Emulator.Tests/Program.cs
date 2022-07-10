using BigMagic_Emulator;

Console.WriteLine("BitMagic - 65c02 Emulator");

var emulator = new Emulator();

var returnCode = emulator.Tests();
Console.WriteLine($"Emulator returned {returnCode}");

