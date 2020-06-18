using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace indyClient
{
    static class CredentialFacilitator
    {
      static public string generateCredValueJson(string schemaAttributes, string credValues)
        {
            Console.WriteLine(schemaAttributes);
            Console.WriteLine(schemaValues);
            JArray array = JArray.Parse(schemaAttributes);
            List<string> attributes = array.ToObject<List<string>>();

            array = JArray.Parse(credValues);
            List<string> values = array.ToObject<List<string>>();

            if (attributes.Count != values.Count)
                Console.WriteLine("Number of arguments is not the same!");


            string output = "{";
            for (int idx = 0; idx < values.Count; ++idx)
            {
                string encoding = values[idx];
                if (!StringFacilitator.IsDigitsOnly(values[idx]))
                {
                    encoding = sha256_hash(values[idx]);
                    encoding = hexToDec(encoding);
                }
                output += "\"" + attributes[idx] + "\": {\"raw\": \"";
                output += values[idx] + "\", \"encoded\": \"" + encoding + "\"}";

                if (idx != values.Count - 1)
                    output += ",";
            }

            output += "}";

            return output;
        }

        static private string hexToDec(string hex)
        {
            BigNumberFacilitator factor = new BigNumberFacilitator("1");
            BigNumberFacilitator result = new BigNumberFacilitator("0");
            while(hex.Length != 0)
            {
                int num = hexToInt(hex[hex.Length - 1]);
                hex = hex.Remove(hex.Length - 1);
                BigNumberFacilitator temp =
                    new BigNumberFacilitator(num.ToString());

                temp.multiply(factor.getNumber());
                result.add(temp.getNumber());

                factor.multiply("16");
            }
            return result.getNumber();
        }

        static private int hexToInt(char hex)
        {
            if (hex >= '0' && hex <= '9')
                return hex - '0';
            else if (hex >= 'a' && hex <= 'f')
                return hex - 'a' + 10;

            return -1;
        }

        static private String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

    }
}
