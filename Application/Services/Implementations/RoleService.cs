using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Extensions;
using Data.Repositories.Interfaces;
using Data.UnitOfWork.Implementations;
using Data.UnitOfWorks.Interfaces;
using Domain.Entities;
using Domain.Models.Creates;
using Domain.Models.Filters;
using Domain.Models.Pagination;
using Domain.Models.Updates;
using Domain.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitofWork , IMapper mapper)
    {
        _unitOfWork = unitofWork;
        _roleRepository = unitofWork.Role;
        _mapper = mapper;
    }
    public IActionResult GetRoles(RoleFilterModel filters, PaginationRequestModel pagination)
    {
        var query = _roleRepository.GetAll();
        if (filters.Name != null)
        {
            query = query.Where(x => x.Name.Contains(filters.Name));
        }
        var totalRows = query.Count();
        var roles = query.OrderByDescending(x => x.CreateAt).Paginate(pagination)
            //map thu cong
            // Select(x => new RoleViewModel()
            // {
            //     Id = x.Id,
            //     Name = x.Name,
            //     CreateAt = x.CreateAt,
            // })
            //auto mapper
            .ProjectTo<RoleViewModel>(_mapper.ConfigurationProvider)
            .ToList();
        return new OkObjectResult(roles.ToPaged(pagination, totalRows));
    }

    public IActionResult GetRoleById(Guid id)
    {
        var role = _roleRepository.Where(x => x.Id.Equals(id))
            .ProjectTo<RoleViewModel>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
        if (role == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(role);
    }

    public async Task<IActionResult> CreateRole(RoleCreateModel model)
    {
        // var role = new Role()
        // {
        //     Id = Guid.NewGuid(),
        //     Name = model.Name,
        //     CreateAt = DateTime.UtcNow,
        // };
        var role = _mapper.Map<Role>(model);
            _roleRepository.Add(role);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return GetRoleById(role.Id);
            }

            return new BadRequestResult();
    }

    public async Task<IActionResult> UpdateRole(Guid id, RoleUpdateModel model)
    {
        var role = await _roleRepository.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        if (role == null)
        {
            return new NotFoundResult();
        }
        var update = _mapper.Map(model, role);
        _roleRepository.Update(update);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return GetRoleById(role.Id);
        }

        return new BadRequestResult();
    }

    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _roleRepository.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        if (role == null)
        {
            return new NotFoundResult();
        }
        _roleRepository.Delete(role);
        await _unitOfWork.SaveChangesAsync();
        return new OkResult();
    }
}