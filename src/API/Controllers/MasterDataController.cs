using Application.MasterData.Queries.GetGeographicData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
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
        return HandleResult(result);
    }

    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvinces()
    {
        var result = await _mediator.Send(new GetProvincesQuery());
        return HandleResult(result);
    }

    [HttpGet("districts/{provinceCode}")]
    public async Task<IActionResult> GetDistricts(int provinceCode)
    {
        var result = await _mediator.Send(new GetDistrictsQuery(provinceCode));
        return HandleResult(result);
    }

    [HttpGet("wards/{districtCode}")]
    public async Task<IActionResult> GetWards(int districtCode)
    {
        var result = await _mediator.Send(new GetWardsQuery(districtCode));
        return HandleResult(result);
    }
}
