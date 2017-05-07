using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Acoustic;


namespace AcousticSPI_Engine
{
    public static class DirectSound
    {
        #region Methods

        /// <summary>
        /// Performs a Direct Sound calculation with obstacles check.
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> Solve(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            for (int i = 0; i < sources.Count; i++)
            {
                for (int j = 0; j < targets.Count; i++)
                {
                    List<Point> rayPts = new List<Point>() { sources[i].Position, targets[j].Position };
                    Polyline path = new Polyline(rayPts);
                    Ray ray = new Ray(path, "S"+i.ToString(), "R"+j.ToString());
                    rays.Add(ray);
                }
            }
            return Generic.CheckObstacles(rays, surfaces);
        }

        #endregion
    }
}
