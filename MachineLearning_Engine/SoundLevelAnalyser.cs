using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.Audio;

namespace MachineLearning_Engine
{
    public class SoundLevelAnalyser
    {
        public class Config
        {
            public Config()
            {
                StartFrame = 0;
                EndFrame = int.MaxValue;
                FrameStep = 1;
                OutFolder = "";
            }

            public int StartFrame;
            public int EndFrame;
            public int FrameStep;
            public string OutFolder;
        }


        public SoundLevelAnalyser()
        {
        }


        /****************************************/
        /****  Public Methods                ****/
        /****************************************/

        public async Task<Dictionary<int, double>> Run(string audioFileName, Config config)
        {
            m_OutFolder = config.OutFolder;
            if (!m_OutFolder.EndsWith("\\"))
                m_OutFolder = m_OutFolder + "\\";

            m_FrameIndex = 0;
            m_Config = config;
            m_SoundLevel = new Dictionary<int, double>();

            m_AudioSource = new Accord.DirectSound.WaveFileAudioSource(audioFileName);
            m_AudioSource.NewFrame += NewFrame;
            m_AudioSource.Start();

            m_AudioSource.WaitForStop();
            SaveData();

            return m_SoundLevel;
        }

        


        /****************************************/
        /****  Private Fields & Methods      ****/
        /****************************************/

        Accord.DirectSound.WaveFileAudioSource m_AudioSource;

        Dictionary<int, double> m_SoundLevel;
        private int m_FrameIndex;
        private Config m_Config;
        private string m_OutFolder;

        /****************************************/

        private void NewFrame(object sender, NewFrameEventArgs e)
        {
            if (m_FrameIndex > m_Config.EndFrame)
            {
                m_AudioSource.SignalToStop();
                return;
            }

            if (m_FrameIndex >= m_Config.StartFrame)
            {
                m_SoundLevel[m_FrameIndex] = e.Signal.GetEnergy();
                Console.WriteLine("Frame " + m_FrameIndex);
            }

            m_FrameIndex++;
        }


        private void SaveData()
        {
            Console.WriteLine("Saving data");
            if (m_OutFolder != "")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(m_OutFolder + "sound.csv");
                file.WriteLine("frame, sound level");

                foreach (KeyValuePair<int, double> kvp in m_SoundLevel)
                {
                    file.WriteLine(kvp.Key + "," + kvp.Value);
                }

                file.Close();
            }
        }

    }
}
