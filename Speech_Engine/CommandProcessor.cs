/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System.Speech.Recognition;


namespace BH.Engine.Speech
{
    public class CommandProcessor
    {
        public delegate void MessageEvent(string data);

        public MessageEvent CommandProcessed;

        SpeechRecognitionEngine SpeechRecEngine = new SpeechRecognitionEngine();

        public CommandProcessor() { }

        public bool Initialise(string[] commands)
        {

            Choices choices = new Choices();
            choices.Add(commands);
            GrammarBuilder gramBuilder = new GrammarBuilder();
            gramBuilder.Append(choices);
            Grammar grammer = new Grammar(gramBuilder);

            SpeechRecEngine.LoadGrammarAsync(grammer);
            SpeechRecEngine.SetInputToDefaultAudioDevice();

            SpeechRecEngine.SpeechRecognized += SpeechRecEngine_SpeechRecognized;

            return true;
        }

        private void SpeechRecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            CommandProcessed.Invoke(e.Result.Text);
        }

        public void Enable()
        {
            SpeechRecEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Disable()
        {
            SpeechRecEngine.RecognizeAsyncStop();
        }
    }
}




