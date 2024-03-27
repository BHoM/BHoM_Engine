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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;
using BH.oM.Physical.Elements;
using BH.Engine.Matter;


namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Evaluates the mass of an object based its Solid Volume and Density.\nRequires a single consistent value of Density to be provided across all MaterialProperties of a given element.")]
        [Input("opening", "The element to evaluate the mass of")]
        [Output("mass", "The physical mass of the element", typeof(Mass))]
        public static double Mass(this IOpening opening)
        {
            MaterialComposition mat = opening.IMaterialComposition();
            return opening.SolidVolume() * mat.Materials.Zip(mat.Ratios, (m,r) => r * m.Density()).Sum();
        }

        /******************************************/
    }
}




