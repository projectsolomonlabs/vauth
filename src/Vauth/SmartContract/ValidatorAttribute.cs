using Vauth.VM.Types;
using System;

namespace Vauth.SmartContract
{
    [AttributeUsage(AttributeTargets.Parameter)]
    abstract class ValidatorAttribute : Attribute
    {
        public abstract void Validate(StackItem item);
    }
}
