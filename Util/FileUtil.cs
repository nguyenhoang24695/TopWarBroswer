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
    }
}
