using Himapp.PM.Application.Features.EquipmentSubParts.Models;
using MediatR;

namespace Himapp.PM.Application.Features.EquipmentSubParts.Commands;

public sealed record CreateEquipmentSubPartCommand(string EquipmentSubPartName, bool IsActive, int CreateBy, int LastModifiedBy) : IRequest<EquipmentSubPartDto>;
