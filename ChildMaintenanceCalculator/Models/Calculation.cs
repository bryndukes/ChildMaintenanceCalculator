﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildMaintenanceCalculator.Models
{
    public class Calculation
    {
        //TODO: Set up all models to use Dependency Injection?
        //TODO: Encapsulate the properties of this class is possible? or do you not do that with MVC models?

        public PayingParent PayingParent = new PayingParent();

        public User User = new User();

        public string RateBand { get; set; } //Should this be a class/Enum

        private decimal totalMaintenancePayable;
        public decimal TotalMaintenancePayable
        {
            get { return Math.Round(totalMaintenancePayable, 2); }

            set{ totalMaintenancePayable = value; }
        }

        //public decimal TotalMaintenancePayable
        //{
        //    get { return PayingParent.ReceivingParents.SelectMany(r => r.Children).Sum(c => c.ChildMaintenanceAmount); }
        //}


        //Calculates the total child maintenance amount payable for each child individually
        public void Calculate()
        {
            var totalChildren = PayingParent.ReceivingParents.SelectMany(p => p.Children).Count(); // TODO: Should this be a property on PP?

            if (PayingParent.RelevantBenefit == true)
            {
                //Flat Rate - With Shared care reductions
                RateBand = "F1";

                //Calculate Total For Each Child - Including Reductions
                foreach(var receivingParent in PayingParent.ReceivingParents)
                {
                    if (receivingParent.Children.Where(c => c.NightsPayingParentCaresForChildPerYearLow == ">=52").Any())
                    {
                        foreach(var child in receivingParent.Children) //TODO: Linq this
                        {
                            child.ChildMaintenanceAmount = 0;
                        }
                    }
                    else
                    {
                        foreach( var child in receivingParent.Children) //TODO: Linq this
                        {
                            child.ChildMaintenanceAmount = decimal.Divide(7, PayingParent.ReceivingParents.SelectMany(r => r.Children).Count());
                        }
                    }
                }

                return;
            }
            else
            {
                if(PayingParent.GrossWeeklyIncome < 7)
                {
                    //Nil Rate
                    RateBand = "Nil";
                    return; //TODO: Is this good?
                }
                else if(PayingParent.GrossWeeklyIncome >= 7 && PayingParent.GrossWeeklyIncome <= 100)
                {
                    //Flat Rate
                    RateBand = "F1";
                    foreach (var child in PayingParent.ReceivingParents.SelectMany(p => p.Children))
                    {
                        child.ChildMaintenanceAmount = decimal.Divide(7, PayingParent.ReceivingParents.SelectMany(p => p.Children).Count());
                    }
                    return;
                }
                else if(PayingParent.GrossWeeklyIncome > 100 && PayingParent.GrossWeeklyIncome < 200)
                {
                    //Reduced Rate
                    if (totalChildren ==1)
                    {
                        if (PayingParent.OtherSupportedChildren == 0)
                        {
                            RateBand = "R1"; //TODO: Make this an Enum

                            //Calculate Child Maintenance Amount with percentage param for R1 - 17%
                            CalculateMaintenanceAmountReduced(17);
                        }
                        if (PayingParent.OtherSupportedChildren == 1)
                        {
                            RateBand = "R4";
                            CalculateMaintenanceAmountReduced(14.10m);
                        }
                        else if (PayingParent.OtherSupportedChildren == 2)
                        {
                            RateBand = "R5";
                            CalculateMaintenanceAmountReduced(13.20m);
                        }
                        else if (PayingParent.OtherSupportedChildren >= 3)
                        {
                            RateBand = "R6";
                            CalculateMaintenanceAmountReduced(12.40m);
                        }
                    }
                    else if(totalChildren == 2)
                    {
                        if (PayingParent.OtherSupportedChildren == 0)
                        {
                            RateBand = "R2";
                            CalculateMaintenanceAmountReduced(25);
                        }
                        if (PayingParent.OtherSupportedChildren == 1)
                        {
                            RateBand = "R7";
                            CalculateMaintenanceAmountReduced(21.20m);
                        }
                        else if (PayingParent.OtherSupportedChildren == 2)
                        {
                            RateBand = "R8";
                            CalculateMaintenanceAmountReduced(19.90m);
                        }
                        else if (PayingParent.OtherSupportedChildren >= 3)
                        {
                            RateBand = "R9";
                            CalculateMaintenanceAmountReduced(18.90m);
                        }
                    }
                    else if(totalChildren >= 3)
                    {
                        if (PayingParent.OtherSupportedChildren == 0)
                        {
                            RateBand = "R3";
                            CalculateMaintenanceAmountReduced(31);
                        }
                        if (PayingParent.OtherSupportedChildren == 1)
                        {
                            RateBand = "R10";
                            CalculateMaintenanceAmountReduced(26.40m);
                        }
                        else if (PayingParent.OtherSupportedChildren == 2)
                        {
                            RateBand = "R11";
                            CalculateMaintenanceAmountReduced(24.90m);
                        }
                        else if (PayingParent.OtherSupportedChildren >= 3)
                        {
                            RateBand = "R12";
                            CalculateMaintenanceAmountReduced(23.80m);
                        }
                    }
                }
                else
                {
                    //Calculate Reduced Income
                    if(PayingParent.OtherSupportedChildren == 0)
                    {
                        PayingParent.ReducedWeeklyIncome = PayingParent.GrossWeeklyIncome;
                    }
                    else if (PayingParent.OtherSupportedChildren == 1)
                    {
                        CalculateReducedIncome(11);
                    }
                    else if (PayingParent.OtherSupportedChildren == 2)
                    {
                        CalculateReducedIncome(14);
                    }
                    else if (PayingParent.OtherSupportedChildren >= 3)
                    {
                        CalculateReducedIncome(16);
                    }

                    if(PayingParent.GrossWeeklyIncome >= 200 && PayingParent.GrossWeeklyIncome <= 800)
                    {
                        //Basic Rate
                        if(totalChildren == 1)
                        {
                            RateBand = "B1";
                            CalculateMaintenanceAmountBasic(12);
                        }
                        else if (totalChildren == 2)
                        {
                            RateBand = "B2";
                            CalculateMaintenanceAmountBasic(16);
                        }
                        else if (totalChildren >= 3)
                        {
                            RateBand = "B3";
                            CalculateMaintenanceAmountBasic(19);
                        }
                    }
                    else if(PayingParent.GrossWeeklyIncome >800 && PayingParent.GrossWeeklyIncome <= 3000)
                    {
                        //Basic Plus Rate
                        if (totalChildren == 1)
                        {
                            RateBand = "BP1";
                            CalculateMaintenanceAmountBasicPlus(12, 9);
                        }
                        else if (totalChildren == 2)
                        {
                            RateBand = "BP2";
                            CalculateMaintenanceAmountBasicPlus(16, 12);
                        }
                        else if (totalChildren >= 3)
                        {
                            RateBand = "BP3";
                            CalculateMaintenanceAmountBasicPlus(19, 15);
                        }
                    }
                    else
                    {
                        RateBand = "Over"; //TODO: Think about how we're gonna handle this
                    }

                }

            }

            //Calculate Maintenance Amount Per Child
            foreach (var child in PayingParent.ReceivingParents.SelectMany(p => p.Children))
            {
                child.ChildMaintenanceAmount = TotalMaintenancePayable / totalChildren;
            }

            //Calculate shared care reductions (for Reduced, Basic and Basic Plus Rates)
            foreach (var child in PayingParent.ReceivingParents.SelectMany(p=> p.Children))
            {
                if(child.NightsPayingParentCaresForChildPerYearHigh == "52-103")
                {
                    child.ChildMaintenanceAmount = child.ChildMaintenanceAmount - (decimal.Divide(child.ChildMaintenanceAmount, 7));
                }
                else if (child.NightsPayingParentCaresForChildPerYearHigh == "104-155")
                {
                    child.ChildMaintenanceAmount = child.ChildMaintenanceAmount - decimal.Multiply(2,(decimal.Divide(child.ChildMaintenanceAmount, 7)));
                }
                else if (child.NightsPayingParentCaresForChildPerYearHigh == "156-174")
                {
                    child.ChildMaintenanceAmount = child.ChildMaintenanceAmount - decimal.Multiply(3, (decimal.Divide(child.ChildMaintenanceAmount, 7)));
                }
                else if (child.NightsPayingParentCaresForChildPerYearHigh == "175>")
                {
                    child.ChildMaintenanceAmount = child.ChildMaintenanceAmount - (decimal.Multiply(child.ChildMaintenanceAmount, 0.50m) - 7);
                }
            }

            //RecaclculateTotal
            TotalMaintenancePayable = PayingParent.ReceivingParents.SelectMany(p => p.Children).Sum(c => c.ChildMaintenanceAmount);


        }

        private void CalculateMaintenanceAmountBasicPlus(decimal lowerThresholdPercentage, decimal upperThresholdPercentage)
        {
            if(PayingParent.ReducedWeeklyIncome >=3000)
            {
                //£3000 per week is the max weekly income that is taken into consideration, therfore anything over £3000 is calculated at the rate for £3000
                decimal lowerThresholdAmount = decimal.Multiply(8, lowerThresholdPercentage);
                decimal upperThresholdAmount = decimal.Multiply(22, upperThresholdPercentage);
                TotalMaintenancePayable = lowerThresholdAmount + upperThresholdAmount;
            }
            else
            {
                decimal lowerThresholdAmount = decimal.Multiply(8, lowerThresholdPercentage);
                decimal upperThresholdAmount = decimal.Multiply(decimal.Divide((PayingParent.ReducedWeeklyIncome - 800), 100), upperThresholdPercentage);
                TotalMaintenancePayable = lowerThresholdAmount + upperThresholdAmount;
            }

        }

        private void CalculateMaintenanceAmountBasic(decimal percentage)
        {
            TotalMaintenancePayable = decimal.Multiply(decimal.Divide(PayingParent.GrossWeeklyIncome, 100), percentage);
        }

        private void CalculateMaintenanceAmountReduced(decimal percentage)
        {
            TotalMaintenancePayable = decimal.Multiply(decimal.Divide((PayingParent.ReducedWeeklyIncome - 100), 100), percentage) + 7;
        }

        //Should there be a separate method for calculating the actual amounts for each child and this one is just for calculating the rate, or have it all in one.

        private void CalculateReducedIncome(decimal reduction)
        {
            PayingParent.ReducedWeeklyIncome = decimal.Multiply(PayingParent.ReducedWeeklyIncome, decimal.Divide((100 - reduction), 100)); 
        }

    }
}
