using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation.Results;
using Rickie.Homework.ShowcaseApp.CustomExceptions;

namespace Rickie.Homework.ShowcaseApp.Services
{
    /// <summary>
    ///     Password verification
    /// </summary>
    public class PasswordService : IPasswordService
    {
        /// <summary>
        ///     Base64 encode
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Base64Decode(string password)
        {
            // Specical char handling for common URL characters
            password = password.Replace('-', '+');
            password = password.Replace('_', '/');
            password = password.PadRight(password.Length + (4 - password.Length % 4) % 4, '=');

            var data = Convert.FromBase64String(password);
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        ///     Check if a string only has base64 chars
        /// </summary>
        /// <param name="s">String to check</param>
        /// <returns>True if the input only contains base64 chars, otherwise false</returns>
        public bool IsBase64String(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;

            s = s.Trim();
            return s.Length % 4 == 0 && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        ///     Password comparison by hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="storedHash"></param>
        /// <param name="storedSalt"></param>
        /// <returns></returns>
        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            IList<ValidationFailure> messages = new List<ValidationFailure>();

            if (password == null) messages.Add(new ValidationFailure("password", "Password is null"));
            if (string.IsNullOrWhiteSpace(password))
                messages.Add(new ValidationFailure("password", "Value cannot be empty or whitespace only string."));
            if (storedHash.Length != 64)
                messages.Add(new ValidationFailure("passwordHash",
                    "Invalid length of password hash (64 bytes expected)."));
            if (storedSalt.Length != 128)
                throw new ArgumentException("passwordHash", "Invalid length of password salt (128 bytes expected).");

            if (messages.Count > 0) throw new ValidationException(messages);

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}