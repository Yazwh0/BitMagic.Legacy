namespace BitMagic.Compiler.Exceptions
{
    public class MachineNotKnownException : CompilerException
    {
        public string MachineName { get; }

        public MachineNotKnownException(string name) : base("Machine not known.")
        {
            MachineName = name;
        }

        public override string ErrorDetail => $"Unknown machine '{MachineName}'.";
    }

}
