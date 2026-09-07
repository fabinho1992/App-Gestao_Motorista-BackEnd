using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ChangePassword
{
    public record ChangePasswordCommand(string Email, string Code, string Password) : IRequest<ResultViewModel<string>>;
}
