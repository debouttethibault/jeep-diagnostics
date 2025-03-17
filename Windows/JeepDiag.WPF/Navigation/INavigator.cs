using JeepDiag.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeepDiag.WPF.Navigation;

public interface INavigator
{
    INavigatableViewModel CurrentViewModel { get; }

    void NavigateTo<T>() where T : INavigatableViewModel;
    void NavigateTo<T>(Action<T> executeViewModelLogic) where T : INavigatableViewModel;
}
