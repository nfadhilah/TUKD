﻿using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using TUKD.Domain;
using TUKD.Web.Pages.Weather;

namespace TUKD.Web.ViewModels
{
    public class
        WeatherForecastViewModel : BaseSimpleCrudLayoutViewModel<WeatherForecast, FetchDataForm, FetchDataFilterForm>
    {
        public WeatherForecastViewModel(HttpClient http, IDialogService dialogService,
            LoadingOverlayViewModel loadingVm,
            ISnackbar snackbar) : base(http, dialogService, loadingVm, snackbar)
        {
            Title = "Weather";
            HeaderTitle = "Weather Forecast";
        }

        protected override async Task GetAllWithFilter(WeatherForecast? filter = null)
        {
            LoadingVm.IsBusy = true;
            await Task.Delay(2000);
            var response = await Http.GetFromJsonAsync<List<WeatherForecast>>("sample-data/weather.json");
            if (filter is not null && response is not null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Summary))
                {
                    response = response.Where(x => x.Summary == filter.Summary).ToList();
                }

                response = response.Where(x => x.TemperatureC == filter.TemperatureC).ToList();
            }

            MainList = new ObservableCollection<WeatherForecast>(response ?? new List<WeatherForecast>());
            LoadingVm.IsBusy = false;
        }

        protected override async Task DoShowFilterForm()
        {
            var result = await FilterDialogResult();
            if (!result.Cancelled)
            {
                FilterChips.Clear();
                var data = result.Data as WeatherForecast;
                if (data is not null && !string.IsNullOrWhiteSpace(data.Summary))
                {
                    FilterChips.Add(data.Summary);
                }

                if (data?.TemperatureC != null)
                {
                    FilterChips.Add(data?.TemperatureC.ToString() ?? "");
                }
                await GetAllWithFilter(data);
            }
        }
    }
}