using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.VectorData;

namespace Catalog.Models;

public class ProductVector
{
    [VectorStoreKey]
    public int Id { get; set; }

    [VectorStoreData]
    public string Name { get; set; }

    [VectorStoreData]
    public string Description { get; set; }

    [VectorStoreData]
    public decimal Price { get; set; }

    [VectorStoreData]
    public string ImageUrl { get; set; }

    [NotMapped]
    [VectorStoreVector(384, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Vector { get; set; }
}
