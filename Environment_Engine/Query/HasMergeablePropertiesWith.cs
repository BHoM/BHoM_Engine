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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;
using BH.Engine.Diffing;
using BH.oM.Diffing;
using BH.oM.Environment.Analysis;
using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object. Environment Edges have no additional data other than their geometry, so this will always return true")]
        [Input("element", "An Environment Edge to compare the properties of with an other Environment Edge")]
        [Input("other", "The Environment Edge to compare with the other Environment Edge.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(this Edge element, Edge other)
        {
            return true; //Environment Edges have no additional data to be checked so geometric edges can be merged
        }

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object")]
        [Input("element", "An Environment Panel to compare the properties of with an other Environment Panel")]
        [Input("other", "The Environment Panel to compare with the other Environment Panel.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(this Panel element, Panel other)
        {
            ComparisonConfig cc = new ComparisonConfig()
            {
                PropertyExceptions = new HashSet<string>
                    {
                        "ExternalEdges",
                        "Openings",
                        "ConnectedSpaces",
                        "Type",
                        "BHoM_Guid",
                        "CustomData",
                    },
                NumericTolerance = BH.oM.Geometry.Tolerance.Distance
            };

            return !Diffing.Query.DifferentProperties(element, other, cc)?.Any() ?? true;
        }

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object")]
        [Input("element", "An Environment Opening to compare the properties of with an other Environment Opening")]
        [Input("other", "The Environment Opening to compare with the other Environment Opening.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(this Opening element, Opening other)
        {
            ComparisonConfig cc = new ComparisonConfig()
            {
                PropertyExceptions = new HashSet<string>
                    {
                        "Edges",
                        "FrameFactorValue",
                        "InnerEdges",
                        "Type",
                        "BHoM_Guid",
                        "CustomData",
                    },
                NumericTolerance = BH.oM.Geometry.Tolerance.Distance
            };

            return !Diffing.Query.DifferentProperties(element, other, cc)?.Any() ?? true;
        }

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object. Environment Nodes are checked for their ID only")]
        [Input("element", "An Environment Node to compare the properties of with an other Environment Node")]
        [Input("other", "The Environment Node to compare with the other Environment Node")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(this Node element, Node other)
        {
            if (element == null || other == null)
                return false; //If either node is null, then it can probably can't have its properties merged

            return element.ID == other.ID; //If the IDs match, then they can be merged assuming their geometrical placement is the same
        }

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object")]
        [Input("element", "An Environment Space to compare the properties of with an other Environment Space")]
        [Input("other", "The Environment Space to compare with the other Environment Space")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(this Space element, Space other)
        {
            ComparisonConfig cc = new ComparisonConfig()
            {
                PropertyExceptions = new HashSet<string>
                    {
                        "Location",
                        "Type",
                        "BHoM_Guid",
                        "CustomData",
                    },
                NumericTolerance = BH.oM.Geometry.Tolerance.Distance,

            };

            return !Diffing.Query.DifferentProperties(element, other, cc)?.Any() ?? true;
        }
    }
}




