using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace SpeechSynthesis_Engine
{
    public class SpeakText
    {
        SpeechSynthesizer m_SpeechSynthesizer = new SpeechSynthesizer();

        public void Speak(string text)
        {
            m_SpeechSynthesizer.Speak(text);
        }

    }
}
