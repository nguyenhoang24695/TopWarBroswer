using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBroswer.Util
{
    class FileUtil
    {
        public static List<T> GetListTextFromFileToEntity<T>(string path)
        {

            var rawText = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<T>>(rawText);
        }

        public static List<string> GetListTextFromFile(string path)
        {

            var rawText = File.ReadAllLines(path).ToList();
            return rawText;
        }

        public static void WriteJsonToFile<T>(T tObject, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(tObject, Formatting.Indented));
        }
    }
}
