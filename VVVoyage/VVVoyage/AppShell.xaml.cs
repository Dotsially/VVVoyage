using VVVoyage.Views;

namespace VVVoyage
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("InstructionsPage", typeof(InstructionsPage));
        }
    }
}
