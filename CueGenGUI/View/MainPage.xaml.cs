namespace CueGenGUI.View;

using CueGenGUI.Model;
using CueGenGUI.ViewModel;
using CueGen.Core;

public partial class MainPage : ContentPage
{
    

    public MainPage()
    {
        InitializeComponent();

        
        BindingContext = new GenOptionSetViewModel();

    }

    


}
