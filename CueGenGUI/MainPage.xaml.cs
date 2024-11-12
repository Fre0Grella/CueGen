using CueGen;

namespace CueGenGUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = MainPageViewModel = new MainPageViewModel();
        }

    }

}
