using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Base;
using BHoM.Geometry;
using BHoM.Acoustic;

namespace AcousticSPI_Engine
{
    public static class Generic
    {

        /// <summary>
        /// Checks for obstacles in rays path. 
        /// </summary>
        /// <param name="rays">Acoustic rays list</param>
        /// <param name="surfaces">Acoustic surface list</param>
        /// <param name="ClearRays">Set true to output clear rays, false if blind rays.</param>
        /// <param name="tol">Default tolerance to 0.00001. Beware of global unit setup.</param>
        /// <returns>Returns a new list of filtered rays, either clear or blind ones.</returns>
        public static List<Ray> CheckObstacles(List<Ray> rays, List<Panel> surfaces, bool ClearRays = true, double tol = 0.00001)
        {
            List<Ray> filteredRays = new List<Ray>();
            for (int i = 0; i<rays.Count; i++)                   //foreach ray
            {
                List<bool> checker = new List<bool>();
                for (int j = 0; j < surfaces.Count; i++ )       //foreach surface
                {
                    // Write Mesh-Polyline (or Mesh-Curve) Intersection and replace when done, due to performance.
                    if (Intersect.PlaneCurve(surfaces[j].mPlane(), rays[i].Path, tol).Count == 0)       // if ray hits a surface
                        checker.Add(true);
                }
                if (ClearRays && checker.Any())     //if rays hits any surface and output is ClearRays
                    filteredRays.Add(rays[i]);
                else if (!ClearRays && !checker.Any())
                    filteredRays.Add(rays[i]);
            } return filteredRays;
        }


        /// <summary>
        /// Filter ray 
        /// </summary>
        /// <param name="rays"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Ray> RayFilter(List<Ray> rays, List<string> filter)
        {
            //IEnumerable<Ray> Rays = rays;
            //return Rays.Where(ray => filter.Contains(ray.Source ));
            return rays.FindAll(ray => filter.Contains(ray.Source));
        }
    }
}
