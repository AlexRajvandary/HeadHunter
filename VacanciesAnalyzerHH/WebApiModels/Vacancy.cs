using System;
using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Models
{
    public class Vacancy
    {
        public string? Id { get; set; }
        public bool? Premium { get; set; }
        public string? Name { get; set; }
        public object? Department { get; set; }
        public bool? HasTest { get; set; }
        public bool? ResponseLetterRequired { get; set; }
        public Area? Area { get; set; }
        public Salary? Salary { get; set; }
        public Type? Type { get; set; }
        public Address? Address { get; set; }
        public object? ResponseUrl { get; set; }
        public object? SortPointDistance { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Archived { get; set; }
        public string? ApplyAlternateUrl { get; set; }
        public object? InsiderInterview { get; set; }
        public string? Url { get; set; }
        public object? AdvResponseUrl { get; set; }
        public string? AlternateUrl { get; set; }
        public List<object>? Relations { get; set; }
        public Employer? Employer { get; set; }
        public Snippet? Snippet { get; set; }
        public object? Contacts { get; set; }
        public Schedule? Schedule { get; set; }
        public List<object>? WorkingDays { get; set; }
        public List<object>? WorkingTimeIntervals { get; set; }
        public List<object>? WorkingTimeModes { get; set; }
        public bool? AcceptTemporary { get; set; }
        public List<ProfessionalRole>? ProfessionalRoles { get; set; }
        public bool? AcceptIncompleteResumes { get; set; }
    }
}
