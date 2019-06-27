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

using BH.oM.Theatron.Elements;
using BH.oM.Theatron.Parameters;
using System;
using System.Collections.Generic;


namespace BH.Engine.Theatron
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods                            ****/
        /***************************************************/

        public static TheatronPlanGeometry PlanGeometry(TheatronParameters parameters)
        {
            var planGeometry = new TheatronPlanGeometry();

            return planGeometry;

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void setSectionPlaneLocations(TheatronParameters bp,ref TheatronPlanGeometry planGeometry)
        {
            planGeometry.SectionPlanes = new List<Plane>();
            
            switch (bp.bowlType)
            {
                case 1:
                    radialPlaneSetUp(bp.pitchWidth, bp.pitchLength, bp.sideBound, bp.sideRad, bp.endBound, bp.endRad, bp.cornerRad, bp.cornerBays, bp.structBay, bp.cornerFraction);
                    break;
                case 2:
                    orthoPlaneSetUp(bp.pitchWidth, bp.pitchLength, bp.endBound, bp.sideBound, bp.cornerRad, bp.structBay, bp.cornerBays);
                    break;
                case 3:
                    noCornerPlaneSetUp(bp.pitchWidth, bp.pitchLength, bp.endBound, bp.structBay, bp.sideBound);
                    break;
                case 4:
                    circularPlaneSetUp(bp.radius, bp.structBay);
                    break;
            }


        }
        private void circularPlaneSetUp(double radius, double structBayW)
        {
            this.sectionPlanes = new List<Plane>();
            this.sectionOrigins = new List<Point3d>();
            int numBays = (int)(Math.Floor(Math.PI * radius * 2 / structBayW));
            double theta = 2 * Math.PI / numBays;
            bool halfbayStart = false;
            this.sectionPlanes = arcSweepBay(0, 0, theta, 0, radius, numBays, halfbayStart, 1.0);
            int bayType = 0;
            for (int i = 0; i < this.sectionPlanes.Count; i++)
            {
                this.bayType.Add(bayType);
                sectionOrigins.Add(sectionPlanes[i].Origin);
            }

        }
        private void noCornerPlaneSetUp(double playWidth, double playLength, double endBound, double structBayW, double sideBound)
        {
            int nSideBays = (int)(Math.Floor(((playLength + endBound) / 2) / structBayW) * 2);
            int nEndBays = (int)(Math.Floor(((playWidth + sideBound) / 2) / structBayW) * 2);
            this.sectionPlanes = new List<Plane>();
            this.sectionOrigins = new List<Point3d>();
            double actualBayW;
            double xMin;
            double yMin;
            double oX, oY, dX, dY;
            int count = 0;
            Point3d origin = new Point3d();
            Vector3d xdir = new Vector3d();
            Vector3d ydir = new Vector3d();
            Plane tempPlane = new Plane();
            int bayType = 0;//0 = side, 1= end, 2 =corner
            for (int i = 0; i < 4; i++)
            {

                if (i % 2 == 0)//side bay
                {
                    bayType = 0;
                    actualBayW = (playLength + endBound) / nSideBays;
                    for (int d = 0; d <= nSideBays; d++)
                    {
                        this.bayType.Add(bayType);
                        if (i == 0)//right side
                        {
                            yMin = (nSideBays * actualBayW) / -2;
                            //origin xyz 
                            oX = ((playWidth + sideBound) / 2);
                            oY = yMin + (actualBayW * d);
                            dX = 1;
                            dY = 0;
                        }
                        else//left side
                        {
                            yMin = (nSideBays * actualBayW) / 2;
                            //origin xyz 
                            oX = -((playWidth + sideBound) / 2);
                            oY = yMin - (actualBayW * d);
                            dX = -1;
                            dY = 0;
                        }
                        origin = new Point3d(oX, oY, 0);
                        sectionOrigins.Add(origin);
                        xdir = new Vector3d(dX, dY, 0);
                        ydir = Vector3d.ZAxis;
                        tempPlane = new Plane(origin, xdir, ydir);
                        sectionPlanes.Add(tempPlane);
                        //sectionPlane[count] = new Plane(new Point(oX, oY, 0), new THREE.Vector3(dX, dY, 0));
                        count++;
                    }
                }

                else
                {
                    bayType = 1;
                    actualBayW = (playWidth + sideBound) / nEndBays;
                    xMin = (nEndBays * actualBayW) / 2;
                    //                                    
                    for (int d = 0; d <= nEndBays; d++)
                    {
                        this.bayType.Add(bayType);
                        if (i == 1)// northEnd
                        {
                            //origin xyz 
                            oX = xMin - (actualBayW * d);
                            oY = ((playLength + endBound) / 2);

                            dX = 0;
                            dY = 1;// 0;
                        }
                        else
                        {
                            //origin xyz 
                            oX = -xMin + (actualBayW * d);
                            oY = -((playLength + endBound) / 2);
                            dX = 0;
                            dY = -1;
                        }
                        origin = new Point3d(oX, oY, 0);
                        sectionOrigins.Add(origin);
                        xdir = new Vector3d(dX, dY, 0);
                        ydir = Vector3d.ZAxis;
                        tempPlane = new Plane(origin, xdir, ydir);
                        sectionPlanes.Add(tempPlane);
                        //sectionPlane[count] = new Plane(new Point(oX, oY, 0), new THREE.Vector3(dX, dY, 0));
                        count++;
                    }

                }

            }

        }
        private void radialPlaneSetUp(double playWidth, double playLength, double sideBound, double sideRadius, double endBound, double endRadius, double cornerR, int nCornerBays, double structBayW, double cornerFraction)
        {
            this.sectionPlanes = new List<Plane>();
            this.sectionOrigins = new List<Point3d>();
            int count = 0;
            double sidecentreX = playWidth / 2 + sideBound - sideRadius;
            double sidecentreY = 0;
            double endcentreX = 0;
            double endcentreY = playLength / 2 + endBound - endRadius;
            Point3d intersect = intersectCircles(sidecentreX, sidecentreY, sideRadius - cornerR, endcentreX, endcentreY, endRadius - cornerR);
            double centreX = intersect.X;
            double centreY = intersect[1];

            double sweepAngleSide = 2 * Math.Atan(centreY / (centreX - sidecentreX));
            double sweepSideBay = 2 * Math.Asin(structBayW / 2 / sideRadius);

            int nSideBays = (int)(Math.Floor(sweepAngleSide / sweepSideBay));

            double sweepAngleEnd = 2 * Math.Atan(centreX / (centreY - endcentreY));
            double sweepEndBay = 2 * Math.Asin(structBayW / 2 / endRadius);
            int nEndBays = (int)(Math.Floor(sweepAngleEnd / sweepEndBay));


            Vector3d endArc = new Vector3d(centreX, centreY - endcentreY, 0);
            Vector3d sideArc = new Vector3d(centreX - sidecentreX, centreY, 0);

            double cornerSweep = Vector3d.VectorAngle(sideArc, endArc);//*Math.PI/180;
            double cornA = cornerSweep / (nCornerBays + 2 * cornerFraction);
            double trueR = cornerR / Math.Cos(cornA / 2);
            double theta = 1, startAngle = 1, radius = 1;
            int numBays = 1;
            bool fractionbayStart = false;
            int bayType = 0; //0 = side, 1= end, 2 =corner

            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0://side right
                        centreX = sidecentreX;
                        centreY = 0;
                        theta = sweepSideBay;
                        numBays = nSideBays;
                        startAngle = (numBays * theta) / -2;
                        radius = sideRadius;
                        fractionbayStart = false;
                        bayType = 0;
                        break;
                    case 1://top right corner
                        centreX = intersect[0];
                        centreY = intersect[1];
                        theta = cornA;
                        startAngle = sweepAngleSide / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = 2;
                        break;
                    case 2://north end
                        centreX = 0;
                        centreY = endcentreY;
                        theta = sweepEndBay;
                        numBays = nEndBays;
                        startAngle = Math.PI / 2 - numBays * theta / 2;
                        radius = endRadius;
                        fractionbayStart = false;
                        bayType = 1;
                        break;
                    case 3://north west corner
                        centreX = -intersect[0];
                        centreY = intersect[1];
                        theta = cornA;
                        startAngle = Math.PI / 2 + sweepAngleEnd / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = 2;
                        break;
                    case 4://west side
                        centreX = -sidecentreX;
                        centreY = 0;
                        theta = sweepSideBay;
                        numBays = nSideBays;
                        startAngle = Math.PI - (numBays * theta) / 2;
                        radius = sideRadius;
                        fractionbayStart = false;
                        bayType = 0;
                        break;
                    case 5://south west conrer
                        centreX = -intersect[0];
                        centreY = -intersect[1];
                        theta = cornA;
                        startAngle = Math.PI + sweepAngleSide / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = 2;
                        break;
                    case 6:// south end
                        centreX = 0;
                        centreY = -endcentreY;
                        theta = sweepEndBay;
                        numBays = nEndBays;
                        startAngle = 1.5 * Math.PI - numBays * theta / 2;
                        radius = endRadius;
                        fractionbayStart = false;
                        bayType = 1;
                        break;
                    case 7://south east corner
                        centreX = intersect[0];
                        centreY = -intersect[1];
                        theta = cornA;
                        startAngle = 1.5 * Math.PI + sweepAngleEnd / 2;
                        radius = cornerR;
                        numBays = nCornerBays;
                        fractionbayStart = true;
                        bayType = 2;
                        break;
                }
                List<Plane> partPlanes = arcSweepBay(centreX, centreY, theta, startAngle, radius, numBays, fractionbayStart, cornerFraction);
                for (int p = 0; p < partPlanes.Count; p++)
                {
                    sectionPlanes.Add(partPlanes[p]);
                    sectionOrigins.Add(partPlanes[p].Origin);
                    this.bayType.Add(bayType);
                    count++;
                }
            }


        }
        private void orthoPlaneSetUp(double playWidth, double playLength, double endBound, double sideBound, double cornerR, double structBayW, int nCornerBays)
        {
            int nSideBays = (int)(Math.Floor(((playLength + endBound) / 2 - cornerR) / structBayW) * 2);
            int nEndBays = (int)(Math.Floor(((playWidth + sideBound) / 2 - cornerR) / structBayW) * 2);
            this.sectionPlanes = new List<Plane>();
            this.sectionOrigins = new List<Point3d>();
            double cornA = Math.PI / 2 / (nCornerBays + 1);
            double trueR = cornerR / Math.Cos(cornA / 2);
            double xMin;
            double yMin;
            double oX, oY, dX, dY;
            int count = 0;
            Point3d origin = new Point3d();
            Vector3d xdir = new Vector3d();
            Vector3d ydir = new Vector3d();
            Plane tempPlane = new Plane();
            int bayType = 0; //0 = side, 1= end, 2 =corner
            for (int i = 0; i < 8; i++)
            {
                if (i == 0 || i == 4)//side bay
                {
                    bayType = 0;
                    for (int d = 0; d <= nSideBays; d++)
                    {
                        this.bayType.Add(bayType);
                        if (i == 0)//right side
                        {
                            yMin = (nSideBays * structBayW) / -2;
                            //origin xyz 
                            oX = ((playWidth + sideBound) / 2);
                            oY = yMin + (structBayW * d);
                            dX = 1;
                            dY = 0;
                        }
                        else//left side
                        {
                            yMin = (nSideBays * structBayW) / 2;
                            //origin xyz 
                            oX = -((playWidth + sideBound) / 2);
                            oY = yMin - (structBayW * d);
                            dX = -1;
                            dY = 0;
                        }
                        origin = new Point3d(oX, oY, 0);
                        sectionOrigins.Add(origin);
                        xdir = new Vector3d(dX, dY, 0);
                        ydir = Vector3d.ZAxis;
                        tempPlane = new Plane(origin, xdir, ydir);
                        sectionPlanes.Add(tempPlane);
                        //sectionPlane[count] = new Plane(new Point(oX, oY, 0), new THREE.Vector3(dX, dY, 0));
                        count++;
                    }
                }
                else
                {

                    if (i == 2 || i == 6)//end bay
                    {
                        bayType = 1;
                        xMin = (nEndBays * structBayW) / 2;
                        //                                    
                        for (int d = 0; d <= nEndBays; d++)
                        {
                            this.bayType.Add(bayType);
                            if (i == 2)// northEnd
                            {
                                //origin xyz 
                                oX = xMin - (structBayW * d);
                                oY = ((playLength + endBound) / 2);
                                dX = 0;
                                dY = 1;
                            }
                            else
                            {
                                //origin xyz 
                                oX = -xMin + (structBayW * d);
                                oY = -((playLength + endBound) / 2);

                                dX = 0;
                                dY = -1;
                            }
                            origin = new Point3d(oX, oY, 0);
                            sectionOrigins.Add(origin);
                            xdir = new Vector3d(dX, dY, 0);
                            ydir = Vector3d.ZAxis;
                            tempPlane = new Plane(origin, xdir, ydir);
                            sectionPlanes.Add(tempPlane);
                            //sectionPlane[count] = new Plane(new Point(oX, oY, 0), new THREE.Vector3(dX, dY, 0));
                            count++;
                        }
                    }
                    else//corners
                    {
                        //local centres cs at fillets
                        bayType = 2;
                        double centreX = (playWidth + sideBound) / 2 - cornerR;
                        double centreY = (playLength + endBound) / 2 - cornerR;
                        double startAngle = 0;
                        if (i == 1) //NE++
                        {

                        }
                        if (i == 3) //NW-+
                        {
                            centreX = -centreX;
                            startAngle = Math.PI / 2;
                        }
                        if (i == 5) //SW--
                        {
                            centreY = -centreY;
                            centreX = -centreX;
                            startAngle = Math.PI;
                        }
                        if (i == 7) //SE+-
                        {
                            centreY = -1 * centreY;
                            startAngle = Math.PI * 1.5;
                        }

                        //                                    
                        for (int d = 0; d <= nCornerBays; d++)
                        {
                            this.bayType.Add(bayType);
                            if (d == 0)//half bay on first
                            {
                                oX = centreX + trueR * Math.Cos(startAngle + cornA / 2);
                                oY = centreY + trueR * Math.Sin(startAngle + cornA / 2);
                                dX = trueR * Math.Cos(startAngle + cornA / 2);
                                dY = trueR * Math.Sin(startAngle + cornA / 2);
                            }
                            else
                            {
                                oX = centreX + trueR * Math.Cos(startAngle + (cornA * d + cornA / 2));
                                oY = centreY + trueR * Math.Sin(startAngle + (cornA * d + cornA / 2));
                                dX = trueR * Math.Cos(startAngle + (cornA * d + cornA / 2));
                                dY = trueR * Math.Sin(startAngle + (cornA * d + cornA / 2));
                            }
                            origin = new Point3d(oX, oY, 0);
                            sectionOrigins.Add(origin);
                            xdir = new Vector3d(dX, dY, 0);
                            ydir = Vector3d.ZAxis;
                            tempPlane = new Plane(origin, xdir, ydir);
                            sectionPlanes.Add(tempPlane);
                            //sectionPlane[count] = new Plane(new Point(oX, oY, 0), new THREE.Vector3(dX, dY, 0));
                            count++;
                        }
                    }
                }
            }

        }
    }
}
