using AutoMapper;

namespace BrandexBusinessSuite.Identity.Models.Identity;

using BrandexBusinessSuite.Identity.Data.Models;
using BrandexBusinessSuite.Models;

public class UsersIds : IMapFrom<ApplicationUser>
{
    public string Id { get; set; }
    public string UserName { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<ApplicationUser, UsersIds>();
}