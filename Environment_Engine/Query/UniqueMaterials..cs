/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Geometry;

using BH.Engine.Geometry;

using BH.Engine.Common;

using BH.oM.Environment.Properties;
using BH.oM.Environment.Materials;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Material> UniqueMaterials(this List<Construction> constructions)
        {
            List<Material> unique = new List<Material>();

            foreach(Construction c in constructions)
            {
                foreach(Material m in c.Materials)
                {
                    Material t = unique.Where(x => x.Name == m.Name).FirstOrDefault();
                    if (t == null)
                        unique.Add(t);
                }
            }

            return unique;
        }

        public static List<Material> UniqueMaterials(this List<BuildingElement> elements)
        {
            return elements.UniqueConstructions().UniqueMaterials();
        }

        public static List<Material> UniqueMaterials(this List<List<BuildingElement>> elementsAsSpaces)
        {
            return elementsAsSpaces.UniqueBuildingElements().UniqueConstructions().UniqueMaterials();
        }
    }
}