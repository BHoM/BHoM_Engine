using BH.Adapter.File;
using BH.Engine.Serialiser;
using BH.oM.Adapter;
using BH.oM.Test.Results;
using System;
using System.Collections.Generic;

namespace RunNullCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            //-----------------------------------------------------//
            //------- Uncomment the checks you want to run. -------//
            //-----------------------------------------------------//

            List<Func<TestResult>> checksToRun = new List<Func<TestResult>>();
            //checksToRun.Add(BH.Test.Engine.Verify.NullChecks);
            //checksToRun.Add(BH.Test.Serialiser.Verify.MethodsToFromJson);
            //checksToRun.Add(BH.Test.Serialiser.Verify.ObjectsToFromJson);
            //checksToRun.Add(BH.Test.Serialiser.Verify.TypesToFromJson);

            //-----------------------------------------------------//

            if (checksToRun.Count == 0)
            {
                Console.WriteLine("No checks have been specified to run. Please uncomment the required checks in the Program.cs file.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
                return;
            }

            string filePath = "";
            try
            {
                filePath = args[0];
            }
            catch(Exception e)
            {
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Path information has not been specified in the application arguments.");
                Console.WriteLine("Go to project properties => debug => application arguments and paste the path there.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Loading assemblies started.");
            BH.Engine.Reflection.Compute.LoadAllAssemblies();
            Console.WriteLine("Loading assemblies finished.");

            Console.WriteLine("Execution of tests started.\n");
            List<TestResult> results = new List<TestResult>();
            foreach (Func<TestResult> del in checksToRun)
            {
                try
                {
                    results.Add(del.Invoke());
                    Console.WriteLine($"{del.Method.DeclaringType.FullName}.{del.Method.Name} run succeeded.");
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{del.Method.DeclaringType.FullName}.{del.Method.Name} run failed with the following error: {e.Message}");
                }
            }

            Console.WriteLine("\nExecution of tests finished.");

            // Push the outputs to the filepath specified in project properties.
            Console.WriteLine("Push to file started.");
            FileAdapter fa = new FileAdapter(filePath);
            fa.Push(results, "", PushType.DeleteThenCreate, new BH.oM.Adapters.File.PushConfig { BeautifyJson = false });
            Console.WriteLine("Push to file finished.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
