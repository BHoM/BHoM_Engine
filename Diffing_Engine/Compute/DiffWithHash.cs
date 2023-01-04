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
using BH.oM.Base.Attributes;
 
using System.Collections;
using BH.Engine.Base;
using BH.Engine.Base.Objects;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes the diffing using the Hash of the input objects. The hash is computed on all objects every time this method is run.")]
        [Input("pastObjects", "Objects whose creation predates 'currentObjects'.")]
        [Input("followingObjs", "Following objects. Objects that were created after 'pastObjects'.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff DiffWithHash(IEnumerable<object> pastObjects, IEnumerable<object> followingObjs)
        {
            BH.Engine.Base.Compute.RecordNote(m_DiffWithHashNote);

            return DiffingWithHash(pastObjects, followingObjs);
        }

        /***************************************************/

        [Description("Computes the diffing using the Hash of the specified objects.")]
        [Input("pastObjects", "Objects whose creation predates 'currentObjects'.")]
        [Input("followingObjs", "Following objects. Objects that were created after 'pastObjects'.")]
        [Input("diffingConfig", "(Optional) Additional configurations for the Diffing.")]
        [Input("storeHash", "(Optional, defaults to false; applies only to BHoMObjects)\nOnce the hash for the object is computed, stores it in a HashFragment added to the BHoMObject's Fragments. The stored hash is only present in the output Diff's objects (the original input objects are not modified).")]
        [Input("retrieveStoredHash", "(Optional, defaults to false; applies only to BHoMObjects)\nAttempts to retrieve the hash from a HashFragment stored in the object, if present.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff DiffWithHash(IEnumerable<object> pastObjects, IEnumerable<object> followingObjs, DiffingConfig diffingConfig = null, bool storeHash = false, bool retrieveStoredHash = false)
        {
            BH.Engine.Base.Compute.RecordNote(m_DiffWithHashNote);

            return DiffingWithHash(pastObjects, followingObjs, diffingConfig, storeHash, retrieveStoredHash);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Diff DiffingWithHash(IEnumerable<object> pastObjects, IEnumerable<object> followingObjs, DiffingConfig diffingConfig = null, bool storeHash = false, bool retrieveStoredHash = false)
        {
            DiffingConfig diffConfigCopy = diffingConfig ?? new DiffingConfig();

            Diff outputDiff = null;
            if (InputObjectsNullOrEmpty(pastObjects, followingObjs, out outputDiff, diffingConfig))
                return outputDiff;

            if (storeHash)
            {
                // Clone objects for immutability in the UI.
                pastObjects = BH.Engine.Base.Query.DeepClone(pastObjects).ToList();
                followingObjs = BH.Engine.Base.Query.DeepClone(followingObjs).ToList();
            }

            // Compute the "Diffing" by means of a VennDiagram.
            // Hashes are computed by the HashComparer, once per each object (the hash is stored in/retrieved from a HashFragment if requested).
            HashComparer<object> hashComparer = new HashComparer<object>(diffingConfig.ComparisonConfig, storeHash, retrieveStoredHash);
            VennDiagram<object> vd = Engine.Data.Create.VennDiagram(pastObjects, followingObjs, hashComparer);

            return new Diff(vd.OnlySet2, vd.OnlySet1, null, diffingConfig, null, vd.Intersection.Select(i => i.Item2));
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static string m_DiffWithHashNote = $"DiffWithHash cannot track modified objects between different revisions." +
               $"\nIt will simply return the objects that appear exclusively in the past set (`{nameof(Diff.RemovedObjects)}`), in the following set (`{nameof(Diff.AddedObjects)}`), and in both (`{nameof(Diff.UnchangedObjects)}`)." +
               $"\nConsider using '{nameof(DiffWithCustomIds)}','{nameof(DiffWithCustomDataKeyId)}', '{nameof(DiffWithFragmentId)}' or '{nameof(DiffRevisions)}' if this feature is needed.";
    }
}




