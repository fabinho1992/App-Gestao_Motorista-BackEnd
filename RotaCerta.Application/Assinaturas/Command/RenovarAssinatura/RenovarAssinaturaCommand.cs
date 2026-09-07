using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Assinaturas.Command.RenovarAssinatura
{
    public record RenovarAssinaturaCommand : IRequest<ResultViewModel<CheckoutAssinaturaViewModel>>;
}
