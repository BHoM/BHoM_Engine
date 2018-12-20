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

using Engine_Explore.BHoM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Materials
{
    /// <summary>
    /// Material class for use in all other object classes and namespaces
    /// </summary>
    public class Material : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public MaterialType Type { get; set; }

        public double YoungsModulus { get; set; }

        public double PoissonsRatio { get; set; }

        public double ShearModulus { get; set; }

        public double DryDensity { get; set; }

        public double CoeffThermalExpansion { get; set; }

        public double DampingRatio { get; set; }

        public double Density { get; set; }

        public double CompressiveYieldStrength { get; set; }

        public double TensileYieldStrength { get; set; }

        public double StainAtYield { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/
        

        public Material(string name = "")
        {
            Name = name;
        }

        /***************************************************/

        public Material(string name, MaterialType type, double E, double v, double tC, double G, double density)
        {
            Name = name;
            Type = type;
            YoungsModulus = E;
            PoissonsRatio = v;
            CoeffThermalExpansion = tC;
            ShearModulus = G;
            Density = density;
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
