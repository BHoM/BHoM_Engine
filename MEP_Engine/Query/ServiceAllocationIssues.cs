/*
* This file is part of the Buildings and Habitats object Model (BHoM)
* Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
*
* Each contributor holds copyright over their respective contributions.
* The project versioning (Git) records all such contribution source information.
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
* along with this code. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.MEP.Criteria;
using BH.oM.MEP.Systems;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> ServiceAllocationIssues(
            Guid spaceId,
            SpaceServiceStrategy resolvedStrategy,
            IEnumerable<ServiceGroupAllocation> allocations)
        {
            if (spaceId == Guid.Empty)
            {
                throw new ArgumentException(
                    "A non-empty space identity is required.",
                    nameof(spaceId));
            }

            if (resolvedStrategy == null)
            {
                throw new ArgumentNullException(
                    nameof(resolvedStrategy));
            }

            if (allocations == null)
            {
                throw new ArgumentNullException(
                    nameof(allocations));
            }

            var spaceAllocations = ServiceAllocationsForSpace(
                spaceId,
                allocations);

            var issues = new List<string>();

            CheckCoolingAllocations(
                spaceId,
                resolvedStrategy.CoolingStrategy,
                spaceAllocations,
                issues);

            CheckOutdoorAirAllocations(
                spaceId,
                resolvedStrategy.OutdoorAirStrategy,
                spaceAllocations,
                issues);

            CheckHeatingAllocations(
                spaceId,
                resolvedStrategy.HeatingStrategy,
                spaceAllocations,
                issues);

            CheckExtractAllocations(
                spaceId,
                resolvedStrategy.ExtractStrategy,
                spaceAllocations,
                issues);

            return issues;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void CheckCoolingAllocations(
            Guid spaceId,
            CoolingStrategy strategy,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            var relevant = allocations
                .Where(x =>
                    x.ServiceRole == ServiceRole.LocalSensibleCooling
                    || x.ServiceRole == ServiceRole.CentralSensibleCooling
                    || x.ServiceRole == ServiceRole.LocalLatentCooling
                    || x.ServiceRole == ServiceRole.CentralLatentCooling)
                .ToList();

            CheckEmptyAllocationGroupIds(
                spaceId,
                relevant,
                issues);

            switch (strategy)
            {
                case CoolingStrategy.Unspecified:
                    issues.Add(
                        $"Space {spaceId}: cooling strategy is "
                        + "Unspecified; the required cooling allocation "
                        + "cannot be determined.");
                    break;

                case CoolingStrategy.NoneRequired:
                    CheckUnexpectedServiceAllocations(
                        spaceId,
                        "cooling",
                        strategy.ToString(),
                        relevant,
                        issues);
                    break;

                case CoolingStrategy.FanCoil:
                case CoolingStrategy.VRF:
                    CheckRequiredServiceAllocation(
                        spaceId,
                        "cooling",
                        strategy.ToString(),
                        ServiceRole.LocalSensibleCooling,
                        relevant,
                        issues);
                    break;

                case CoolingStrategy.CentralAir:
                    CheckRequiredServiceAllocation(
                        spaceId,
                        "cooling",
                        strategy.ToString(),
                        ServiceRole.CentralSensibleCooling,
                        relevant,
                        issues);
                    break;

                case CoolingStrategy.Other:
                    issues.Add(
                        $"Space {spaceId}: cooling strategy is Other; "
                        + "an explicit allocation rule is required.");
                    break;

                default:
                    issues.Add(
                        $"Space {spaceId}: unsupported cooling "
                        + $"strategy value {(int)strategy}.");
                    break;
            }
        }

        /***************************************************/

        private static void CheckOutdoorAirAllocations(
            Guid spaceId,
            OutdoorAirStrategy strategy,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            var relevant = allocations
                .Where(x =>
                    x.ServiceRole == ServiceRole.OutdoorAirSupply)
                .ToList();

            CheckEmptyAllocationGroupIds(
                spaceId,
                relevant,
                issues);

            switch (strategy)
            {
                case OutdoorAirStrategy.Unspecified:
                    issues.Add(
                        $"Space {spaceId}: outdoor-air strategy is "
                        + "Unspecified; the required allocation "
                        + "cannot be determined.");
                    break;

                case OutdoorAirStrategy.NoneRequired:
                    CheckUnexpectedServiceAllocations(
                        spaceId,
                        "outdoor-air",
                        strategy.ToString(),
                        relevant,
                        issues);
                    break;

                case OutdoorAirStrategy.Natural:
                    if (relevant.Count > 0)
                    {
                        issues.Add(
                            $"Space {spaceId}: outdoor-air strategy is "
                            + "Natural, but an OutdoorAirSupply allocation "
                            + "exists. Review whether a mechanical or "
                            + "mixed-mode strategy is intended.");
                    }
                    break;

                case OutdoorAirStrategy.CentralMechanical:
                case OutdoorAirStrategy.LocalMechanical:
                    CheckRequiredServiceAllocation(
                        spaceId,
                        "outdoor-air",
                        strategy.ToString(),
                        ServiceRole.OutdoorAirSupply,
                        relevant,
                        issues);
                    break;

                case OutdoorAirStrategy.Other:
                    issues.Add(
                        $"Space {spaceId}: outdoor-air strategy is Other; "
                        + "an explicit allocation rule is required.");
                    break;

                default:
                    issues.Add(
                        $"Space {spaceId}: unsupported outdoor-air "
                        + $"strategy value {(int)strategy}.");
                    break;
            }
        }

        /***************************************************/

        private static void CheckHeatingAllocations(
            Guid spaceId,
            HeatingStrategy strategy,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            var relevant = allocations
                .Where(x =>
                    x.ServiceRole == ServiceRole.LocalHeating
                    || x.ServiceRole == ServiceRole.CentralHeating)
                .ToList();

            CheckEmptyAllocationGroupIds(
                spaceId,
                relevant,
                issues);

            switch (strategy)
            {
                case HeatingStrategy.Unspecified:
                    issues.Add(
                        $"Space {spaceId}: heating strategy is "
                        + "Unspecified; the required allocation "
                        + "cannot be determined.");
                    break;

                case HeatingStrategy.NoneRequired:
                    CheckUnexpectedServiceAllocations(
                        spaceId,
                        "heating",
                        strategy.ToString(),
                        relevant,
                        issues);
                    break;

                case HeatingStrategy.FanCoil:
                case HeatingStrategy.Radiator:
                    CheckRequiredServiceAllocation(
                        spaceId,
                        "heating",
                        strategy.ToString(),
                        ServiceRole.LocalHeating,
                        relevant,
                        issues);
                    break;

                case HeatingStrategy.CentralAir:
                    CheckRequiredServiceAllocation(
                        spaceId,
                        "heating",
                        strategy.ToString(),
                        ServiceRole.CentralHeating,
                        relevant,
                        issues);
                    break;

                case HeatingStrategy.Other:
                    issues.Add(
                        $"Space {spaceId}: heating strategy is Other; "
                        + "an explicit allocation rule is required.");
                    break;

                default:
                    issues.Add(
                        $"Space {spaceId}: unsupported heating "
                        + $"strategy value {(int)strategy}.");
                    break;
            }
        }

        /***************************************************/

        private static void CheckExtractAllocations(
            Guid spaceId,
            ExtractStrategy strategy,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            var relevant = allocations
                .Where(x =>
                    x.ServiceRole == ServiceRole.Extract)
                .ToList();

            CheckEmptyAllocationGroupIds(
                spaceId,
                relevant,
                issues);

            switch (strategy)
            {
                case ExtractStrategy.Unspecified:
                    issues.Add(
                        $"Space {spaceId}: extract strategy is "
                        + "Unspecified; the required allocation "
                        + "cannot be determined.");
                    break;

                case ExtractStrategy.NoneRequired:
                    CheckUnexpectedServiceAllocations(
                        spaceId,
                        "extract",
                        strategy.ToString(),
                        relevant,
                        issues);
                    break;

                case ExtractStrategy.CentralMechanical:
                case ExtractStrategy.DedicatedMechanical:
                    CheckRequiredServiceAllocation(
                        spaceId,
                        "extract",
                        strategy.ToString(),
                        ServiceRole.Extract,
                        relevant,
                        issues);
                    break;

                case ExtractStrategy.Other:
                    issues.Add(
                        $"Space {spaceId}: extract strategy is Other; "
                        + "an explicit allocation rule is required.");
                    break;

                default:
                    issues.Add(
                        $"Space {spaceId}: unsupported extract "
                        + $"strategy value {(int)strategy}.");
                    break;
            }
        }

        /***************************************************/

        private static void CheckRequiredServiceAllocation(
            Guid spaceId,
            string serviceName,
            string strategyName,
            ServiceRole requiredRole,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            int count = allocations.Count(x =>
                x.ServiceRole == requiredRole);

            if (count == 0)
            {
                issues.Add(
                    $"Space {spaceId}: {serviceName} strategy "
                    + $"{strategyName} requires one {requiredRole} "
                    + "allocation, but none exists.");
            }
            else if (count > 1)
            {
                issues.Add(
                    $"Space {spaceId}: {serviceName} strategy "
                    + $"{strategyName} requires one {requiredRole} "
                    + $"allocation, but {count} records exist. "
                    + "Check for duplicate or conflicting allocations.");
            }
        }

        /***************************************************/

        private static void CheckUnexpectedServiceAllocations(
            Guid spaceId,
            string serviceName,
            string strategyName,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            if (allocations.Count > 0)
            {
                issues.Add(
                    $"Space {spaceId}: {serviceName} strategy is "
                    + $"{strategyName}, but {allocations.Count} "
                    + $"{serviceName} allocation(s) exist.");
            }
        }

        /***************************************************/

        private static void CheckEmptyAllocationGroupIds(
            Guid spaceId,
            List<ServiceGroupAllocation> allocations,
            List<string> issues)
        {
            foreach (var allocation in allocations)
            {
                if (allocation.ServiceGroupId == Guid.Empty)
                {
                    issues.Add(
                        $"Space {spaceId}: allocation for "
                        + $"{allocation.ServiceRole} has an empty "
                        + "ServiceGroupId.");
                }
            }
        }

        /***************************************************/
    }
}