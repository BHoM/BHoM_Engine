using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diffing_Engine
{
    public static partial class Compute
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/
        /// Diffing requires to memorize the hash of the object somewhere.
        /// If using BHoMObjects, we can save the hash inside the objects themselves.
        /// Otherwise another support is needed (table with | object | Hash | )

        public static Delta Diffing(string projectName, List<IBHoMObject> CurrentObjs)
        {
            // Clone the current objects
            List<IBHoMObject> CurrentObjs_cloned = CurrentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Create project fragment
            DiffProjFragment diffProj = new DiffProjFragment(projectName);

            // Define exceptions (will be exposed as parameter)
            List<string> exceptions = new List<string>() { "BHoM_Guid" };

            // Calculate and set the object hashes
            CurrentObjs_cloned.ForEach(obj => 
                obj.Fragments.Add(
                    new DiffHashFragment(Compute.SHA256Hash(obj, exceptions), diffProj)
                    ));

            //CurrentObjs_cloned.ForEach(obj => obj.CustomData.Add("hash", Compute.SHA256Hash(obj, exceptions))); //- Alternative using customData

            return new Delta(diffProj, CurrentObjs_cloned);
        }

        public static Delta Diffing(List<IBHoMObject> CurrentObjs, List<IBHoMObject> ReadObjs)
        {
            // Clone the current objects
            List<IBHoMObject> CurrentObjs_cloned = CurrentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Get project fragment from one of the objects
            DiffProjFragment diffProj = CurrentObjs_cloned[0].Fragments.OfType<DiffHashFragment>().First().DiffingProject;

            // Define exceptions (will be exposed as parameter)
            List<string> exceptions = new List<string>() { "BHoM_Guid" };

            // Calculate and set the object hashes
            CurrentObjs_cloned.ForEach(obj =>
                obj.Fragments.Add(
                    new DiffHashFragment(Compute.SHA256Hash(obj, exceptions), diffProj)
                    ));

            List<IBHoMObject> newObjects = new List<IBHoMObject>();
            List<string> newObjects_hashes = new List<string>();

            List<IBHoMObject> toBeUpdated = new List<IBHoMObject>();
            List<string> toBeUpdated_hashes = new List<string>();

            List<IBHoMObject> toBeDeleted = new List<IBHoMObject>();
            List<string> toBeDeleted_hashes = new List<string>();

            List<IBHoMObject> readObjs_cloned = ReadObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // TODO: here we might have current objects that have been created for the first time.
            // We need to apply to them the same Project fragment that the others have, and calculate their hashes too.

            CurrentObjs
                .Where(c => 
                    !ReadObjs.Any(r => r.Fragments.OfType<DiffHashFragment>().First().Hash == c.Fragments.OfType<DiffHashFragment>().First().Hash)).ToList()
                .ForEach(obj =>
                {
                    newObjects.Add(obj);
                    newObjects_hashes.Add(obj.Fragments.OfType<DiffHashFragment>().First().Hash);
                });

            ReadObjs
                .Where(r => 
                    !CurrentObjs.Any(c => r.Fragments.OfType<DiffHashFragment>().First().Hash == c.Fragments.OfType<DiffHashFragment>().First().Hash)).ToList()
                .ForEach(obj =>
                {
                    toBeDeleted.Add(obj);
                    toBeDeleted_hashes.Add(obj.Fragments.OfType<DiffHashFragment>().First().Hash);
                });

            ReadObjs
               .Where(r => 
                    CurrentObjs.Any(c => r.Fragments.OfType<DiffHashFragment>().First().Hash == c.Fragments.OfType<DiffHashFragment>().First().Hash)).ToList()
               .ForEach(obj =>
               {
                   toBeUpdated.Add(obj);
                   toBeUpdated_hashes.Add(obj.Fragments.OfType<DiffHashFragment>().First().Hash);
               });

            return new Delta(diffProj, newObjects, newObjects_hashes, toBeDeleted, toBeDeleted_hashes, toBeUpdated, toBeUpdated_hashes);
        }

        ///***************************************************/

    }
}
