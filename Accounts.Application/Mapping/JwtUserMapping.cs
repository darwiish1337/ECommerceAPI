using Accounts.Application.DTOs;
using Accounts.Domain.Users;
using Mapster;
using SharedKernel.DTOs;

namespace Accounts.Application.Mapping;

public class JwtUserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, JwtUserData>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Roles, src => src.Roles.Select(r => r.Name).ToList());

        config.NewConfig<UserDto, JwtUserData>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.Roles, src => src.Roles);
    }
}