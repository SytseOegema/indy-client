using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using SecretSharingDotNet.Cryptography;
using SecretSharingDotNet.Math;

namespace indyClient
{
    public static class SecretSharingFacilitator
    {
        private static ExtendedEuclideanAlgorithm<BigInteger> gcd =
            new ExtendedEuclideanAlgorithm<BigInteger>();

        public static List<string> createSharedSecret(string secretInput, int minimum, int total)
        {
            // Create Shamir's Secret Sharing instance with BigInteger
            var split = new ShamirsSecretSharing<BigInteger>(gcd);

            string password = secretInput;
            // Minimum number of shared secrets for reconstruction: minimum
            // Maximum number of shared secrets: total
            // Attention: The secret length changes the security level set by the ctor
            var x = split.MakeShares (minimum, total, password);

            List<string> secrets = new List<string>();
            foreach(var item in x.Item2)
            {
                secrets.Add(item.ToString());
                Console.WriteLine(item);
            }

            return secrets;
        }

        public static string combineSharedSecrets(List<string> secrets)
        {
            List<FinitePoint<BigInteger>> points = new List<FinitePoint<BigInteger>>();
            foreach(var secret in secrets)
            {
                points.Add(FinitePoint<BigInteger>.Parse(secret));
            }

            var combine = new ShamirsSecretSharing<BigInteger>(gcd);

            return combine.Reconstruction(points.ToArray()).ToString();
        }


    }
}
