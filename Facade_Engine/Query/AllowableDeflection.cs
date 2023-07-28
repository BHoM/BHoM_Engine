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

using System;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Analytical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Geometry;

using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Facade.Enums;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double AllowableDeflection(double mullionLength, BuildingCode code)
        {
            if (mullionLength <= 0)
            {
                BH.Engine.Base.Compute.RecordError("Mullion length needs to be a positive number.");
                return double.NaN;
            }

            if (code == BuildingCode.BSEN19902002)
            {
                if (mullionLength < 3)
                    return mullionLength / 200;
                else if (mullionLength <= 7.5)
                    return mullionLength / 300;
                else
                    return mullionLength / 250;
            }
            else if ((code == BuildingCode.ASCE705) || (code == BuildingCode.ASCE710) || (code == BuildingCode.ASCE716))
            {
                if (mullionLength < 13.5 * 3.048)
                    return mullionLength / 175;
                else
                    return mullionLength / 240 + 0.25 * 0.0254;
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Code not supported.");
                return double.NaN;
            }
        }
    }
}


