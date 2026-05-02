using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MasterData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MasterData.Queries.GetGeographicData;

public record GetCountriesQuery() : IQuery<List<CountryDto>>;
public record GetProvincesQuery() : IQuery<List<ProvinceDto>>;
public record GetDistrictsQuery(int ProvinceCode) : IQuery<List<DistrictDto>>;
public record GetWardsQuery(int DistrictCode) : IQuery<List<WardDto>>;

public record CountryDto(string Name, string IsoCode);
public record ProvinceDto(string Name, int Code);
public record DistrictDto(string Name, int Code);
public record WardDto(string Name, int Code);

public class GetGeographicDataQueryHandler : 
    IRequestHandler<GetCountriesQuery, Result<List<CountryDto>>>,
    IRequestHandler<GetProvincesQuery, Result<List<ProvinceDto>>>,
    IRequestHandler<GetDistrictsQuery, Result<List<DistrictDto>>>,
    IRequestHandler<GetWardsQuery, Result<List<WardDto>>>
{
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<Province> _provinceRepository;
    private readonly IRepository<District> _districtRepository;
    private readonly IRepository<Ward> _wardRepository;

    public GetGeographicDataQueryHandler(
        IRepository<Country> countryRepository,
        IRepository<Province> provinceRepository,
        IRepository<District> districtRepository,
        IRepository<Ward> wardRepository)
    {
        _countryRepository = countryRepository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _wardRepository = wardRepository;
    }

    public async Task<Result<List<CountryDto>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var data = await _countryRepository.Query()
            .OrderBy(x => x.Name)
            .Select(x => new CountryDto(x.Name, x.IsoCode))
            .ToListAsync(cancellationToken);
        return Result<List<CountryDto>>.Success(data);
    }

    public async Task<Result<List<ProvinceDto>>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
    {
        var data = await _provinceRepository.Query()
            .OrderBy(x => x.Name)
            .Select(x => new ProvinceDto(x.Name, x.Code))
            .ToListAsync(cancellationToken);
        return Result<List<ProvinceDto>>.Success(data);
    }

    public async Task<Result<List<DistrictDto>>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
    {
        var provinceId = await _provinceRepository.Query()
            .Where(p => p.Code == request.ProvinceCode)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (provinceId == Guid.Empty) return Result<List<DistrictDto>>.Success(new List<DistrictDto>());

        var data = await _districtRepository.Query()
            .Where(x => x.ProvinceId == provinceId)
            .OrderBy(x => x.Name)
            .Select(x => new DistrictDto(x.Name, x.Code))
            .ToListAsync(cancellationToken);
        return Result<List<DistrictDto>>.Success(data);
    }

    public async Task<Result<List<WardDto>>> Handle(GetWardsQuery request, CancellationToken cancellationToken)
    {
        var districtId = await _districtRepository.Query()
            .Where(d => d.Code == request.DistrictCode)
            .Select(d => d.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (districtId == Guid.Empty) return Result<List<WardDto>>.Success(new List<WardDto>());

        var data = await _wardRepository.Query()
            .Where(x => x.DistrictId == districtId)
            .OrderBy(x => x.Name)
            .Select(x => new WardDto(x.Name, x.Code))
            .ToListAsync(cancellationToken);
        return Result<List<WardDto>>.Success(data);
    }
}
