using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
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
        // BACKEND STATE DATA MATRIX BOUNDS
        // ==========================================================
        private ProjectSummary projectModel = new();
        private List<ProjectSummary>? existingProjects;
        private string statusMessage = string.Empty;
        private bool isSaving = false;
        private bool isLoadingProjects = true;

        // Modal Control State variables
        private bool isModalOpen = false;
        private int editingId;
        private ProjectSummary? editingModel;

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
                existingProjects = await Http.GetFromJsonAsync<List<ProjectSummary>>("http://apiservice/api/projects");
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
                using var request = new HttpRequestMessage(HttpMethod.Post, "http://apiservice/api/projects");
                request.Headers.Add("X-Admin-Token", AdminState.Token);
                request.Content = JsonContent.Create(projectModel);

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
        /// Commits mutated changes from inside the tracking update instance back into live application persistence.
        /// </summary>
        private async Task UpdateProject()
        {
            if (editingModel == null) return;
            isSaving = true;
            statusMessage = string.Empty;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, $"http://apiservice/api/projects/{editingId}");
                request.Headers.Add("X-Admin-Token", AdminState.Token);
                request.Content = JsonContent.Create(editingModel);

                var response = await Http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    statusMessage = $"Successfully updated project '{editingModel.Title}' details.";
                    CloseEditModal();
                    await LoadProjects(); // Re-sync view context
                }
            }
            catch (Exception)
            {
                statusMessage = "Error pushing dataset updates to backend service.";
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
                using var request = new HttpRequestMessage(HttpMethod.Delete, $"http://apiservice/api/projects/{id}");
                request.Headers.Add("X-Admin-Token", AdminState.Token);

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
        private void OpenEditModal(ProjectSummary target)
        {
            editingId = target.Id;
            // Fully map all fields to prevent structural properties from dropping on edit form init
            editingModel = new ProjectSummary
            {
                Id = target.Id,
                Title = target.Title,
                ClientName = target.ClientName,
                Description = target.Description,
                TechStack = target.TechStack,
                LiveUrl = target.LiveUrl,
                ImagePath = target.ImagePath,
                ImageAlt = target.ImageAlt,
                Industry = target.Industry,
                Package = target.Package,
                Featured = target.Featured,
                IsPublic = target.IsPublic,
                DisplayOrder = target.DisplayOrder
            };
            isModalOpen = true;
        }

        private void CloseEditModal()
        {
            isModalOpen = false;
            editingModel = null;
        }

        // ==========================================================
        // UTILITY UTTERANCE & FORMATTING HELPERS
        // ==========================================================
        private string FormatTechStack(string? rawTechStack)
        {
            if (string.IsNullOrWhiteSpace(rawTechStack)) return string.Empty;
            var delimiters = new[] { ',', ';', '•' };
            var parsedTags = rawTechStack.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(tag => tag.Trim())
                                        .Where(tag => !string.IsNullOrEmpty(tag));
            return string.Join(" • ", parsedTags);
        }
    }
}