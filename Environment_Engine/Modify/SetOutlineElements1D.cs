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

using BH.oM.Environment.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Modify.SetOutlineElements1D => Assign a new collection of external 1D boundaries to an Environment Opening")]
        [Input("opening", "An Environment Opening to update")]
        [Input("outlineElements1D", "A collection of outline 1D elements to assign to the opening")]
        [Output("opening", "The updated Environment Opening")]
        public static Opening SetOutlineElements1D(this Opening opening, List<IElement1D> outlineElements1D)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline 1D elements of a null opening.");
                return opening;
            }

            if(outlineElements1D == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set null outline elements to an opening.");
                return opening;
            }

            Opening o = opening.ShallowClone();
            o.Edges = outlineElements1D.Cast<ICurve>().ToList().ToEdges();
            return o;
        }

        [Description("BH.Engine.Environment.Modify.SetOutlineElements1D => Assign a new collection of external 1D boundaries to an Environment Opening")]
        [Input("panel", "An Environment Panel to update")]
        [Input("outlineElements1D", "A collection of outline 1D elements to assign to the opening")]
        [Output("panel", "The updated Environment Opening")]
        public static Panel SetOutlineElements1D(this Panel panel, List<IElement1D> outlineElements1D)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline 1D elements of a null panel.");
                return panel;
            }

            if (outlineElements1D == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set null outline elements to a panel.");
                return panel;
            }

            Panel pp = panel.ShallowClone();
            pp.ExternalEdges = outlineElements1D.Cast<ICurve>().ToList().ToEdges();
            return pp;
        }

        [Description("Assign a new collection of external 1D boundaries to an Environment Space")]
        [Input("space", "An Environment Space to update")]
        [Input("outlineElements1D", "A collection of outline 1D elements to assign to the Space")]
        [Output("space", "The updated Environment Space")]
        public static Space SetOutlineElements1D(this Space space, List<IElement1D> outlineElements1D)
        {
            if (space == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline 1D elements of a null space.");
                return space;
            }

            if (outlineElements1D == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set null outline elements to a space.");
                return space;
            }

            Space r = space.ShallowClone();
            r.Perimeter = BH.Engine.Geometry.Compute.IJoin(outlineElements1D.Cast<ICurve>().ToList())[0];
            return r;
        }
    }
}




