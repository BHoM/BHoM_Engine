using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using Alea;
using Alea.Parallel;
using OpenCL.Net;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using BH.oM.Geometry;
using BH.oM.Acoustic;


namespace BH.Engine.Acoustic
{
    public static partial class Analyse
    {
        #region Methods
        
        /// <summary>
        /// Performs a Direct Sound calculation with sequential CPU calculation
        /// </summary>
        /// <param name="source">BHoM acoustic Speaker</param>
        /// <param name="target">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static Ray DirectSound(Speaker source, Receiver target, List<Panel> surfaces)
        {
            Line path = new Line(source.Position, target.Position);
            Ray ray = new Ray(path, source.SpeakerID, target.ReceiverID);
            return Verify.CheckObstacles(ray,surfaces) ? ray : default(Ray);
        }
        
        /// <summary>
        /// Performs a Direct Sound calculation with sequential CPU calculation
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> DirectSound(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            {
                for (int i = 0; i < sources.Count; i++)
                {
                    for (int j = 0; j < targets.Count; j++)
                    {
                        Line path = new Line(sources[i].Position, targets[j].Position);
                        rays.Add(new Ray(path, i, j));
                    }
                }
                if (surfaces == null || surfaces.Count == 0) { return rays; }
                else { return Verify.CheckObstacles(rays, surfaces); }
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
        public static List<Ray> DirectSoundCPU(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            /*
            ConcurrentBag<Ray> rays = new ConcurrentBag<Ray>();
            //List<Ray> rays = new List<Ray>();
            for (int i = 0; i < sources.Count; i++)
            {
                Parallel.For(
                    0,
                    targets.Count,
                    new ParallelOptions { MaxDegreeOfParallelism = System.Environment.ProcessorCount },
                    () => new List<Ray>(),                                        // Initialise local variable
                    (int j, ParallelLoopState state, List<Ray> subray) =>          // body: The delegate that is invoked once per iteration.
                    {
                        Line path = new Line(sources[i].Position, targets[j].Position);
                        subray.Add(new Ray(path, i, j));
                        return subray;
                    },
                    (subray) =>
                        {
                            foreach(Ray ray in subray)
                            rays.Add(ray);
                        }
                );
            }
            */

            //List<Ray> rays = new List<Ray>();
            List<Ray> rays = new List<Ray>();
            for (int i = 0; i < sources.Count; i++)
            {
                for (int j = 0; j < targets.Count; j++)
                {
                    int a = i;
                    int b = j;
                    int pLevel = TaskScheduler.Current.MaximumConcurrencyLevel;
                    Task t = Task.Factory.StartNew(() =>
                   {
                       Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                       rays.Add(new Ray(new Line(sources[i].Position, targets[j].Position), a, b))
                       )
                       );
                   });
                    t.Wait();
                }
            }

            //if (surfaces == null || surfaces.Count == 0) { return rays.ToList(); }
            //else { return Verify.CheckObstacles(rays.ToList(), surfaces); }
            return rays;
        }


        /// <summary>
        /// Performs a Direct Sound calculation with GPU parallel calculation based on CUDA
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        [GpuManaged]
        public static List<Ray> DirectSoundCuda(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            Gpu.Default.For(0, sources.Count,
                   i =>
                   {
                       for (int j = 0; j < targets.Count; j++)
                       {
                           List<Point> rayPts = new List<Point>() { sources[i].Position, targets[j].Position };
                           Polyline path = new Polyline(rayPts);
                           rays.Add(new Ray(path, i, j));
                       }
                   });
            if (surfaces == null || surfaces.Count == 0) { return rays; }
            else { return Verify.CheckObstacles(rays, surfaces); }
        }

        /// <summary>
        /// Performs a Direct Sound calculation with GPU parallel calculation based on OpenCL
        /// </summary>
        /// <param name="sources">BHoM acoustic Speaker</param>
        /// <param name="targets">BHoM acoustic Receivers</param>
        /// <param name="surfaces">BHoM acoustic Panel</param>
        /// <returns>Returns a list of BHoM Acoustic Rays</returns>
        public static List<Ray> DirectSoundOpenCL(List<Speaker> sources, List<Receiver> targets, List<Panel> surfaces)
        {
            // Formatting inputs
            Speaker[] _sources = sources.ToArray();
            Receiver[] _targets = targets.ToArray();
            Panel[] _surfaces = surfaces.ToArray();

            // Defining the context
            CudafyModes.Target = eGPUType.OpenCL;
            CudafyModes.DeviceId = 2;
            //CudafyTranslator.Language = eLanguage.OpenCL;

            // Initialise the computing device
            GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
            CudafyModule km = CudafyTranslator.Cudafy(typeof(Speaker), typeof(Receiver), typeof(Panel), typeof(Ray), typeof(Analyse));
            gpu.LoadModule(km);

            Speaker[] dev_sources = gpu.Allocate<Speaker>(sources.Count);
            Receiver[] dev_targets = gpu.Allocate<Receiver>(targets.Count);
            Panel[] dev_surfaces = gpu.Allocate<Panel>(surfaces.Count);

            gpu.CopyToDevice(_sources, dev_sources);
            gpu.CopyToDevice(_targets);
            gpu.CopyToDevice(_surfaces);

            Ray[] dev_rays = gpu.Allocate<Ray>(sources.Count * targets.Count);

            //gpu.Launch(1, sources.Count, ((Action<GThread, >)), dev_sources, dev_targets, dev_surfaces);


            return new List<Ray>();
        }
                               
        #endregion
    }
}
