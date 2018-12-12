﻿using BH.oM.Structure.Properties.Framing;
using BH.oM.Structure.Properties.Section;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConstantFramingElementProperty ConstantFramingElementProperty(ISectionProperty sectionProperty, double orientationAngle, string name = "")
        {
            return new ConstantFramingElementProperty { SectionProperty = sectionProperty, OrientationAngle = orientationAngle, Name = name };
        }

        /***************************************************/
    }
}

