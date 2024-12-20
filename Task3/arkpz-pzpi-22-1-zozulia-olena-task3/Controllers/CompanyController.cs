using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Helpers;
using Models.DTO;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyRepository.GetAllAsync();
            var companyDtos = companies.Select(MappingHelper.MapToDto).ToList();
            return Ok(companyDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            var companyDto = MappingHelper.MapToDto(company);
            return Ok(companyDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = MappingHelper.MapToEntity(companyDto);
            await _companyRepository.AddAsync(company);

            var createdCompanyDto = MappingHelper.MapToDto(company);
            return CreatedAtAction(nameof(GetById), new { id = createdCompanyDto.Id }, createdCompanyDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCompany = await _companyRepository.GetByIdAsync(id);
            if (existingCompany == null)
            {
                return NotFound();
            }

            existingCompany.Name = companyDto.Name;
            existingCompany.Address = companyDto.Address;
            existingCompany.ContactEmail = companyDto.ContactEmail;
            existingCompany.AdminId = companyDto.AdminId;

            await _companyRepository.UpdateAsync(existingCompany);

            var updatedCompanyDto = MappingHelper.MapToDto(existingCompany);
            return Ok(updatedCompanyDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            await _companyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
