using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.oM.Geometry
{
    public static class XBHoMGeometry
    {
        public static void Transform(this BHoMGeometry geometry, Transform t)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Transform(geometry as Point, t);
                    break;
                case BH.oM.Geometry.GeometryType.Vector:
                    XVector.Transform(geometry as Vector, t);
                    break;
                case BH.oM.Geometry.GeometryType.Plane:
                    XPlane.Transform(geometry as Plane, t);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    XCurve.Transform(geometry as Curve, t);
                    break;
                case BH.oM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Transform(geometry as PolyCurve, t);
                    break;
                case BH.oM.Geometry.GeometryType.Surface:
                    XSurface.Transform(geometry as Surface, t);
                    break;
                case BH.oM.Geometry.GeometryType.PolySurface:
                    break;
                case BH.oM.Geometry.GeometryType.Loft:
                    XLoft.Transform(geometry as Loft, t);
                    break;
                case BH.oM.Geometry.GeometryType.Extrusion:
                case BH.oM.Geometry.GeometryType.Pipe:
                case BH.oM.Geometry.GeometryType.Mesh:
                    break;
                case BH.oM.Geometry.GeometryType.Group:
                    XGroup.Transform(geometry as IGroup, t);
                    break;
            }

        }

        public static void Translate(this BHoMGeometry geometry, Vector v)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Translate(geometry as Point, v);
                    break;
                case BH.oM.Geometry.GeometryType.Vector:
                    XVector.Translate(geometry as Vector, v);
                    break;
                case BH.oM.Geometry.GeometryType.Plane:
                    XPlane.Translate(geometry as Plane, v);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    XCurve.Translate(geometry as Curve, v);
                    break;
                case BH.oM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Translate(geometry as PolyCurve, v);
                    break;
                case BH.oM.Geometry.GeometryType.Surface:
                    XSurface.Translate(geometry as Surface, v);
                    break;
                case BH.oM.Geometry.GeometryType.PolySurface:
                    XPolySurface.Translate(geometry as PolySurface, v);
                    break;
                case BH.oM.Geometry.GeometryType.Loft:
                    XLoft.Translate(geometry as Loft, v);
                    break;
                case BH.oM.Geometry.GeometryType.Extrusion:
                case BH.oM.Geometry.GeometryType.Pipe:
                case BH.oM.Geometry.GeometryType.Mesh:
                    break;
                case BH.oM.Geometry.GeometryType.Group:
                    XGroup.Translate(geometry as IGroup, v);
                    break;
            }
        }

        public static void Mirror(this BHoMGeometry geometry, Plane p)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Mirror(geometry as Point, p);
                    break;
                case BH.oM.Geometry.GeometryType.Vector:
                    XVector.Mirror(geometry as Vector, p);
                    break;
                case BH.oM.Geometry.GeometryType.Plane:
                    XPlane.Mirror(geometry as Plane, p);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    XCurve.Mirror(geometry as Curve, p);
                    break;
                case BH.oM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Mirror(geometry as PolyCurve, p);
                    break;
                case BH.oM.Geometry.GeometryType.Surface:
                    XSurface.Mirror(geometry as Surface, p);
                    break;
                case BH.oM.Geometry.GeometryType.PolySurface:
                    break;
                case BH.oM.Geometry.GeometryType.Loft:
                    XLoft.Mirror(geometry as Loft, p);
                    break;
                case BH.oM.Geometry.GeometryType.Extrusion:
                case BH.oM.Geometry.GeometryType.Pipe:
                case BH.oM.Geometry.GeometryType.Mesh:
                    break;
                case BH.oM.Geometry.GeometryType.Group:
                    XGroup.Mirror(geometry as IGroup, p);
                    break;
            }
        }

        public static void Project(this BHoMGeometry geometry, Plane p)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Project(geometry as Point, p);
                    break;
                case BH.oM.Geometry.GeometryType.Vector:
                    XVector.Project(geometry as Vector, p);
                    break;
                case BH.oM.Geometry.GeometryType.Plane:
                    XPlane.Project(geometry as Plane, p);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    break;
                case BH.oM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Project(geometry as PolyCurve, p);
                    break;
                case BH.oM.Geometry.GeometryType.Surface:
                    XSurface.Project(geometry as Surface, p);
                    break;
                case BH.oM.Geometry.GeometryType.PolySurface:
                    break;
                case BH.oM.Geometry.GeometryType.Loft:
                    XLoft.Project(geometry as Loft, p);
                    break;
                case BH.oM.Geometry.GeometryType.Extrusion:
                case BH.oM.Geometry.GeometryType.Pipe:
                case BH.oM.Geometry.GeometryType.Mesh:
                    break;
                case BH.oM.Geometry.GeometryType.Group:
                    XGroup.Project(geometry as IGroup, p);
                    break;
            }
        }
    }
}
