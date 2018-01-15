using Engine_Explore.BHoM.Base;
using Engine_Explore.BHoM.Geometry;
using Engine_Explore.BHoM.Structural.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Structural.Elements
{
    public class Bar : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Node StartNode { get; set; } = new Node();

        public Node EndNode { get; set; } = new Node();

        public SectionProperty SectionProperty { get; set; }

        /*public BarRelease Release { get; set; }

        public BarConstraint Spring { get; set; }

        public Offset Offset { get; set; }

        public BarStructuralUsage StructuralUsage { get; set; }

        public BarFEAType FEAType { get; set; }*/


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Bar() { }

        /***************************************************/

        public Bar(Point start, Point end, string name = "")
        {
            StartNode = new Node(start);
            EndNode = new Node(end);
            Name = name;
        }

        /***************************************************/

        public Bar(Node start, Node end, string name = "")
        {
            StartNode = start;
            EndNode = end;
            Name = name;
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
