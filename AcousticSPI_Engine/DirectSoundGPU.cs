using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using BHoM.Acoustic;
using BHoM.Geometry;

using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

namespace AcousticSPI_Engine
{
    public static class DirectSoundGPU
    {
        [Cudafy]
        public static void thekernel(GThread thread, Speaker[] sources, Receiver[] targets, Ray[] rays)
        {
            int tId = thread.threadIdx.x;
            rays[tId] = new Ray(new Line(sources[tId].Position, targets[tId].Position), tId, tId);
        }
        
        public static Ray[] Solve(Speaker[] sources, Receiver[] targets)
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
    }
}