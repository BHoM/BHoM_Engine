using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;


namespace SpeechRecognition_Engine
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
