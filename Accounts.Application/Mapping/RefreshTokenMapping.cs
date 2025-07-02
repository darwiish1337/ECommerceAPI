using Accounts.Application.DTOs;
using Accounts.Domain.Users;
using Mapster;

namespace Accounts.Application.Mapping;

public class RefreshTokenMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RefreshToken, RefreshTokenDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Token, src => src.Token)
            .Map(dest => dest.ExpiresAt, src => src.ExpiresAt)
            .Map(dest => dest.IsActive, src => src.IsActive);
    }
}
