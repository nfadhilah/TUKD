using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace TUKD.Web.Interfaces
{
    public interface IBaseCrudLayout<T>
    {
        ObservableCollection<T> MainList { get; set; }
        HashSet<T> SelectedItems { get; set; }
        string SearchString { get; set; }
        ObservableCollection<string> FilterChips { get; set; }
        Task GetAll();
        Task Add();
        Task Update(T context);
        Task Delete(T weatherForecast);
        Task DeleteAll();
        bool OnSearch(T arg);
        Task OnCloseChips(MudChip chip);
        Task OnClearChips();
        Task GetAllWithFilter(T? filter);
        Task ShowFilterForm();
    }
}
