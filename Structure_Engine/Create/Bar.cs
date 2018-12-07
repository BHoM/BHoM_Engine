using System;

using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.Properties;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

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

        public static Bar Bar(Line line, ISectionProperty property = null, Vector normal =  null, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {
            double orientationAngle;

            if (normal == null)
                orientationAngle = 0;
            else
            {
                normal = normal.Normalise();
                Vector tan = (line.End - line.Start).Normalise();

                double dot = normal.DotProduct(tan);

                if (Math.Abs(1 - dot) < oM.Geometry.Tolerance.Angle)
                {
                    Reflection.Compute.RecordError("The normal is parallell to the centreline of the bar");
                    return null;
                }
                else if (Math.Abs(dot) > oM.Geometry.Tolerance.Angle)
                {
                    Reflection.Compute.RecordWarning("Normal not othogonal to the centreline and will get projected");
                }

                Vector reference;

                if (!line.IsVertical())
                    reference = Vector.ZAxis;
                else
                {
                    reference = tan.CrossProduct(Vector.YAxis);
                }

                orientationAngle = reference.Angle(normal, new Plane { Normal = tan });

            }
                

            return Bar(new Node { Position = line.Start }, new Node { Position = line.End }, property, orientationAngle, release, feaType, name);
        }

        /***************************************************/
    }
}
