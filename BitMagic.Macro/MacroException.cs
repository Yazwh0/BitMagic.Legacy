using System;

namespace BigMagic.Macro
{
    public abstract class MacroException : Exception
    {
        public abstract string ErrorDetail { get; }

        public MacroException(string message) : base(message)
        {
        }
    }
}
