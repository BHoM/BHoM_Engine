using BH.oM.Structural.Properties;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConstantFramingElementProperty FramingElement(ISectionProperty sectionProperty, double orientationAngle, string name = "")
        {
            return new ConstantFramingElementProperty { SectionProperty = sectionProperty, OrientationAngle = orientationAngle, Name = name };
        }

        /***************************************************/
    }
}

