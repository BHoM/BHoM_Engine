/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Facade.Elements;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Facade.Fragments;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Facade.Results;
using BH.oM.Facade.SectionProperties;
using BH.oM.Physical.FramingProperties;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Changes the depth of mullion represented as SectionProperties")]
        [Input("FrameEdgeProperty", "FrameEdgeProperty with SectionProperties to modify and FrameExtensionBox fragment")]
        [Input("MullionDepth", "New depth for mullion. Measured from interior face of glazing.")]
        [Output("FrameEdgeProperty", "FrameEdgeProperty with modified SectionProperties depth")]
        public static FrameEdgeProperty FrameEdgeProperty(this FrameEdgeProperty frameEdgeProp, double newDepth)
        {
            //Initial Checks
            if (frameEdgeProp == null || frameEdgeProp.SectionProperties == null)
                return null;

            List<IFragment> extensionBoxes = frameEdgeProp.GetAllFragments(typeof(FrameExtensionBox));

            if (extensionBoxes.Count <= 0)
            {
                BH.Engine.Base.Compute.RecordError($"FrameEdgeProperty {frameEdgeProp.BHoM_Guid} does not have FrameExtensionBox fragment applied.");
                return null;
            }

            FrameExtensionBox extensionBox = extensionBoxes[0] as FrameExtensionBox;

            Double currentDepth = frameEdgeProp.Depth();
            if (currentDepth > newDepth)
            {
                BH.Engine.Base.Compute.RecordError($"New depth cannot be less than current mullion depth.");
                return null;
            }

            ICurve extBox = extensionBox.BoundingBoxCurve;
            foreach (ConstantFramingProperty prop in frameEdgeProp.SectionProperties)
            {
                if (prop.Profile != null)
                {
                    IProfile newProfile = ExtendProfile(prop.Profile, extBox, newDepth - currentDepth);
                    prop.Profile = newProfile;
                }
            }
            return frameEdgeProp;
        }
    }
}




