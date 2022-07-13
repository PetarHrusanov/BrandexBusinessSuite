namespace BrandexBusinessSuite.SalesAnalysis.Models.Map;

using AutoMapper;

public interface IMapFrom<T>
{
    void Mapping(Profile mapper) => mapper.CreateMap(typeof(T), this.GetType());
}