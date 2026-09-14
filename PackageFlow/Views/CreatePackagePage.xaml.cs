using PackageFlow.ViewModels;
using System.Windows.Controls;

namespace PackageFlow.Views
{
    /// <summary>
    /// Interaction logic for CreatePackagePage.xaml
    /// </summary>
    public partial class CreatePackagePage : Page
    {
        public CreatePackagePage(CreatePackageViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
