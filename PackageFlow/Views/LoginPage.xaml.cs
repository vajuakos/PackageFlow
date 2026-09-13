using PackageFlow.ViewModels;
using System.Windows.Controls;

namespace PackageFlow.Views
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage(LoginViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
