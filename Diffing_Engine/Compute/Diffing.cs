using BH.oM.Base;
using BH.Engine;
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
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Testing;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        /// Diffing requires to memorize the hash of the object somewhere.
        /// If using BHoMObjects, we can save the hash inside the objects themselves.
        /// Otherwise another support is needed (table with | object | Hash | )

        public static Delta Diffing(string projectName, List<IBHoMObject> currentObjs, List<string> exceptions = null, bool useDefaultExceptions = true)
        {
            // Clone the current objects
            List<IBHoMObject> CurrentObjs_cloned = currentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Create project fragment
            DiffProjFragment diffProj = new DiffProjFragment(projectName);

            if (useDefaultExceptions)
                SetDefaultExceptions(ref exceptions);

            // Calculate and set the object hashes
            CurrentObjs_cloned.ForEach(obj =>
                obj.Fragments.Add(
                    new DiffHashFragment(Compute.SHA256Hash(obj, exceptions), diffProj)
                    ));

            return new Delta(diffProj, CurrentObjs_cloned, null, null, null);
        }

        public static Delta Diffing(List<IBHoMObject> currentObjs, List<IBHoMObject> readObjs, bool propertyLevelDiffing = true, List<string> exceptions = null, bool useDefaultExceptions = true)
        {
            // Clone the objects to assure immutability
            List<IBHoMObject> CurrentObjs_cloned = currentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();
            List<IBHoMObject> ReadObjs_cloned = readObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Get project fragment from one of the objects and use it as the base
            DiffProjFragment diffProj = ReadObjs_cloned
                .Where(obj => obj.Fragments.Exists(fragm => fragm?.GetType() == typeof(DiffHashFragment)))
                .First()
                .GetHashFragment().DiffingProject;

            if (useDefaultExceptions)
                SetDefaultExceptions(ref exceptions);

            // Check and process the DiffHashFragment of the objects
            CurrentObjs_cloned.ForEach(obj =>
            {
                DiffHashFragment hashFragment = obj.GetHashFragment();

                if (hashFragment == null)
                    // Current objs may not have any DiffHashFragment if they have been created new, or if their modification was done through reinstantiating them.
                    // We need to calculate their hash for the first time, and add to them a DiffHashFragment with that hash. 
                    obj.Fragments.Add(new DiffHashFragment(Compute.SHA256Hash(obj, exceptions), diffProj));
                else
                {
                    // Calculate and set the new object hash, keeping track of its old hash
                    string previousHash = hashFragment.Hash;
                    string newHash = Compute.SHA256Hash(obj, exceptions);

                    obj.Fragments[obj.Fragments.IndexOf(hashFragment)] = new DiffHashFragment(newHash, previousHash, diffProj);
                }
            });

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> toBeCreated = new List<IBHoMObject>();
            List<string> toBeCreated_hashes = new List<string>();

            List<IBHoMObject> toBeUpdated = new List<IBHoMObject>();
            List<string> toBeUpdated_hashes = new List<string>();

            List<IBHoMObject> toBeDeleted = new List<IBHoMObject>();
            List<string> toBeDeleted_hashes = new List<string>();

            List<IBHoMObject> unchanged = new List<IBHoMObject>();
            List<string> unchanged_hashes = new List<string>();

            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (var obj in CurrentObjs_cloned)
            {
                var hashFragm = obj.GetHashFragment();

                if (hashFragm?.PreviousHash == null)
                {
                    toBeCreated.Add(obj); // It's a new object
                }

                else if (hashFragm.PreviousHash == hashFragm.Hash)
                {
                    unchanged.Add(obj); // It's NOT been modified
                    unchanged_hashes.Add(hashFragm.Hash);
                }

                else if (hashFragm.PreviousHash != hashFragm.Hash)
                {
                    toBeUpdated.Add(obj); // It's been modified
                    toBeUpdated_hashes.Add(hashFragm.Hash);

                    if (propertyLevelDiffing)
                    {
                        // Determine changed properties
                        var oldObjState = ReadObjs_cloned.Single(rObj => rObj.GetHashFragment().Hash == hashFragm.PreviousHash);

                        IsEqualConfig ignoreProps = new IsEqualConfig();
                        ignoreProps.PropertiesToIgnore = exceptions;
                        var differentProps = BH.Engine.Testing.Query.IsEqual(obj, oldObjState, ignoreProps);
                        var differentProps1 = BH.Engine.Testing.Query.DifferentProperties(obj, oldObjState, ignoreProps);
                        var changes = new Tuple<List<string>, List<string>>(differentProps.Item2, differentProps.Item3);

                        objModifiedProps.Add(hashFragm.Hash, differentProps1);
                    }
                }

                else
                {

                    BH.Engine.Reflection.Compute.RecordError("Could not find hash information to perform Diffing on some objects.");
                    return null;
                }
            }

            foreach (var obj in ReadObjs_cloned)
            {
                var hashFragm = obj.GetHashFragment();

                if (!CurrentObjs_cloned.Any(cObj => cObj.GetHashFragment().PreviousHash == hashFragm.Hash))
                {
                    toBeDeleted.Add(obj); // It doesn't exist anymore (it's not among the current objects)
                    toBeDeleted_hashes.Add(hashFragm.Hash);
                    continue;
                }
            }

            return new Delta(diffProj, toBeCreated, toBeCreated_hashes, toBeDeleted, toBeDeleted_hashes, toBeUpdated, toBeUpdated_hashes, unchanged, unchanged_hashes, objModifiedProps);
        }

        /***************************************************/


        private static void SetDefaultExceptions(ref List<string> exceptions)
        {
            List<string> defaultExceptions = new List<string>() { "BHoM_Guid", "CustomData", "Fragments" };

            if (exceptions == null)
                exceptions = defaultExceptions;
            else
                exceptions.AddRange(defaultExceptions);
        }
    }
}
