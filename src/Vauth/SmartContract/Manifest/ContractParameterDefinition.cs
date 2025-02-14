using Vauth.IO.Json;
using Vauth.VM;
using Vauth.VM.Types;
using System;

namespace Vauth.SmartContract.Manifest
{
    /// <summary>
    /// Represents a parameter of an event or method in ABI.
    /// </summary>
    public class ContractParameterDefinition : IInteroperable
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the parameter. It can be any value of <see cref="ContractParameterType"/> except <see cref="ContractParameterType.Void"/>.
        /// </summary>
        public ContractParameterType Type { get; set; }

        void IInteroperable.FromStackItem(StackItem stackItem)
        {
            Struct @struct = (Struct)stackItem;
            Name = @struct[0].GetString();
            Type = (ContractParameterType)(byte)@struct[1].GetInteger();
        }

        public StackItem ToStackItem(ReferenceCounter referenceCounter)
        {
            return new Struct(referenceCounter) { Name, (byte)Type };
        }

        /// <summary>
        /// Converts the parameter from a JSON object.
        /// </summary>
        /// <param name="json">The parameter represented by a JSON object.</param>
        /// <returns>The converted parameter.</returns>
        public static ContractParameterDefinition FromJson(JObject json)
        {
            ContractParameterDefinition parameter = new()
            {
                Name = json["name"].GetString(),
                Type = Enum.Parse<ContractParameterType>(json["type"].GetString())
            };
            if (string.IsNullOrEmpty(parameter.Name))
                throw new FormatException();
            if (!Enum.IsDefined(parameter.Type) || parameter.Type == ContractParameterType.Void)
                throw new FormatException();
            return parameter;
        }

        /// <summary>
        /// Converts the parameter to a JSON object.
        /// </summary>
        /// <returns>The parameter represented by a JSON object.</returns>
        public JObject ToJson()
        {
            var json = new JObject();
            json["name"] = Name;
            json["type"] = Type.ToString();
            return json;
        }
    }
}
