using Application.Common.Exceptions;
using Application.Common.Extensions.Mappings;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Entities;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public UserService(IUserRepository userRepository, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
    }

    public async Task<UserDataResponse> GetUserByIdAsync(Guid userId)
    {
        User? searchedUser = await _userRepository.GetUserByIdAsync(userId);

        if (searchedUser is null)
        {
            throw new NotFoundException("Não foi possível obter o usuário com o id especificado.");
        }

        return searchedUser.ToUserDataResponse();
    }

    public async Task<UserResponse> RegisterAsync(CreateUserRequest createCreateUserRequest)
    {
        User userToCreate = new()
        {
            Id = Guid.NewGuid(),
            FullName = createCreateUserRequest.FullName,
            UserName = createCreateUserRequest.Username,
            PhoneNumber = createCreateUserRequest.PhoneNumber,
            Email = createCreateUserRequest.Email,
            EmailConfirmed = false,
        };

        User? userWithEmailAlreadyExists = await _userRepository.GetUserByEmailAsync(createCreateUserRequest.Email);
        if (userWithEmailAlreadyExists is not null)
        {
            throw new ConflictException("Usuário com o e-mail especificado já existe.");
        }

        User? userWithUsernameAlreadyExists =
            await _userRepository.GetUserByUsernameAsync(createCreateUserRequest.Username);
        if (userWithUsernameAlreadyExists is not null)
        {
            throw new ConflictException("Usuário com o username especificado já existe.");
        }

        IdentityResult registrationResult =
            await _userRepository.RegisterUserAsync(userToCreate, createCreateUserRequest.Password);

        IdentityResult lockoutResult = await _userRepository.SetLockoutEnabledAsync(userToCreate, false);
        if (!registrationResult.Succeeded || !lockoutResult.Succeeded)
        {
            throw new InternalServerErrorException();
        }

        TokensResponse tokens = _tokenGenerator.GenerateTokens(userToCreate.Id, userToCreate.FullName);

        return userToCreate.ToUserResponse(tokens);
    }

    public async Task<UserDataResponse> EditAsync(EditUserRequest editUserRequest, Guid userId, Guid routeId)
    {
        if (routeId != userId)
        {
            throw new ForbiddenException("Você não possui permissão para editar este usuário.");
        }

        User? user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("Usuário com o id especificado não existe.");
        }

        user.FullName = editUserRequest.FullName;

        await _userRepository.CommitAsync();

        return user.ToUserDataResponse();
    }

    public async Task<UserResponse> LoginAsync(LoginUserRequest loginUserRequest)
    {
        User? userToLogin = await _userRepository.GetUserByUsernameAsync(loginUserRequest.Username);
        if (userToLogin is null)
        {
            // The userToLogin object is assigned to avoid time based attacks where
            // it's possible to enumerate valid emails based on response times from
            // the server
            userToLogin = new User()
            {
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        SignInResult signInResult = await _userRepository.CheckCredentials(userToLogin, loginUserRequest.Password);

        if (!signInResult.Succeeded || userToLogin is null)
        {
            if (signInResult.IsLockedOut)
            {
                throw new LockedException("Essa conta está bloqueada, aguarde e tente novamente.");
            }

            throw new UnauthorizedException("Credenciais inválidas.");
        }

        TokensResponse tokens = _tokenGenerator.GenerateTokens(userToLogin.Id, userToLogin.FullName);

        return userToLogin.ToUserResponse(tokens);
    }
}