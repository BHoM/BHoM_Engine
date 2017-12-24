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

        public static IBHoMGeometry IGeometry(this BHoMObject obj)
        {
            return Geometry(obj as dynamic);
        }

        /***************************************************/

        public static IBHoMGeometry Geometry(this CustomObject obj)
        {
            List<IBHoMGeometry> geometries = new List<IBHoMGeometry>();

            foreach (object item in obj.CustomData.Values)
            {
                IBHoMGeometry geometry = item.Geometry();
                if (geometry != null)
                    geometries.Add(geometry);
            }

            return new CompositeGeometry(geometries);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IBHoMGeometry Geometry(this object obj)
        {
            if (obj is IBHoMGeometry)
                return obj as IBHoMGeometry;
            else if (obj is BHoMObject)
                return ((BHoMObject)obj).IGeometry();
            else if (obj is IEnumerable)
            {
                List<IBHoMGeometry> geometries = new List<IBHoMGeometry>();
                foreach (object item in (IEnumerable)obj)
                {
                    IBHoMGeometry geometry = item.Geometry();
                    if (geometry != null)
                        geometries.Add(geometry);
                }
                if (geometries.Count() > 0)
                    return new CompositeGeometry(geometries);
                else
                    return null;
            }
            else
                return null;   
        }

        /***************************************************/

        private static IBHoMGeometry Geometry(this BHoMObject obj)
        {
            return null;
        }

        /***************************************************/
    }
}
