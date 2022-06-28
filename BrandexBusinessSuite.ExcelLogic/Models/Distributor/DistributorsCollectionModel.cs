﻿namespace BrandexBusinessSuite.ExcelLogic.Models.Distributor;

using System.Collections.Generic;

public class DistributorsCollectionModel
{
    public DistributorsCollectionModel()
    {
        this.Distributors = new HashSet<string>();
    }

    public string Name { get; set; }

    public ICollection<string> Distributors { get; set; }
}