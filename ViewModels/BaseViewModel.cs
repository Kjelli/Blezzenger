using System;

namespace Blezzenger.ViewModels
{
    public abstract class BaseViewModel
    {
        public Action OnChange { get; set; }

        public abstract void OnInitialized();
        public abstract void OnAfterRender(bool firstRender);

        protected void NotifyStateChange() => OnChange?.Invoke();
    }
}
