/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection;
using System.Collections;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        public static Diff Diff(IEnumerable<object> pastObjects, IEnumerable<object> currentObjects, DiffConfig diffConfig = null)
        {
            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffConfig diffConfigCopy = diffConfig == null ? new DiffConfig() : (DiffConfig)diffConfig.GetShallowClone();

            // Output lists.
            List<object> allOldObjs = new List<object>();
            List<object> allNewObjs = new List<object>();

            // Filter out the non-BHoMObjects.
            IEnumerable<IBHoMObject> prevObjs_BHoM = pastObjects.OfType<IBHoMObject>();
            IEnumerable<IBHoMObject> currObjs_BHoM = currentObjects.OfType<IBHoMObject>();

            Diff bhomObjectsDiff = null;
            if (pastObjects.Count() != 0 && pastObjects.Count() == prevObjs_BHoM.Count() && currentObjects.Count() == currObjs_BHoM.Count())
            {
                bhomObjectsDiff = DiffRevisionObjects(
                    Modify.PrepareForRevision(prevObjs_BHoM, diffConfigCopy),
                    Modify.PrepareForRevision(currObjs_BHoM, diffConfigCopy), 
                    diffConfigCopy);

                allNewObjs.AddRange(bhomObjectsDiff.AddedObjects);
                allOldObjs.AddRange(bhomObjectsDiff.RemovedObjects);
            }

            // Filter out the BHoMObjects.
            IEnumerable<object> prevObjs_nonBHoM = pastObjects.Where(o => !(o is IBHoMObject));
            IEnumerable<object> currObjs_nonBHoM = currentObjects.Where(o => !(o is IBHoMObject));

            if (prevObjs_nonBHoM.Count() > 0 || currObjs_BHoM.Count() > 0) {
                // Compute the generic Diffing for the generic objects.
                // This is left to the VennDiagram with a HashComparer (specifically, this doesn't use the HashFragment).
                VennDiagram<object> vd = Engine.Data.Create.VennDiagram(prevObjs_nonBHoM, currObjs_nonBHoM, new DiffingHashComparer<object>(diffConfig));
                allOldObjs.AddRange(vd.OnlySet1);
                allOldObjs.AddRange(vd.OnlySet2);
            }

            // Return the final, actual diff.
            return new Diff(allNewObjs, allOldObjs, bhomObjectsDiff?.ModifiedObjects, diffConfigCopy, bhomObjectsDiff?.ModifiedPropsPerObject, bhomObjectsDiff?.UnchangedObjects);
        }
    }
}

