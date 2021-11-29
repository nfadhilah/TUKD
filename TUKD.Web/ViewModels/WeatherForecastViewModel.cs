using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using MudBlazor;
using TUKD.Domain;
using TUKD.Web.Services;
using TUKD.Web.Shared;

namespace TUKD.Web.ViewModels
{
    // public interface IWeatherForecastViewModel
    // {
    //     bool IsBusy { get; set; }
    //     ObservableCollection<WeatherForecast>? WeatherForecastList { get; set; }
    //     Task GetWeatherForecasts(WeatherForecastFilter? filter);
    //     Task AddWeatherForecast(WeatherForecast weatherForecast);
    //     Task UpdateWeatherForecast(WeatherForecast target, WeatherForecast updated);
    //     Task DeleteWeatherForecast(WeatherForecast weatherForecast);
    //     Task DeleteWeatherForecast(List<WeatherForecast> weatherForecast);
    //     event PropertyChangedEventHandler? PropertyChanged;
    //     bool OnSearch(WeatherForecast arg, string searchTerm);
    // }

    public class WeatherForecastViewModel : BaseViewModel
    {
        private readonly HttpClient _client;
        private readonly ILoadingService _loadingService;
        private readonly IDialogService _dialogService;
        private readonly ISnackbar _snackbar;

        private ObservableCollection<WeatherForecast>? _weatherForecastList;

        public ObservableCollection<WeatherForecast>? WeatherForecastList
        {
            get => _weatherForecastList;
            set => SetValue(ref _weatherForecastList, value);
        }

        private string _searchString = string.Empty;

        public string SearchString
        {
            get => _searchString;
            set => SetValue(ref _searchString, value);
        }

        private HashSet<WeatherForecast> _selectedItems = new();
        public HashSet<WeatherForecast> SelectedItems
        {
            get => _selectedItems;
            set => SetValue(ref _selectedItems, value);
        }

        private ObservableCollection<string> _filterChips = new();
        public ObservableCollection<string> FilterChips
        {
            get => _filterChips;
            set => SetValue(ref _filterChips, value);
        }

        public WeatherForecastViewModel(HttpClient client, ILoadingService loadingService, IDialogService dialogService,
            ISnackbar snackbar)
        {
            _client = client;
            _loadingService = loadingService;
            _dialogService = dialogService;
            _snackbar = snackbar;
        }

        public async Task GetWeatherForecasts(WeatherForecastFilter? filter = null)
        {
            _loadingService.SetLoading();
            await Task.Delay(2000);
            var response = await _client.GetFromJsonAsync<List<WeatherForecast>>("sample-data/weather.json");
            if (filter is not null && response is not null)
            {
                response = response.Where(x => x.Summary == filter.Summary).ToList();
            }

            _weatherForecastList = new ObservableCollection<WeatherForecast>(response ?? new List<WeatherForecast>());
            _loadingService.StopLoading();
        }

        public async Task ShowFilterForm()
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
                var data = result.Data as WeatherForecastFilter;
                FilterChips.Add(data!.Summary!);
                await GetWeatherForecasts((WeatherForecastFilter)result.Data);
            }
        }

        public void OnCloseChips(MudChip chip)
        {
            FilterChips.Remove(chip.Text);
        }

        public async Task OnClearChips()
        {
            FilterChips.Clear();
            await GetWeatherForecasts();
        }

        public bool OnSearch(WeatherForecast arg)
        {
            var props = arg.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var prop in props)
            {
                sb.Append(prop.GetValue(arg, null));
            }

            return sb.ToString().ToLower().Contains(SearchString.ToLower());
        }

        public async Task AddWeatherForecast()
        {
            var result = await ShowFormDialog("Add Data");
            if (!result.Cancelled)
            {
                _loadingService.SetLoading();
                await Task.Delay(2000);
                _weatherForecastList?.Add((WeatherForecast)result.Data);
                _snackbar.Add("Data has been added successfully", Severity.Success);
                _loadingService.StopLoading();
            }
        }

        public async Task UpdateWeatherForecast(WeatherForecast context)
        {
            var result = await ShowFormDialog("Edit Data", context);
            if (!result.Cancelled)
            {
                _loadingService.SetLoading();
                await Task.Delay(2000);
                var index = _weatherForecastList?.IndexOf(context);
                if (index.HasValue) _weatherForecastList![index.Value] = (WeatherForecast)result.Data;
                _loadingService.StopLoading();
                _snackbar.Add("Data has been updated successfully", Severity.Success);
            }
        }

        public async Task DeleteWeatherForecast(WeatherForecast weatherForecast)
        {
            var result = await ShowDeleteConfirmation();
            if (!result.Cancelled)
            {
                _loadingService.SetLoading();
                await Task.Delay(2000);
                _weatherForecastList?.Remove(weatherForecast);
                _snackbar.Add("Data has been deleted successfully", Severity.Success);
                _loadingService.StopLoading();
            }
        }

        public async Task DeleteWeatherForecast()
        {
            var result = await ShowDeleteConfirmation();
            if (!result.Cancelled)
            {
                _loadingService.SetLoading();
                await Task.Delay(2000);
                foreach (var weatherForecast in SelectedItems)
                {
                    _weatherForecastList?.Remove(weatherForecast);
                }

                _snackbar.Add("Data has been deleted successfully", Severity.Success);
                _loadingService.StopLoading();
            }
        }

        private async Task<DialogResult> ShowFormDialog(string title, WeatherForecast? context = null)
        {
            var dialogParameters = new DialogParameters { ["Model"] = context };
            var dialogOptions = new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Small,
                DisableBackdropClick = true,
                CloseButton = true
            };
            var dialog = _dialogService.Show<FetchDataForm>(title, dialogParameters, dialogOptions);
            return await dialog.Result;
        }

        private async Task<DialogResult> ShowDeleteConfirmation()
        {
            var dialogParameters = new DialogParameters { ["ContentText"] = "Are you sure want to delete?" };
            var dialog = _dialogService.Show<ConfirmationDialog>("Delete Confirmation", dialogParameters,
                new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall });
            return await dialog.Result;
        }
    }
}