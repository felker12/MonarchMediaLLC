using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MonarchMediaLLC.Shared;
using MonarchMediaLLC.Web.Services;

namespace MonarchMediaLLC.Web.Components.Pages.MonarchHQ
{
    public partial class HqProjects
    {
        // ==========================================================
        // DEPENDENCY INJECTION MATRIX
        // ==========================================================
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private AdminStateProvider AdminState { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // ==========================================================
        // Other Variables
        // ==========================================================
        private bool isEditModalOpen;

        private ProjectSummary? editProject;

        // ==========================================================
        // API ENDPOINTS
        // ==========================================================
        private const string ProjectApi = "http://apiservice/api/projects";

        // ==========================================================
        // BACKEND STATE DATA MATRIX BOUNDS
        // ==========================================================
        private ProjectSummary projectModel = new();
        private List<ProjectSummary>? existingProjects;
        private string statusMessage = string.Empty;
        private bool isSaving = false;
        private bool isLoadingProjects = true;

        // ==========================================================
        // BLAZOR COMPONENT LIFECYCLE INITIALIZATION
        // ==========================================================
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!AdminState.IsAuthenticated)
                {
                    Navigation.NavigateTo("/monarch-hq");
                    return;
                }
                await LoadProjects();
                StateHasChanged();
            }
        }

        // ==========================================================
        // DATA ACCESS SERVICE SYNCHRONIZATION (HTTP REST METHODS)
        // ==========================================================

        /// <summary>
        /// Populates the frontend matrix with stored portfolio models from the backend service.
        /// </summary>
        private async Task LoadProjects()
        {
            isLoadingProjects = true;
            try
            {
                existingProjects = (await Http.GetFromJsonAsync<List<ProjectSummary>>(ProjectApi))
                    ?.OrderBy(p => p.DisplayOrder)
                    .ToList();
            }
            catch (Exception)
            {
                statusMessage = "Warning: Failed to fetch existing portfolio items from backend engine.";
            }
            finally
            {
                isLoadingProjects = false;
            }
        }

        /// <summary>
        /// Validates and transmits a newly configured project dataset block to the backend database.
        /// </summary>
        private async Task SubmitProject()
        {
            isSaving = true;
            statusMessage = string.Empty;

            try
            {
                using var request = CreateAuthorizedRequest(HttpMethod.Post, ProjectApi, projectModel);

                var response = await Http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    statusMessage = $"Successfully deployed project '{projectModel.Title}' into SQLite system storage.";
                    projectModel = new();
                    await LoadProjects();
                }
            }
            catch (Exception)
            {
                statusMessage = "Error updating backend database records.";
            }
            finally
            {
                isSaving = false;
            }
        }

        /// <summary>
        /// Executes a database target row eviction request after receiving manual verification.
        /// </summary>
        private async Task DeleteProject(int id, string title)
        {
            statusMessage = string.Empty;
            bool confirmed = await JS.InvokeAsync<bool>("confirm", $"Are you sure you want to permanently delete project '{title}'?");

            if (!confirmed) return;

            try
            {
                using var request = CreateAuthorizedRequest(
                    HttpMethod.Delete, $"{ProjectApi}/{id}");

                var response = await Http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    statusMessage = $"Successfully removed project '{title}' from live SQLite system storage.";
                    await LoadProjects();
                }
            }
            catch (Exception)
            {
                statusMessage = "Network error: Failed to drop record row.";
            }
        }

        // ==========================================================
        // INTERACTION UI OPERATIONS & WINDOW LAYER CONTROLS
        // ==========================================================
        private void OpenEditModal(ProjectSummary project)
        {
            editProject = new ProjectSummary(project);
            isEditModalOpen = true;
        }

        private void CloseEditModal()
        {
            isEditModalOpen = false;
            editProject = null;
        }

        private async Task SaveProjectChanges(ProjectSummary project)
        {
            isSaving = true;

            try
            {
                using var request = CreateAuthorizedRequest(
                    HttpMethod.Put, $"{ProjectApi}/{project.Id}", project);

                var response = await Http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    statusMessage = $"Updated '{project.Title}' successfully.";

                    CloseEditModal();

                    await LoadProjects();
                }
            }
            catch
            {
                statusMessage = "Failed to update project.";
            }
            finally
            {
                isSaving = false;
            }
        }

        // ==========================================================
        // UTILITY UTTERANCE & FORMATTING HELPERS
        // ==========================================================
        private static string FormatTechStack(string? rawTechStack)
        {
            if (string.IsNullOrWhiteSpace(rawTechStack)) return string.Empty;
            var delimiters = new[] { ',', ';', '•' };
            var parsedTags = rawTechStack.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(tag => tag.Trim())
                                        .Where(tag => !string.IsNullOrEmpty(tag));
            return string.Join(" • ", parsedTags);
        }

        private HttpRequestMessage CreateAuthorizedRequest(
            HttpMethod method,
            string url,
            object? body = null)
        {
            var request = new HttpRequestMessage(method, url);

            request.Headers.Add("X-Admin-Token", AdminState.Token);

            if (body is not null)
                request.Content = JsonContent.Create(body);

            return request;
        }
    }
}