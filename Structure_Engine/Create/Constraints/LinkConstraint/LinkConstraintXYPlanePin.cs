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

using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LinkConstraint where the directions are linked to give rigidity in the xy-plane, but the rotations of the secondary nodes are not linked to the primary, and there is no constraint out of plane.")]
        [Input("name", "Name of the created LinkConstraint. Defaults to 'xy-Plane Pin'. This is required for various structural packages to create the object.")]
        [Output("linkConstraint", "The created LinkConstraint.")]
        public static LinkConstraint LinkConstraintXYPlanePin(string name = "xy-Plane Pin")
        {
            LinkConstraint constr = new LinkConstraint();
            constr.XtoX = true;
            constr.XtoZZ = true;
            constr.YtoY = true;
            constr.YtoZZ = true;
            constr.Name = name;
            return constr;
        }

        /***************************************************/
    }
}





