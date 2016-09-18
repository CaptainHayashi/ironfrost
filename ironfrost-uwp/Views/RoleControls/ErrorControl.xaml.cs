using Windows.UI.Xaml.Controls;


namespace Ironfrost.Views
{
    /// <summary>
    /// Interaction logic for ErrorControl.xaml
    /// </summary>
    public partial class ErrorControl : UserControl
    {
        /// <summary>
        ///   The Role connected to this <c>ErrorControl</c>.
        /// </summary>
        public IClientRole Role { get; }

        public ErrorControl(IClientRole role)
        {
            Role = role;
            DataContext = role;

            InitializeComponent();
        }
    }
}
