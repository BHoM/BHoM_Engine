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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Environment.Properties;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Modify MergeBuildingElement => merges the properties two building elements together and returns a copied element with the smallest area")]
        [Input("element1", "BuildingElement")]
        [Input("element2", "BuildingElement")]
        [Output("BHoM Environmental BuildingElement")]
        public static BuildingElement MergeBuildingElement(this BuildingElement element1, BuildingElement element2)
        {
            BuildingElement rtnElement = null;

            if(element1.Area() > element2.Area())
                rtnElement = element2.Copy();
            else
                rtnElement = element1.Copy();

            rtnElement.ExtendedProperties.Remove(rtnElement.ContextProperties());

            //Move any properties over which need to be merged between the two elements... Currently only adjacent spaces
            BuildingElementContextProperties masterProperties = new BuildingElementContextProperties();

            BuildingElementContextProperties mergeToProps = (element1.ContextProperties() as BuildingElementContextProperties);
            if (mergeToProps != null)
            {
                masterProperties.Colour = mergeToProps.Colour;
                masterProperties.ConnectedSpaces = new List<string>(mergeToProps.ConnectedSpaces);
                masterProperties.IsAir = mergeToProps.IsAir;
                masterProperties.IsGround = mergeToProps.IsGround;
                masterProperties.Reversed = mergeToProps.Reversed;
            }

            BuildingElementContextProperties mergeProps = (element2.ContextProperties() as BuildingElementContextProperties);
            if (mergeProps == null)
                mergeProps = new BuildingElementContextProperties(); //Hopefully this will never be needed...

            if (mergeProps.ConnectedSpaces.Count > 0)
            {
                if (masterProperties.ConnectedSpaces.Count == 1)
                    masterProperties.ConnectedSpaces.Add(mergeProps.ConnectedSpaces.Where(x => x != "-1").FirstOrDefault());
                else
                    masterProperties.ConnectedSpaces[masterProperties.ConnectedSpaces.IndexOf("-1")] = mergeProps.ConnectedSpaces.Where(x => x != "-1").FirstOrDefault();
            }

            rtnElement.ExtendedProperties.Add(masterProperties);
            return rtnElement;
        }

        public static BuildingElement MergeBuildingElement(this List<BuildingElement> elements)
        {
            if (elements.Count == 1)
                return elements[0];

            if (elements.Count > 1)
                return elements[0].MergeBuildingElement(elements[1]);

            return null; //Something went wrong with the input           
        }
    }
}
