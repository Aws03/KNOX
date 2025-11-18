//
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace JadaraITKnowledgeSystem.Infrastructure.Services.Password
//{
//    public class PasswordService : IPasswordService
//    {
//        // Constants for PBKDF2 configuration
//        private const int SaltSize = 16;     // Size of the salt in bytes (128 bits)
//        private const int HashSize = 32;     // Size of the hash in bytes (256 bits)
//        private const int Iterations = 10000; // Number of iterations (adjust for security/performance)

//        /// <summary>
//        /// Creates a password hash and salt using PBKDF2 with SHA256.
//        /// </summary>
//        /// <param name="password">The plaintext password to hash.</param>
//        /// <param name="salt">The generated salt.</param>
//        /// <param name="hash">The generated password hash.</param>
//        public void CreatePasswordHash(string password, out byte[] salt, out byte[] hash)
//        {
//            if (string.IsNullOrWhiteSpace(password))
//                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

//            // Generate a cryptographic random salt
//            using (var rng = RandomNumberGenerator.Create())
//            {
//                salt = new byte[SaltSize];
//                rng.GetBytes(salt);
//            }

//            // Generate the hash using PBKDF2 with SHA256
//            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
//            {
//                hash = pbkdf2.GetBytes(HashSize);
//            }
//        }

//        /// <summary>
//        /// Verifies a password against a stored hash and salt.
//        /// </summary>
//        /// <param name="password">The plaintext password to verify.</param>
//        /// <param name="storedHash">The stored password hash.</param>
//        /// <param name="storedSalt">The stored salt.</param>
//        /// <returns>True if the password matches; otherwise, false.</returns>
//        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
//        {
//            if (string.IsNullOrWhiteSpace(password))
//                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

//            if (storedHash == null || storedHash.Length != HashSize)
//                throw new ArgumentException($"Invalid length of password hash (expected {HashSize} bytes).", nameof(storedHash));

//            if (storedSalt == null || storedSalt.Length != SaltSize)
//                throw new ArgumentException($"Invalid length of password salt (expected {SaltSize} bytes).", nameof(storedSalt));

//            // Generate hash from the input password and stored salt
//            using (var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, Iterations, HashAlgorithmName.SHA256))
//            {
//                var computedHash = pbkdf2.GetBytes(HashSize);

//                // Compare the computed hash with the stored hash securely
//                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
//            }
//        }

//        public bool CheckPasswordValidation(string password)
//        {
//            if (string.IsNullOrWhiteSpace(password))
//                return false;

//            // 1. Minimum length
//            if (password.Length < 8)
//                return false;

//            // 2. At least one uppercase
//            if (!Regex.IsMatch(password, "[A-Z]"))
//                return false;

//            // 3. At least one lowercase
//            if (!Regex.IsMatch(password, "[a-z]"))
//                return false;

//            // 4. At least one digit
//            if (!Regex.IsMatch(password, @"\d"))
//                return false;

//            // 5. At least one special character
//            if (!Regex.IsMatch(password, @"[\W_]"))
//                return false;

//            return true;
//        }
//    }
//}
