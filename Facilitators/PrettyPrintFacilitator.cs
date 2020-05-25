using System;

namespace indyClient
{
    class PrettyPrintFacilitator
    {
        public string dePrettyJsonMember(string json, string member)
        {
            int startIdx = json.IndexOf(member);
            int openIdx = json.IndexOf('{', startIdx);
            int openIdx2 = openIdx + 1;
            int closeIdx = openIdx2;

            while (true)
            {
                closeIdx = json.IndexOf('}', closeIdx);
                // check if there is an object within the object
                openIdx2 = json.IndexOf('{', openIdx2);

                Console.WriteLine("close: " + closeIdx);
                Console.WriteLine("open: " + openIdx2);
                if (openIdx2 == -1 ^ openIdx2 > closeIdx)
                    break;

                closeIdx++;
                openIdx2++;
            }

            string begin = json.Substring(0, openIdx);
            Console.WriteLine("begin:  \n" + begin);
            string sub = json.Substring(openIdx, closeIdx - openIdx + 1);
            Console.WriteLine("sub:  \n" + sub);
            string end = json.Substring(closeIdx + 1);
            Console.WriteLine("end:  \n" + end);

            sub = sub.Replace(" ", "");
            sub = sub.Replace("\n", "");
            return begin + sub + end;
        }

    }
}
