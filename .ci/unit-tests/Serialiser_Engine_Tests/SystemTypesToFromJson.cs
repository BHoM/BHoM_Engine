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

using BH.oM.Base;
using BH.oM.Structure.Elements;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace BH.Tests.Engine.Serialiser
{

    public class SystemTypesToFromJson
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/


        [Test]
        public void ToFromJsonListEnums()
        {
            CustomObject custom = new CustomObject();
            List<object> enums = new List<object> { BarFEAType.Axial, BH.oM.Structure.Loads.CaseType.Envelope };

            custom.CustomData["list"] = enums;
            custom.CustomData["single"] = BarFEAType.Axial;

            string json = BH.Engine.Serialiser.Convert.ToJson(custom);

            CustomObject retObj = BH.Engine.Serialiser.Convert.FromJson(json) as CustomObject;

            retObj.ShouldNotBeNull();

            retObj.CustomData.ShouldNotBeNull();
            retObj.CustomData.ShouldContainKey("single");
            retObj.CustomData["single"].ShouldBeEquivalentTo(BarFEAType.Axial);
            retObj.CustomData.ShouldContainKey("list");
            List<object> retList = retObj.CustomData["list"] as List<object>;
            retList.ShouldNotBeNull();
            retList.ShouldContain(BarFEAType.Axial);
            retList.ShouldContain(BH.oM.Structure.Loads.CaseType.Envelope);
        }

        [Test]
        public void ToFromJsonColor()
        {
            System.Drawing.Color color = System.Drawing.Color.Turquoise;

            string json = BH.Engine.Serialiser.Convert.ToJson(color);
            object retObj = BH.Engine.Serialiser.Convert.FromJson(json);

            retObj.ShouldBeOfType(typeof(System.Drawing.Color));

            System.Drawing.Color retColor = (System.Drawing.Color)retObj;
            retColor.A.ShouldBeEquivalentTo(color.A);
            retColor.R.ShouldBeEquivalentTo(color.R);
            retColor.G.ShouldBeEquivalentTo(color.G);
            retColor.B.ShouldBeEquivalentTo(color.B);
        }

        [Test]
        public void ToFromJsonColorObjectProperty()
        {
            CustomObject custom = new CustomObject();
            System.Drawing.Color color = System.Drawing.Color.Turquoise;
            custom.CustomData["Colour"] = color;

            string json = BH.Engine.Serialiser.Convert.ToJson(custom);
            CustomObject retObj = BH.Engine.Serialiser.Convert.FromJson(json) as CustomObject;

            retObj.ShouldNotBeNull();
            retObj.CustomData.ShouldNotBeNull();
            retObj.CustomData.ShouldContainKey("Colour");
            object retColObj = retObj.CustomData["Colour"];
            retColObj.ShouldNotBeNull();

            retColObj.ShouldBeOfType(typeof(System.Drawing.Color));

            System.Drawing.Color retColor = (System.Drawing.Color)retColObj;
            retColor.A.ShouldBeEquivalentTo(color.A);
            retColor.R.ShouldBeEquivalentTo(color.R);
            retColor.G.ShouldBeEquivalentTo(color.G);
            retColor.B.ShouldBeEquivalentTo(color.B);
        }

        [Test]
        public void ToFromJsonDateTimeOffsetObjectProperty()
        {
            ToFromJsonCustomDataProperty(DateTimeOffset.UtcNow);
        }


        /***************************************************/

        [Test]
        public void ToFromJsonTimeSpanObjectProperty()
        {
            ToFromJsonCustomDataProperty(TimeSpan.FromSeconds(43543));
        }


        /***************************************************/

        [Test]
        public void ToFromJsonRegexObjectProperty()
        {
            ToFromJsonCustomDataProperty(new Regex("/d*"));
        }
        /***************************************************/

        [Test]
        public void ToFromJsonIntPtrObjectProperty()
        {
            ToFromJsonCustomDataProperty(new IntPtr(34234));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonGuidProperty()
        {
            ToFromJsonCustomDataProperty(new Guid("E5DD4655-3A14-47E2-A19C-A042EE62EF42"));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonDictionary()
        {
            Dictionary<string, double> dict = new Dictionary<string, double>()
            {
                ["test1"] = 4,
                ["test2"] = 5
            };

            string json = BH.Engine.Serialiser.Convert.ToJson(dict);
            object ret = BH.Engine.Serialiser.Convert.FromJson(json);
        }

        /***************************************************/

        [Test]
        public void ToFromJsonDictionaryAsProperty()
        {
            Dictionary<string, double> dict = new Dictionary<string, double>()
            {
                ["test1"] = 4,
                ["test2"] = 5
            };

            ToFromJsonCustomDataProperty(dict);
        }

        /***************************************************/

        [Test]
        public void ToFromJsonReadOnlyCollectionAsProperty()
        {
            ToFromJsonCustomDataProperty(new ReadOnlyCollection<int>(new List<int> { 5, 6, 7 }));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonSortedDictionary()
        {
            SortedDictionary<double, double> dict = new SortedDictionary<double, double>();

            dict.Add(432.3, 21312);
            dict.Add(342.2, 12312);

            string json = BH.Engine.Serialiser.Convert.ToJson(dict);
            object ret = BH.Engine.Serialiser.Convert.FromJson(json);

            CheckDictionary(ret, dict);
        }

        /***************************************************/

        [Test]
        public void ToFromJsonSortedDictionaryAsProperty()
        {
            SortedDictionary<double, double> dict = new SortedDictionary<double, double>();

            dict.Add(432.3, 21312);
            dict.Add(342.2, 12312);

            ToFromJsonCustomDataProperty(dict);
        }

        [Test]
        public void ToFromJsonArrayAsProperty()
        {
            ToFromJsonCustomDataProperty(new int[] { 6, 7, 8 });
        }

        [Test]
        public void ToFromJsonTuple2Property()
        {
            ToFromJsonCustomDataProperty(new Tuple<int, string>(5, "test"));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonTuple3Property()
        {
            ToFromJsonCustomDataProperty(new Tuple<int, string, double>(5, "test", 234.2));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonTuple4Property()
        {
            ToFromJsonCustomDataProperty(new Tuple<int, string, double, string>(5, "test", 423.3, "asd"));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonTuple5Property()
        {
            ToFromJsonCustomDataProperty(new Tuple<int, string, double, double, double>(5, "test", 34534.3, 0.423, 0.2432));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonTuple5OtherSystemTypesProperty()
        {
            ToFromJsonCustomDataProperty(new Tuple<DateTimeOffset, TimeSpan, Regex, IntPtr, System.Drawing.Color>(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(43543), new Regex("/d*"), new IntPtr(982374), System.Drawing.Color.FromArgb(3)));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonHashSetObjectProperty()
        {
            ToFromJsonCustomDataProperty(new HashSet<int>(new List<int> { 5, 6, 7 }));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonListObjectProperty()
        {
            ToFromJsonCustomDataProperty(new List<Node> { new Node(), new Node() });
        }

        /***************************************************/

        [Test]
        public void ToFromJsonReadOnlyCollectionObjectProperty()
        {
            ToFromJsonCustomDataProperty(new ReadOnlyCollection<Node>(new List<Node> { new Node(), new Node() }));
        }

        /***************************************************/

        [Test]
        public void ToFromJsonBitMapObjectProperty()
        {
            ToFromJsonCustomDataProperty(RandomBitmap());       
        }

        /***************************************************/

        private void ToFromJsonCustomDataProperty<T>(T value)
        {
            string keyItem = "item";
            string keyList = "list";
            string keyGenList = "genList";

            string keyDict1 = "dict1";
            string keyDict2 = "dict2";
            string keyDict3 = "dict3";
            string keyDict4 = "dict4";
            string keyDict5 = "dict5";

            CustomObject custom = new CustomObject();
            custom.CustomData[keyItem] = value;
            custom.CustomData[keyList] = new List<object> { value };
            custom.CustomData[keyGenList] = new List<T> { value };


            var dict1 = new Dictionary<string, object> { { "key", value } };
            var dict2 = new Dictionary<decimal, object> { { 4.4m, value } };
            var dict3 = new Dictionary<string, T> { { "key", value } };
            var dict4 = new Dictionary<T, string> { { value, "key" } };
            var dict5 = new Dictionary<object, object> { { value, value } };

            custom.CustomData[keyDict1] = dict1;
            custom.CustomData[keyDict2] = dict2;
            custom.CustomData[keyDict3] = dict3;
            custom.CustomData[keyDict4] = dict4;
            custom.CustomData[keyDict5] = dict5;

            string json = BH.Engine.Serialiser.Convert.ToJson(custom);

            CustomObject retCustom = BH.Engine.Serialiser.Convert.FromJson(json) as CustomObject;

            retCustom.ShouldNotBeNull();
            retCustom.CustomData.ShouldNotBeNull();
            retCustom.CustomData.ShouldContainKey(keyItem);
            object retValue = retCustom.CustomData[keyItem];
            retValue.ShouldNotBeNull();

            retValue.ShouldBeOfType(typeof(T));

            T retOffset = (T)retValue;

            EquivalentCheckList(retOffset, value);

            retCustom.CustomData.ShouldContainKey(keyList);
            object listVal = retCustom.CustomData[keyList];
            listVal.ShouldNotBeNull();

            listVal.ShouldBeOfType(typeof(List<object>));

            List<object> retList = (List<object>)listVal;
            retList.Count.ShouldBe(1);
            EquivalentCheckList(retList[0], value);


            retCustom.CustomData.ShouldContainKey(keyGenList);
            object genListVal = retCustom.CustomData[keyGenList];
            genListVal.ShouldNotBeNull();

            genListVal.ShouldBeOfType(typeof(List<T>));

            List<T> retGenList = (List<T>)genListVal;
            retGenList.Count.ShouldBe(1);
            EquivalentCheckList(retGenList[0], value);

            CheckDictionary(retCustom, keyDict1, dict1);
            CheckDictionary(retCustom, keyDict2, dict2);
            CheckDictionary(retCustom, keyDict3, dict3);
            CheckDictionary(retCustom, keyDict4, dict4);
            CheckDictionary(retCustom, keyDict5, dict5);
        }

        /***************************************************/

        private void CheckDictionary<T1, T2>(CustomObject retCustom, string key, IDictionary<T1, T2> val)
        {
            retCustom.CustomData.ShouldContainKey(key);
            object dictVal = retCustom.CustomData[key];
            CheckDictionary(dictVal, val);
        }

        /***************************************************/

        private void CheckDictionary<T1, T2>(object dictVal, IDictionary<T1, T2> val)
        {
            dictVal.ShouldNotBeNull();

            dictVal.ShouldBeOfType(val.GetType());

            IDictionary<T1, T2> retDict = (IDictionary<T1, T2>)dictVal;
            retDict.Count.ShouldBe(val.Count);
            EquivalentCheckList(retDict.Keys.ToArray()[0], val.Keys.ToArray()[0]);
            EquivalentCheckList(retDict.Values.ToArray()[0], val.Values.ToArray()[0]);
        }

        /***************************************************/

        public static void EquivalentCheckList(object val, object refVal)
        {
            if (refVal == null)
                return;

            val.ShouldBeOfType(refVal.GetType());

            if ((refVal is IEnumerable) && !(refVal is string))
            {
                List<object> valEnum = ((IEnumerable)val).Cast<object>().ToList();
                List<object> refEnumerable = ((IEnumerable)refVal).Cast<object>().ToList();

                valEnum.Count.ShouldBe(refEnumerable.Count);
                for (int i = 0; i < valEnum.Count; i++)
                {
                    EquivalentCheckList(valEnum[i], refEnumerable[i]);
                }

            }
            else
            {
                val.ShouldBeEquivalentTo(refVal);
            }
        }

        /***************************************************/

        private static Bitmap RandomBitmap()
        {
            Bitmap bitmap = new Bitmap(80, 20, PixelFormat.Format24bppRgb);

            // 2. Get access to the raw bitmap data
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            // 3. Generate RGB noise and write it to the bitmap's buffer.
            // Note that we are assuming that data.Stride == 3 * data.Width for simplicity/brevity here.
            byte[] noise = new byte[data.Width * data.Height * 3];
            new Random(3).NextBytes(noise);
            Marshal.Copy(noise, 0, data.Scan0, noise.Length);

            bitmap.UnlockBits(data);

            // 4. Save as JPEG and convert to Base64
            MemoryStream jpegStream = new MemoryStream();

            bitmap.Save(jpegStream, ImageFormat.Bmp);

            return new Bitmap(jpegStream);
        }

        /***************************************************/
    }
}


