/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

using BH.oM.Base.Attributes;

using BH.oM.Environment.Climate;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Serialise a weatherfile object as an EPW file")]
        [Input("weatherfile", "A BHoM Weatherfile object")]
        [Input("outputPath", "Full path to target output file")]
        [Output("outputPath", "Full path to target output file")]
        public static string ExportEPW(WeatherFile weatherfile, string outputPath)
        {
            if (weatherfile == null)
                return "";

            string directoryName = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(directoryName))
            {
                Base.Compute.RecordError($"The following path does not appear to exist: {directoryName}. Please ensure that this path exists, and try again.");
                return null;
            }

            List<string> outputStrings = new List<string>();

            // header
            outputStrings.Add(String.Format("LOCATION,{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                weatherfile.CityName,
                weatherfile.StateName,
                weatherfile.Country,
                "BHoM custom data",
                weatherfile.WMOID,
                weatherfile.SpaceTime.Location.Latitude,
                weatherfile.SpaceTime.Location.Longitude,
                weatherfile.SpaceTime.Location.UtcOffset,
                weatherfile.SpaceTime.Location.Elevation));
            outputStrings.Add("DESIGN CONDITIONS,0");
            outputStrings.Add("TYPICAL / EXTREME PERIODS,0");
            outputStrings.Add("GROUND TEMPERATURES,0");
            outputStrings.Add("HOLIDAYS / DAYLIGHT SAVINGS,No,0,0,0");
            outputStrings.Add(String.Format("COMMENTS 1, This file was generated using BHoM on {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
            outputStrings.Add("COMMENTS 2, NA");
            outputStrings.Add("DATA PERIODS,1,1,Data,Sunday,1 / 1,12 / 31");

            // data
            int n = 0;
            for (DateTime date = new DateTime(2018, 1, 1, 0, 0, 0); date <= new DateTime(2018, 12, 31, 23, 30, 0); date = date.AddHours(1))
            {
                outputStrings.Add(
                    String.Format(
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34}",
                        date.Year,
                        date.Month,
                        date.Day,
                        date.Hour + 1,
                        date.Minute + 60,
                        weatherfile.UncertaintyFlag[n],
                        weatherfile.DryBulbTemperature[n],
                        weatherfile.DewPointTemperature[n],
                        weatherfile.RelativeHumidity[n],
                        weatherfile.AtmosphericStationPressure[n],
                        weatherfile.ExtraterrestrialHorizontalRadiation[n],
                        weatherfile.ExtraterrestrialDirectNormalRadiation[n],
                        weatherfile.HorizontalInfraredRadiationIntensity[n],
                        weatherfile.GlobalHorizontalRadiation[n],
                        weatherfile.DirectNormalRadiation[n],
                        weatherfile.DiffuseHorizontalRadiation[n],
                        weatherfile.GlobalHorizontalIlluminance[n],
                        weatherfile.DirectNormalIlluminance[n],
                        weatherfile.DiffuseHorizontalIlluminance[n],
                        weatherfile.ZenithLuminance[n],
                        weatherfile.WindDirection[n],
                        weatherfile.WindSpeed[n],
                        weatherfile.TotalSkyCover[n],
                        weatherfile.OpaqueSkyCover[n],
                        weatherfile.Visibility[n],
                        weatherfile.CeilingHeight[n],
                        weatherfile.PresentWeatherObservation[n],
                        weatherfile.PresentWeatherCodes[n],
                        weatherfile.PrecipitableWater[n],
                        weatherfile.AerosolOpticalDepth[n],
                        weatherfile.SnowDepth[n],
                        weatherfile.DaysSinceLastSnowfall[n],
                        weatherfile.Albedo[n],
                        weatherfile.LiquidPrecipitationDepth[n],
                        weatherfile.LiquidPrecipitationQuantity[n]
                    )
                );
                n += 1;
            }

            // write to file
            System.IO.File.WriteAllLines(outputPath, outputStrings);

            return outputPath;
        }
    }
}




