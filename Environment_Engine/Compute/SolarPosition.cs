/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculate the solar azimuth (degrees clockwise from 0 at North) from Datetime and Location objects")]
        [Input("latitude", "The latitude of the location to calculate the solar azimuth from. This should be given in degrees. Default 0")]
        [Input("longitude", "The longitude of the location to calculate the solar azimuth from. This should be given in degrees. Default 0")]
        [Input("dateTime", "The date and time for the calculation of the solar azimuth. Default null - will default to the 1st January 1900 00:00:00.0")]
        [Input("utcOffset", "The number of hours offset from UTC time, default 0")]
        [Output("solarAzimuth", "The calculated solar azimuth (degrees clockwise from 0 at North)")]
        public static double SolarAzimuth(double latitude = 0, double longitude = 0, DateTime? dateTime = null, double utcOffset = 0)
        {
            DateTime dt;
            if (dateTime == null)
                dt = new DateTime(1900, 1, 1, 0, 0, 0);
            else
                dt = dateTime.Value;

            double radiansLat = Convert.ToRadians(latitude);
            double solarZenithAngle = SolarZenithAngle(latitude, longitude, dt, utcOffset);
            double sunDeclination = SunDeclination(dt);
            double hourAngle = HourAngle(longitude, dt, utcOffset);

            // NOTE: Method below split into throwaway variable names for ease of coding/interpretation
            double A = Math.Sin(radiansLat);
            double B = Math.Cos(radiansLat);
            double C = Math.Sin(Convert.ToRadians(solarZenithAngle));
            double D = Math.Cos(Convert.ToRadians(solarZenithAngle));
            double E = Math.Sin(Convert.ToRadians(sunDeclination));
            double F = Convert.ToDegrees(Math.Acos(((A * D) - E) / (B * C)));
            double G = 0;

            if (hourAngle > 0)
            {
                G += 180 + F % 360;
            }
            else
            {
                G += 180 - F % 360;
            }

            return G;
        }

        [Description("Calculate the solar altitude from a date time and location")]
        [Input("latitude", "The latitude of the location to calculate the solar azimuth from. This should be given in degrees. Default 0")]
        [Input("longitude", "The longitude of the location to calculate the solar azimuth from. This should be given in degrees. Default 0")]
        [Input("dateTime", "The date and time for the calculation of the solar azimuth. Default null - will default to the 1st January 1900 00:00:00.0")]
        [Input("utcOffset", "The number of hours offset from UTC time, default 0")]
        [Input("pressure", "Atmospheric pressure, default 101325 - pressure at sea level")]
        [Input("temperature", "Temperature in degrees celcius - default 15")]
        [Output("SolarAltitude", "Calculate the solar altitude")]
        public static double SolarAltitude(double latitude = 0, double longitude = 0, DateTime? dateTime = null, double utcOffset = 0, double pressure = 101325, double temperature = 15)
        {
            DateTime dt;
            if (dateTime == null)
                dt = new DateTime(1900, 1, 1, 0, 0, 0);
            else
                dt = dateTime.Value;

            double solarElevationAngle = 90 - SolarZenithAngle(latitude, longitude, dt, utcOffset);
            double atmosphericRefraction = 0;
            if (solarElevationAngle >= -(0.26667 + 0.5667)) //0.26667 is SunRadius, 0.5667 is Atmospheric refraction constant
                atmosphericRefraction = (pressure * 2.83 * 1.02) / (1010 * temperature * 60 * Math.Tan(Convert.ToRadians(solarElevationAngle + (10.3 / (solarElevationAngle + 5.11)))));

            return solarElevationAngle + atmosphericRefraction;
        }

        [Description("Calculate the solar zenith angle from a Date Time and a Location")]
        [Input("latitude", "The latitude of the location to calculate the solar zenith angle from. This should be given in degrees. Default 0")]
        [Input("longitude", "The longitude of the location to calculate the solar zenith angle from. This should be given in degrees. Default 0")]
        [Input("dateTime", "The date and time for the calculation of the solar zenith angle. Default null - will default to the 1st January 1900 00:00:00.0")]
        [Input("utcOffset", "The number of hours offset from UTC time, default 0")]
        [Output("solarZenithAngle", "The solar zenith angle")]
        public static double SolarZenithAngle(double latitude = 0, double longitude = 0, DateTime? dateTime = null, double utcOffset = 0)
        {
            DateTime dt;
            if (dateTime == null)
                dt = new DateTime(1900, 1, 1, 0, 0, 0);
            else
                dt = dateTime.Value;

            double radiansLat = Convert.ToRadians(latitude);
            double julianCentury = ((dt.ToOADate() + 2415018.5) - 2451545) / 36525; //2451545 is 1 January AD 2000 at 15:00, 36525 is the length of a Julian century in days

            double meanEclipticObliquity = 23 + (26 + ((21.448 - julianCentury * (46.815 + julianCentury * (0.00059 - julianCentury * 0.001813)))) / 60) / 60;
            double meanEclipticObliquityCorrection = meanEclipticObliquity + 0.00256 * Math.Cos(Convert.ToRadians(125.04 - 1934.136 * julianCentury));

            double sunGeometricMeanAnomaly = 357.52911 + julianCentury * (35999.05029 - 0.0001537 * julianCentury);
            double sunGeometricMeanLongitude = 280.46646 + julianCentury * (36000.76983 + julianCentury * 0.0003032) % 360;

            double sunDeclination = SunDeclination(dt);

            double hourAngle = HourAngle(longitude, dt, utcOffset);            

            return Convert.ToDegrees(Math.Acos(Math.Sin(radiansLat) * Math.Sin(Convert.ToRadians(sunDeclination)) + Math.Cos(radiansLat) * Math.Cos(Convert.ToRadians(sunDeclination)) * Math.Cos(Convert.ToRadians(hourAngle))));
        }

        [Description("Get the declination of the sun from a Datetime object")]
        [Input("dateTime", "The date and time for the calculation of the solar zenith angle. Default null - will default to the 1st January 1900 00:00:00.0")]
        [Output("sunDeclination", "The declination of the sun")]
        public static double SunDeclination(DateTime? dateTime = null)
        {
            DateTime dt;
            if (dateTime == null)
                dt = new DateTime(1900, 1, 1, 0, 0, 0);
            else
                dt = dateTime.Value;

            double julianCentury = ((dt.ToOADate() + 2415018.5) - 2451545) / 36525; //2451545 is 1 January AD 2000 at 15:00, 36525 is the length of a Julian century in days
            double meanEclipticObliquity = 23 + (26 + ((21.448 - julianCentury * (46.815 + julianCentury * (0.00059 - julianCentury * 0.001813)))) / 60) / 60;
            double meanEclipticObliquityCorrection = meanEclipticObliquity + 0.00256 * Math.Cos(Convert.ToRadians(125.04 - 1934.136 * julianCentury));

            double sunGeometricMeanAnomaly = 357.52911 + julianCentury * (35999.05029 - 0.0001537 * julianCentury);
            double sunGeometricMeanLongitude = 280.46646 + julianCentury * (36000.76983 + julianCentury * 0.0003032) % 360;
            double sunEquationOfCenter = Math.Sin(Convert.ToRadians(sunGeometricMeanAnomaly)) * (1.914602 - julianCentury * (0.004817 + 0.000014 * julianCentury)) + Math.Sin(Convert.ToRadians(2 * sunGeometricMeanAnomaly)) * (0.019993 - 0.000101 * julianCentury) + Math.Sin(Convert.ToRadians(3 * sunGeometricMeanAnomaly)) * 0.000289;
            double sunApparentLongitude = (sunEquationOfCenter + sunGeometricMeanLongitude) - 0.00569 - 0.00478 * Math.Sin(Convert.ToRadians(125.04 - 1934.136 * julianCentury));

            return Convert.ToDegrees(Math.Asin(Math.Sin(Convert.ToRadians(meanEclipticObliquityCorrection)) * Math.Sin(Convert.ToRadians(sunApparentLongitude))));
        }

        [Description("Calculate the hour angle from a given date time and a location")]
        [Input("longitude", "The longitude of the location to calculate the solar zenith angle from. This should be given in degrees. Default 0")]
        [Input("dateTime", "The date and time for the calculation of the solar zenith angle. Default null - will default to the 1st January 1900 00:00:00.0")]
        [Input("utcOffset", "The number of hours offset from UTC time, default 0")]
        [Output("hourAngle", "The hour angle")]
        public static double HourAngle(double longitude = 0, DateTime? dateTime = null, double utcOffset = 0)
        {
            DateTime dt;
            if (dateTime == null)
                dt = new DateTime(1900, 1, 1, 0, 0, 0);
            else
                dt = dateTime.Value;

            double julianCentury = ((dt.ToOADate() + 2415018.5) - 2451545) / 36525; //2451545 is 1 January AD 2000 at 15:00, 36525 is the length of a Julian century in days
            double meanEclipticObliquity = 23 + (26 + ((21.448 - julianCentury * (46.815 + julianCentury * (0.00059 - julianCentury * 0.001813)))) / 60) / 60;
            double meanEclipticObliquityCorrection = meanEclipticObliquity + 0.00256 * Math.Cos(Convert.ToRadians(125.04 - 1934.136 * julianCentury));

            double sunGeometricMeanAnomaly = 357.52911 + julianCentury * (35999.05029 - 0.0001537 * julianCentury);
            double sunGeometricMeanLongitude = 280.46646 + julianCentury * (36000.76983 + julianCentury * 0.0003032) % 360;

            double earthOrbitalEccentricity = 0.016708634 - julianCentury * (0.000042037 + 0.0000001267 * julianCentury);
            double meanTime = Math.Tan(Convert.ToRadians(meanEclipticObliquityCorrection / 2)) * Math.Tan(Convert.ToRadians(meanEclipticObliquityCorrection / 2));
            double equationOfTime = 4 * Convert.ToDegrees(meanTime * Math.Sin(2 * Convert.ToRadians(sunGeometricMeanLongitude)) - 2 * earthOrbitalEccentricity * Math.Sin(Convert.ToRadians(sunGeometricMeanAnomaly)) + 4 * earthOrbitalEccentricity * meanTime * Math.Sin(Convert.ToRadians(sunGeometricMeanAnomaly)) * Math.Cos(2 * Convert.ToRadians(sunGeometricMeanLongitude)) - 0.5 * meanTime * meanTime * Math.Sin(4 * Convert.ToRadians(sunGeometricMeanLongitude)) - 1.25 * earthOrbitalEccentricity * earthOrbitalEccentricity * Math.Sin(2 * Convert.ToRadians(sunGeometricMeanAnomaly)));
            double trueSolarTime = (dt.TimeOfDay.TotalSeconds / 86400) * 1440 + equationOfTime + 4 * longitude - 60 * utcOffset % 1440;

            return (trueSolarTime / 4 < 0 ? trueSolarTime / 4 + 180 : trueSolarTime / 4 - 180);
        }

    }

}