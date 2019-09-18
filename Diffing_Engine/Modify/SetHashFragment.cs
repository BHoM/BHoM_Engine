/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

namespace BH.Engine.Diffing
{
    public static partial class Modify
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static void SetHashFragment(IEnumerable<IBHoMObject> objs, List<string> exceptions = null, bool useDefaultExceptions = true)
        {
            if (useDefaultExceptions)
                Compute.SetDefaultExceptions(ref exceptions);

            // Calculate and set the object hashes
            foreach (var obj in objs)
            {
                string hash = BH.Engine.Diffing.Compute.DiffingHash(obj, exceptions, useDefaultExceptions);

                HashFragment existingFragm = obj.GetHashFragment();

                if (existingFragm != null)
                {
                    obj.Fragments.RemoveAll(fr => (fr as HashFragment) != null);
                    obj.Fragments.Add(new HashFragment(hash, existingFragm.Hash));
                }
                else
                    obj.Fragments.Add(new HashFragment(hash, null));
            }
        }
    }
}
