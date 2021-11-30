using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace TUKD.Web.Interfaces
{
    public interface ICommonDialogForm<T>
    {
        MudDialogInstance? MudDialogInstance { get; set; }
        T? Model { get; set; }
    }
}
