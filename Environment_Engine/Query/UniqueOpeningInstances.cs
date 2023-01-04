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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

using BH.oM.Physical.Elements;
using BH.Engine.Geometry;

using BH.oM.Spatial.SettingOut;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment Query, Returns a collection of Environment Openings that are unique by their instance data from their origin context fragment")]
        [Input("openings", "A collection of Environment Opening to filter")]
        [Output("openings", "A collection of Environment Opening objects with one per instance")]
        public static List<Opening> UniqueOpeningInstances(this List<Opening> openings)
        {
            List<Opening> returnOpenings = new List<Opening>();

            foreach (Opening p in openings)
            {
                OriginContextFragment o = p.FindFragment<OriginContextFragment>(typeof(OriginContextFragment));
                if (o != null)
                {
                    Opening testCheck = returnOpenings.Where(x => x.FindFragment<OriginContextFragment>(typeof(OriginContextFragment)) != null && x.FindFragment<OriginContextFragment>(typeof(OriginContextFragment)).TypeName == o.TypeName).FirstOrDefault();
                    if (testCheck == null)
                        returnOpenings.Add(p);
                }
            }

            return returnOpenings;
        }
    }
}



