namespace SpravkiFirstDraft.Models.Brandex
{
    using System;
    using Microsoft.AspNetCore.Http;

    public class BrandexInputModel
    {
        public string Date { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
