/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections;
using BH.Engine.Base;

namespace BH.Engine.Base.Objects
{
    [Description("Computes and compares the Hash of the given Objects.")]
    public class HashComparer<T> : IEqualityComparer<T>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("If true, stores the computed hash for input BHoMObjects as a new HashFragment. False by default.")]
        public bool StoreHash { get; set; } = false;

        [Description("If true, attempts to retrieve a hash stored in the object's Fragments instead of computing it. False by default.")]
        public bool RetrieveStoredHash { get; set; } = false;

        [Description("If the objects are IObjects, computes the BHoM Hash using these configurations.")]
        public BaseComparisonConfig ComparisonConfig { get; set; } = new ComparisonConfig();

        public HashComparer() { }

        [Input("comparisonConfig", "If the objects are IObjects, computes the BHoM Hash using these configurations.")]
        public HashComparer(BaseComparisonConfig comparisonConfig)
        {
            ComparisonConfig = comparisonConfig;
        }

        [Input("comparisonConfig", "If the objects are IObjects, computes the BHoM Hash using these configurations.")]
        [Input("storeHash", "If true, stores the computed hash for input BHoMObjects as a new HashFragment. False by default.")]
        public HashComparer(BaseComparisonConfig comparisonConfig, bool storeHash) : this(comparisonConfig)
        {
            StoreHash = storeHash;
        }

        [Input("comparisonConfig", "If the objects are IObjects, computes the BHoM Hash using these configurations.")]
        [Input("storeHash", "If true, stores the computed hash for input BHoMObjects as a new HashFragment. False by default.")]
        [Input("retrieveStoredHash", "If true, attempts to retrieve a hash stored in the object's Fragments instead of computing it. False by default.")]
        public HashComparer(BaseComparisonConfig comparisonConfig, bool storeHash, bool retrieveStoredHash) : this(comparisonConfig, storeHash)
        {
            RetrieveStoredHash = retrieveStoredHash;
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

                IObject xbHoM = x as IObject;
                IObject ybHoM = y as IObject;

                if (xbHoM != null && ybHoM != null)
                {
                    xHash = xbHoM.Hash(ComparisonConfig, RetrieveStoredHash);
                    yHash = ybHoM.Hash(ComparisonConfig, RetrieveStoredHash);

                    if (StoreHash && xbHoM is IBHoMObject)
                        x = (T)SetHashFragment(xbHoM as IBHoMObject, xHash);

                    if (StoreHash && ybHoM is IBHoMObject)
                        y = (T)SetHashFragment(ybHoM as IBHoMObject, yHash);

                    return xHash == yHash;
                }


                return GetHashCode(x) == GetHashCode(y);
            }

            return false;
        }

        /***************************************************/

        public int GetHashCode(T obj)
        {
            IObject iObj = obj as IObject;

            if (iObj != null)
                return iObj.Hash(ComparisonConfig).GetHashCode();

            return obj?.GetHashCode() ?? 0;
        }

        /***************************************************/

        private Y SetHashFragment<Y>(Y obj, string hash) where Y : IBHoMObject
        {
            obj.Fragments.AddOrReplace(new HashFragment() { Hash = hash });

            return obj;
        }
    }
}





