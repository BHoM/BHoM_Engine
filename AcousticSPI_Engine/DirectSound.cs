using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

using System.Threading;
using System.Threading.Tasks;

using Alea;
using Alea.Parallel;
using OpenCL;
using Cudafy;

using BHoM.Geometry;
using BHoM.Acoustic;


namespace AcousticSPI_Engine
{
    public static class DirectSound
    {
        #region Methods

        /// <summary>
        /// Performs a Direct Sound calculation with sequential CPU calculation
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> Solve(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            {
                for (int i = 0; i < sources.Count; i++)
                {
                    for (int j = 0; j < targets.Count; j++)
                    {
                        List<Point> rayPts = new List<Point>() { sources[i].Position, targets[j].Position };
                        Polyline path = new Polyline(rayPts);
                        rays.Add(new Ray(path, i, j));
                    }
                }
                if (surfaces == null || surfaces.Count == 0) { return rays; }
                else { return Utils.CheckObstacles(rays, surfaces); }
            }
            // else
            //     throw new Exception("Parallel parameter cannot be left blank or be higher than 3. Please specify calculation method: [0] Serial, [1] CPU Threaded, [2] CUDA accelerated. WIP: GPU not working, [3] OpenCL accelerated. WIP: Not Working");
        }

        /// <summary>
        /// Performs a Direct Sound calculation with parallel CPU calculation
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> SolveCpu(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            for (int i = 0; i < sources.Count; i++)
            {
                Parallel.For(0, targets.Count,
                    () => new List<Ray>(),                                        // Initialise local variable
                    (int j, ParallelLoopState state, List<Ray> subray) =>          // body: The delegate that is invoked once per iteration.
                    {
                        Polyline path = new Polyline(new List<Point>() { sources[i].Position, targets[j].Position });
                        subray.Add(new Ray(path, i, j));
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

        /// <summary>
        /// Performs a Direct Sound calculation with GPU parallel calculation based on CUDA
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> SolveCuda(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            Gpu.Default.For(0, sources.Count,
                i =>
                {
                    for (int j = 0; j < targets.Count; j++)
                    {
                        Point[] rayPts = new Point[] { sources[i].Position, targets[j].Position };
                        Polyline path = new Polyline(rayPts.ToList());
                        rays.Add(new Ray(path, i, j));
                    }
                });
            if (surfaces == null || surfaces.Count == 0) { return rays; }
            else { return Utils.CheckObstacles(rays, surfaces); }
        }
        
        /// <summary>
        /// Performs a Direct Sound calculation with GPU parallel calculation based on OpenCL
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> SolveOpenCL(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();

            Cudafy.Host.GPGPU gpu = new Cudafy.Host.OpenCLDevice();
            CudafyModule km = new CudafyModule();
            gpu.LoadModule(km);

            var gpuLatitudes = gpu.CopyToDevice(sources.ToString());
            var gpuLongitudes = gpu.CopyToDevice(targets.ToString());
            //var gpuAnswer = gpu.Allocate<Ray>(10);
            return new List<Ray>();
        }
                               
        #endregion
    }
}
