using LonelyInterview.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonelyInterview.Application.Requests.Hr
{
    public record CreateVacancyRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; init; } = string.Empty;

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Description { get; init; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string RequiredSkills { get; init; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Location { get; init; } = string.Empty;

        [StringLength(200)]
        public string? ShortDescription { get; init; }

        [StringLength(500)]
        public string? NiceToHaveSkills { get; init; }

        [Range(0, 50)]
        public int? MinYearsOfExperience { get; init; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryFrom { get; init; }

        [Range(0, double.MaxValue)]
        public decimal? SalaryTo { get; init; }

        [StringLength(3)]
        public string? Currency { get; init; } = "RUB";

        public EmploymentType EmploymentType { get; init; } = EmploymentType.FullTime;

        public WorkFormat WorkFormat { get; init; } = WorkFormat.Office;
    }
}
