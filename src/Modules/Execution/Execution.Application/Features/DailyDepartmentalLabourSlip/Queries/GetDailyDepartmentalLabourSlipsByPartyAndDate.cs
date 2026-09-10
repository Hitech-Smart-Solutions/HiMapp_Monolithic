using Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Models;
using Himapp.Execution.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Himapp.Execution.Application.Features.DailyDepartmentalLabourSlip.Queries;

public sealed class GetDailyDepartmentalLabourSlipsByPartyAndDate : IRequest<bool>
{
    public CreateDailyDepartmentalLabourSlipRequest Request { get; }
    public GetDailyDepartmentalLabourSlipsByPartyAndDate(CreateDailyDepartmentalLabourSlipRequest request) => Request = request;
}