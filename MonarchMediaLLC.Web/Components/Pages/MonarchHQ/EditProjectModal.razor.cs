using Microsoft.AspNetCore.Components;
using MonarchMediaLLC.Shared;

namespace MonarchMediaLLC.Web.Components.Pages.MonarchHQ
{
    public partial class EditProjectModal
    {
        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public ProjectSummary? Project { get; set; }

        [Parameter]
        public EventCallback<ProjectSummary> OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private async Task Save()
        {
            if (Project is null)
                return;

            await OnSave.InvokeAsync(Project);
        }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }
    }
}
