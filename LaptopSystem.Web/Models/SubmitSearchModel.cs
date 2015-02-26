namespace LaptopSystem.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SubmitSearchModel
    {
        public string ModelSearch { get; set; }

        public string ManufacturerSearch { get; set; }

        public decimal PriceSearch { get; set; }
    }
}