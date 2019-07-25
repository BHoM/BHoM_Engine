using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diffing_Engine.Test
{
    public static class TestHashing_Json
    {
        public static void Run(List<object> objs = null)
        {
            if (objs == null)
            {
                objs = new List<object>();
                List<string> listStringHashes = new List<string>();


                Bar bar = new Bar();
                Bar bar1 = new Bar();

                bar.Name = "bar";
                objs.Add(bar);
                bar1.Name = "bar1";
                objs.Add(bar1);

                TestclassA point = new TestclassA();
                point.X = 10;
                point.Y = 10;
                TestclassB coord = new TestclassB();
                coord.X = 10;
                coord.Y = 10;

                objs.Add(point);
                objs.Add(coord);

                //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(BH.oM.Geometry.Point)));

                //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));
                //objs.Add(BH.Engine.Base.Create.RandomObject(typeof(Bar)));

            }


            for (int i = 0; i < objs.Count; i++)
            {
                var json = objs[i].ToDiffingJson(new List<string>() { "BHoM_Guid" });
                //var json = objs[i].PrepareJson(typeof(BH.oM.Base.BHoMObject).GetProperties());

                var hashString = Compute.SHA256Hash(json);
                Console.WriteLine(objs[i].GetType().Name + ": " + hashString);
            }

        }

    }
}

