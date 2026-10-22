using Himapp.PM.Application.Features.EquipmentSubParts.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentSubParts.Commands;

public sealed record UpdateEquipmentSubPartCommand(int Id, string EquipmentSubPartName, bool IsActive, int LastModifiedBy) : IRequest<EquipmentSubPartDto?>;
