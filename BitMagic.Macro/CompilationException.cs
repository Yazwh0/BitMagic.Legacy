using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigMagic.Macro
{
    public class CompilationException : Exception {
        public List<Diagnostic> Errors { get; set; } = new List<Diagnostic>();

        public string GeneratedCode { get; set; } = "";

        public override string Message
        {
            get
            {
                string errors = string.Join("\n", this.Errors.Where(w => w.IsWarningAsError || w.Severity == DiagnosticSeverity.Error));
                return "Unable to compile template: " + errors;
            }
        }
    }

}
