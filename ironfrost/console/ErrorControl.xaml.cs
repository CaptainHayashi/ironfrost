using System.Windows.Controls;


namespace Ironfrost
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
