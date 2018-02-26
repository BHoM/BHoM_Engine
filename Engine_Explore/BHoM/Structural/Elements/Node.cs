using Engine_Explore.BHoM.Base;
using Engine_Explore.BHoM.Structural.Properties;
using Engine_Explore.BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Structural.Elements
{
    public class Node : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Point Point { get; set; } = new Point();

        public NodeConstraint Constraint { get; set; } = new NodeConstraint();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Node() { }

        /***************************************************/

        public Node(Point pt, string name = "")
        {
            Point = pt;
            Name = name;
        }

        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
