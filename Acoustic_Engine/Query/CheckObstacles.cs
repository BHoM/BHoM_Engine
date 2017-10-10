using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alea;
using Alea.Parallel;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Acoustic;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<Ray> CheckObstacles(this List<Ray> rays, List<Panel> surfaces, bool ClearRays = true, double tol = 0.00001)
        {
            List<Ray> filteredRays = new List<Ray>();
            for (int i = 0; i<rays.Count; i++)                   //foreach ray
            {
                List<bool> checker = new List<bool>();
                for (int j = 0; j < surfaces.Count; j++ )       //foreach surface
                {
                    if (Geometry.Query.GetIntersections(rays[i].Geometry, surfaces[i].Geometry) == null)       // if ray hits a surface
                        checker.Add(true);
                }
                if (ClearRays && checker.Any())     //if rays hits any surface and output is ClearRays
                    filteredRays.Add(rays[i]);
                else if (!ClearRays && !checker.Any())
                    filteredRays.Add(rays[i]);
            }
            return filteredRays;
        }

        /***************************************************/

        public static List<Ray> CheckObstaclesCUDA(this List<Ray> rays, List<Panel> surfaces, bool ClearRays = true, double tol = 0.00001)
        {
            List<Ray> filteredRays = new List<Ray>();

            Gpu.Default.For(0, rays.Count,
                i =>
                {
                    List<bool> checker = new List<bool>();
                    for (int j = 0; j < surfaces.Count; j++)       //foreach surface
                    {
                        if (Geometry.Query.GetIntersections(rays[i].Geometry, surfaces[i].Geometry) == null)       // if ray hits a surface
                            checker.Add(true);
                    }
                    if (ClearRays && checker.Any())     //if rays hits any surface and output is ClearRays
                        filteredRays.Add(rays[i]);
                    else if (!ClearRays && !checker.Any())
                        filteredRays.Add(rays[i]);
                });
            return filteredRays;
        }

        /***************************************************/

        public static List<Ray> CheckObstaclesCPU(this List<Ray> rays, List<Panel> surfaces, bool ClearRays = true, double tol = 0.00001)    // NOT EFFICIENT for now, since it locks the rays list to be thread safe
        {
            List<Ray> filteredRays = new List<Ray>();

            Parallel.For(0, rays.Count,
                () => new List<Ray>(),
                (int i, ParallelLoopState loop, List<Ray> localRay) =>
                {
                    List<bool> localCheck = new List<bool>();
                    for (int j = 0; j < surfaces.Count; j++)       //foreach surface
                    {
                        if (Geometry.Query.GetIntersections(rays[i].Geometry, surfaces[i].Geometry) == null)
                            localCheck.Add(true);
                    }
                    if (ClearRays && localCheck.Any())     //if rays hits any surface and output is ClearRays
                        localRay.Add(rays[i]);
                    else if (!ClearRays && !localCheck.Any())
                        localRay.Add(rays[i]);
                    return localRay;
                },
                (localRay) =>
                {
                    lock (rays)
                    {
                        filteredRays.AddRange(localRay);
                    }
                    
                });
            return filteredRays;
        }
    }
}
