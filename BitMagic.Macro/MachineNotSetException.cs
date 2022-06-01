namespace BigMagic.Macro
{
    public class MachineNotSetException : MacroException
    {
        public MachineNotSetException() : base("Machine not set.")
        {
        }

        public override string ErrorDetail => "Set a machine using 'machine' or command line argument.";
    }
}
