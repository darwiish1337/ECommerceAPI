using Accounts.Application.DTOs;
using Accounts.Domain.Users;
using Mapster;

namespace Accounts.Application.Mapping;

public class UserVerificationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserVerificationDto, UserVerification>()
            .ConstructUsing(dto => new UserVerification(
                dto.UserId,
                dto.Code,
                dto.ExpiresAt,
                dto.Type
            ))
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.IsUsed, src => src.IsUsed);
    }
}