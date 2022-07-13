namespace BrandexBusinessSuite.SalesAnalysis.Services
{
    using System.Text.RegularExpressions;

    public class NumbersChecker :INumbersChecker
    {
        public bool WholeNumberCheck(string input)
        {
            return Regex.IsMatch(input, @"^\d+$");
        }

        public bool NegativeNumberIncludedCheck(string input)
        {
            return Regex.IsMatch(input, @"^[-+]?\d+(\.\d+)?$");
        }
    }
}
