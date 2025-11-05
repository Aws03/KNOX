using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.ValueObjects
{
    public class FullName
    {
        public string Value { get; }

        public FullName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Full name is required");
            Value = value;
        }

        public override string ToString() => Value;
    }
}
