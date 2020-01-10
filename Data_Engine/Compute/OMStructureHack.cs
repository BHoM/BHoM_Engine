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
using BH.oM.Data.Requests;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BHoMObject> OmStructureHack(List<string> oMs)
        {
            if (oMs == null)
                return null;

            List<System.Type> allTypes = Reflection.Query.AllTypeList().Where(x => x.Namespace != null && x.Namespace.StartsWith("BH.oM")).ToList();

            allTypes = allTypes.Where(x => oMs.Any(y => x.Namespace.Contains(y))).ToList();

            List<BHoMObject> result = allTypes.Select(x => ToRelationObj(x)).ToList();

            result = EvaluateDepth(result);

            return result;
        }

        /***************************************************/

        private static BHoMObject ToRelationObj(System.Type type)
        {
            var allInterfaces = type.GetInterfaces();
            allInterfaces = allInterfaces.Where(x => x != null && x.Namespace.StartsWith("BH.")).ToArray();
            var minimalInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces()));

            List<System.Type> usedTypes = Reflection.Query.UsedTypes(type, true, false);

            BHoMObject result = new BHoMObject()
            {
                Name = type.FullName,
                CustomData = new Dictionary<string, object>()
                {
                    {"Depth", (int)0 },
                    {"Parents", minimalInterfaces.Select(x => x).ToList() },
                    {"UsedTypes", usedTypes.Select(x => x).ToList() }
                }
            };

            return result;
        }

        /***************************************************/

        private static List<BHoMObject> EvaluateDepth(List<BHoMObject> realtions)
        {
            bool done = false;
            while (!done)
            {
                done = true;
                for (int i = 0; i < realtions.Count; i++)
                {
                    List<System.Type> parents = (List<System.Type>)realtions[i].CustomData["Parents"];
                    int depth = (int)realtions[i].CustomData["Depth"];
                    
                    foreach (System.Type parent in parents)
                    {
                        BHoMObject currParent;
                        try
                        {
                            currParent = realtions.Single(x => x.Name == parent.FullName);
                        } catch
                        {
                            currParent = ToRelationObj(parent);
                            realtions.Add(currParent);
                            done = false;
                        }

                        if ((int)currParent.CustomData["Depth"] >= depth)
                        {
                            depth = (int)currParent.CustomData["Depth"] + 1;
                            done = false;
                        }

                        realtions[i].CustomData["Depth"] = depth;
                    }
                }
            }
            return realtions;
        }

        /***************************************************/

    }
}
