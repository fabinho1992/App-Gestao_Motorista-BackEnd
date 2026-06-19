using MediatR;
using RotaCerta.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RotaCerta.Application.VeiculoHandler.Commands.ExcluirVeiculo
{
    public record ExcluirVeiculoCommand(Guid VeiculoId)
    : IRequest<ResultViewModel>;
}
