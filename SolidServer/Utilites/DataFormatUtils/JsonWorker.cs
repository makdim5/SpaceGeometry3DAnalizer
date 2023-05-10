using Newtonsoft.Json;
using SolidServer.SolidWorksPackage.ResearchPackage;
using System;
using System.Collections.Generic;
using System.IO;

namespace SolidServer.Utitlites
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

        public static Dictionary<string, string> LoadData()
        {
            try
            {
                using (StreamReader reader = new StreamReader(SETTINGS_PATH))
                {

                    string json = "";
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        json += line;

                    return json!= ""?
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(json):
                        new Dictionary<string, string>();
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(SETTINGS_PATH).Close();
                return new Dictionary<string, string>();
            }

        }

        public static string SerializeNodesToPointsInJSON(IEnumerable<Node> nodes)
        {
            List<Point3D> pointsJson = new();

            foreach (var node in nodes)
            {
                pointsJson.Add(node.point);

            }

            return JsonConvert.SerializeObject(pointsJson);
        }

    }


}
