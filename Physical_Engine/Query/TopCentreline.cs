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

using BH.Engine.Geometry;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Physical.FramingProperties;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Physical.Elements;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        [Description("Returns the top centreline of an IFramingElement.")]
        [Input("element", "The IFramingElement to query the top centreline of.")]
        [Output("curve", "The top centreline of the IFramingElement.")]
        public static ICurve TopCentreline(this IFramingElement element)
        {   
            if(element == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the top centreline of a null framing element.");
                return null;
            }

            ICurve location = element.Location;

            Vector normal = null;

            try
            {
                normal = BH.Engine.Physical.Query.Normal(element);
            }
            catch
            {
                Engine.Base.Compute.RecordError("IFramingElement must have linear location line.");
                return null;
            }

            if (normal == null)
            {
                Engine.Base.Compute.RecordError("Was not able to compute element normal.");
                return null;
            }

            if (element.Property is ConstantFramingProperty)
            {
                ConstantFramingProperty constantProperty = element.Property as ConstantFramingProperty;

                IProfile profile = constantProperty.Profile;

                BoundingBox profileBounds = profile.Edges.Bounds();

                return location.ITranslate(normal * profileBounds.Max.Y);
            }
            else
            {
                Engine.Base.Compute.RecordError("Element does not have a suitable framing property, so the section height could not be calculated.");
                return null;
            }
        }
    }
}





