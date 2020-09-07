namespace SpravkiFirstDraft.Models.Pharmnet
{
    using System;
    using Microsoft.AspNetCore.Http;

    public class PharmnetInputModel
    {
        public string Date { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
