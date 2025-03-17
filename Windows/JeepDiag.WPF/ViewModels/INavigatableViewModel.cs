namespace JeepDiag.WPF.ViewModels;

public interface INavigatableViewModel : IViewModel
{
    public void OnNavigate() {}
    public void OnNavigateAway() {}
}
