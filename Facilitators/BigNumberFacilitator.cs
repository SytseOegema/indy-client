using System;
using System.Text;

namespace indyClient
{
    class BigNumberFacilitator
    {
        private string d_number;

        public BigNumberFacilitator(string bigNumber)
        {
            d_number = bigNumber;
        }

        public string getNumber()
        {
            return d_number;
        }

        public void add(string number)
        {
            string longest = (d_number.Length > number.Length ? d_number : number);
            string smallest = (longest != d_number ? d_number : number);

            // insert 0's before smallest to make both strings the same length;
            int sizeDif = longest.Length - smallest.Length;
            smallest = prependZeros(smallest, sizeDif);

            d_number = addNumbers(longest, smallest, 0, longest.Length - 1);
        }

        public void multiply(string number)
        {
            BigNumberFacilitator result = new BigNumberFacilitator("0");
            for (int idx = number.Length, offset = 0; --idx != -1; ++offset)
            {
                int digit = number[idx] - '0';
                string product;
                switch (digit)
                {
                    case 0:
                        product = "0";
                        break;
                    case 1:
                        product = d_number;
                        break;
                    default:
                        product = multiplyNumber(d_number, digit, 0,
                            d_number.Length - 1);
                        break;
                }
                if (product != "0")
                    product = appendZeros(product, offset);

                result.add(product);
            }
            d_number = result.getNumber();
        }

        private string multiplyNumber(string num, int digit,
            int remainder, int position)
        {
            if (position == -1)
            {
                if (remainder == 0)
                    return num;

                return num.Insert(0, remainder.ToString());
            }
            int n = num[position] - '0';
            int product = n * digit + remainder;
            remainder = product / 10;

            num = setNumber(num, (char)('0' + (product % 10)), position);
            return multiplyNumber(num, digit, remainder, position - 1);
        }

        private string addNumbers(string num, string addition,
            int remainder, int position)
        {
            if (position == -1)
            {
                if (remainder == 0)
                    return num;

                return num.Insert(0, remainder.ToString());
            }

            int a = addition[position] - '0';
            int n = num[position] - '0';

            int sum = a + n + remainder;
            remainder = sum / 10;
            int res = sum % 10;

            num = setNumber(num, (char)('0' + res), position);

            return addNumbers(num, addition, remainder, position - 1);
        }

        private string setNumber(string bigNumber, char number, int pos)
        {
            StringBuilder builder = new StringBuilder(bigNumber);
            builder[pos] = number;
            return builder.ToString();
        }

        private string prependZeros(string str, int count)
        {
            for (int idx = 0; idx < count; ++idx)
            {
                str = str.Insert(0, "0");
            }
            return str;
        }

        private string appendZeros(string str, int count)
        {
            for (int idx = 0; idx < count; ++idx)
            {
                str += "0";
            }
            return str;
        }
    }
}
