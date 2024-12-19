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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the width of a generic Environment Object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its width queried")]
        [Output("width", "The width of the environment object")]
        public static double Width(this IEnvironmentObject environmentObject)
        {
            if(environmentObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the width of a null environment object.");
                return -1;
            }

            return Width(environmentObject as dynamic);
        }

        [Description("Returns the width of an Environment Panel")]
        [Input("panel", "An Environment Panel")]
        [Output("width", "The width of the panel")]
        public static double Width(this Panel panel)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the width of a null panel.");
                return -1;
            }

            return panel.Polyline().Width();
        }

        [Description("Returns the width of an Environment Opening")]
        [Input("opening", "An Environment Opening")]
        [Output("width", "The width of the opening")]
        public static double Width(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the width of a null opening.");
                return -1;
            }

            return opening.Polyline().Width();
        }

        [Description("Returns the width of a BHoM Geometry ICurve based on the bounding box of the curve")]
        [Input("panelCurve", "BHoM Geometry ICurve")]
        [Output("width", "The width of the curve based on the difference in XY values for its bounding box")]
        public static double Width(this ICurve panelCurve)
        {
            if (panelCurve == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the width of a null curve.");
                return -1;
            }

            BoundingBox bBox = panelCurve.IBounds();

            double diffX = Math.Abs(bBox.Max.X - bBox.Min.X);
            double diffY = Math.Abs(bBox.Max.Y - bBox.Min.Y);

            return Math.Sqrt((diffX * diffX) + (diffY * diffY));
        }
    }
}






