namespace JadaraITKnowledgeSystem.Domain.Users.ValueObjects
{
    public class Email
    {
        public string Address { get; }

        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address) || !address.Contains('@'))
                throw new ArgumentException("Invalid email address");

            Address = Normalize(address);
        }

        /// <summary>
        /// The canonical stored form of an address. Compare stored addresses against
        /// <c>Normalize(input)</c> rather than relying on the database collation.
        /// </summary>
        public static string Normalize(string address) => address.Trim().ToUpperInvariant();

        public override string ToString() => Address;
    }
}
