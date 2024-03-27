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

using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.Engine.Geometry;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural Opening from a closed curve.")]
        [Input("outline", "Closed curve defining the outline of the Opening.")]
        [Output("opening", "Created structural Opening.")]
        public static Opening Opening(ICurve outline)
        {
            if (outline.IsNull())
                return null;
            else if (outline.IIsClosed())
                return new Opening { Edges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList() };
            else
            {
                Base.Compute.RecordError("Provided curve is not closed. Could not create opening.");
                return null;
            }
        }

        /***************************************************/

        [Description("Creates a structural Opening from a collection of curves forming a closed loop.")]
        [Input("edges", "Closed curve defining the outline of the Opening.")]
        [Output("opening", "Created structural Opening.")]
        public static Opening Opening(IEnumerable<ICurve> edges)
        {
            if (edges.IsNullOrEmpty() || edges.Any(x => x.IsNull()))
                return null;

            List<PolyCurve> joined = Geometry.Compute.IJoin(edges.ToList());

            if (joined.Count == 0)
            {
                Base.Compute.RecordError("Could not join Curves. Opening not Created.");
                return null;
            }
            else if (joined.Count > 1)
            {
                Base.Compute.RecordError("Provided curves could not be joined to a single curve. Opening not created.");
                return null;
            }

            //Single joined curve
            if (joined[0].IIsClosed())
                return new Opening { Edges = edges.Select(x => new Edge { Curve = x }).ToList() };
            else
            {
                Base.Compute.RecordError("Provided curves does not form a closed loop. Could not create opening.");
                return null;
            }

        }

        /***************************************************/
    }
}





