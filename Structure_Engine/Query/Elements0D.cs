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
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using System.ComponentModel;
using BH.Engine.Structure;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Gets the Element0Ds of a Pile, which for the case of a Pile means getting the StartNode and EndNode. Method necessary for IElement pattern.")]
        [Input("pile", "The Pile to extract IElement0ds from.")]
        [Output("element0Ds", "The list of Elements0D of the Pile, i.e. the StartNode and EndNode.")]
        public static List<IElement0D> Elements0D(this Pile pile)
        {
            return pile.IsNull() ? null : new List<IElement0D> { pile.TopNode, pile.BottomNode };
        }

        /******************************************/
    }
}




