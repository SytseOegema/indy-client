using System;

namespace indyClient
{
    public class DoctorProofModel
    {
        public string nonce = "1242510923874109246129";
        public string name = "Emergency-Doctor-Check";
        public string version = "1.0";
        public string requested_attributes;
        public string requested_predicates;

        public DoctorProofModel()
        {
            requested_attributes = "{";
              requested_attributes += "attr1_referent: ";
              requested_attributes += "{";
                requested_attributes += "name: name";
              requested_attributes += "},";
              requested_attributes += "attr2_referent: ";
              requested_attributes += "{";
                requested_attributes += "name: school,";
                requested_attributes += "restrictions: [{cred_def_id: }]";
              requested_attributes += "},";
            requested_attributes = "}";

            requested_predicates = "{";
              requested_predicates += "predicate1_referent: ";
              requested_predicates += "{";
                requested_predicates += "name: is_emergency_doctor,";
                requested_predicates += "p_type: ==,";
                requested_predicates += "p_value: 1,";
                requested_predicates += "restrictions: [{cred_def_id: }]";
              requested_predicates += "}";
            requested_predicates += "}";
        }
    }
}
