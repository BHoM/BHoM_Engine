using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural Pile. This object can be used with a PileFoundation or as a standalone foundation.")]
        [Input("line", "The definining geometry for the pile.")]
        [InputFromProperty("sectionProperty")]
        [InputFromProperty("orientationAngle")]
        [Output("pile", "The created Pile with a centreline matching the provided geometrical Line.")]
        public static Pile Pile(Line line, ISectionProperty pileSection = null, double orientationAngle = 0)
        {
            return new Pile() { TopNode = (Node)line.Start, BottomNode = (Node)line.End, Section = pileSection, OrientationAngle = orientationAngle };
        }

        /***************************************************/

        [Description("Creates a structural Pile. This object can be used with a PileFoundation or as a standalone foundation.")]
        [Input("topNode", "The node at the top of the pile.")]
        [Input("bottomNode", "The node at the bottom of the pile.")]
        [InputFromProperty("sectionProperty")]
        [InputFromProperty("orientationAngle")]
        [Output("pile", "The created Pile with a centreline defined by the provided nodes.")]
        public static Pile Pile(Node topNode, Node bottomNode, ISectionProperty pileSection = null, double orientationAngle = 0)
        {
            return new Pile() { TopNode = topNode, BottomNode = bottomNode, Section = pileSection, OrientationAngle = orientationAngle };
        }

        /***************************************************/

    }
}