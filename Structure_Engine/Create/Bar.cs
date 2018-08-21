using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar Bar(Line line, ISectionProperty property = null, double orientationAngle = 0, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {          
            return Bar(new Node { Position = line.Start }, new Node { Position = line.End }, property, orientationAngle, release, feaType, name);
        }

        /***************************************************/

        public static Bar Bar(Node startNode, Node endNode, ISectionProperty property = null, double orientationAngle = 0, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural,  string name = "")
        {
            return new Bar
            {
                Name = name,
                StartNode = startNode,
                EndNode = endNode,
                SectionProperty = property,
                Release = release == null ? BarReleaseFixFix() : release,
                FEAType = feaType,
                OrientationAngle = orientationAngle
            };
        }


        /***************************************************/
    }
}
