using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ConfirmacaoEmail
{
    public record SolicitarConfirmacaoEmailCommand : IRequest<ResultViewModel<string>>;
}
