using System.Text.Json;

namespace Vauth.IO.Json
{
    /// <summary>
    /// Represents a JSON boolean value.
    /// </summary>
    public class JBoolean : JObject
    {
        /// <summary>
        /// Gets the value of the JSON object.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JBoolean"/> class with the specified value.
        /// </summary>
        /// <param name="value">The value of the JSON object.</param>
        public JBoolean(bool value = false)
        {
            this.Value = value;
        }

        public override bool AsBoolean()
        {
            return Value;
        }

        /// <summary>
        /// Converts the current JSON object to a floating point number.
        /// </summary>
        /// <returns>The number 1 if value is <see langword="true"/>; otherwise, 0.</returns>
        public override double AsNumber()
        {
            return Value ? 1 : 0;
        }

        public override string AsString()
        {
            return Value.ToString().ToLowerInvariant();
        }

        public override bool GetBoolean() => Value;

        public override string ToString()
        {
            return AsString();
        }

        internal override void Write(Utf8JsonWriter writer)
        {
            writer.WriteBooleanValue(Value);
        }

        public override JObject Clone()
        {
            return this;
        }

        public static implicit operator JBoolean(bool value)
        {
            return new JBoolean(value);
        }
    }
}
