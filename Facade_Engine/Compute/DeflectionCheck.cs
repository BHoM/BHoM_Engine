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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Facade.Enums;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        public static bool DeflectionCheck(Bar bar, double windLoad, double tributaryWidth, BuildingCode buildingCode)
        {
            if (bar == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot check a null bar.");
                return false;
            }

            double allowableDeflection = Query.AllowableDeflection(bar.Length(), buildingCode);
            if (double.IsNaN(allowableDeflection))
            {
                BH.Engine.Base.Compute.RecordError("Deflection check could not be executed.");
                return false;
            }

            SupportType supportType = bar.SupportType();
            if (supportType == oM.Facade.Enums.SupportType.Undefined)
            {
                BH.Engine.Base.Compute.RecordError("Deflection check could not be executed because of undefined support type.");
                return false;
            }

            double momentOfInertia = bar.MomentOfInertia();
            if (double.IsNaN(momentOfInertia) || momentOfInertia == 0)
            {
                BH.Engine.Base.Compute.RecordError("Moment of inertia of a bar could not be found in its properties.");
                return false;
            }

            double youngsModulus = bar.YoungsModulus();
            if (double.IsNaN(youngsModulus) || youngsModulus == 0)
            {
                BH.Engine.Base.Compute.RecordError("Young's modulus of a bar could not be found in its properties.");
                return false;
            }

            double actualDeflection = Query.ActualDeflection(supportType, windLoad, tributaryWidth, bar.Length(), momentOfInertia, youngsModulus);
            if (double.IsNaN(actualDeflection))
            {
                BH.Engine.Base.Compute.RecordError("Deflection check could not be executed.");
                return false;
            }

            return actualDeflection <= allowableDeflection;
        }


        /***************************************************/
        /****          Private Methods                  ****/
        /***************************************************/

        private static double MomentOfInertia(this Bar bar)
        {
            ISectionProperty property = bar?.SectionProperty;
            if (property == null)
                return double.NaN;
            else
                return property.Iy;
        }

        /***************************************************/

        private static double YoungsModulus(this Bar bar)
        {
            IIsotropic material = bar?.SectionProperty?.Material as IIsotropic;
            if (material == null)
                return double.NaN;
            else
                return material.YoungsModulus;
        }

        /***************************************************/
    }
}




