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

using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Environment.Interface;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("BH.Engine.Environment.Query.PropertiesByType => gets a set of extended properties for a building object by the type of property")]
        [Input("obj", "IBuildingObject - an environmental building object")]
        [Input("propertyType", "Type - the type of extended properties to get")]
        [Output("BHoM Extended Properties - null if property type is not on the object")]
        public static IBHoMExtendedProperties PropertiesByType(this IBuildingObject obj, Type propertyType)
        {
            return obj.ExtendedProperties.Where(x => x.GetType() == propertyType).FirstOrDefault();
        }

        [Description("BH.Engine.Environment.Query.ContextProperties => gets a set of context extended properties for a building object")]
        [Input("obj", "IBuildingObject - an environmental building object")]
        [Output("BHoM Extended Properties - null if property type is not on the object")]
        public static IBHoMExtendedProperties ContextProperties(this IBuildingObject obj)
        {
            return ContextProperties(obj as dynamic);
        }

        [Description("BH.Engine.Environment.Query.AnalyticalProperties => gets a set of analytical extended properties for a building object")]
        [Input("obj", "IBuildingObject - an environmental building object")]
        [Output("BHoM Extended Properties - null if property type is not on the object")]
        public static IBHoMExtendedProperties AnalyticalProperties(this IBuildingObject obj)
        {
            return AnalyticalProperties(obj as dynamic);
        }

        [Description("BH.Engine.Environment.Query.ResultProperties => gets a set of result extended properties for a building object")]
        [Input("obj", "IBuildingObject - an environmental building object")]
        [Output("BHoM Extended Properties - null if property type is not on the object")]
        public static IBHoMExtendedProperties ResultProperties(this IBuildingObject obj)
        {
            return ResultProperties(obj as dynamic);
        }

        [Description("BH.Engine.Environment.Query.ElementProperties => gets a set of element extended properties for a building object")]
        [Input("obj", "IBuildingObject - an environmental building object")]
        [Output("BHoM Extended Properties - null if property type is not on the object")]
        public static IBHoMExtendedProperties ElementProperties(this IBuildingObject obj)
        {
            return ElementProperties(obj as dynamic);
        }

        //Context properties
        private static IBHoMExtendedProperties ContextProperties(this Building building)
        {
            return building.PropertiesByType(typeof(BuildingContextProperties));
        }

        private static IBHoMExtendedProperties ContextProperties(this BuildingElement element)
        {
            return element.PropertiesByType(typeof(BuildingElementContextProperties));
        }

        private static IBHoMExtendedProperties ContextProperties(this Opening opening)
        {
            return null; //Openings do not currently have any context properties
        }

        private static IBHoMExtendedProperties ContextProperties(this Space space)
        {
            return null; //Spaces do not currently have any context properties
        }

        //Analytical properties
        private static IBHoMExtendedProperties AnalyticalProperties(this Building building)
        {
            return building.PropertiesByType(typeof(BuildingAnalyticalProperties));
        }

        private static IBHoMExtendedProperties AnalyticalProperties(this BuildingElement element)
        {
            return element.PropertiesByType(typeof(BuildingElementAnalyticalProperties));
        }

        private static IBHoMExtendedProperties AnalyticalProperties(this Opening opening)
        {
            return null; //Openings do not currently have any analytical properties
        }

        private static IBHoMExtendedProperties AnalyticalProperties(this Space space)
        {
            return space.PropertiesByType(typeof(SpaceAnalyticalProperties));
        }

        //Result properties
        private static IBHoMExtendedProperties ResultProperties(this Building building)
        {
            return building.PropertiesByType(typeof(BuildingResultsProperties));
        }

        private static IBHoMExtendedProperties ResultProperties(this BuildingElement element)
        {
            return null; //Building Elements do not currently have result properties
        }

        private static IBHoMExtendedProperties ResultProperties(this Opening opening)
        {
            return null; //Openings do not currently have any analytical properties
        }

        private static IBHoMExtendedProperties ResultProperties(this Space space)
        {
            return null; //Spaces do not currently have any analytical properties
        }

        //Element properties
        private static IBHoMExtendedProperties ElementProperties(this Building building)
        {
            return building.PropertiesByType(typeof(ElementProperties));
        }

        private static IBHoMExtendedProperties ElementProperties(this BuildingElement element)
        {
            return element.PropertiesByType(typeof(ElementProperties));
        }

        private static IBHoMExtendedProperties ElementProperties(this Opening opening)
        {
            return opening.PropertiesByType(typeof(ElementProperties));
        }

        private static IBHoMExtendedProperties ElementProperties(this Space space)
        {
            return space.PropertiesByType(typeof(ElementProperties));
        }
    }
}
