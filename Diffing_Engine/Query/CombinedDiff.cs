/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        [Description("Combines two given diffs into one, appending objects of the second to the first.")]
        [Input("diff", "First diff object.")]
        [Input("toAdd", "Second diff object.")]
        [Output("diff", "Merged Diff object.")]
        [PreviousVersion("5.0", "BH.Engine.Diffing.Modify.CombineDiffs(BH.oM.Diffing.Diff, BH.oM.Diffing.Diff")]
        public static Diff CombinedDiff(this Diff diff, Diff toAdd)
        {
            if (diff == null)
                return toAdd;

            if (toAdd == null)
                return diff;

            return new Diff(
                diff.AddedObjects != null ? diff.AddedObjects.Concat(toAdd.AddedObjects ?? new List<object>()) : toAdd.AddedObjects ?? new List<object>(),
                diff.RemovedObjects != null ? diff.RemovedObjects.Concat(toAdd.RemovedObjects ?? new List<object>()) : toAdd.RemovedObjects ?? new List<object>(),
                diff.ModifiedObjects != null ? diff.ModifiedObjects.Concat(toAdd.ModifiedObjects ?? new List<object>()) : toAdd.ModifiedObjects ?? new List<object>(),
                diff?.DiffingConfig ?? new DiffingConfig(),
                diff.ModifiedPropsPerObject != null ?
                        diff.ModifiedPropsPerObject
                        .Concat(toAdd.ModifiedPropsPerObject ?? new Dictionary<string, Dictionary<string, Tuple<object, object>>>())
                        .ToDictionary(x => x.Key, x => x.Value)
                        : toAdd.ModifiedPropsPerObject ?? new Dictionary<string, Dictionary<string, Tuple<object, object>>>(),
                diff.UnchangedObjects != null ? diff.UnchangedObjects.Concat(toAdd.UnchangedObjects ?? new List<object>()) : toAdd.UnchangedObjects ?? new List<object>()
                );
        }
    }
}


