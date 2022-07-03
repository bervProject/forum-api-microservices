namespace UserService.Model;

using AutoMapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreation, Users>();
    }
}