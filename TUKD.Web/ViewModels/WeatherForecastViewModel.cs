using System.Collections.ObjectModel;
using System.Net.Http.Json;
using MudBlazor;
using TUKD.Domain;
using TUKD.Web.Pages.Weather;

namespace TUKD.Web.ViewModels
{
    public class WeatherForecastViewModel : BaseSimpleCrudLayoutViewModel<WeatherForecast, FetchDataForm>
    {
        private readonly HttpClient _http;
        private readonly LoadingOverlayViewModel _loadingVm;
        private readonly IDialogService _dialogService;
        private readonly ISnackbar _snackbar;


        public WeatherForecastViewModel(HttpClient http, LoadingOverlayViewModel loadingVm, IDialogService dialogService,
            ISnackbar snackbar) : base(http, dialogService, loadingVm, snackbar)
        {
            _http = http;
            _loadingVm = loadingVm;
            _dialogService = dialogService;
            _snackbar = snackbar;
        }

        public override async Task GetAllWithFilter(WeatherForecast? filter = null)
        {
            _loadingVm.IsBusy = true;
            await Task.Delay(2000);
            var response = await _http.GetFromJsonAsync<List<WeatherForecast>>("sample-data/weather.json");
            if (filter is not null && response is not null)
            {
                response = response.Where(x => x.Summary == filter.Summary).ToList();
            }

            MainList = new ObservableCollection<WeatherForecast>(response ?? new List<WeatherForecast>());
            _loadingVm.IsBusy = false;
        }

        public override async Task ShowFilterForm()
        {
            var dialogOptions = new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Small,
                DisableBackdropClick = true,
                CloseButton = true
            };
            var dialog = _dialogService.Show<FetchDataFilterForm>("Filter", dialogOptions);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                FilterChips.Clear();
                var data = result.Data as WeatherForecast;
                FilterChips.Add(data?.Summary ?? "");
                await GetAllWithFilter(data);
            }
        }
    }
}