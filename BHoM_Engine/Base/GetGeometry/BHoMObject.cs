using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry IGetGeometry(this BHoMObject obj)
        {
            return GetGeometry(obj as dynamic);
        }

        /***************************************************/

        public static IBHoMGeometry GetGeometry(this CustomObject obj)
        {
            List<IBHoMGeometry> geometries = new List<IBHoMGeometry>();

            foreach (object item in obj.CustomData.Values)
            {
                IBHoMGeometry geometry = item.GetGeometry();
                if (geometry != null)
                    geometries.Add(geometry);
            }

            return new CompositeGeometry(geometries);
        }

        /***************************************************/

        private static IBHoMGeometry GetGeometry(this object obj)
        {
            if (obj is IBHoMGeometry)
                return obj as IBHoMGeometry;
            else if (obj is BHoMObject)
                return ((BHoMObject)obj).IGetGeometry();
            else if (obj is IEnumerable)
            {
                List<IBHoMGeometry> geometries = new List<IBHoMGeometry>();
                foreach (object item in (IEnumerable)obj)
                {
                    IBHoMGeometry geometry = item.GetGeometry();
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

        private static IBHoMGeometry GetGeometry(this BHoMObject obj)
        {
            return null;
        }
    }
}
