using AutoMapper;
using ECommerce.Users.Core.DTOs;
using ECommerce.Users.Core.Entities;
using ECommerce.Users.Core.RepositoryContacts;
using ECommerce.Users.Core.ServiceContacts;

namespace ECommerce.Users.Core.Services;

internal class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public async Task<AuthenticationResponse?> Login(LoginRequest loginRequest)
    {
        AppplicaitonUser? user = await _usersRepository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);
        if (user == null)
        {
            return null;
        }
        return _mapper.Map<AuthenticationResponse>(user) with { Success = true, Token = "token" };
    }

    public async Task<AuthenticationResponse?> RegisterAsync(RegisterRequest registerRequest)
    {
        AppplicaitonUser user = new AppplicaitonUser()
        {
            UserID = Guid.NewGuid(),
            Email = registerRequest.Email,
            Password = registerRequest.Password,
            PersonName = registerRequest.PersonName,
            Gender = registerRequest.Gender.ToString()
        };
        AppplicaitonUser? userNew = await _usersRepository.AddUser(user);
        if (userNew == null)
        {
            return null;
        }
        return _mapper.Map<AuthenticationResponse>(user) with { Success = true, Token = "token" };
    }
}