using MediatR;
using Microsoft.AspNetCore.Mvc;
using RotaCerta.Application.Dtos;
using RotaCerta.Application.MotoristaHandler.Commands.CriarMotorista;
using RotaCerta.Domain.ModelsAuthentication;
using RotaCerta.Domain.Services.IAuthService;

namespace RotaCerta.API.Controllers;

/// <summary>
/// Gerencia autenticação e registro de motoristas.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoginUser _loginUser;

    public AuthController(IMediator mediator, ILoginUser loginUser)
    {
        _mediator = mediator;
        _loginUser = loginUser;
    }

    /// <summary>Registra um novo motorista no sistema.</summary>
    /// <response code="201">Motorista criado com sucesso</response>
    /// <response code="400">Erro de validação</response>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultViewModel<Guid>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] CriarMotoristaCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Created(string.Empty, result);
    }

    /// <summary>Autentica o motorista e retorna o token JWT.</summary>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Email ou senha inválidos</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ResultViewModel<ResponseLogin>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultViewModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var response = await _loginUser.LoginAsync(login);

        if (response.Status != "Sucess 200")
            return BadRequest(ResultViewModel.Error(response.Message!));

        return Ok(ResultViewModel<ResponseLogin>.Success(response));
    }
}
