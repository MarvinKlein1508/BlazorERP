using BlazorERP.Core.Utilities;

namespace BlazorERP.Components.Pages.Admin
{
    public partial class GesperrteSeiten
    {
        protected override Task OnInitializedAsync()
        {
            for (int i = ActivePages.Pages.Count - 1; i >= 0; i--)
            {
                ActivePage activePage = ActivePages.Pages[i];

                if (activePage.Timestamp.AddMinutes(10) < DateTime.Now)
                {
                    ActivePages.RemoveActivePage(activePage);
                }
            }

            return Task.CompletedTask;
        }
        private async Task RemovePageAsync(ActivePage page)
        {
            ActivePages.RemoveActivePage(page);
            //await jsRuntime.ShowToastAsync(ToastType.success, $"Seite wurde erfolgreich freigegeben.");
        }
    }
}