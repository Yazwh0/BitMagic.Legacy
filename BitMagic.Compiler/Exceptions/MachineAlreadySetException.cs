namespace BitMagic.Compiler.Exceptions
{
    public class MachineAlreadySetException : CompilerException
    {
        public string ExistingMachineName { get; }
        public string NewMachineName { get; }

        public MachineAlreadySetException(string existing, string newName) : base("Machine is already set.")
        {
            ExistingMachineName = existing;
            NewMachineName = newName;
        }

        public override string ErrorDetail => $"Machine is already set to {ExistingMachineName}, but is being changed to {NewMachineName}.";
    }

}
