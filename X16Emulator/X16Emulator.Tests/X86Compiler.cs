//using Binarysharp.Assemblers.Fasm;
//using Process.NET;
//using Process.NET.Memory;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace BigMagic_Emulator
//{
//    internal static class X86Compiler
//    {
//        public delegate int EmulatorFunction();

//        public static CompiledFunction GetEmulator()
//        {
//            var fasm = new FasmNet();

//            fasm.AddLine("use32");
//            fasm.AddLine("mov eax, [ebp+4]");
//            fasm.AddLine("ret");

//            byte[] code = fasm.Assemble();

//            var currentProcess = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess(), MemoryType.Local);
//            var allocatedMemory = currentProcess.MemoryFactory.Allocate(
//                name: "Example",
//                size: code.Length,
//                protection: Process.NET.Native.Types.MemoryProtectionFlags.ExecuteReadWrite
//                );

//            var function = Marshal.GetDelegateForFunctionPointer<EmulatorFunction>(allocatedMemory.BaseAddress);

//            return new CompiledFunction(function, allocatedMemory);
//        }
//    }

//    internal class CompiledFunction : IDisposable
//    {
//        private readonly X86Compiler.EmulatorFunction _function;
//        private readonly IAllocatedMemory _memory;

//        public CompiledFunction(X86Compiler.EmulatorFunction function, IAllocatedMemory memory)
//        {
//            _function = function;
//            _memory = memory;
//        }

//        public int Invoke() => _function();        

//        public void Dispose() => _memory.Dispose();
//    }
//}
