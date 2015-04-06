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

namespace LicenseManager.UI
{
    /// <summary>
    /// Logic for AboutWindow
    /// </summary>
    public partial class AboutWindow : Window
    {
        /// <summary>
        ///  Instantiate a new AboutWindow
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
           
            try
            {
                // Escape -> Close window 
                RoutedCommand escapeHotkey = new RoutedCommand();
                escapeHotkey.InputGestures.Add(new KeyGesture(Key.Escape));
                CommandBindings.Add(new CommandBinding(escapeHotkey, Image_MouseUp));
            }
            catch (Exception)
            {
                // Do nothing on errors...
            }
        }

        /// <summary>
        ///  Handle mouseclick on big image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseUp(object sender, RoutedEventArgs e)
        {
            // Close the aboutwindow
            this.Close();
        }

    }
}
