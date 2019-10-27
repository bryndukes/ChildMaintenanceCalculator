using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChildMaintenanceCalculator.Models;
using NUnit.Framework;
using FluentAssertions;

namespace ChildMaintenanceCalculator.Tests
{
    [TestFixture]
    class CalculationTests
    {
        private Calculation sut;

        [SetUp]
        public void SetUp()
        {
            sut = new Calculation();
            sut.PayingParent.ReceivingParents = new List<ReceivingParent>();
        }

        [Test]
        public void Calculate_FlatRate_WithBenefit_ReceivingParentWithTwoChildrenHavingDifferentSharedCare_ChildMainteanceAmountShouldBeZeroForThatHousehold()
        {
            //Arrange
            sut.PayingParent.RelevantBenefit = true;
            sut.PayingParent.ReceivingParents.Add(GenerateReceivingParentWithSingleChild(true, Child.SharedCare.LessThan52));
            sut.PayingParent.ReceivingParents.First().Children.Add(GenerateChild(true, Child.SharedCare.MoreThanOrEqualTo52));
            sut.PayingParent.ReceivingParents.Add(GenerateReceivingParentWithSingleChild(true,Child.SharedCare.LessThan52));

            //Act
            sut.Calculate();

            //Assert
            sut.RateBand.Should().Be("F1");
            sut.TotalMaintenancePayable.Should().Be((decimal)2.33);
            sut.PayingParent.ReceivingParents.First().MaintenanceEntitlementAmount.Should().Be(0);
            sut.PayingParent.ReceivingParents.First().Children
                .All(child => child.ChildMaintenanceAmount == 0).Should().BeTrue();
            sut.PayingParent.ReceivingParents.Last().MaintenanceEntitlementAmount.Should().Be((decimal)7/3);
            sut.PayingParent.ReceivingParents.Last().Children
                .First().ChildMaintenanceAmount.Should().Be((decimal)7/3); // 
        }


        [TestCase(1, 0, 0, 0, ExpectedResult = "Nil")]
        [TestCase(1, 364, 0, 0, ExpectedResult = "Nil")]
        [TestCase(1, 500, 137, 0, ExpectedResult = "Nil")]
        [TestCase(1, 365, 0, 0, ExpectedResult = "F1")]
        [TestCase(1, 5214.28, 0, 0, ExpectedResult = "F1")]
        [TestCase(1, 5214.81, 0, 0, ExpectedResult = "R1")]
        [TestCase(1, 10428.05, 0, 0, ExpectedResult = "R1")]
        [TestCase(2, 5214.81, 0, 0, ExpectedResult = "R2")]
        [TestCase(2, 10428.05, 0, 0, ExpectedResult = "R2")]
        [TestCase(3, 5214.81, 0, 0, ExpectedResult = "R3")]
        [TestCase(10, 5214.81, 0, 0, ExpectedResult = "R3")]
        [TestCase(3, 10428.05, 0, 0, ExpectedResult = "R3")]
        [TestCase(10, 10428.05, 0, 0, ExpectedResult = "R3")]
        [TestCase(1, 5214.81, 0, 1, ExpectedResult = "R4")]
        [TestCase(1, 10428.05, 0, 1, ExpectedResult = "R4")]
        [TestCase(1, 5214.81, 0, 2, ExpectedResult = "R5")]
        [TestCase(1, 10428.05, 0, 2, ExpectedResult = "R5")]
        [TestCase(1, 5214.81, 0, 3, ExpectedResult = "R6")]
        [TestCase(1, 10428.05, 0, 3, ExpectedResult = "R6")]
        [TestCase(1, 5214.81, 0, 10, ExpectedResult = "R6")]
        [TestCase(1, 10428.05, 0, 10, ExpectedResult = "R6")]
        [TestCase(2, 5214.81, 0, 1, ExpectedResult = "R7")]
        [TestCase(2, 10428.05, 0, 1, ExpectedResult = "R7")]
        [TestCase(2, 5214.81, 0, 2, ExpectedResult = "R8")]
        [TestCase(2, 10428.05, 0, 2, ExpectedResult = "R8")]
        [TestCase(2, 5214.81, 0, 3, ExpectedResult = "R9")]
        [TestCase(2, 10428.05, 0, 3, ExpectedResult = "R9")]
        [TestCase(2, 5214.81, 0, 10, ExpectedResult = "R9")]
        [TestCase(2, 10428.05, 0, 10, ExpectedResult = "R9")]
        [TestCase(3, 5214.81, 0, 1, ExpectedResult = "R10")]
        [TestCase(3, 10428.05, 0, 1, ExpectedResult = "R10")]
        [TestCase(10, 5214.81, 0, 1, ExpectedResult = "R10")]
        [TestCase(10, 10428.05, 0, 1, ExpectedResult = "R10")]
        [TestCase(3, 5214.81, 0, 2, ExpectedResult = "R11")]
        [TestCase(3, 10428.05, 0, 2, ExpectedResult = "R11")]
        [TestCase(10, 5214.81, 0, 2, ExpectedResult = "R11")]
        [TestCase(10, 10428.05, 0, 2, ExpectedResult = "R11")]
        [TestCase(3, 5214.81, 0, 3, ExpectedResult = "R12")]
        [TestCase(3, 10428.05, 0, 3, ExpectedResult = "R12")]
        [TestCase(10, 5214.81, 0, 10, ExpectedResult = "R12")]
        [TestCase(10, 10428.05, 0, 10, ExpectedResult = "R12")]
        [TestCase(1, 10428.58, 0, 0, ExpectedResult = "B1")]
        [TestCase(1, 41714.28, 0, 0, ExpectedResult = "B1")]
        [TestCase(2, 10428.58, 0, 0, ExpectedResult = "B2")]
        [TestCase(2, 41714.28, 0, 0, ExpectedResult = "B2")]
        [TestCase(3, 10428.58, 0, 0, ExpectedResult = "B3")]
        [TestCase(3, 41714.28, 0, 0, ExpectedResult = "B3")]
        [TestCase(10, 10428.58, 0, 0, ExpectedResult = "B3")]
        [TestCase(10, 41714.28, 0, 0, ExpectedResult = "B3")]
        [TestCase(1, 41714.81, 0, 0, ExpectedResult = "BP1")]
        [TestCase(1, 156428.57, 0, 1, ExpectedResult = "BP1")]
        [TestCase(1, 200000, 0, 0, ExpectedResult = "BP1")]
        [TestCase(2, 41714.81, 0, 0, ExpectedResult = "BP2")]
        [TestCase(2, 156428.57, 0, 0, ExpectedResult = "BP2")]
        [TestCase(2, 200000, 0, 1, ExpectedResult = "BP2")]
        [TestCase(3, 41714.81, 0, 0, ExpectedResult = "BP3")]
        [TestCase(3, 156428.57, 0, 0, ExpectedResult = "BP3")]
        [TestCase(3, 200000, 0, 1, ExpectedResult = "BP3")]
        [TestCase(10, 41714.81, 0, 0, ExpectedResult = "BP3")]
        [TestCase(10, 156428.57, 0, 0, ExpectedResult = "BP3")]
        [TestCase(10, 200000, 0, 1, ExpectedResult = "BP3")]
        public string
            Calculate_WithNoBenefit_WithGivenNumberOfChildren_GivenIncomeAndPension_AndGivenNumberOfOtherDependents_ShouldResultInExpectedRate(int children, decimal income, decimal pension, int dependents)
        {
            //Arrange
            sut.PayingParent.RelevantBenefit = false;
            sut.PayingParent.OtherSupportedChildren = dependents;
            sut.PayingParent.AnnualIncome = income;
            sut.PayingParent.AnnualPension = pension;
            sut.PayingParent.ReceivingParents.Add(GenerateReceivingParentWithSingleChild(false, Child.SharedCare.LessThan52));
            for (int i = 1; i < children; i++)
            {
                sut.PayingParent.ReceivingParents.First().Children.Add(GenerateChild(false, Child.SharedCare.LessThan52));
            }

            //Act
            sut.Calculate();

            //Assert
            return sut.RateBand;
        }

