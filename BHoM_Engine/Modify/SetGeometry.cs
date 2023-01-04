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

using BH.oM.Base;
using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Tries to set the geometry to the object. The type of geometry needs to be compatible with the object. If the method fails to set the geometry, it will return the incoming object without modification.")]
        [Input("obj", "The object to set the geometry to.")]
        [Input("geometry", "The geometry to set to the object. The type of geometry needs to be compatible with the object.")]
        [Output("obj", "The object with updated geometry.")]
        public static IBHoMObject ISetGeometry(this IBHoMObject obj, IGeometry geometry)
        {
            if (obj == null)
            {
                Compute.RecordError("Cannot set geometry to a null object.");
                return null;
            }
            return SetGeometry(obj as dynamic, geometry);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IBHoMObject SetGeometry(this IBHoMObject obj, IGeometry geometry)
        {
            object result;
            if (Compute.TryRunExtensionMethod(obj, "SetGeometry", new object[] { geometry }, out result))
                return result as IBHoMObject;
            else
            {
                string geomType = geometry == null ? "null geometry" : $"geometry of type {geometry.GetType().Name}";
                Compute.RecordError($"Cannot set {geomType} to a {obj.GetType().Name}.");
            }

            return obj;
        }

        /***************************************************/
    }
}




