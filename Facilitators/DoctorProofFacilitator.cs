using System;
using Newtonsoft.Json;


namespace indyClient
{
    class DoctorProofFacilitator
    {

        public string getProofRequest()
        {
            DoctorProofModel model = new DoctorProofModel();
            return JsonConvert.SerializeObject(model);
        }
    }
}
