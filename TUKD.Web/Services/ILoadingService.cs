using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUKD.Web.Services
{
    public interface ILoadingService
    {
        bool IsLoading { get; set; }
        Action? OnLoading { get; set; }
        void SetLoading();
        void StopLoading();
    }
}
