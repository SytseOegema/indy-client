using System;
using Newtonsoft.Json;


namespace indyClient
{
    class WalletImportConfig
    {
        public string command_handle;
        public string pool_name;
        public string name;
        public string storage_type;
        public string config;
        public string credentials;
        public string import_config_json;


        public WalletImportConfig(string pool_name,
                                  string name,
                                  string storage_type,
                                  string credential_key,
                                  string file_path,
                                  string export_key)
            :this("i32",
                  pool_name,
                  name,
                  storage_type,
                  "",
                  "",
                  "{ \"path\":" + file_path + ", \"key\":" + export_key + "}")
        {}

        public WalletImportConfig(string command_handle,
                                  string pool_name,
                                  string name,
                                  string storage_type,
                                  string config,
                                  string credentials,
                                  string import_config_json)
        {
            this.command_handle = command_handle;
            this.pool_name = pool_name;
            this.name = name;
            this.storage_type = storage_type;
            this.config = config;
            this.credentials = credentials;
            this.import_config_json = import_config_json;
        }

        public string toJson()
        {
            Console.WriteLine(this.import_config_json);
            return JsonConvert.SerializeObject(this);
        }
    }
}
