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

using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Environment.Climate;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculate the solar azimuth (degrees clockwise from 0 at North) from Datetime and Location objects")]
        [Input("spaceTime", "A SpaceTime object containing latitude, longitude, azimuth, and date data to calculate the solar position from")]
        [Output("sun", "The sun with calculated position")]
        public static Sun SolarPosition(this SpaceTime spaceTime)
        {
            if (spaceTime == null)
                return null;

            double latitude = spaceTime.Location.Latitude;
            double longitude = spaceTime.Location.Longitude;
            double utcOffset = spaceTime.Location.UtcOffset;
            DateTime dt = spaceTime.DateTime();

            double[][][] R_TERMS = new double[][][]
            {
                new double[][]
                {
                    new double[] {100013989.0, 0, 0},
                    new double[] {1670700.0, 3.0984635, 6283.07585},
                    new double[] {13956.0, 3.05525, 12566.1517},
                    new double[] {3084.0, 5.1985, 77713.7715},
                    new double[] {1628.0, 1.1739, 5753.3849},
                    new double[] {1576.0, 2.8469, 7860.4194},
                    new double[] {925.0, 5.453, 11506.77},
                    new double[] {542.0, 4.564, 3930.21},
                    new double[] {472.0, 3.661, 5884.927},
                    new double[] {346.0, 0.964, 5507.553},
                    new double[] {329.0, 5.9, 5223.694},
                    new double[] {307.0, 0.299, 5573.143},
                    new double[] {243.0, 4.273, 11790.629},
                    new double[] {212.0, 5.847, 1577.344},
                    new double[] {186.0, 5.022, 10977.079},
                    new double[] {175.0, 3.012, 18849.228},
                    new double[] {110.0, 5.055, 5486.778},
                    new double[] {98, 0.89, 6069.78},
                    new double[] {86, 5.69, 15720.84},
                    new double[] {86, 1.27, 161000.69},
                    new double[] {65, 0.27, 17260.15},
                    new double[] {63, 0.92, 529.69},
                    new double[] {57, 2.01, 83996.85},
                    new double[] {56, 5.24, 71430.7},
                    new double[] {49, 3.25, 2544.31},
                    new double[] {47, 2.58, 775.52},
                    new double[] {45, 5.54, 9437.76},
                    new double[] {43, 6.01, 6275.96},
                    new double[] {39, 5.36, 4694},
                    new double[] {38, 2.39, 8827.39},
                    new double[] {37, 0.83, 19651.05},
                    new double[] {37, 4.9, 12139.55},
                    new double[] {36, 1.67, 12036.46},
                    new double[] {35, 1.84, 2942.46},
                    new double[] {33, 0.24, 7084.9},
                    new double[] {32, 0.18, 5088.63},
                    new double[] {32, 1.78, 398.15},
                    new double[] {28, 1.21, 6286.6},
                    new double[] {28, 1.9, 6279.55},
                    new double[] {26, 4.59, 10447.39}
                },
                new double[][]
                {
                    new double[] {103019.0, 1.10749, 6283.07585},
                    new double[] {1721.0, 1.0644, 12566.1517},
                    new double[] {702.0, 3.142, 0},
                    new double[] {32, 1.02, 18849.23},
                    new double[] {31, 2.84, 5507.55},
                    new double[] {25, 1.32, 5223.69},
                    new double[] {18, 1.42, 1577.34},
                    new double[] {10, 5.91, 10977.08},
                    new double[] {9, 1.42, 6275.96},
                    new double[] {9, 0.27, 5486.78}
                },
                new double[][]
                {
                    new double[] {4359.0, 5.7846, 6283.0758},
                    new double[] {124.0, 5.579, 12566.152},
                    new double[] {12, 3.14, 0},
                    new double[] {9, 3.63, 77713.77},
                    new double[] {6, 1.87, 5573.14},
                    new double[] {3, 5.47, 18849.23}
                },
                new double[][]
                {
                    new double[] {145.0, 4.273, 6283.076},
                    new double[] {7, 3.92, 12566.15}
                },
                new double[][]
                {
                    new double[] {4, 2.56, 6283.08}
                }
            };
            int[] r_subcount = { 40, 10, 6, 2, 1 };
            int[][] Y_TERMS = new int[][]
            {
                new int[] {0, 0, 0, 0, 1},
                new int[] {-2, 0, 0, 2, 2},
                new int[] {0, 0, 0, 2, 2},
                new int[] {0, 0, 0, 0, 2},
                new int[] {0, 1, 0, 0, 0},
                new int[] {0, 0, 1, 0, 0},
                new int[] {-2, 1, 0, 2, 2},
                new int[] {0, 0, 0, 2, 1},
                new int[] {0, 0, 1, 2, 2},
                new int[] {-2, -1, 0, 2, 2},
                new int[] {-2, 0, 1, 0, 0},
                new int[] {-2, 0, 0, 2, 1},
                new int[] {0, 0, -1, 2, 2},
                new int[] {2, 0, 0, 0, 0},
                new int[] {0, 0, 1, 0, 1},
                new int[] {2, 0, -1, 2, 2},
                new int[] {0, 0, -1, 0, 1},
                new int[] {0, 0, 1, 2, 1},
                new int[] {-2, 0, 2, 0, 0},
                new int[] {0, 0, -2, 2, 1},
                new int[] {2, 0, 0, 2, 2},
                new int[] {0, 0, 2, 2, 2},
                new int[] {0, 0, 2, 0, 0},
                new int[] {-2, 0, 1, 2, 2},
                new int[] {0, 0, 0, 2, 0},
                new int[] {-2, 0, 0, 2, 0},
                new int[] {0, 0, -1, 2, 1},
                new int[] {0, 2, 0, 0, 0},
                new int[] {2, 0, -1, 0, 1},
                new int[] {-2, 2, 0, 2, 2},
                new int[] {0, 1, 0, 0, 1},
                new int[] {-2, 0, 1, 0, 1},
                new int[] {0, -1, 0, 0, 1},
                new int[] {0, 0, 2, -2, 0},
                new int[] {2, 0, -1, 2, 1},
                new int[] {2, 0, 1, 2, 2},
                new int[] {0, 1, 0, 2, 2},
                new int[] {-2, 1, 1, 0, 0},
                new int[] {0, -1, 0, 2, 2},
                new int[] {2, 0, 0, 2, 1},
                new int[] {2, 0, 1, 0, 0},
                new int[] {-2, 0, 2, 2, 2},
                new int[] {-2, 0, 1, 2, 1},
                new int[] {2, 0, -2, 0, 1},
                new int[] {2, 0, 0, 0, 1},
                new int[] {0, -1, 1, 0, 0},
                new int[] {-2, -1, 0, 2, 1},
                new int[] {-2, 0, 0, 0, 1},
                new int[] {0, 0, 2, 2, 1},
                new int[] {-2, 0, 2, 0, 1},
                new int[] {-2, 1, 0, 2, 1},
                new int[] {0, 0, 1, -2, 0},
                new int[] {-1, 0, 1, 0, 0},
                new int[] {-2, 1, 0, 0, 0},
                new int[] {1, 0, 0, 0, 0},
                new int[] {0, 0, 1, 2, 0},
                new int[] {0, 0, -2, 2, 2},
                new int[] {-1, -1, 1, 0, 0},
                new int[] {0, 1, 1, 0, 0},
                new int[] {0, -1, 1, 2, 2},
                new int[] {2, -1, -1, 2, 2},
                new int[] {0, 0, 3, 2, 2},
                new int[] {2, -1, 0, 2, 2},
            };
            double[][] PE_TERMS = new double[][]
            {
                new double[] {-171996, -174.2, 92025, 8.9},
                new double[] {-13187, -1.6, 5736, -3.1},
                new double[] {-2274, -0.2, 977, -0.5},
                new double[] {2062, 0.2, -895, 0.5},
                new double[] {1426, -3.4, 54, -0.1},
                new double[] {712, 0.1, -7, 0},
                new double[] {-517, 1.2, 224, -0.6},
                new double[] {-386, -0.4, 200, 0},
                new double[] {-301, 0, 129, -0.1},
                new double[] {217, -0.5, -95, 0.3},
                new double[] {-158, 0, 0, 0},
                new double[] {129, 0.1, -70, 0},
                new double[] {123, 0, -53, 0},
                new double[] {63, 0, 0, 0},
                new double[] {63, 0.1, -33, 0},
                new double[] {-59, 0, 26, 0},
                new double[] {-58, -0.1, 32, 0},
                new double[] {-51, 0, 27, 0},
                new double[] {48, 0, 0, 0},
                new double[] {46, 0, -24, 0},
                new double[] {-38, 0, 16, 0},
                new double[] {-31, 0, 13, 0},
                new double[] {29, 0, 0, 0},
                new double[] {29, 0, -12, 0},
                new double[] {26, 0, 0, 0},
                new double[] {-22, 0, 0, 0},
                new double[] {21, 0, -10, 0},
                new double[] {17, -0.1, 0, 0},
                new double[] {16, 0, -8, 0},
                new double[] {-16, 0.1, 7, 0},
                new double[] {-15, 0, 9, 0},
                new double[] {-13, 0, 7, 0},
                new double[] {-12, 0, 6, 0},
                new double[] {11, 0, 0, 0},
                new double[] {-10, 0, 5, 0},
                new double[] {-8, 0, 3, 0},
                new double[] {7, 0, -3, 0},
                new double[] {-7, 0, 0, 0},
                new double[] {-7, 0, 3, 0},
                new double[] {-7, 0, 3, 0},
                new double[] {6, 0, 0, 0},
                new double[] {6, 0, -3, 0},
                new double[] {6, 0, -3, 0},
                new double[] {-6, 0, 3, 0},
                new double[] {-6, 0, 3, 0},
                new double[] {5, 0, 0, 0},
                new double[] {-5, 0, 3, 0},
                new double[] {-5, 0, 3, 0},
                new double[] {-5, 0, 3, 0},
                new double[] {4, 0, 0, 0},
                new double[] {4, 0, 0, 0},
                new double[] {4, 0, 0, 0},
                new double[] {-4, 0, 0, 0},
                new double[] {-4, 0, 0, 0},
                new double[] {-4, 0, 0, 0},
                new double[] {3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
                new double[] {-3, 0, 0, 0},
            };
            double[][][] L_TERMS = new double[][][]
            {
                new double[][]
                {
                    new double[] {175347046.0, 0, 0},
                    new double[] {3341656.0, 4.6692568, 6283.07585},
                    new double[] {34894.0, 4.6261, 12566.1517},
                    new double[] {3497.0, 2.7441, 5753.3849},
                    new double[] {3418.0, 2.8289, 3.5231},
                    new double[] {3136.0, 3.6277, 77713.7715},
                    new double[] {2676.0, 4.4181, 7860.4194},
                    new double[] {2343.0, 6.1352, 3930.2097},
                    new double[] {1324.0, 0.7425, 11506.7698},
                    new double[] {1273.0, 2.0371, 529.691},
                    new double[] {1199.0, 1.1096, 1577.3435},
                    new double[] {990, 5.233, 5884.927},
                    new double[] {902, 2.045, 26.298},
                    new double[] {857, 3.508, 398.149},
                    new double[] {780, 1.179, 5223.694},
                    new double[] {753, 2.533, 5507.553},
                    new double[] {505, 4.583, 18849.228},
                    new double[] {492, 4.205, 775.523},
                    new double[] {357, 2.92, 0.067},
                    new double[] {317, 5.849, 11790.629},
                    new double[] {284, 1.899, 796.298},
                    new double[] {271, 0.315, 10977.079},
                    new double[] {243, 0.345, 5486.778},
                    new double[] {206, 4.806, 2544.314},
                    new double[] {205, 1.869, 5573.143},
                    new double[] {202, 2.458, 6069.777},
                    new double[] {156, 0.833, 213.299},
                    new double[] {132, 3.411, 2942.463},
                    new double[] {126, 1.083, 20.775},
                    new double[] {115, 0.645, 0.98},
                    new double[] {103, 0.636, 4694.003},
                    new double[] {102, 0.976, 15720.839},
                    new double[] {102, 4.267, 7.114},
                    new double[] {99, 6.21, 2146.17},
                    new double[] {98, 0.68, 155.42},
                    new double[] {86, 5.98, 161000.69},
                    new double[] {85, 1.3, 6275.96},
                    new double[] {85, 3.67, 71430.7},
                    new double[] {80, 1.81, 17260.15},
                    new double[] {79, 3.04, 12036.46},
                    new double[] {75, 1.76, 5088.63},
                    new double[] {74, 3.5, 3154.69},
                    new double[] {74, 4.68, 801.82},
                    new double[] {70, 0.83, 9437.76},
                    new double[] {62, 3.98, 8827.39},
                    new double[] {61, 1.82, 7084.9},
                    new double[] {57, 2.78, 6286.6},
                    new double[] {56, 4.39, 14143.5},
                    new double[] {56, 3.47, 6279.55},
                    new double[] {52, 0.19, 12139.55},
                    new double[] {52, 1.33, 1748.02},
                    new double[] {51, 0.28, 5856.48},
                    new double[] {49, 0.49, 1194.45},
                    new double[] {41, 5.37, 8429.24},
                    new double[] {41, 2.4, 19651.05},
                    new double[] {39, 6.17, 10447.39},
                    new double[] {37, 6.04, 10213.29},
                    new double[] {37, 2.57, 1059.38},
                    new double[] {36, 1.71, 2352.87},
                    new double[] {36, 1.78, 6812.77},
                    new double[] {33, 0.59, 17789.85},
                    new double[] {30, 0.44, 83996.85},
                    new double[] {30, 2.74, 1349.87},
                    new double[] {25, 3.16, 4690.48}
                },
                new double[][]
                {
                    new double[] {628331966747.0, 0, 0},
                    new double[] {206059.0, 2.678235, 6283.07585},
                    new double[] {4303.0, 2.6351, 12566.1517},
                    new double[] {425.0, 1.59, 3.523},
                    new double[] {119.0, 5.796, 26.298},
                    new double[] {109.0, 2.966, 1577.344},
                    new double[] {93, 2.59, 18849.23},
                    new double[] {72, 1.14, 529.69},
                    new double[] {68, 1.87, 398.15},
                    new double[] {67, 4.41, 5507.55},
                    new double[] {59, 2.89, 5223.69},
                    new double[] {56, 2.17, 155.42},
                    new double[] {45, 0.4, 796.3},
                    new double[] {36, 0.47, 775.52},
                    new double[] {29, 2.65, 7.11},
                    new double[] {21, 5.34, 0.98},
                    new double[] {19, 1.85, 5486.78},
                    new double[] {19, 4.97, 213.3},
                    new double[] {17, 2.99, 6275.96},
                    new double[] {16, 0.03, 2544.31},
                    new double[] {16, 1.43, 2146.17},
                    new double[] {15, 1.21, 10977.08},
                    new double[] {12, 2.83, 1748.02},
                    new double[] {12, 3.26, 5088.63},
                    new double[] {12, 5.27, 1194.45},
                    new double[] {12, 2.08, 4694},
                    new double[] {11, 0.77, 553.57},
                    new double[] {10, 1.3, 6286.6},
                    new double[] {10, 4.24, 1349.87},
                    new double[] {9, 2.7, 242.73},
                    new double[] {9, 5.64, 951.72},
                    new double[] {8, 5.3, 2352.87},
                    new double[] {6, 2.65, 9437.76},
                    new double[] {6, 4.67, 4690.48}
                },
                new double[][]
                {
                    new double[] {52919.0, 0, 0},
                    new double[] {8720.0, 1.0721, 6283.0758},
                    new double[] {309.0, 0.867, 12566.152},
                    new double[] {27, 0.05, 3.52},
                    new double[] {16, 5.19, 26.3},
                    new double[] {16, 3.68, 155.42},
                    new double[] {10, 0.76, 18849.23},
                    new double[] {9, 2.06, 77713.77},
                    new double[] {7, 0.83, 775.52},
                    new double[] {5, 4.66, 1577.34},
                    new double[] {4, 1.03, 7.11},
                    new double[] {4, 3.44, 5573.14},
                    new double[] {3, 5.14, 796.3},
                    new double[] {3, 6.05, 5507.55},
                    new double[] {3, 1.19, 242.73},
                    new double[] {3, 6.12, 529.69},
                    new double[] {3, 0.31, 398.15},
                    new double[] {3, 2.28, 553.57},
                    new double[] {2, 4.38, 5223.69},
                    new double[] {2, 3.75, 0.98}
                },
                new double[][]
                {
                    new double[] {289.0, 5.844, 6283.076},
                    new double[] {35, 0, 0},
                    new double[] {17, 5.49, 12566.15},
                    new double[] {3, 5.2, 155.42},
                    new double[] {1, 4.72, 3.52},
                    new double[] {1, 5.3, 18849.23},
                    new double[] {1, 5.97, 242.73}
                },
                new double[][]
                {
                    new double[] {114.0, 3.142, 0},
                    new double[] {8, 4.13, 6283.08},
                    new double[] {1, 3.84, 12566.15}
                },
                new double[][]
                {
                    new double[] {1, 3.14, 0}
                }
            };
            int[] l_subcount = { 64, 34, 20, 7, 3, 1 };
            double[][][] B_TERMS = new double[][][]
            {
                new double[][]
                {
                    new double[] {280.0, 3.199, 84334.662},
                    new double[] {102.0, 5.422, 5507.553},
                    new double[] {80, 3.88, 5223.69},
                    new double[] {44, 3.7, 2352.87},
                    new double[] {32, 4, 1577.34}
                },
                new double[][]
                {
                    new double[] {9, 3.9, 5507.55},
                    new double[] {6, 1.73, 5223.69}
                }
            };
            int[] b_subcount = { 5, 2 };

            double PI = 3.1415926535897932384626433832795028841971;

            double julianDay = JulianDay(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, utcOffset);
            double julianCentury = (julianDay - 2451545.0) / 36525.0;
            double julianEphemerisDay = julianDay; // / 86400.0;
            double julianEphemerisCentury = (julianEphemerisDay - 2451545.0) / 36525.0;
            double julianEphemerisMillenium = (julianEphemerisCentury / 10.0);

            //EarthRadiusVector
            double[] sum = new double[5];
            for (int x2 = 0; x2 < 5; x2++)
            {
                double s = 0;
                for (int y2 = 0; y2 < r_subcount[x2]; y2++)
                    s += R_TERMS[x2][y2][0] * Math.Cos(R_TERMS[x2][y2][1] + R_TERMS[x2][y2][2] * julianEphemerisMillenium);
                sum[x2] = s;
            }
            //Earth Values
            double R = 0;
            for (int x3 = 0; x3 < 5; x3++)
                R += sum[x3] * Math.Pow(julianEphemerisMillenium, x3);
            R /= 1.0e8;

            double[] meanTerms = new double[5];
            meanTerms[0] = ThirdOrderPolynomial(1.0 / 189474.0, -0.0019142, 445267.11148, 297.85036, julianEphemerisCentury); //MeanElongationMoonSun
            meanTerms[1] = ThirdOrderPolynomial(-1.0 / 300000.0, -0.0001603, 35999.05034, 357.52772, julianEphemerisCentury); //MeanAnomalySun
            meanTerms[2] = ThirdOrderPolynomial(1.0 / 56250.0, 0.0086972, 477198.867398, 134.96298, julianEphemerisCentury); //MeanAnomalyMoon
            meanTerms[3] = ThirdOrderPolynomial(1.0 / 327270.0, -0.0036825, 483202.017538, 93.27191, julianEphemerisCentury); //ArgumentLatitudeMoon
            meanTerms[4] = ThirdOrderPolynomial(1.0 / 450000.0, 0.0020708, -1934.136261, 125.04452, julianEphemerisCentury); //AscendingLongitudeMoon

            //Nutation Longitude and Obliquity
            double delPsi = 0;
            double delEpsilon = 0;
            for (int x4 = 0; x4 < 63; x4++)
            {
                double xyTermSummation = 0;
                for (int y3 = 0; y3 < meanTerms.Length; y3++)
                    xyTermSummation += meanTerms[y3] * Y_TERMS[x4][y3];
                xyTermSummation = ToRadians(xyTermSummation, PI);
                delPsi += (PE_TERMS[x4][0] + julianEphemerisCentury * PE_TERMS[x4][1]) * Math.Sin(xyTermSummation);
                delEpsilon += (PE_TERMS[x4][2] + julianEphemerisCentury * PE_TERMS[x4][3]) * Math.Cos(xyTermSummation);
            }
            delPsi /= 36000000.0;
            delEpsilon /= 36000000.0;

            double u = julianEphemerisMillenium / 10;
            double epsilon0 = 84381.448 + u * (-4680.93 + u * (-1.55 + u * (1999.25 + u * (-51.38 + u * (-249.67 + u * (-39.05 + u * (7.12 + u * (27.87 + u * (5.79 + u * 2.45))))))))); //EclipticMeanObliquity
            double epsilon = delEpsilon + epsilon0 / 3600.0; //EclipticTrueObliquity

            //Greenwich Mean Sidereal Time
            double nu0 = 280.46061837 + 360.98564736629 * (julianDay - 2451545.0) + julianCentury * julianCentury * (0.000387933 - julianCentury / 38710000.0);
            nu0 /= 360;
            double limit = 360 * (nu0 - Math.Floor(nu0));
            if (limit < 0) limit += 360;
            nu0 = limit;

            double nu = nu0 + delPsi * Math.Cos(ToRadians(epsilon, PI)); //Greenwich Sidereal Time

            //EarthHeliocentricLongitude
            double l = 0;
            double[] sum2 = new double[6];
            for (int x5 = 0; x5 < sum2.Length; x5++)
            {
                for (int y5 = 0; y5 < l_subcount[x5]; y5++)
                    sum2[x5] += L_TERMS[x5][y5][0] * Math.Cos(L_TERMS[x5][y5][1] + L_TERMS[x5][y5][2] * julianEphemerisMillenium);
            }
            for (int x6 = 0; x6 < sum2.Length; x6++)
                l += sum2[x6] * Math.Pow(julianEphemerisMillenium, x6);
            l /= 1.0e8;
            l = ToDegrees(l, PI);
            l /= 360;
            limit = 360 * (l - Math.Floor(l));
            if (limit < 0) limit += 360;
            l = limit;

            //EarthHeliocentricLatitude
            double b = 0;
            sum2 = new double[2];
            for(int x7 = 0; x7 < sum2.Length; x7++)
            {
                for (int y7 = 0; y7 < b_subcount[x7]; y7++)
                    sum2[x7] += B_TERMS[x7][y7][0] * Math.Cos(B_TERMS[x7][y7][1] + B_TERMS[x7][y7][2] * julianEphemerisMillenium);
            }
            for (int x8 = 0; x8 < sum2.Length; x8++)
                b += sum2[x8] * Math.Pow(julianEphemerisMillenium, x8);
            b /= 1.0e8;
            b = ToDegrees(b, PI);

            //GeocentricLongitude
            double theta = l + 180;
            if (theta >= 360) theta -= 360;

            //GeocentricLatitude
            double beta = -b;

            double delTau = -20.4898 / (3600.0 * R); //AberrationCorrection

            double lamba = theta + delPsi + delTau; //ApparentSunLongitude

            //GeocentricRightAscension
            double alpha = ToDegrees(Math.Atan2(Math.Sin(ToRadians(lamba, PI)) * Math.Cos(ToRadians(epsilon, PI) - Math.Tan(ToRadians(beta, PI)) * Math.Sin(ToRadians(epsilon, PI))), Math.Cos(ToRadians(lamba, PI))), PI);
            alpha /= 360;
            limit = 360 * (alpha - Math.Floor(alpha));
            if (limit < 0) limit += 360;
            alpha = limit;

            double delta = ToDegrees(Math.Asin(Math.Sin(ToRadians(beta, PI)) * Math.Cos(ToRadians(epsilon, PI)) + Math.Cos(ToRadians(beta, PI)) * Math.Sin(ToRadians(epsilon, PI) * Math.Sin(ToRadians(lamba, PI)))), PI); //GeocentricDeclination

            //Observe Hour Angle
            double H = nu + longitude - alpha;
            H /= 360;
            limit = 360 * (H - Math.Floor(H));
            if (limit < 0) limit += 360;
            H = limit;

            double xi = 8.794 / (3600.0 * R); //SunEquatorialHorizontalParallax

            //RightAscensionParallaxAndTopocentricDec
            double latRad = ToRadians(latitude, PI);
            double xiRad = ToRadians(xi, PI);
            double hRad = ToRadians(H, PI);
            double deltaRad = ToRadians(delta, PI);
            double u2 = Math.Atan(0.99664719 * Math.Tan(latRad));
            double y = 0.99664719 * Math.Sin(u2) * Math.Sin(latRad) / 6378140.0;
            double x = Math.Cos(u2) * Math.Cos(latRad) / 6378140.0;

            var deltaAlphaRad = Math.Atan2(-x * Math.Sin(xiRad) * Math.Sin(hRad),
                Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad));

            double deltaPrime = ToDegrees(Math.Atan2((Math.Sin(deltaRad) - y * Math.Sin(xiRad)) * Math.Cos(deltaAlphaRad), Math.Cos(deltaRad) - x * Math.Sin(xiRad) * Math.Cos(hRad)), PI);
            double deltaAlpha = ToDegrees(deltaAlphaRad, PI);

            double hPrime = H - deltaAlpha; //TopocentricLocalHourAngle

            //TopocentricAzimuthAngleAstro
            double azimuthAstro = ToDegrees(Math.Atan2(Math.Sin(ToRadians(hPrime, PI)), Math.Cos(ToRadians(hPrime, PI)) * Math.Sin(ToRadians(latitude, PI)) - Math.Tan(ToRadians(deltaPrime, PI)) * Math.Cos(ToRadians(latitude, PI))), PI);
            azimuthAstro /= 360;
            limit = 360 * (azimuthAstro - Math.Floor(azimuthAstro));
            if (limit < 0) limit += 360;
            azimuthAstro = limit;

            //TopocentricAzimuthAngle
            double azimuth = azimuthAstro + 180;
            azimuth /= 360;
            limit = 360 * (azimuth - Math.Floor(azimuth));
            if (limit < 0) limit += 360;
            azimuth = limit;

            double e0 = ToDegrees(Math.Asin(Math.Sin(latRad) * Math.Sin(ToRadians(deltaPrime, PI)) + Math.Cos(latRad) * Math.Cos(ToRadians(deltaPrime, PI)) * Math.Cos(ToRadians(hPrime, PI))), PI); //TopocentricElevationAngle

            double zenith = 90.0 - e0;

            return new Sun { Altitude = (90 - zenith), Azimuth = azimuth };
        }

        private static double ToDegrees(double radians, double pi = Math.PI)
        {
            return radians * (180 / pi);
        }

        private static double ToRadians(double degrees, double pi = Math.PI)
        {
            return degrees * (pi / 180);
        }
    }
}



