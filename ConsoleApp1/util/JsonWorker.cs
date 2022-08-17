using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.util
{
    internal class JsonWorker
    {
        public static string SETTINGS_PATH = AppDomain.CurrentDomain.BaseDirectory + "settings.json";

        public static void SaveData(Dictionary<string, string> data)
        {
            using (StreamWriter writer = new StreamWriter(SETTINGS_PATH, false))
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                writer.WriteLine(json);
            }
        }

        public static Dictionary<string, string>? LoadData()
        {
            try
            {
                using (StreamReader reader = new StreamReader(SETTINGS_PATH))
                {

                    string json = "";
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                        json += line;

                    return json!= ""?
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(json):
                        new Dictionary<string, string>();
                }
            }
            catch (FileNotFoundException ex)
            {
                File.Create(SETTINGS_PATH).Close();
                return new Dictionary<string, string>();
            }

        }

    }


}
