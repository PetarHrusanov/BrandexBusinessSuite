namespace BrandexSalesAdapter.ExcelLogic.Models.Map
{
    using System;
    using AutoMapper;

    public interface IMapFrom<T>
    {
        void Mapping(Profile mapper) => mapper.CreateMap(typeof(T), this.GetType());
    }
}
