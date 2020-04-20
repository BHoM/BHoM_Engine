using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;
using BH.Engine.Diffing;
using BH.oM.Diffing;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object. Environment Edges have no additional data other than their geometry, so this will always return true")]
        [Input("element", "An Environment Edge to compare the properties of with an other Environment Edge")]
        [Input("other", "The Environment Edge to compare with the other Environment Edge.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(Edge element, Edge other)
        {
            return true; //Environment Edges have no additional data to be checked so geometric edges can be merged
        }

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object")]
        [Input("element", "An Environment Panel to compare the properties of with an other Environment Panel")]
        [Input("other", "The Environment Panel to compare with the other Environment Panel.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(Panel element, Panel other)
        {
            DiffConfig config = new DiffConfig()
            {
                PropertiesToIgnore = new List<string>
                {
                    "ExternalEdges",
                    "Openings",
                    "ConnectedSpaces",
                    "Type",
                },
                NumericTolerance = BH.oM.Geometry.Tolerance.Distance,
            };

            string elementDiff = element.DiffingHash(config);
            string otherDiff = other.DiffingHash(config);

            return elementDiff.Equals(otherDiff);
        }

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object")]
        [Input("element", "An Environment Opening to compare the properties of with an other Environment Opening")]
        [Input("other", "The Environment Opening to compare with the other Environment Opening.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object")]
        public static bool HasMergeablePropertiesWith(Opening element, Opening other)
        {
            DiffConfig config = new DiffConfig()
            {
                PropertiesToIgnore = new List<string>
                {
                    "Edges",
                    "FrameFactorValue",
                    "InnerEdges",
                    "Type",
                },
                NumericTolerance = BH.oM.Geometry.Tolerance.Distance,
            };

            string elementDiff = element.DiffingHash(config);
            string otherDiff = other.DiffingHash(config);

            return elementDiff.Equals(otherDiff);
        }
    }
}
