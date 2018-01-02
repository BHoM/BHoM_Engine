using System.Speech.Synthesis;

namespace BH.Engine.Speech
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