        [TestCase(1, 0, 0, 0, ExpectedResult = 0)]
        [TestCase(1, 364, 0, 0, ExpectedResult = 0)]
        [TestCase(1, 500, 137, 0, ExpectedResult = 0)]
        [TestCase(1, 365, 0, 0, ExpectedResult = 7)]
        [TestCase(1, 5214.28, 0, 0, ExpectedResult = 7)]
        [TestCase(1, 5214.81, 0, 0, ExpectedResult = 7)]
        [TestCase(1, 10428.05, 0, 0, ExpectedResult = 24)]
        [TestCase(2, 5214.81, 0, 0, ExpectedResult = 7)]
        [TestCase(2, 10428.05, 0, 0, ExpectedResult = 32)]
        [TestCase(3, 5214.81, 0, 0, ExpectedResult = 7)]
        [TestCase(3, 10428.05, 0, 0, ExpectedResult = 38)]
        [TestCase(1, 5214.81, 0, 1, ExpectedResult = 7)]
        [TestCase(1, 10428.05, 0, 1, ExpectedResult = 21.10)]
        [TestCase(1, 5214.81, 0, 2, ExpectedResult = 7)]
        [TestCase(1, 10428.05, 0, 2, ExpectedResult = 20.20)]
        [TestCase(1, 5214.81, 0, 3, ExpectedResult = 7)]
        [TestCase(1, 10428.05, 0, 3, ExpectedResult = 19.40)]
        [TestCase(2, 5214.81, 0, 1, ExpectedResult = 7)]
        [TestCase(2, 10428.05, 0, 1, ExpectedResult = 28.20)]
        [TestCase(2, 5214.81, 0, 2, ExpectedResult = 7)]
        [TestCase(2, 10428.05, 0, 2, ExpectedResult = 26.90)]
        [TestCase(2, 5214.81, 0, 3, ExpectedResult = 7)]
        [TestCase(2, 10428.05, 0, 3, ExpectedResult = 25.90)]
        [TestCase(3, 5214.81, 0, 1, ExpectedResult = 7)]
        [TestCase(3, 10428.05, 0, 1, ExpectedResult = 33.40)] 
        [TestCase(3, 5214.81, 0, 2, ExpectedResult = 7)]
        [TestCase(3, 10428.05, 0, 2, ExpectedResult = 31.90)]
        [TestCase(3, 5214.81, 0, 3, ExpectedResult = 7)]
        [TestCase(3, 10428.05, 0, 3, ExpectedResult = 30.80)]
        [TestCase(1, 10428.58, 0, 0, ExpectedResult = 24)]
        [TestCase(1, 10428.58, 0, 1, ExpectedResult = 21.36)]
        [TestCase(1, 10428.58, 0, 2, ExpectedResult = 20.64)]
        [TestCase(1, 10428.58, 0, 3, ExpectedResult = 20.16)]
        [TestCase(2, 10428.58, 0, 0, ExpectedResult = 32)]
        [TestCase(2, 10428.58, 0, 1, ExpectedResult = 28.48)]
        [TestCase(2, 10428.58, 0, 2, ExpectedResult = 27.52)]
        [TestCase(2, 10428.58, 0, 3, ExpectedResult = 26.88)]
        [TestCase(3, 10428.58, 0, 0, ExpectedResult = 38)]
        [TestCase(3, 10428.58, 0, 1, ExpectedResult = 33.82)]
        [TestCase(3, 10428.58, 0, 2, ExpectedResult = 32.68)]
        [TestCase(3, 10428.58, 0, 3, ExpectedResult = 31.92)]
        [TestCase(1, 41714.81, 0, 0, ExpectedResult = 96)]
        [TestCase(1, 41714.81, 0, 1, ExpectedResult = 85.44)]
        [TestCase(1, 41714.81, 0, 2, ExpectedResult = 82.56)]
        [TestCase(1, 41714.81, 0, 3, ExpectedResult = 80.64)]
        [TestCase(1, 156428.57, 0, 0, ExpectedResult = 294)]
        [TestCase(1, 200000, 0, 0, ExpectedResult = 294)]
        [TestCase(2, 41714.81, 0, 0, ExpectedResult = 128)]
        [TestCase(2, 41714.81, 0, 1, ExpectedResult = 113.92)]
        [TestCase(2, 41714.81, 0, 2, ExpectedResult = 110.08)]
        [TestCase(2, 41714.81, 0, 3, ExpectedResult = 107.52)]
        [TestCase(2, 156428.57, 0, 0, ExpectedResult = 392)]
        [TestCase(2, 200000, 0, 0, ExpectedResult = 392)]
        [TestCase(3, 41714.81, 0, 0, ExpectedResult = 152)]
        [TestCase(3, 41714.81, 0, 1, ExpectedResult = 135.28)]
        [TestCase(3, 41714.81, 0, 2, ExpectedResult = 130.72)]
        [TestCase(3, 41714.81, 0, 3, ExpectedResult = 127.68)]
        [TestCase(3, 156428.57, 0, 0, ExpectedResult = 482)]
        [TestCase(3, 200000, 0, 0, ExpectedResult = 482)]
        public decimal
            Calculate_WithNoBenefit_WithGivenNumberOfChildren_GivenIncomeAndPension_AndGivenNumberOfOtherDependents_ShouldResultInExpectedMaintenanceAmount(
                int children, decimal income, decimal pension, int dependents)
        {
            //Arrange
            sut.PayingParent.RelevantBenefit = false;
            sut.PayingParent.OtherSupportedChildren = dependents;
            sut.PayingParent.AnnualIncome = income;
            sut.PayingParent.AnnualPension = pension;
            sut.PayingParent.ReceivingParents.Add(GenerateReceivingParentWithSingleChild(false, Child.SharedCare.LessThan52));
            for (int i = 1; i < children; i++)
            {
                sut.PayingParent.ReceivingParents.First().Children.Add(GenerateChild(false, Child.SharedCare.LessThan52));
            }

            //Act
            sut.Calculate();

            //Assert
            return sut.TotalMaintenancePayable;
        }


        //TODO: Add tests for total maintenenace amount and per child amount

        private ReceivingParent GenerateReceivingParentWithSingleChild(bool benefit, Child.SharedCare sharedCare, decimal preexistingArrangements = 0)
        {
            return new ReceivingParent()
            {
                Children = new List<Child>()
                {
                    GenerateChild(benefit, sharedCare, preexistingArrangements)
                }
            };
        }

        private Child GenerateChild(bool benefit, Child.SharedCare sharedCare, decimal preexistingArrangements = 0)
        {
            var child = new Child();
            if (benefit)
            {
                child.NightsPayingParentCaresForChildPerYearLow = sharedCare;
            }
            else
            {
                child.NightsPayingParentCaresForChildPerYearHigh = sharedCare;
            }

            if (preexistingArrangements > 0)
            {
                child.PreExistingMaintenanceArrangementsAmount = preexistingArrangements;
            }

            return child;
        }


    }
}
