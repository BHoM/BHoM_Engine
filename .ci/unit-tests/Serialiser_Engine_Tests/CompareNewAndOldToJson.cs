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

namespace BH.Tests.Engine.Base.Query
{
    public class OldNewToJson
    {

        [Test]
        public void CompareNewAndOldToJson()
        {
            BH.Engine.Base.Compute.LoadAllAssemblies();
            //string regexFilter = $"^Revit_.*_20\\d\\d$";
            //BH.Engine.Base.Compute.LoadAllAssemblies("", regexFilter);

            List<object> dummies = new List<object>();
            foreach (Type type in BH.Engine.Base.Query.BHoMTypeList())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                try
                {
                    object dummy = BH.Engine.Test.Compute.DummyObject(type);
                    if(dummy != null)
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

            for (int i = 0; i < newBack.Count; i++)
            {
                if (oldBack[i].IsEqual(newBack[i]))
                    success++;
                else
                {
                    failureJson.Add(newBack[i].GetType().FullName);
                    failures++;
                }
            }

            Console.WriteLine($"Equal: {success}, Failure: {failures}");
            foreach (string f in failureJson)
            {
                Console.WriteLine(f);
            }
        }
    }
}
