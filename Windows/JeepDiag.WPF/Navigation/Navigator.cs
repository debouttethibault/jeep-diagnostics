using CommunityToolkit.Mvvm.ComponentModel;
using JeepDiag.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JeepDiag.WPF.Navigation
{
    public partial class Navigator : ObservableObject, INavigator
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private INavigatableViewModel? _currentViewModel;

        public Navigator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<T>() where T : INavigatableViewModel
        {
            CurrentViewModel?.OnNavigateAway();

            CurrentViewModel = _serviceProvider.GetRequiredService<T>();
            
            CurrentViewModel.OnNavigate();
        }

        public void NavigateTo<T>(Action<T> executeViewModelLogic) where T : INavigatableViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<T>();

            executeViewModelLogic(viewModel);

            CurrentViewModel?.OnNavigateAway();
            
            CurrentViewModel = viewModel;
            CurrentViewModel.OnNavigate();
        }
    }
}
