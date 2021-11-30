using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TUKD.Web.Interfaces;
using TUKD.Web.Shared;

namespace TUKD.Web.ViewModels;

public abstract class BaseSimpleCrudLayoutViewModel<T, TF> : BaseViewModel, IBaseCrudLayout<T>
    where T : class where TF : ComponentBase, ICommonDialogForm<T>
{
    private readonly HttpClient _http;
    private readonly IDialogService _dialogService;
    private readonly LoadingOverlayViewModel _loadingVm;
    private readonly ISnackbar _snackbar;

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
        _http = http;
        _dialogService = dialogService;
        _loadingVm = loadingVm;
        _snackbar = snackbar;
    }

    public abstract Task GetAllWithFilter(T? filter = null);
    public abstract Task ShowFilterForm();

    public virtual async Task GetAll()
    {
        _loadingVm.IsBusy = true;
        await Task.Delay(2000);
        var response = await _http.GetFromJsonAsync<List<T>>("sample-data/weather.json");
        MainList = new ObservableCollection<T>(response ?? new List<T>());
        _loadingVm.IsBusy = false;
    }

    public virtual async Task Add()
    {
        var result = await ShowFormDialog("Add Data");
        if (!result.Cancelled)
        {
            _loadingVm.IsBusy = true;
            await Task.Delay(2000);
            MainList?.Add((T)result.Data);
            _snackbar.Add("Data has been added successfully", Severity.Success);
            _loadingVm.IsBusy = false;
        }
    }

    public virtual async Task Update(T context)
    {
        var result = await ShowFormDialog("Edit Data", context);
        if (!result.Cancelled)
        {
            _loadingVm.IsBusy = true;
            await Task.Delay(2000);
            var index = MainList?.IndexOf(context);
            if (index.HasValue) MainList![index.Value] = (T)result.Data;
            _loadingVm.IsBusy = false;
            _snackbar.Add("Data has been updated successfully", Severity.Success);
        }
    }

    public async Task Delete(T weatherForecast)
    {
        var result = await ShowDeleteConfirmation();
        if (!result.Cancelled)
        {
            _loadingVm.IsBusy = true;
            await Task.Delay(2000);
            MainList?.Remove(weatherForecast);
            _snackbar.Add("Data has been deleted successfully", Severity.Success);
            _loadingVm.IsBusy = false;
        }
    }


    public async Task DeleteAll()
    {
        var result = await ShowDeleteConfirmation();
        if (!result.Cancelled)
        {
            _loadingVm.IsBusy = true;
            await Task.Delay(2000);
            foreach (var item in SelectedItems)
            {
                MainList?.Remove(item);
            }

            _snackbar.Add("Data has been deleted successfully", Severity.Success);
            _loadingVm.IsBusy = false;
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

    protected virtual async Task<DialogResult> ShowFormDialog(string title, T? context = null)
    {
        var dialogParameters = new DialogParameters { ["Model"] = context };
        var dialogOptions = new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
            DisableBackdropClick = true,
            CloseButton = true
        };
        var dialog = _dialogService.Show<TF>(title, dialogParameters, dialogOptions);
        return await dialog.Result;
    }

    protected virtual async Task<DialogResult> ShowDeleteConfirmation()
    {
        var dialogParameters = new DialogParameters { ["ContentText"] = "Are you sure want to delete?" };
        var dialog = _dialogService.Show<ConfirmationDialog>("Delete Confirmation", dialogParameters,
            new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.ExtraSmall });
        return await dialog.Result;
    }
}