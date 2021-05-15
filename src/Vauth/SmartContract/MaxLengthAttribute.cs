using Vauth.VM.Types;
using System;

namespace Vauth.SmartContract
{
    class MaxLengthAttribute : ValidatorAttribute
    {
        public readonly int MaxLength;

        public MaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }

        public override void Validate(StackItem item)
        {
            if (item.GetSpan().Length > MaxLength)
                throw new InvalidOperationException("The input exceeds the maximum length.");
        }
    }
}
