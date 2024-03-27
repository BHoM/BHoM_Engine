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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Physical.Constructions;
using BH.Engine.Physical;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the thickness of an Environment Panel based on its construction")]
        [Input("panel", "An Environment Panel")]
        [Output("thickness", "The total thickness of the panel as a result of the construction placed on it")]
        public static double Thickness(this Panel panel)
        {
            if (panel == null || panel.Construction == null)
                return 0;

            return panel.Construction.IThickness();
        }

        [Description("Returns the thickness of an Environment Opening based on its construction")]
        [Input("opening", "An Environment Opening")]
        [Output("thickness", "The thickness of the opening as the largest thickness between the frame construction and opening construction")]
        public static double Thickness(this Opening opening)
        {
            if (opening == null || opening.OpeningConstruction == null && opening.FrameConstruction == null)
                return 0;

            if(opening.OpeningConstruction != null && opening.FrameConstruction != null)
                return Math.Max(opening.OpeningConstruction.IThickness(), opening.FrameConstruction.IThickness());

            if (opening.OpeningConstruction != null)
                return opening.OpeningConstruction.IThickness();

            if (opening.FrameConstruction != null)
                return opening.FrameConstruction.IThickness();

            return 0;
        }

        [Description("Returns the thickness of an Environment Opening based on its construction")]
        [Input("opening", "An Environment Opening")]
        [Input("useFrameConstruction", "Determine whether to use the frame construction for thickness or the glazing construction. Default false - default will be to use the glazing construction")]
        [Output("thickness", "The thickness of the opening")]
        public static double Thickness(this Opening opening, bool useFrameConstruction = false)
        {
            if (opening == null)
                return 0;

            if (useFrameConstruction && opening.FrameConstruction != null)
                return opening.FrameConstruction.IThickness();
            else if (opening.OpeningConstruction != null)
                return opening.OpeningConstruction.IThickness();
            else
                return 0;
        }
    }
}




