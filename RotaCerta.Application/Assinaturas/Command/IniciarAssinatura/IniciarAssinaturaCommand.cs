using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.Assinaturas.Command.IniciarAssinatura
{
    public record IniciarAssinaturaCommand : IRequest<ResultViewModel<CheckoutAssinaturaViewModel>>;
}
