/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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
using BH.oM.Base;
using BH.oM.Graphics;

namespace BH.Tests.Engine.Serialiser
{
    [Ignore("Ignoring NUnit versioning tests as overlapping with centralised check Serialisation. Test kept for debugging purposes.")]
    public class ToFromJsonTest : BaseLoader
    {
        [Test]
        public void ToFromJson()
        {
            List<object> bars = new List<object>();
            for (int i = 0; i < 800; i++)
            {
                bars.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));
                bars.Add(BH.Engine.Base.Create.RandomObject(typeof(Panel)));
            }

            Console.WriteLine($"{bars.Count} number of Bars");

            BH.Engine.Serialiser.Convert.FromOldJson(bars[0].ToOldJson());

            Stopwatch sw = Stopwatch.StartNew();

            List<string> json = bars.Select(x => x.ToOldJson()).ToList();

            sw.Stop();
            Console.WriteLine($"Time to json OLD: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            List<object> objects = json.Select(x => BH.Engine.Serialiser.Convert.FromOldJson(x)).ToList();

            sw.Stop();
            Console.WriteLine($"Time from json OLD: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            List<string> json2 = bars.Select(x => x.ToJson()).ToList();

            sw.Stop();
            Console.WriteLine($"Time to json NEW: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            List<object> objects2 = json2.Select(x => BH.Engine.Serialiser.Convert.FromJson(x)).ToList();

            sw.Stop();
            Console.WriteLine($"Time from json NEW: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            json = bars.Select(x => x.ToOldJson()).ToList();

            sw.Stop();
            Console.WriteLine($"Time to json OLD: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            objects = json.Select(x => BH.Engine.Serialiser.Convert.FromOldJson(x)).ToList();

            sw.Stop();
            Console.WriteLine($"Time from json OLD: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            json2 = bars.Select(x => x.ToJson()).ToList();

            sw.Stop();
            Console.WriteLine($"Time to json NEW: {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            objects2 = json2.Select(x => BH.Engine.Serialiser.Convert.FromJson(x)).ToList();

            sw.Stop();
            Console.WriteLine($"Time from json NEW: {sw.ElapsedMilliseconds} ms");
        }

        [Test]
        public void ToFromJsonAllTypes()
        {

            int pass = 0;
            int fail = 0;
            List<string> failures = new List<string>();

            foreach (Type type in BH.Engine.Base.Query.BHoMTypeList())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                try
                {
                    object dummy = BH.Engine.Test.Compute.DummyObject(type);
                    string json = dummy.ToJson();
                    object back = BH.Engine.Serialiser.Convert.FromJson(json);

                    if (BH.Engine.Test.Query.IsEqual(dummy, back))
                        pass++;
                    else
                    {
                        fail++;
                        failures.Add(type.FullName);
                    }

                }
                catch (Exception)
                {
                    fail++;
                    failures.Add(type.FullName);
                }


            }

            foreach (string f in failures.Distinct())
            {
                Console.WriteLine(f);
            }
        }


    }
}

