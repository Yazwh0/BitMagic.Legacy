namespace BitMagic.Compiler.Exceptions
{
    public class MachineNotSetException : CompilerException
    {
        public MachineNotSetException() : base("Machine not set.")
        {
        }

        public override string ErrorDetail => "Set a machine using '.machine' or command line argument.";
    }

}
