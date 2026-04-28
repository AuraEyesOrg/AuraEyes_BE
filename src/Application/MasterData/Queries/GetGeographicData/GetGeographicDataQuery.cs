using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.MasterData;
using Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MasterData.Queries.GetGeographicData;

public record GetCountriesQuery : IRequest<List<CountryDto>>;
public record GetProvincesQuery : IRequest<List<ProvinceDto>>;
public record GetDistrictsQuery(int ProvinceCode) : IRequest<List<DistrictDto>>;
public record GetWardsQuery(int DistrictCode) : IRequest<List<WardDto>>;

public class CountryDto { public string Name { get; set; } = default!; public string IsoCode { get; set; } = default!; }
public class ProvinceDto { public string Name { get; set; } = default!; public int Code { get; set; } }
public class DistrictDto { public string Name { get; set; } = default!; public int Code { get; set; } }
public class WardDto { public string Name { get; set; } = default!; public int Code { get; set; } }

public class GetGeographicDataQueryHandler : 
    IRequestHandler<GetCountriesQuery, List<CountryDto>>,
    IRequestHandler<GetProvincesQuery, List<ProvinceDto>>,
    IRequestHandler<GetDistrictsQuery, List<DistrictDto>>,
    IRequestHandler<GetWardsQuery, List<WardDto>>
{
    private readonly IRepository<Country> _countryRepo;
    private readonly IRepository<Province> _provinceRepo;
    private readonly IRepository<District> _districtRepo;
    private readonly IRepository<Ward> _wardRepo;

    public GetGeographicDataQueryHandler(
        IRepository<Country> countryRepo,
        IRepository<Province> provinceRepo,
        IRepository<District> districtRepo,
        IRepository<Ward> wardRepo)
    {
        _countryRepo = countryRepo;
        _provinceRepo = provinceRepo;
        _districtRepo = districtRepo;
        _wardRepo = wardRepo;
    }

    public async Task<List<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        return await _countryRepo.Query()
            .Select(c => new CountryDto { Name = c.Name, IsoCode = c.IsoCode })
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProvinceDto>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
    {
        return await _provinceRepo.Query()
            .Select(p => new ProvinceDto { Name = p.Name, Code = p.Code })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DistrictDto>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        // Find the province first to get its ID, or join
        var province = await _provinceRepo.Query().FirstOrDefaultAsync(p => p.Code == request.ProvinceCode, cancellationToken);
        if (province == null) return new List<DistrictDto>();

        return await _districtRepo.Query()
            .Where(d => d.ProvinceId == province.Id)
            .Select(d => new DistrictDto { Name = d.Name, Code = d.Code })
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WardDto>> Handle(GetWardsQuery request, CancellationToken cancellationToken)
    {
        var district = await _districtRepo.Query().FirstOrDefaultAsync(d => d.Code == request.DistrictCode, cancellationToken);
        if (district == null) return new List<WardDto>();

        return await _wardRepo.Query()
            .Where(w => w.DistrictId == district.Id)
            .Select(w => new WardDto { Name = w.Name, Code = w.Code })
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }
}
