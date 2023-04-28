using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Serialiser;
using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;
using BH.oM.Base;
using BH.Engine.Test;
using Microsoft.VisualBasic;
using BH.oM.UnitTest.Results;
using BH.oM.Test.Results;
using Bogus;
using System.IO;
using ICSharpCode.Decompiler.TypeSystem;

namespace BH.Tests.Engine.Serialiser
{
    public class Versioning : BaseLoader
    {


        [Test]
        public void A_VersioningFromJson()
        {

            List<string> versions = new List<string> { "6.1", "6.0", "5.3", "5.2", "5.1", "5.0", "4.3", "4.2", "4.1", "4.0", "3.3" };

            //List<string> versions = new List<string> { "6.1" };
            Dictionary<string, int> passes = new Dictionary<string, int>();
            Dictionary<string, int> failures = new Dictionary<string, int>();
            Dictionary<string, List<string>> jsonErrors = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> jsonSuccess = new Dictionary<string, List<string>>();

            string type = "Objects";

            string filePath = Helpers.TemporaryLogPath($"{type}.json", true);

            Stopwatch sw = Stopwatch.StartNew();

            foreach (string  version in versions)
            {
                string file = $@"C:\ProgramData\BHoM\Datasets\TestSets\Versioning\{version}\{type}.json";
                string exceptions = "Grasshopper|Rhinoceros";

                IEnumerable<string> json = System.IO.File.ReadAllLines(file).Where(x => !string.IsNullOrWhiteSpace(x) && (exceptions.Length == 0 || !Regex.IsMatch(x, exceptions)));

                int pass = 0;
                int fail = 0;
                List<string> errors = new List<string>();
                List<string> successes = new List<string>();

                foreach (string line in json)
                {
                    bool success = true;
                    try
                    {
                        BH.Engine.Base.Compute.ClearCurrentEvents();
                        object obj = BH.Engine.Serialiser.Convert.FromJson(line);
                        if ((obj == null || obj is CustomObject))
                        {
                            bool detected = BH.Engine.Base.Query.CurrentEvents().Any(x => x.Message.StartsWith("No upgrade for"));

                            if (!detected)
                            {
                                if (type == "Methods")
                                {
                                    if(!Helpers.CanReplaceMethodWithType(line))
                                        success = false;
                                }
                                else
                                    success = false;
                                
                            }
                        }
                    }
                    catch
                    {
                        success = false;
                    }

                    if (success)
                    {
                        pass++;
                        if(line.Contains("BH.Revit"))
                            successes.Add(line);
                    }
                    else
                    {
                        errors.Add(line);
                        fail++;
                    }
                }

                jsonSuccess[version] = successes;
                passes[version] = pass;
                failures[version] = fail;
                jsonErrors[version] = errors;

                File.AppendAllLines(filePath, new List<string> { $"BHoMVersion: {version}" });
                File.AppendAllLines(filePath, errors);
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

        }


        [Test]
        public void B_VersioningFromJsonFailures()
        {
            string type = "Objects";

            string file = Helpers.TemporaryLogPath($"{type}.json", false);
            string exceptions = "Grasshopper|Rhinoceros";

            IEnumerable<string> json = System.IO.File.ReadAllLines(file).Where(x => !string.IsNullOrWhiteSpace(x) && (exceptions.Length == 0 || !Regex.IsMatch(x, exceptions)));

            int pass = 0;
            int fail = 0;
            List<string> errors = new List<string>();

            string filePath = Helpers.TemporaryLogPath($"{type}Details.json", true);

            foreach (string line in json)
            {
                if (line.StartsWith("BHoMVersion"))
                {
                    File.AppendAllLines(filePath, new List<string> { line, "" });
                    continue;
                }

                List<string> messages = new List<string>();

                bool success = true;
                try
                {
                    BH.Engine.Base.Compute.ClearCurrentEvents();
                    object obj = BH.Engine.Serialiser.Convert.FromJson(line);
                    if (obj == null || obj is CustomObject)
                    {
                        bool detected = BH.Engine.Base.Query.CurrentEvents().Any(x => x.Message.StartsWith("No upgrade for"));
                        success = detected;
                    }
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordError(ex);
                    success = false;
                }

                if (success)
                    pass++;
                else
                {
                    errors.Add(line);
                    fail++;
                }

                if (!success)
                {
                    messages.Add(line);
                    messages.Add("");
                    messages.AddRange(BH.Engine.Base.Query.CurrentEvents().Select(x => x.Message));
                    messages.Add("");
                    messages.Add("");
                    File.AppendAllLines(filePath, messages);
                }
            }

        }

        [Test]
        public void A_VersioningFromJsonCompareOldAndNew()
        {
            List<string> versions = new List<string> { "6.1", "6.0", "5.3", "5.2", "5.1", "5.0", "4.3", "4.2", "4.1", "4.0", "3.3" };

            //List<string> versions = new List<string> { "6.1" };
            Dictionary<string, int> passes = new Dictionary<string, int>();
            Dictionary<string, int> failures = new Dictionary<string, int>();
            Dictionary<string, List<string>> jsonErrors = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> jsonSuccess = new Dictionary<string, List<string>>();

            string type = "Objects";

            Stopwatch sw = Stopwatch.StartNew();

            string filePath = Helpers.TemporaryLogPath($"{type}_OldNewCompare.json", true);

            foreach (string version in versions)
            {
                string file = $@"C:\ProgramData\BHoM\Datasets\TestSets\Versioning\{version}\{type}.json";
                string exceptions = "Grasshopper|Rhinoceros|\"_t\" : \"Autodesk.Revit";

                IEnumerable<string> json = System.IO.File.ReadAllLines(file).Where(x => !string.IsNullOrWhiteSpace(x) && (exceptions.Length == 0 || !Regex.IsMatch(x, exceptions)));

                int pass = 0;
                int fail = 0;
                List<string> errors = new List<string>();
                List<string> successes = new List<string>();

                foreach (string line in json)
                {
                    bool success = true;
                    try
                    {
                        BH.Engine.Base.Compute.ClearCurrentEvents();
                        object obj = BH.Engine.Serialiser.Convert.FromJson(line);

                        bool detected = BH.Engine.Base.Query.CurrentEvents().Any(x => x.Message.StartsWith("No upgrade for"));
                        if (detected)
                            continue;

                        object objOld = BH.Engine.Serialiser.Convert.FromOldJson(line);
                        success = obj.IsEqual(objOld);
                    }
                    catch
                    {
                        success = false;
                    }

                    if (success)
                    {
                        pass++;
                        successes.Add(line);
                    }
                    else
                    {
                        errors.Add(line);
                        fail++;
                    }
                }

                jsonSuccess[version] = successes;
                passes[version] = pass;
                failures[version] = fail;
                jsonErrors[version] = errors;

                File.AppendAllLines(filePath, new List<string>{ $"BHoMVersion: {version}" });
                File.AppendAllLines(filePath, errors);
            }

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void B_CheckFailuresNewVsOld()
        {
            string type = "Objects";

            string file = Helpers.TemporaryLogPath($"{type}_OldNewCompare.json", false);
            IEnumerable<string> json = System.IO.File.ReadAllLines(file).Where(x => !string.IsNullOrWhiteSpace(x));

            string filePath = Helpers.TemporaryLogPath($"{type}_OldNewCompareDiff.json", true);

            int pass = 0;
            int fail = 0;
            List<string> errors = new List<string>();
            HashSet<Type> handledTypes = new HashSet<Type>();
            foreach (string line in json)
            {
                if (line.StartsWith("BHoMVersion"))
                    continue;

                bool success = true;
                string errorMessage = "";
                try
                {
                    BH.Engine.Base.Compute.ClearCurrentEvents();
                    object obj = BH.Engine.Serialiser.Convert.FromJson(line);

                    if (obj != null)
                    {
                        if (handledTypes.Contains(obj.GetType()))
                        {
                            continue;
                        }
                        else
                            handledTypes.Add(obj.GetType());
                    }

                    object objOld = BH.Engine.Serialiser.Convert.FromOldJson(line);

                    var diffResult = BH.Engine.Test.Query.IsEqual(objOld, obj, null);

                    if (diffResult.Item1)
                        continue;
                    TestResult testRes = new TestResult();

                    for (int j = 0; j < diffResult.Item2.Count; j++)
                    {
                        testRes.Information.Add(new ComparisonDifference()
                        {
                            Property = diffResult.Item2[j],
                            ReferenceValue = diffResult.Item3[j],
                            RunValue = diffResult.Item4[j],
                            Status = oM.Test.TestStatus.Error,
                        });
                    }
                    testRes.Description = obj?.GetType().Name ?? objOld?.GetType().Name ?? "";
                    testRes.Message = $"Old type: {objOld?.GetType().Name ?? ""}. New type: {obj?.GetType().Name ?? ""}";
                    errorMessage =testRes.FullMessage();
                }
                catch
                {
                    errorMessage = $"CRASH!!!!!!!!!{line}";
                }

                errors.Add(errorMessage);

                if(!string.IsNullOrEmpty(errorMessage))
                    File.AppendAllLines(filePath, new List<string> { errorMessage });
            }

            
        }




    }



}
