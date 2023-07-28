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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Facade.Enums;
using BH.oM.Quantities.Attributes;
using Mono.Cecil.Cil;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/


        /***************************************************/

        public static bool PlasticModulusCheck(Bar bar, double windLoad, double tributaryWidth, BuildingCode buildingCode)
        {
            if (bar == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot check a null bar.");
                return false;
            }

            SupportType supportType = bar.SupportType();
            if (supportType == oM.Facade.Enums.SupportType.Undefined)
            {
                BH.Engine.Base.Compute.RecordError("Undefined support type.");
                return false;
            }

            double yieldStress = bar.YieldStress();
            if (double.IsNaN(yieldStress) || yieldStress == 0)
            {
                // no yield stress error
                return false;
            }

            double plasticModulus = bar.PlasticModulus();
            if (double.IsNaN(plasticModulus) || plasticModulus == 0)
            {
                // no plastic modulus error
                return false;
            }

            double requiredZx = Query.RequiredZx(supportType, windLoad, tributaryWidth, bar.Length(), yieldStress, buildingCode);
            if (double.IsNaN(requiredZx))
            {
                BH.Engine.Base.Compute.RecordError("Check could not be executed.");
                return false;
            }

            return plasticModulus >= requiredZx;
        }


        private static double PlasticModulus(this Bar bar)
        {
            ISectionProperty property = bar?.SectionProperty;
            if (property == null)
                return double.NaN;
            else
                return property.Wplz;
        }

        private static double YieldStress(this Bar bar)
        {
            BH.Engine.Base.Compute.RecordWarning("Default value for aluminium used.");
            // Value for Aluminium σ_y from https://www.engineeringtoolbox.com/young-modulus-d_417.html.
            return 9.5e+7; 
        }

    }
}