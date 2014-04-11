using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        /* Constructors */
        public Settings()
        {
            InitializeComponent();
        }

        /* UI Element Event Handlers */
        private void eArmA2OAPathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eArmA2OAPath.Text = dialog.FileName;
            }
        }

        private void eArmA3PathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eArmA3Path.Text = dialog.FileName;
            }
        }

        private void eArmA2PathBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Executable|*.exe|All files|*.*", Multiselect = false, CheckPathExists = true, CheckFileExists = true };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eArmA2Path.Text = dialog.FileName;
            }
        }

        private void eOKButton_Click(object sender, RoutedEventArgs e)
        {
            if (eArmA2Path.Text != String.Empty)
                Properties.Settings.Default.A2_Path = eArmA2Path.Text;

            if (eArmA2OAPath.Text != String.Empty)
                Properties.Settings.Default.A2OA_Path = eArmA2OAPath.Text;

            if (eArmA3Path.Text != String.Empty)
                Properties.Settings.Default.A3_Path = eArmA3Path.Text;

            Properties.Settings.Default.Save();
            this.Close();
        }

        private void eCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            eArmA2Path.Text = Properties.Settings.Default.A2_Path;
            eArmA2OAPath.Text = Properties.Settings.Default.A2OA_Path;
            eArmA3Path.Text = Properties.Settings.Default.A3_Path;
        }
    }
}
