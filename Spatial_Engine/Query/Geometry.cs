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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.SettingOut;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****  Public Methods                  ****/
        /******************************************/

        [Description("Queries the defining Curve from the grid.")]
        [Input("grid", "The Grid to get the defining curve from.")]
        [Output("curve", "The Grids base geometrical curve object.")]
        public static ICurve Geometry(this Grid grid)
        {
            return grid?.Curve;
        }

        /******************************************/

        [Description("Queries edge curves from an IProfile.")]
        [Input("profile", "The profile to get the geometry from.")]
        [Output("edges", "The edge curves of the profile as a CompositeGeometry.")]
        public static CompositeGeometry Geometry(this IProfile profile)
        {
            if (profile?.Edges == null)
                return null;
            else
                return new CompositeGeometry { Elements = profile.Edges.ToList<IGeometry>() };
        }

        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Queries the defining geometrical object which all spatial operations will act on.")]
        [Input("element0D", "The IElement0D to get the defining geometry from.")]
        [Output("point", "The IElement0Ds base geometrical point object.")]
        public static Point IGeometry(this IElement0D element0D)
        {
            return Base.Compute.RunExtensionMethod(element0D, "Geometry") as Point;
        }

        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Queries the defining geometrical object which all spatial operations will act on.")]
        [Input("element1D", "The IElement1D to get the defining geometry from.")]
        [Output("curve", "The IElement1Ds base geometrical curve object.")]
        public static ICurve IGeometry(this IElement1D element1D)
        {
            return Base.Compute.RunExtensionMethod(element1D, "Geometry") as ICurve;
        }

        /******************************************/


    }
}




