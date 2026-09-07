using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ResetPassword
{
    public record ResetCommand(string Email) : IRequest<ResultViewModel<string>>;
}
