using System.Windows.Controls;


namespace ironfrost
{
    /// <summary>
    /// Interaction logic for NullControl.xaml
    /// </summary>
    public partial class NullControl : UserControl
    {
        /// <summary>
        ///   The Role connected to this <c>NullControl</c>.
        /// </summary>
        public IClientRole Role { get; }

        public NullControl(IClientRole role)
        {
            Role = role;
            DataContext = role;

            InitializeComponent();
        }
    }
}
