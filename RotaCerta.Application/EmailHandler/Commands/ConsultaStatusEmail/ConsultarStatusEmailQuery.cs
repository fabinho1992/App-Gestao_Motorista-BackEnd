using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.EmailHandler.Commands.ConsultaStatusEmail
{
    public record ConsultarStatusEmailQuery : IRequest<ResultViewModel<StatusEmailViewModel>>;
}
