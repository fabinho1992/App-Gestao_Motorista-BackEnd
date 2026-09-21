using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ConfirmarEmail
{
    public record ConfirmarEmailCommand(string Code) : IRequest<ResultViewModel<string>>;
}
