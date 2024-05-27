using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Validations;
using Application.Common.Validations.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public UserController(IUserService userService, IUserAuthorizationService userAuthorizationService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userAuthorizationService = userAuthorizationService ??
                                    throw new ArgumentNullException(nameof(userAuthorizationService));
    }

    [HttpGet("{userId:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDataResponse>> GetUserById(Guid userId)
    {
        return await _userService.GetUserByIdAsync(userId);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(CreateUserRequest createUserRequest)
    {
        CreateUserRequestValidator requestRequestValidator = new();
        ValidationResult validationResult = await requestRequestValidator.ValidateAsync(createUserRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        UserResponse createdUser = await _userService.RegisterAsync(createUserRequest);

        return new CreatedAtRouteResult(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginUserRequest loginUserRequest)
    {
        LoginUserValidator requestValidator = new();
        ValidationResult validationResult = await requestValidator.ValidateAsync(loginUserRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        UserResponse loggedInUser = await _userService.LoginAsync(loginUserRequest);

        return Ok(loggedInUser);
    }

    [Authorize]
    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<UserDataResponse>> Update(EditUserRequest editUserRequest, Guid userId)
    {
        EditUserRequestValidator requestValidator = new();
        ValidationResult validationResult = await requestValidator.ValidateAsync(editUserRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }

        Guid loggedInUserId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _userService.EditAsync(editUserRequest, loggedInUserId, userId);
    }
}