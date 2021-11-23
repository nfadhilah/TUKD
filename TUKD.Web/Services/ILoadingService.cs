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

    public class LoadingService : ILoadingService
    {
        public Action? OnLoading { get; set; }
        public bool IsLoading { get; set; }

        public void SetLoading()
        {
            IsLoading = true;
            OnLoading?.Invoke();
        }

        public void StopLoading()
        {
            IsLoading = false;
            OnLoading?.Invoke();
        }
    }
}
