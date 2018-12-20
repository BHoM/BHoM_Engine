/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using Accord.Audio;

namespace BH.Engine.MachineLearning
{
    public static partial class Compute
    {
        /****************************************/
        /****  Public Methods                ****/
        /****************************************/

        public static void SoundLevel(string audioFileName, int startFrame = 0, int endFrame = int.MaxValue, int frameStep = 1, string outFolder = "")
        {
            if (!outFolder.EndsWith("\\"))
                outFolder = outFolder + "\\";

            Accord.DirectSound.WaveFileAudioSource audioSource = new Accord.DirectSound.WaveFileAudioSource(audioFileName);

            SoundLevelAnalyser analyser = new SoundLevelAnalyser
            {
                StartFrame = startFrame,
                EndFrame = endFrame,
                FrameStep = frameStep,
                OutFolder = outFolder,
                FrameIndex = 0,
                SoundLevel = new Dictionary<int, double>(),
                AudioSource = audioSource
            };

            audioSource.NewFrame += analyser.NewFrame;
            audioSource.Start();
        }


        /****************************************/
        /****  Public Definitions            ****/
        /****************************************/

        public delegate void SoundResultEvent(Dictionary<int, double> result);


        /****************************************/
        /****  Private Class Definition      ****/
        /****************************************/

        private class SoundLevelAnalyser
        {
            /****************************************/

            public int StartFrame;
            public int EndFrame;
            public int FrameStep;
            public string OutFolder;

            public SoundResultEvent ResultReceived = null;

            public Accord.DirectSound.WaveFileAudioSource AudioSource;
            public Dictionary<int, double> SoundLevel;
            public int FrameIndex;

            /****************************************/

            public void NewFrame(object sender, NewFrameEventArgs e)
            {
                if (FrameIndex > EndFrame)
                {
                    AudioSource.SignalToStop();
                    SaveData();

                    if (ResultReceived != null)
                        ResultReceived.Invoke(SoundLevel);

                    return;
                }

                if (FrameIndex >= StartFrame)
                {
                    SoundLevel[FrameIndex] = e.Signal.GetEnergy();
                    Console.WriteLine("Frame " + FrameIndex);
                }

                FrameIndex++;
            }

            /****************************************/

            private void SaveData()
            {
                Console.WriteLine("Saving data");
                if (OutFolder != "")
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(OutFolder + "sound.csv");
                    file.WriteLine("frame, sound level");

                    foreach (KeyValuePair<int, double> kvp in SoundLevel)
                    {
                        file.WriteLine(kvp.Key + "," + kvp.Value);
                    }

                    file.Close();
                }
            }
        }

        /****************************************/
    }
}
