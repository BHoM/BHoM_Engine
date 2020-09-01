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
using System.Reflection;
using BH.Engine.Serialiser;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection;
using BH.Engine.Diffing;

namespace BH.Engine.Diffing
{
    public class DiffingHashComparer<T> : IEqualityComparer<T> //where T : IBHoMObject
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public bool StoreHash { get; set; } = false;
        public DiffConfig DiffConfig { get; set; } = new DiffConfig();

        public DiffingHashComparer(DiffConfig diffConfig = null)
        {
            if (diffConfig != null)
                DiffConfig = diffConfig;
        }

        public DiffingHashComparer(DiffConfig diffConfig = null, bool storeHash = false) : this(diffConfig)
        {
            StoreHash = storeHash;
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(T x, T y)
        {
            if (x?.GetType() == y?.GetType())
            {
                string xHash = null;
                string yHash = null;

                IBHoMObject xbHoM = x as IBHoMObject;
                IBHoMObject ybHoM = y as IBHoMObject;

                if (xbHoM != null && ybHoM != null)
                {
                    xHash = xbHoM?.GetHashFragment()?.CurrentHash;
                    yHash = ybHoM?.GetHashFragment()?.CurrentHash;

                    if (string.IsNullOrWhiteSpace(xHash))
                    {
                        xHash = x.DiffingHash(DiffConfig);

                        if (StoreHash)
                            SetHashFragment(xbHoM, xHash);
                    }

                    if (string.IsNullOrWhiteSpace(yHash))
                    {
                        yHash = y.DiffingHash(DiffConfig);

                        if (StoreHash)
                            SetHashFragment(ybHoM, yHash);
                    }

                    return xHash == yHash;
                }


                return x.DiffingHash(DiffConfig) == y.DiffingHash(DiffConfig);
            }

            return false;
        }

        /***************************************************/

        public int GetHashCode(T obj)
        {
            if (typeof(IBHoMObject).IsAssignableFrom(typeof(T)))
            {
                IBHoMObject bHoMObject = (IBHoMObject)obj;
                HashFragment hashFragment = bHoMObject.GetHashFragment();
                if (!string.IsNullOrWhiteSpace(hashFragment?.CurrentHash))
                    return hashFragment.CurrentHash.GetHashCode();
            }

            return obj.DiffingHash(DiffConfig).GetHashCode();
        }

        /***************************************************/

        // Modify in-place.
        private static bool SetHashFragment(IBHoMObject obj, string hash)
        {
            HashFragment existingFragm = obj.GetHashFragment();

            return obj.Fragments.AddOrReplace(new HashFragment(hash, existingFragm?.CurrentHash));
        }
    }
}

