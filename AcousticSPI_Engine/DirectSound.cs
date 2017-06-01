using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

using System.Threading;
using System.Threading.Tasks;

using Alea;
using Alea.Parallel;

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
        public static List<Ray> Solve(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces, int parallel)
        {
            List<Ray> rays = new List<Ray>();

            if (parallel == 0)
            {
                for (int i = 0; i < sources.Count; i++)
                {
                    for (int j = 0; j < targets.Count; j++)
                    {
                        List<Point> rayPts = new List<Point>() { sources[i].Position, targets[j].Position };
                        Polyline path = new Polyline(rayPts);
                        rays.Add(new Ray(path, "S" + i.ToString(), "R" + j.ToString()));
                    }
                }
                if (surfaces == null || surfaces.Count == 0) { return rays; }
                else { return Utils.CPUCheckObstacles(rays, surfaces); }
            }
            
            else if (parallel == 1)
            {
                for (int i = 0; i < sources.Count; i++)
                {
                    Parallel.For(0, targets.Count,
                        () => new List<Ray>(),                                        // Initialise local variable
                        (int j, ParallelLoopState state, List<Ray> subray) =>          // body: The delegate that is invoked once per iteration.
                        {
                            Polyline path = new Polyline(new List<Point>() { sources[i].Position, targets[j].Position });
                            subray.Add(new Ray(path, "S" + i.ToString(), "R" + j.ToString()));
                            return subray;
                        },
                        (subray) =>
                        {
                            lock (sources)
                            {
                                rays.AddRange(subray);
                            }
                        }
                    );
                }
                if (surfaces == null || surfaces.Count == 0) { return rays; }
                else { return Utils.CheckObstacles(rays, surfaces); }
            }
            /*
            else if (parallel == 2)
            {
                // GPU accelerated ######################################   ##
                Gpu.Default.For(0, sources.Count,
                    i =>
                    {
                        for (int j = 0; j < targets.Count; j++)
                        {
                            List<Point> rayPts = new List<Point>() { sources[i].Position, targets[j].Position };
                            Polyline path = new Polyline(rayPts);
                            rays.Add(new Ray(path, "S" + i.ToString(), "R" + j.ToString()));
                        }
                    });
                if (surfaces == null || surfaces.Count == 0) { return rays; }
                else { return Utils.CPUCheckObstacles(rays, surfaces); }
            }
            */

            else
                throw new Exception("Parallel parameter cannot be left blank or be higher than 2. Please specify calculation method: [0] Serial, [1] CPU Threaded, [2] GPU Threaded. WIP: GPU not working");
        }

        #endregion
    }
}
