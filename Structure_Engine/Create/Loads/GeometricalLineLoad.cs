/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a GeometricalLineLoad with a constant force across the length of the Line.")]
        [InputFromProperty("location")]
        [InputFromProperty("loadcase")]
        [Input("force", "The force to be applied to the full length of the Line.", typeof(BH.oM.Quantities.Attributes.Force))]
        [Input("name", "The name of the created load.")]
        [Output("geoLineLoad", "The created GeometricalLineLoad.")]
        public static GeometricalLineLoad GeometricalLineLoad(Line location, Loadcase loadcase, Vector force, string name = "")
        {
            return new GeometricalLineLoad
            {
                Location = location,
                Loadcase = loadcase,
                ForceA = force,
                ForceB = force,
                Name = name
            };
        }

        [Description("Creates a GeometricalLineLoad with a constant force across the length of the Line.")]
        [InputFromProperty("location")]
        [InputFromProperty("loadcase")]
        [Input("force", "The force to be applied to the full length of the Line.", typeof(BH.oM.Quantities.Attributes.Force))]
        [Input("moment", "The momentto be applied to the full length of the Line.", typeof(BH.oM.Quantities.Attributes.Moment))]
        [Input("objects", "The IAreaElement (i.e. Panels or FEMesh) to apply the GeometricalLineLoad to.")]
        [Input("name", "The name of the created load.")]
        [Output("geoLineLoad", "The created GeometricalLineLoad.")]
        public static GeometricalLineLoad GeometricalLineLoad(Line location, Loadcase loadcase, Vector force= null, Vector moment= null, IEnumerable<IAreaElement> objects = null, string name = "")
        {
            BHoMGroup<IAreaElement> group = new BHoMGroup<IAreaElement>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            if(force == null)
                force = new Vector();
            
            if(moment == null)
                moment = new Vector();

            return new GeometricalLineLoad
            {
                Location = location,
                Loadcase = loadcase,
                ForceA = force,
                ForceB = force,
                MomentA = moment,
                MomentB = moment,
                Name = name,
                Objects = group
            };
        }

        /***************************************************/
    }
}






