using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurio.Classes;

namespace Structurio.Interfaces
{
    /// <summary>
    /// API Service für Login, Projekte, Issues, Spalten
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Login mit EMail und Passwort
        /// </summary>
        Task<LoginResponse?> LoginAsync(string email, string password);

        /// <summary>
        /// Registrierung von neuem User
        /// </summary>
        Task<bool> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Erstellt neues Projekt
        /// </summary>
        Task<Project?> CreateProjectAsync(ProjectRequest request);

        /// <summary>
        /// Fügt neues Issue hinzu
        /// </summary>
        Task<Issue?> AddIssueAsync(AddIssueRequest request);

        /// <summary>
        /// Ändert ein Issue
        /// </summary>
        Task<bool> UpdateIssueAsync(UpdateIssueRequest request);

        /// <summary>
        /// Löscht ein Issue
        /// </summary>
        Task<bool> DeleteIssueAsync(int id);

        /// <summary>
        /// Fügt neue Spalte hinzu
        /// </summary>
        Task<Column?> AddColumnAsync(AddColumnRequest request);

        /// <summary>
        /// Ändert Spaltennamen
        /// </summary>
        Task<bool> UpdateColumnAsync(UpdateColumnRequest request);

        /// <summary>
        /// Löscht Projekt
        /// </summary>
        Task<bool> DeleteProjectAsync(int projectId);

        /// <summary>
        /// Ändert ein Projekt
        /// </summary>
        Task<bool> UpdateProjectAsync(Project project);
    }
}