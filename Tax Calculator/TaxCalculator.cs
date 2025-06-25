using System;

namespace TaxCalculatorApp
{
    public class TaxCalculator
    {
        public double CalculateNDFl(double income, int rate)
        {
            return income * rate / 100;
        }

        public double CalculatePropertyTax(double salePrice)
        {
            const double deduction = 1_000_000;
            double taxableAmount = Math.Max(0, salePrice - deduction);
            return taxableAmount * 13 / 100;
        }
    }
}