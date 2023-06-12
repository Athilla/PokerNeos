using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Transversals.Business.UserPermissions.Application.AzureADB2C
{
    /// <summary>
    /// Password Generator for ADB2C Users
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generate a random password
        /// </summary>
        /// <param name="lowercase"></param>
        /// <param name="uppercase"></param>
        /// <param name="numerics"></param>
        /// <returns></returns>
        public static string GenerateNewPassword(int lowercase = 2, int uppercase = 4, int numerics = 2)
        {
            const string lowers = "abcdefghijklmnopqrstuvwxyz";
            const string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "0123456789";

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    RandomNumberGenerator.GetInt32(0, generated.Length),
                    lowers[RandomNumberGenerator.GetInt32(0, lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    RandomNumberGenerator.GetInt32(0, generated.Length),
                    uppers[RandomNumberGenerator.GetInt32(0, uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    RandomNumberGenerator.GetInt32(0, generated.Length),
                    number[RandomNumberGenerator.GetInt32(0, number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);
        }

    }
}
