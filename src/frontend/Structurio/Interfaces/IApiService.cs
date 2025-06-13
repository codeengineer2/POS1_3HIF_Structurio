using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurio.Classes;

namespace Structurio.Interfaces
{
    public interface IApiService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<Project?> CreateProjectAsync(ProjectRequest request);
        Task<Issue?> AddIssueAsync(AddIssueRequest request);
        Task<bool> UpdateIssueAsync(UpdateIssueRequest request);
    }
}