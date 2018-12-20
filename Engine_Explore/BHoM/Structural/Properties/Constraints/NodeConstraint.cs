/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using Engine_Explore.BHoM.Base;
using Engine_Explore.BHoM.Structural;
using Engine_Explore.BHoM.Geometry;

namespace Engine_Explore.BHoM.Structural.Properties
{
    public class NodeConstraint : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public double KX { get; set; } = 0;

        public double KY { get; set; } = 0;

        public double KZ { get; set; } = 0;

        public double HX { get; set; } = 0;

        public double HY { get; set; } = 0;

        public double HZ { get; set; } = 0;

        public DOFType UX { get; set; } = DOFType.Free;

        public DOFType UY { get; set; } = DOFType.Free;

        public DOFType UZ { get; set; } = DOFType.Free;

        public DOFType RX { get; set; } = DOFType.Free;

        public DOFType RY { get; set; } = DOFType.Free;

        public DOFType RZ { get; set; } = DOFType.Free;


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public NodeConstraint(string name = "")
        {
            Name = name;
        }

        /***************************************************/

        public NodeConstraint(string name, bool[] fixity, double[] values)
        {
            Name = name;
            KX = values[0];
            KY = values[1];
            KZ = values[2];
            HX = values[3];
            HY = values[4];
            HZ = values[5];
            UX = (fixity[0]) ? DOFType.Fixed : (values[0] == 0) ? DOFType.Free : DOFType.Spring;
            UY = (fixity[1]) ? DOFType.Fixed : (values[1] == 0) ? DOFType.Free : DOFType.Spring;
            UZ = (fixity[2]) ? DOFType.Fixed : (values[2] == 0) ? DOFType.Free : DOFType.Spring;
            RX = (fixity[3]) ? DOFType.Fixed : (values[3] == 0) ? DOFType.Free : DOFType.Spring;
            RY = (fixity[4]) ? DOFType.Fixed : (values[4] == 0) ? DOFType.Free : DOFType.Spring;
            RZ = (fixity[5]) ? DOFType.Fixed : (values[5] == 0) ? DOFType.Free : DOFType.Spring;
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
