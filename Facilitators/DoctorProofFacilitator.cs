using System;
using System.IO;
using Newtonsoft.Json;


namespace indyClient
{
    class DoctorProofFacilitator
    {

        public string getProofRequest()
        {
            return File.ReadAllText("Models/DoctorProofRequest.json");
        }
    }
}
