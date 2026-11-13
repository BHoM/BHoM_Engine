/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.MEP.Airside;

namespace BH.Engine.MEP
{
    // Internal numerical kernel: no public parallel psychrometric API.
    internal static class AirsideMath
    {
        internal const double MinTemperature = -100.0;
        internal const double MaxTemperature = 200.0;
        internal const double HumidityTolerance = 1e-10;
        internal static void Finite(double x, string name)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                throw new ArgumentOutOfRangeException(name, "A finite value is required.");
        }
        internal static void Temperature(double t)
        {
            Finite(t, nameof(t));
            if (t < MinTemperature || t > MaxTemperature)
                throw new ArgumentOutOfRangeException(nameof(t), "Temperature must be in [-100, 200] degrees Celsius.");
        }
        internal static void Pressure(double p)
        {
            Finite(p, nameof(p));
            if (p <= 0) throw new ArgumentOutOfRangeException(nameof(p));
        }
        internal static void Fraction(double x, string name, bool strictlyPositive = false)
        {
            Finite(x, name);
            if (x < 0 || x > 1 || (strictlyPositive && x == 0))
                throw new ArgumentOutOfRangeException(name, "A fraction in the permitted range is required.");
        }
        internal static double Pws(double t)
        {
            Temperature(t);
            double k = t + 273.15;
            // ASHRAE saturation relations; phase switch at triple point.
            double ln = t <= 0.01
                ? -5674.5359 / k + 6.3925247 - 0.009677843 * k
                  + 0.00000062215701 * k * k + 2.0747825e-9 * k * k * k
                  - 9.484024e-13 * k * k * k * k + 4.1635019 * Math.Log(k)
                : -5800.2206 / k + 1.3914993 - 0.048640239 * k
                  + 0.000041764768 * k * k - 1.4452093e-8 * k * k * k
                  + 6.5459673 * Math.Log(k);
            return Math.Exp(ln); // Pa
        }
        internal static double WFromPv(double pv, double p)
        {
            Pressure(p); Finite(pv, nameof(pv));
            if (pv < 0 || pv >= p)
                throw new ArgumentOutOfRangeException(nameof(pv), "Vapour pressure must be nonnegative and below total pressure.");
            return 0.621945 * pv / (p - pv);
        }
        internal static double Pv(double w, double p) { return p * w / (0.621945 + w); }
        internal static double H(double t, double w) { return 1006.0 * t + w * (2501000.0 + 1860.0 * t); }
        internal static double TFromHW(double h, double w) { return (h - 2501000.0 * w) / (1006.0 + 1860.0 * w); }
        internal static double Cp(double w) { return 1006.0 + 1860.0 * w; }
        internal static double Volume(AirState a)
        {
            return 287.042 * (a.DryBulbTemperature + 273.15) * (1 + 1.607858 * a.HumidityRatio) / a.Pressure;
        }
        internal static void State(AirState a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            Temperature(a.DryBulbTemperature); Pressure(a.Pressure);
            Finite(a.HumidityRatio, nameof(a.HumidityRatio));
            if (a.HumidityRatio < 0)
                throw new ArgumentOutOfRangeException(nameof(a.HumidityRatio));
            if (Pv(a.HumidityRatio, a.Pressure) > Pws(a.DryBulbTemperature) * (1 + 1e-8))
                throw new ArgumentException("Supersaturated air is unsupported; condensation/frost must be resolved explicitly.");
        }
        internal static AirState NewState(double t, double w, double p)
        {
            var a = new AirState { DryBulbTemperature = t, HumidityRatio = w, Pressure = p };
            State(a); return a;
        }
        internal static void Stream(AirStream a, bool allowZero = false)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            State(a.State); Finite(a.DryAirMassFlowRate, nameof(a.DryAirMassFlowRate));
            if (a.DryAirMassFlowRate < 0 || (!allowZero && a.DryAirMassFlowRate == 0))
                throw new ArgumentOutOfRangeException(nameof(a.DryAirMassFlowRate));
        }
        internal static AirStream Copy(AirStream a)
        {
            return new AirStream { State = NewState(a.State.DryBulbTemperature, a.State.HumidityRatio, a.State.Pressure),
                DryAirMassFlowRate = a.DryAirMassFlowRate };
        }
        internal static void SamePressure(double p1, double p2)
        {
            if (Math.Abs(p1 - p2) > 1.0)
                throw new ArgumentException("Stream pressures must agree within 1 Pa; pressure changes are not modelled.");
        }
        internal static double Bisect(Func<double, double> f, double lo, double hi)
        {
            double fl = f(lo), fh = f(hi);
            Finite(fl, "lowerResidual"); Finite(fh, "upperResidual");
            if (Math.Abs(fl) < 1e-12) return lo;
            if (Math.Abs(fh) < 1e-12) return hi;
            if (Math.Sign(fl) == Math.Sign(fh))
                throw new ArgumentException("Solution is outside the supported temperature bracket.");
            for (int i = 0; i < 200; i++)
            {
                double mid = (lo + hi) / 2, fm = f(mid);
                Finite(fm, "residual");
                if (hi - lo < 1e-7 || fm == 0) return mid;
                if (Math.Sign(fl) == Math.Sign(fm)) { lo = mid; fl = fm; }
                else hi = mid;
            }
            throw new InvalidOperationException("Psychrometric bisection did not converge.");
        }
        internal static double WFromWB(double db, double wb, double p)
        {
            Temperature(db); Temperature(wb); Pressure(p);
            if (wb > db) throw new ArgumentException("Wet bulb must not exceed dry bulb.");
            double ws = WFromPv(Pws(wb), p);
            double w = wb >= 0
                ? ((2501.0 - 2.326 * wb) * ws - 1.006 * (db - wb)) / (2501.0 + 1.86 * db - 4.186 * wb)
                : ((2830.0 - 0.24 * wb) * ws - 1.006 * (db - wb)) / (2830.0 + 1.86 * db - 2.1 * wb);
            if (w < -HumidityTolerance) throw new ArgumentException("Wet-bulb pair implies negative humidity ratio.");
            return Math.Max(0, w);
        }
        internal static double WetBulb(AirState a)
        {
            State(a);
            double db = a.DryBulbTemperature, p = a.Pressure, target = a.HumidityRatio;
            if (Math.Abs(Pv(target, p) - Pws(db)) < 1e-5) return db;
            Func<double, double> residual = wb => {
                double ws = WFromPv(Pws(wb), p);
                return (wb >= 0
                    ? ((2501 - 2.326 * wb) * ws - 1.006 * (db - wb)) / (2501 + 1.86 * db - 4.186 * wb)
                    : ((2830 - 0.24 * wb) * ws - 1.006 * (db - wb)) / (2830 + 1.86 * db - 2.1 * wb)) - target;
            };
            // Solve each freezing branch separately; do not converge across a jump.
            double lo = MinTemperature, hi = db;
            if (db > 0 && Pws(db) >= p)
                hi = Bisect(t => Pws(t) - p * (1 - 1e-9), MinTemperature, db);
            if (hi >= 0 && residual(0) <= 0) return Bisect(residual, 0, hi);
            double frozenHi = Math.Min(hi, -1e-8);
            if (residual(frozenHi) >= 0) return Bisect(residual, lo, frozenHi);
            throw new ArgumentException("No wet-bulb solution on the supported freezing branches.");
        }
    }
}
