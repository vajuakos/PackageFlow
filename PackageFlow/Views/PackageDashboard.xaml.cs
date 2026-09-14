using PackageFlow.ViewModels;
using System.Windows.Controls;

namespace PackageFlow.Views
{
    /// <summary>
    /// Interaction logic for PackageDashboard.xaml
    /// </summary>
    public partial class PackageDashboard : Page
    {
        public PackageDashboard(PackageDashboardViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
