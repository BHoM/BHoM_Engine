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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.oM.Base.Debugging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Facade.Elements;
using BH.oM.Facade.Fragments;
using BH.Engine.Physical;

namespace BH.Engine.Facade
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets an opening offset based on a specified value defining the desired distance between the panel's exterior face and the openings exterior face.")]
        [Input("panel", "A panel to base opening offset on. The panel must have a construction thickness.")]
        [Input("opening", "An opening to set the offset on based on the specified value and provided panel. The opening must have a glazing thickness.")]
        [Input("useroffset", "A user defined offset (exterior face of panel to exterior face of glazing).")]
        [Output("opening", "The opening with the specified offset applied as a fragment.")]
        public static void SetOffsetFromFacetoFaceDist(this Opening opening, Panel panel, double useroffset)
        {
            //Check for null case
            if (panel == null || opening == null || useroffset == double.NaN || useroffset < 0)
            {
                BH.Engine.Base.Compute.RecordError("Invalid inputs, offset was not applied.");
                return;
            }

            //Retrieve panel width and opening glazing width
            double panelwidth = panel.Construction.IThickness();
            double glzwidth = opening.OpeningConstruction.IThickness();

            //Check for existing offset on reference panel
            double panelOffset = 0;
            ConstructionOffset panelOffsetFragment = BH.Engine.Base.Query.FindFragment<ConstructionOffset>(panel);
            if (panelOffsetFragment != null)
            {
                panelOffset = panelOffsetFragment.OffsetDistance;
            }

            //Calculate and return offset from defined panel centerline to glazing (opening) centerline
            double offset = panelOffset + 0.5 * panelwidth - 0.5 * glzwidth - useroffset;

            //Apply offset value to existing fragment (or new if not yet existing)
            ConstructionOffset offsetFragment = BH.Engine.Base.Query.FindFragment<ConstructionOffset>(opening);
            if (offsetFragment == null)
            {
                ConstructionOffset newOffsetFragment = new ConstructionOffset() { OffsetDistance = offset };
                opening.Fragments.Add(newOffsetFragment);
            }
            else
            {
                offsetFragment.OffsetDistance = offset;
            }
        }
    }
}


