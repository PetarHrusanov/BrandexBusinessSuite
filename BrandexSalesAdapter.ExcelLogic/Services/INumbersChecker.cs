namespace BrandexSalesAdapter.ExcelLogic.Services
{
    public interface INumbersChecker
    {
        public bool WholeNumberCheck(string input);

        public bool NegativeNumberIncludedCheck(string input);
    }
}
