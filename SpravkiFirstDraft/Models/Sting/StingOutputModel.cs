namespace SpravkiFirstDraft.Models.Sting
{
    using System;
    using System.Collections.Generic;

    public class StingOutputModel
    {
        public string Date { get; set; }

        public string Table { get; set; }

        public Dictionary<int, string> Errors { get; set; }
    }
}
