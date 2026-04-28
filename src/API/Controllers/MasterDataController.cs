using Application.MasterData.Queries.GetGeographicData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MasterDataController : BaseApiController
{
    private readonly IMediator _mediator;

    public MasterDataController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var result = await _mediator.Send(new GetCountriesQuery());
        return OkResponse(result);
    }

    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvinces()
    {
        var result = await _mediator.Send(new GetProvincesQuery());
        return OkResponse(result);
    }

    [HttpGet("districts/{provinceCode}")]
    public async Task<IActionResult> GetDistricts(int provinceCode)
    {
        var result = await _mediator.Send(new GetDistrictsQuery(provinceCode));
        return OkResponse(result);
    }

    [HttpGet("wards/{districtCode}")]
    public async Task<IActionResult> GetWards(int districtCode)
    {
        var result = await _mediator.Send(new GetWardsQuery(districtCode));
        return OkResponse(result);
    }
}
