using Newtonsoft.Json.Linq;

namespace Axe.Extended
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //using newtonsoft.json, read the file `axe-rgaa-extension-empty.json` and parse it into a JObject called axeConfig
            var axeConfig = JObject.Parse(File.ReadAllText("axe-rgaa-extension-empty.json"));

            //the file has two array, checks and rules. we will read rules and checks from the files in the folder
            var rules = axeConfig["rules"] as JArray;
            var checks = axeConfig["checks"] as JArray;

            //read each .json files in the folder `rules` and add it to the rules array
            foreach (var file in Directory.GetFiles("rules", "*.json"))
            {
                var rule = JObject.Parse(File.ReadAllText(file));
                rules!.Add(rule);
            }

            const string checksFolder = "checks";

            //read each .json files in the folder `checks` and add it to the checks array
            foreach (var file in Directory.GetFiles(checksFolder, "*.json"))
            {
                var check = JObject.Parse(File.ReadAllText(file));
                //in the check object, there is a property called "evaluate", which can be in a pattern of "${filename.js}".
                //if thats the case, read the content of the file and replace the value of "evaluate" with the content of the file
                if (check["evaluate"] is JValue evaluate && evaluate.Value is string evaluateString && evaluateString.StartsWith("${") && evaluateString.EndsWith("}"))
                {
                    var evaluateFile = evaluateString[2..^1];
                    var evaluateContent = File.ReadAllText(Path.Combine(checksFolder, evaluateFile));
                    check["evaluate"] = evaluateContent;
                }


                //in check object, there may have a property called "after", which can be in a pattern of "${filename.js}".
                //if thats the case, read the content of the file and replace the value of "after" with the content of the file
                if (check["after"] is JValue after && after.Value is string afterString && afterString.StartsWith("${") && afterString.EndsWith("}"))
                {
                    var afterFile = afterString[2..^1];
                    var afterContent = File.ReadAllText(Path.Combine(checksFolder, afterFile));
                    check["after"] = afterContent;
                }

                checks!.Add(check);
            }

            //write the new axeconfig.json file
            File.WriteAllText("../../../../Axe.Extended.HtmlReport/Assets/axe-rgaa-extension.json", axeConfig.ToString());

        }
    }
}