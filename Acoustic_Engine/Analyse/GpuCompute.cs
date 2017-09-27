using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

namespace BH.Engine.Acoustic
{
    public static partial class Analyse
    {
        [Cudafy]
        public static GPGPU InitialiseOpenCLDevice()
        {
            GPGPU gpu = new OpenCLDevice();
            CudafyModule km = CudafyTranslator.Cudafy();
            gpu.Launch().thekernel();
            return gpu;
        }

        [Cudafy]
        public static void thekernel(GThread thread, object in0, object in1, object out0)
        {
            int threadID = thread.threadIdx.x;

        }

    }
}
