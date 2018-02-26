using BH.oM.Base;
using BH.oM.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IGeometry IGeometry(this IBHoMObject obj)
        {
            return Geometry(obj as dynamic);
        }

        /***************************************************/

        public static IGeometry Geometry(this CustomObject obj)
        {
            List<IGeometry> geometries = new List<IGeometry>();

            foreach (object item in obj.CustomData.Values)
            {
                IGeometry geometry = item.Geometry();
                if (geometry != null)
                    geometries.Add(geometry);
            }

            return new CompositeGeometry { Elements = geometries.ToList() };
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IGeometry Geometry(this object obj)
        {
            if (obj is IGeometry)
                return obj as IGeometry;
            else if (obj is IBHoMObject)
                return ((IBHoMObject)obj).IGeometry();
            else if (obj is IEnumerable)
            {
                List<IGeometry> geometries = new List<IGeometry>();
                foreach (object item in (IEnumerable)obj)
                {
                    IGeometry geometry = item.Geometry();
                    if (geometry != null)
                        geometries.Add(geometry);
                }
                if (geometries.Count() > 0)
                    return new CompositeGeometry { Elements = geometries.ToList() };
                else
                    return null;
            }
            else
                return null;   
        }

        /***************************************************/

        private static IGeometry Geometry(this IBHoMObject obj)
        {
            return Reflection.Compute.RunExtentionMethod(obj, "Geometry") as IGeometry;
        }

        /***************************************************/
    }
}
