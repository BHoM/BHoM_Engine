using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar Bar(Line line, ISectionProperty property = null, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {          
            return Bar(new Node { Position = line.Start }, new Node { Position = line.End }, property, release, feaType, name);
        }

        /***************************************************/

        public static Bar Bar(Node startNode, Node endNode, ISectionProperty property = null, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {
            release = release == null ? new BarRelease() { StartRelease = FixConstraint6DOF(), EndRelease = FixConstraint6DOF() } : release;
            return new Bar
            {
                Name = name,
                StartNode = startNode,
                EndNode = endNode,
                SectionProperty = property,
                Release = release,
                FEAType = feaType
            };
        }


        /***************************************************/
    }
}
