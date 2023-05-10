using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Serialiser;
using NUnit.Framework;
using System.Diagnostics;
using Bogus.Bson;
using System.Linq.Expressions;
using BH.Engine.Test;
using BH.oM.UnitTest.Results;
using BH.oM.Test.Results;
using System.IO;
using System.Text.RegularExpressions;

namespace BH.Tests.Engine.Serialiser
{
    [Ignore("Ignoring NUnit versioning tests as only relevant for debugging until old serialisation is removed. Test kept for debugging purposes.")]
    public class OldNewToJson : BaseLoader
    {

        [Test]
        public void A_CompareNewAndOldToJson()
        {

            List<object> dummies = new List<object>();
            foreach (Type type in BH.Engine.Base.Query.BHoMTypeList())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                try
                {
                    object dummy = BH.Engine.Test.Compute.DummyObject(type);
                    if (dummy != null)
                        dummies.Add(dummy);
                }
                catch { }
            }


            int pass = 0;
            int fail = 0;
            List<string> failuresNew = new List<string>();
            List<string> failuresOld = new List<string>();


            List<string> newJson = new List<string>();
            List<string> oldJson = new List<string>();

            Stopwatch sw = Stopwatch.StartNew();

            foreach (object dummy in dummies)
            {
                string jsonNew;
                try
                {
                    jsonNew = dummy.ToJson();

                }
                catch (Exception)
                {
                    jsonNew = "";
                }
                //newJson.Add(jsonNew);
            }

            sw.Stop();

            sw.Restart();

            foreach (object dummy in dummies)
            {
                string jsonNew;
                try
                {
                    jsonNew = dummy.ToJson();

                }
                catch (Exception)
                {
                    jsonNew = "";
                }
                newJson.Add(jsonNew);
            }

            sw.Stop();
            Console.WriteLine($"ToJson new: {sw.Elapsed.ToString()}. Failures: {newJson.Where(x => string.IsNullOrEmpty(x)).Count()}");

            sw.Restart();

            foreach (object dummy in dummies)
            {
                string jsonOld;
                try
                {
                    jsonOld = dummy.ToOldJson();

                }
                catch (Exception)
                {
                    jsonOld = "";
                }
                oldJson.Add(jsonOld);
            }

            sw.Stop();
            Console.WriteLine($"ToJson old: {sw.Elapsed.ToString()}. Failures: {oldJson.Where(x => string.IsNullOrEmpty(x)).Count()}");

            List<object> newBack = new List<object>();
            List<object> oldBack = new List<object>();

            sw.Restart();
            foreach (string json in newJson)
            {
                object obj;
                try
                {
                    obj = BH.Engine.Serialiser.Convert.FromJson(json);

                }
                catch (Exception)
                {
                    obj = null;
                }
                newBack.Add(obj);
            }

            sw.Stop();
            Console.WriteLine($"FromJson new: {sw.Elapsed.ToString()}. Failures: {newBack.Where(x => x == null).Count()}");

            sw.Restart();
            foreach (string json in oldJson)
            {
                object obj;
                try
                {
                    obj = BH.Engine.Serialiser.Convert.FromOldJson(json);

                }
                catch (Exception)
                {
                    obj = null;
                }
                oldBack.Add(obj);
            }

            sw.Stop();
            Console.WriteLine($"FromJson old: {sw.Elapsed.ToString()}. Failures: {oldBack.Where(x => x == null).Count()}");

            int failures = 0;
            int success = 0;
            List<string> failureJson = new List<string>();



            List<string> failingTypes = new List<string>();

            for (int i = 0; i < newBack.Count; i++)
            {
                bool equal;
                string errorMessage = "";
                try
                {
                    var diffResult = BH.Engine.Test.Query.IsEqual(oldBack[i], newBack[i], null);

                    if (equal = diffResult.Item1)
                    {
                        success++;
                        continue;
                    }
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
                    testRes.Description = newBack[i]?.GetType().FullName ?? oldBack[i]?.GetType().FullName ?? "";
                    testRes.Message = $"Old type: {oldBack[i]?.GetType().Name ?? ""}. New type: {newBack[i]?.GetType().Name ?? ""}";
                    errorMessage = testRes.FullMessage();
                }
                catch (Exception)
                {
                    errorMessage = $"Compare crash for: {newBack[i]?.GetType().FullName ?? oldBack[i]?.GetType().FullName ?? ""}";
                    equal = false;
                }

                if (equal)
                    success++;
                else
                {
                    failingTypes.Add(newBack[i]?.GetType().FullName ?? oldBack[i]?.GetType().FullName ?? "");
                    failureJson.Add(errorMessage);
                    failures++;
                }
            }

            System.IO.File.WriteAllLines(Helpers.TemporaryLogPath($"ToFromJsonOldVsNew.json", true), failingTypes.Distinct());

            Console.WriteLine($"Equal: {success}, Failure: {failures}");
            foreach (string f in failureJson)
            {
                Console.WriteLine(f);
            }
        }

        [Test]
        public void B_CompareNewAndOldToJson()
        {

            string file = Helpers.TemporaryLogPath($"ToFromJsonOldVsNew.json", false);

            List<TestResult> results = new List<TestResult>();

            foreach (string line in File.ReadAllLines(file))
            {
                try
                {
                    Type type = BH.Engine.Base.Create.Type(line);
                    object dummy = BH.Engine.Test.Compute.DummyObject(type);

                    string jsonNew = dummy.ToJson();
                    string jsonOld = dummy.ToOldJson();

                    object newBack = BH.Engine.Serialiser.Convert.FromJson(jsonNew);
                    object oldBack = BH.Engine.Serialiser.Convert.FromOldJson(jsonOld);

                    var diffResult = BH.Engine.Test.Query.IsEqual(oldBack, newBack, null);

                    if (diffResult.Item1)
                    {
                        continue;
                    }

                    if (BH.Engine.Test.Query.IsEqual(dummy, newBack, null).Item1)
                    {
                        continue;
                    }

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
                    testRes.Description = newBack?.GetType().FullName ?? oldBack?.GetType().FullName ?? "";
                    testRes.Message = $"Old type: {oldBack?.GetType().Name ?? ""}. New type: {newBack?.GetType().Name ?? ""}";
                    results.Add(testRes);
                }
                catch (Exception)
                {

                }

            }

            File.WriteAllLines(Helpers.TemporaryLogPath("ToFromJsonOldVsNewDetails.Json", true), results.Select(x => x.FullMessage()));
        }


    }
}
