using System;
namespace SpravkiFirstDraft.Services
{
    public interface INumbersChecker
    {
        public bool WholeNumberCheck(string input);

        public bool NegativeNumberIncludedCheck(string input);
    }
}
