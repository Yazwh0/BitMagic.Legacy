using BitMagic.Common;
using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;

namespace BitMagic.Compiler
{
    internal class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly CodingSeb.ExpressionEvaluator.ExpressionEvaluator _evaluator = new();
        private bool _requiresReval;
        private ParameterSize _size;
        private IVariables? _variables = null;
        private readonly CompileState _state;

        public List<string> RequiresRevalNames = new();

        public ExpressionEvaluator(CompileState state)
        {
            _state = state;
        }

        // not thread safe!!!
        public (int Result, bool RequiresRecalc) Evaluate(string expression, IVariables variables, ParameterSize size)
        {
            _variables = variables;
            _requiresReval = false;
            _size = size;
            _evaluator.PreEvaluateVariable += _evaluator_PreEvaluateVariable;
            var result = (int)_evaluator.Evaluate(expression);
            _evaluator.PreEvaluateVariable -= _evaluator_PreEvaluateVariable;

            return new(result, _requiresReval);
        }

        private void _evaluator_PreEvaluateVariable(object? sender, VariablePreEvaluationEventArg e)
        {
            if (_variables == null)
                throw new NullReferenceException("_procedure is null");

            if (_variables.TryGetValue(e.Name, 0, out var result))
            {
                e.Value = result;
                _requiresReval = false;
            }
            else
            {
                RequiresRevalNames.Add(e.Name);
                _requiresReval = true;

                // activate when we have preprocess constant collection
                //e.Value = _size switch
                //{
                //    ParameterSize.Bit8 => 0xab,
                //    ParameterSize.Bit16 => 0xabcd,
                //    ParameterSize.Bit32 => 0xabcdabcd,
                //    _ => throw new InvalidOperationException($"Unknown size {_size}")
                //};
                e.Value = 0xabcd; // random two byte number
            }
        }

        public void Reset()
        {
            RequiresRevalNames.Clear();
        }
    }
}
