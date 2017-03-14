using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHG = BHoM.Geometry;
using BHA = BHoM.Acoustic;

namespace AcousticSPI_Engine
{
    public class STICalculator
    {
        public STICalculator(BHA.AcousticParameters param)
        {
            Parameters = param;
        }

        private BHA.AcousticParameters Parameters { get; set; }

        public List<double> CalculateRASTI(List<BHA.Speaker> speakers, BHA.Zone zone, List<double> frequencies, List<double> octaves)
        {
            List<double> STI = new List<double>();
            List<double> RASTI = new List<double>();
            List<double> sn = new List<double>();

            Dictionary<double, List<double>> STI_OCT = new Dictionary<double, List<double>>();

            foreach (double oct in octaves)
            {
                STI_OCT[oct] = new List<double>();
            }

            List<BHG.Point> Location = zone.SamplePoints;

            double freqCount = frequencies.Count;

            foreach (BHoM.Geometry.Point point in Location)
            {
                foreach (double octave in octaves)
                {

                    double STI_oct = 0;
                    double snApp = 0;
                    double totalSN = 0;

                    foreach (double frequency in frequencies)
                    {
                        double iterationLevel = 0;
                        double totalLevel = 0;
                        double amb_pascals = 0;
                        double levelSum = 0;
                        double closestDist = Double.PositiveInfinity;

                        double revDist = 0;
                        double timeConstant = 0;
                        double gain = 0;

                        double speech = Parameters.GetSpeech(frequency, octave);


                        foreach (BHA.Speaker speaker in speakers)
                        {
                            // 1. CalcSoundLevel
                            CalcSoundLevel(point, speaker, zone, frequency, octave, closestDist, out iterationLevel, out amb_pascals, out revDist, out timeConstant, out closestDist, out gain);
                            totalLevel += iterationLevel;
                            levelSum = speech + 10 * Math.Log10(totalLevel);
                        }

                        // 2. Calculate SoundToNoise Ratio
                        CalcSoundToNoiseRatio(closestDist, amb_pascals, levelSum, revDist, timeConstant, frequency, octave, gain, out snApp);
                        totalSN += snApp;
                        sn.Add(snApp);
                    }

                    // 3. Calculate STI
                    CalcSTI(totalSN, freqCount, out STI_oct);

                    STI_OCT[octave].Add(STI_oct);
                    STI.Add(STI_oct);
                }
            }

            for (int i = 0; i < STI_OCT[500].Count; i++)
            {
                double rasti = ((4d / 9d) * STI_OCT[500][i]) + ((5d / 9d) * STI_OCT[2000][i]);
                RASTI.Add(rasti);
            }
            return RASTI;
        }
        private void CalcSoundLevel(BHG.Point location, BHA.Speaker speaker, BHA.Zone zone, double frequency, double octave, double closestDist, out double level, out double amb_pascals, out double revDist, out double timeConstant, out double closestdist, out double gain)
        {

            BHG.Vector deltaPos = location - speaker.Position;
            double recieverAngle = BHG.VectorUtils.VectorAngle(deltaPos, speaker.Direction) * (180 / Math.PI);
            double distance = deltaPos.Length;

            double orientationFactor = speaker.GetGainAngleFactor(recieverAngle, octave);  // take out octave, Matlab does some weird thing here where frequency is tied to octave
            gain = Parameters.GetGain(frequency, octave) * Math.Pow(10, orientationFactor / 10);
            double speech = Parameters.GetSpeech(frequency, octave);

            double volume = zone.Volume;
            double revTime = Parameters.GetRevTime(frequency, octave);
            double sAlpha = (0.163 * volume / revTime) - (4.0 * 2.6 * volume / 1000);  // It would be good to clarify all those constants
            double alpha = sAlpha / zone.Area;

            double roomConstant = sAlpha / (1 - alpha);
            if (Parameters.ReverberationTimes.Min() < 0.01)
                roomConstant = Double.PositiveInfinity;

            revDist = Math.Sqrt(0.0032 * volume / revTime);
            timeConstant = revTime / 13.8155;    // Only used outside of the loop ... Clearly something wrong here

            level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);

            if (distance < closestDist)
            {
                //highest_level = speech + 10 * Math.Log10((gain / (4 * Math.PI * distance * distance)) + (4 / roomConstant));
                closestdist = distance;
            }

            else
            {
                closestdist = closestDist;
            }

            if (distance > revDist)
            {
                amb_pascals = gain / ((4.0 * Math.PI * distance * distance) + (4.0 / roomConstant));
            }

            else
            {
                amb_pascals = 0.0;
            }
        }

        private void CalcSoundToNoiseRatio(double closestDist, double amb_pascals, double levelSum, double revDist, double timeConstant, double frequency, double octave, double gain, out double snApp)
        {

            double speech = Parameters.GetSpeech(frequency, octave);
            double noise = Parameters.GetNoiseLevel(frequency, octave);

            double amb_noise = Parameters.GetNoiseLevel(frequency, octave);
            if (amb_pascals > 0)
            {
                double amb_sum = speech + 10 * Math.Log10(amb_pascals);
                amb_noise = 10 * Math.Log10(Math.Pow(10, noise / 10) + Math.Pow(10, amb_sum / 10));
            }

            double i_n = Math.Pow(10, (amb_noise - levelSum) / 10);

            //double gain = Parameters.GetGain(frequency, octave) * Math.Pow(10, orientationFactor / 10);

            double reciever_q = 1.5; // What is reciever_q?

            double cap_a = ((gain * reciever_q) / (closestDist * closestDist)) + ((1.0 / (revDist * revDist)) / (1.0 + Math.Pow(2.0 * Math.PI * timeConstant * frequency, 2.0)));
            double cap_b = ((2.0 * Math.PI * timeConstant * frequency) / (revDist * revDist)) / (1.0 + Math.Pow(2.0 * Math.PI * timeConstant * frequency, 2.0));
            double cap_c = ((gain * reciever_q) / (closestDist * closestDist)) + (1.0 / (revDist * revDist)) + (gain * i_n);


            double m_f1 = Math.Sqrt(cap_a * cap_a + cap_b * cap_b) / cap_c;

            double m_f = m_f1; // ask Mathew H.

            snApp = 10.0 * Math.Log10(m_f / (1.0 - m_f));

            if (snApp > 15.0)
            {
                snApp = 15.0;
            }
            if (snApp < -15.0)
            {
                snApp = -15.0;
            }
        }

        private void CalcSTI(double totalSN, double freqCount, out double STI_oct) // double highest_level & double amb_noise not included yet + levelSum
        {
            STI_oct = ((totalSN / freqCount) + 15.0) / 30.0;
        }
    }
}