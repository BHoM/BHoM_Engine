using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Geometry
{
    public abstract class BHoMGeometry
    {
        protected BHoMGeometry() { }

        public BHoMGeometry ShallowClone()
        {
            return (BHoMGeometry)this.MemberwiseClone();
        }


        public abstract BHoMGeometry.Type GetGeometryType();

        public enum Type
        {
            Point,
            Vector,
            Plane,
            Mesh,
            Arc,
            Circle,
            Line,
            Polyline,
            PolyCurve,
            NurbCurve,
            Surface,
            Loft,
            Pipe,
            Extrusion,
            PolySurface,
            Group
        }
    }
}
