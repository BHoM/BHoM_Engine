using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BHoM.Geometry
{
    public static class XGeometryBase
    {
        public static void Transform(this GeometryBase geometry, Transform t)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Transform(geometry as Point, t);
                    break;
                case BHoM.Geometry.GeometryType.Vector:
                    XVector.Transform(geometry as Vector, t);
                    break;
                case BHoM.Geometry.GeometryType.Plane:
                    XPlane.Transform(geometry as Plane, t);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    XCurve.Transform(geometry as Curve, t);
                    break;
                case BHoM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Transform(geometry as PolyCurve, t);
                    break;
                case BHoM.Geometry.GeometryType.Surface:
                    XSurface.Transform(geometry as Surface, t);
                    break;
                case BHoM.Geometry.GeometryType.PolySurface:
                    break;
                case BHoM.Geometry.GeometryType.Loft:
                    XLoft.Transform(geometry as Loft, t);
                    break;
                case BHoM.Geometry.GeometryType.Extrusion:
                case BHoM.Geometry.GeometryType.Pipe:
                case BHoM.Geometry.GeometryType.Mesh:
                    break;
                case BHoM.Geometry.GeometryType.Group:
                    XGroup.Transform(geometry as IGroup, t);
                    break;
            }

        }

        public static void Translate(this GeometryBase geometry, Vector v)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Translate(geometry as Point, v);
                    break;
                case BHoM.Geometry.GeometryType.Vector:
                    XVector.Translate(geometry as Vector, v);
                    break;
                case BHoM.Geometry.GeometryType.Plane:
                    XPlane.Translate(geometry as Plane, v);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    XCurve.Translate(geometry as Curve, v);
                    break;
                case BHoM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Translate(geometry as PolyCurve, v);
                    break;
                case BHoM.Geometry.GeometryType.Surface:
                    XSurface.Translate(geometry as Surface, v);
                    break;
                case BHoM.Geometry.GeometryType.PolySurface:
                    XPolySurface.Translate(geometry as PolySurface, v);
                    break;
                case BHoM.Geometry.GeometryType.Loft:
                    XLoft.Translate(geometry as Loft, v);
                    break;
                case BHoM.Geometry.GeometryType.Extrusion:
                case BHoM.Geometry.GeometryType.Pipe:
                case BHoM.Geometry.GeometryType.Mesh:
                    break;
                case BHoM.Geometry.GeometryType.Group:
                    XGroup.Translate(geometry as IGroup, v);
                    break;
            }
        }

        public static void Mirror(this GeometryBase geometry, Plane p)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Mirror(geometry as Point, p);
                    break;
                case BHoM.Geometry.GeometryType.Vector:
                    XVector.Mirror(geometry as Vector, p);
                    break;
                case BHoM.Geometry.GeometryType.Plane:
                    XPlane.Mirror(geometry as Plane, p);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    XCurve.Mirror(geometry as Curve, p);
                    break;
                case BHoM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Mirror(geometry as PolyCurve, p);
                    break;
                case BHoM.Geometry.GeometryType.Surface:
                    XSurface.Mirror(geometry as Surface, p);
                    break;
                case BHoM.Geometry.GeometryType.PolySurface:
                    break;
                case BHoM.Geometry.GeometryType.Loft:
                    XLoft.Mirror(geometry as Loft, p);
                    break;
                case BHoM.Geometry.GeometryType.Extrusion:
                case BHoM.Geometry.GeometryType.Pipe:
                case BHoM.Geometry.GeometryType.Mesh:
                    break;
                case BHoM.Geometry.GeometryType.Group:
                    XGroup.Mirror(geometry as IGroup, p);
                    break;
            }
        }

        public static void Project(this GeometryBase geometry, Plane p)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.Point:
                    XPoint.Project(geometry as Point, p);
                    break;
                case BHoM.Geometry.GeometryType.Vector:
                    XVector.Project(geometry as Vector, p);
                    break;
                case BHoM.Geometry.GeometryType.Plane:
                    XPlane.Project(geometry as Plane, p);
                    break;
                case GeometryType.Arc:
                case GeometryType.Circle:
                case GeometryType.Line:
                case GeometryType.Polyline:
                case GeometryType.NurbCurve:
                    break;
                case BHoM.Geometry.GeometryType.PolyCurve:
                    XPolyCurve.Project(geometry as PolyCurve, p);
                    break;
                case BHoM.Geometry.GeometryType.Surface:
                    XSurface.Project(geometry as Surface, p);
                    break;
                case BHoM.Geometry.GeometryType.PolySurface:
                    break;
                case BHoM.Geometry.GeometryType.Loft:
                    XLoft.Project(geometry as Loft, p);
                    break;
                case BHoM.Geometry.GeometryType.Extrusion:
                case BHoM.Geometry.GeometryType.Pipe:
                case BHoM.Geometry.GeometryType.Mesh:
                    break;
                case BHoM.Geometry.GeometryType.Group:
                    XGroup.Project(geometry as IGroup, p);
                    break;
            }
        }
    }
}
