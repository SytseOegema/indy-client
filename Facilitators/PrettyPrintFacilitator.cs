using System;

namespace indyClient
{
    class PrettyPrintFacilitator
    {
        public string dePrettyJsonMember(string json, string member)
        {
            int startIdx = json.IndexOf(member);
            while (startIdx != -1)
            {
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

                // split the json in begin, sub, end
                int originalLength = closeIdx - openIdx + 1
                string begin = json.Substring(0, openIdx);
                string sub = json.Substring(openIdx, originalLength);
                string end = json.Substring(closeIdx + 1);


                // remove newlines and spaces
                sub = sub.Replace(" ", "");
                sub = sub.Replace("\n", "");
                json = begin + sub + end;

                // correct the closeIdx
                closeIdx -= originalLength - sub.Length();
                startIdx = json.IndexOf(member, closeIdx);
            }
            return json;
        }

    }
}
