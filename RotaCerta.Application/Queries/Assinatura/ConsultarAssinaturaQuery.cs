using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Queries.Assinatura
{
    public record ConsultarAssinaturaQuery : IRequest<ResultViewModel<StatusAssinaturaViewModel>>;
}
