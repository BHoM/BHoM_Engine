using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BH.oM.Acoustic;
using BH.oM.Geometry;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        [Cudafy]
        public static void thekernel(GThread thread, Speaker[] sources, Receiver[] targets, Ray[] rays)
        {
            int tId = thread.threadIdx.x;
            rays[tId] = new Ray(sources[tId].Geometry.Polyline(targets[tId].Geometry), tId, tId);
        }

        /***************************************************/

        public static Ray[] DirectSoundGPU(Speaker[] sources, Receiver[] targets)
        {
            CudafyModes.Target = eGPUType.OpenCL;
            CudafyModes.DeviceId = 2;
            CudafyTranslator.Language = eLanguage.OpenCL;

            CudafyModule km = CudafyTranslator.Cudafy(typeof(Speaker), typeof(Receiver), typeof(Ray));

            GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
            gpu.LoadModule(km);

            Ray[] rays = new Ray[sources.Length * targets.Length];

            Speaker[] dev_sources = gpu.Allocate<Speaker>(sources.Length);
            Receiver[] dev_targets = gpu.Allocate<Receiver>(targets.Length);
            Ray[] dev_rays = gpu.Allocate<Ray>(sources.Length * targets.Length);

            gpu.CopyToDevice(sources, dev_sources);
            gpu.CopyToDevice(targets, dev_targets);

            gpu.Launch(1, sources.Length * targets.Length, (Action<GThread, Speaker[], Receiver[], Ray[]>)thekernel, dev_sources, dev_targets, dev_rays);
            gpu.CopyFromDevice(dev_rays, rays);

            gpu.FreeAll();
            return rays;
        }

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
    }
}