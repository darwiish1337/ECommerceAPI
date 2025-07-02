using Accounts.Application.DTOs;
using Accounts.Domain.Users;
using Mapster;

namespace Accounts.Application.Mapping;

public class PermissionMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<PermissionDto, Permission>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
        
    }
}
