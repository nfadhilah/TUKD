using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TUKD.Web.Interfaces;
using TUKD.Web.Shared;

namespace TUKD.Web.ViewModels;

public abstract class BaseSimpleCrudLayoutViewModel<T, TF, TFilter> : BaseViewModel
    where T : class
    where TF : ComponentBase, ICommonDialogForm<T>
    where TFilter : ComponentBase
{
    protected readonly HttpClient Http;
    protected readonly IDialogService DialogService;
    protected readonly LoadingOverlayViewModel LoadingVm;
    protected readonly ISnackbar Snackbar;

    private ObservableCollection<T> _mainList = new();
    public ObservableCollection<T> MainList
    {
        get => _mainList;
        set => SetValue(ref _mainList, value);
    }

    private string _searchString = string.Empty;
    public string SearchString
    {
        get => _searchString;
        set => SetValue(ref _searchString, value);
    }

    private HashSet<T> _selectedItems = new();
    public HashSet<T> SelectedItems
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

    protected BaseSimpleCrudLayoutViewModel(HttpClient http, IDialogService dialogService,
        LoadingOverlayViewModel loadingVm, ISnackbar snackbar)
    {
        Http = http;
        DialogService = dialogService;
        LoadingVm = loadingVm;
        Snackbar = snackbar;
    }

    protected abstract Task GetAllWithFilter(T? filter = null);
    protected abstract Task DoShowFilterForm();

    public virtual async Task GetAll(T? filter = null)
    {
        LoadingVm.IsBusy = true;
        if (filter != null)
        {
            await GetAllWithFilter(filter);
        }
        else
        {
            var response = await Http.GetFromJsonAsync<List<T>>("sample-data/weather.json");
            MainList = new ObservableCollection<T>(response ?? new List<T>());
        }

        LoadingVm.IsBusy = false;
    }

    public virtual async Task Add()
    {
        var result = await MainFormDialogResult("Add Data");
        if (!result.Cancelled)
        {
            LoadingVm.IsBusy = true;
            MainList?.Add((T)result.Data);
            Snackbar.Add("Data has been added successfully", Severity.Success);
            LoadingVm.IsBusy = false;
        }
    }

    public virtual async Task Update(T context)
    {
        var result = await MainFormDialogResult("Edit Data", context);
        if (!result.Cancelled)
        {
            LoadingVm.IsBusy = true;
            var index = MainList?.IndexOf(context);
            if (index.HasValue) MainList![index.Value] = (T)result.Data;
            LoadingVm.IsBusy = false;
            Snackbar.Add("Data has been updated successfully", Severity.Success);
        }
    }

    public async Task Delete(T weatherForecast)
    {
        var result = await DeleteConfirmDialogResult();
        if (!result.Cancelled)
        {
            LoadingVm.IsBusy = true;
            MainList?.Remove(weatherForecast);
            Snackbar.Add("Data has been deleted successfully", Severity.Success);
            LoadingVm.IsBusy = false;
        }
    }


    public async Task DeleteAll()
    {
        var result = await DeleteConfirmDialogResult();
        if (!result.Cancelled)
        {
            LoadingVm.IsBusy = true;
            foreach (var item in SelectedItems)
            {
                MainList?.Remove(item);
            }

            Snackbar.Add("Data has been deleted successfully", Severity.Success);
            LoadingVm.IsBusy = false;
        }
    }

    public bool OnSearch(T arg)
    {
        var props = arg.GetType().GetProperties();
        var sb = new StringBuilder();
        foreach (var prop in props)
        {
            sb.Append(prop.GetValue(arg, null));
        }

        return sb.ToString().ToLower().Contains(SearchString.ToLower());
    }

    public async Task OnCloseChips(MudChip chip)
    {
        FilterChips.Remove(chip.Text);
        await GetAll();
    }

    public async Task OnClearChips()
    {
        FilterChips.Clear();
        await GetAll();
    }

    public async Task ShowFilterForm()
    {
        await DoShowFilterForm();
    }

    protected async Task<DialogResult> MainFormDialogResult(string title, T? context = null)
    {
        var dialogParameters = new DialogParameters { ["Model"] = context };
        var dialogOptions = new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            DisableBackdropClick = true,
            CloseButton = true
        };
        var dialog = DialogService.Show<TF>(title, dialogParameters, dialogOptions);
        return await dialog.Result;
    }

    protected async Task<DialogResult> DeleteConfirmDialogResult()
    {
        var dialogParameters = new DialogParameters { ["ContentText"] = "Are you sure want to delete?" };
        var dialog = DialogService.Show<ConfirmationDialog>("Delete Confirmation", dialogParameters,
            new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall });
        return await dialog.Result;
    }

    protected async Task<DialogResult> FilterDialogResult()
    {
        var dialogOptions = new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            DisableBackdropClick = true,
            CloseButton = true
        };
        var dialog = DialogService.Show<TFilter>("Filter", dialogOptions);
        return await dialog.Result;
    }
}