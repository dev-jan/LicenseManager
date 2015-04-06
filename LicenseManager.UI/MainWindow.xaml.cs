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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Ribbon;
using LicenseManager.Logic;
using Microsoft.Win32;
using System.Diagnostics;

namespace LicenseManager.UI
{
    /// <summary>
    /// Logic behind the MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        /// <summary>
        /// Base window title (if a file is open, " - Somefile.lmd" will be added)
        /// </summary>
        private const String windowTitle = "LicenseManager";

        /// <summary>
        /// Management/Holderobject for the data
        /// </summary>
        private Manager manager;

        /// <summary>
        /// Dynamic right click context menu item for adding a new license to a group
        /// </summary>
        private MenuItem contextMenuItemAddLicense;

        /// <summary>
        /// Initialize the MainWindow
        /// </summary>
        public MainWindow()
        {
            // Load XAML Structure
            InitializeComponent();

            // Create a new Dataholder
            manager = new Manager();

            // Instantiate the right click context menu
            instantiateContextmenu();

            // Instantiate keybord shortcuts
            addHotKeys();

            // Bind Dataholder on the TreeView
            LicensesTreeView.ItemsSource = manager.Groups;

            // Set Window title
            updateAppTitle();
        }

        /// <summary>
        /// Register keyboard shortcuts
        /// </summary>
        private void addHotKeys()
        {
            try
            {
                // CTRL + S  ->  Saving 
                RoutedCommand saveHotkey = new RoutedCommand();
                saveHotkey.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
                CommandBindings.Add(new CommandBinding(saveHotkey, SaveMenuItem_Click));

                // CTRL + N  ->  New File
                RoutedCommand newHotkey = new RoutedCommand();
                newHotkey.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
                CommandBindings.Add(new CommandBinding(newHotkey, NewMenuItem_Click));

                // Delete  ->  Delete selected Group/License
                RoutedCommand deletekey = new RoutedCommand();
                deletekey.InputGestures.Add(new KeyGesture(Key.Delete, ModifierKeys.None));
                CommandBindings.Add(new CommandBinding(deletekey, RemoveMenuItem_Click));
            }
            catch (Exception)
            {
                // Do nothing on errors...
            }
        }

        /// <summary>
        /// Instantiate the dynamic context menu items
        /// </summary>
        private void instantiateContextmenu()
        {
            // Instantiate the dynamic rightclick menu item
            contextMenuItemAddLicense = new MenuItem();
            contextMenuItemAddLicense.Header = "Add License";
            contextMenuItemAddLicense.Click += NewLicenseButton_Click;
        }

        /// <summary>
        /// Open a existing file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult saveChanges = askSaveChanges();
            if (saveChanges != MessageBoxResult.Cancel)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "LicenseManager Datafile|*.lmd";
                if (openFileDialog.ShowDialog() == true)
                {
                    manager = LicenseManager.DataAccess.Fileaccess.load(openFileDialog.FileName);
                    LicensesTreeView.ItemsSource = manager.Groups;
                    manager.HasChange = false;
                    updateAppTitle();
                }
            }
        }

        /// <summary>
        /// Save into a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "LicenseManager Datafile|*.lmd";
            if (saveFileDialog.ShowDialog() == true)
            {
                manager.Path = saveFileDialog.FileName;
                LicenseManager.DataAccess.Fileaccess.save(manager, manager.Path);
                updateAppTitle();
            }
        }

        /// <summary>
        /// Save into the open file (if open file exists)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (manager.Path != null)
            {
                LicenseManager.DataAccess.Fileaccess.save(manager, manager.Path);
            }
            else
            {
                SaveAsMenuItem_Click(sender, e);
            }
        }

        /// <summary>
        /// Create a new LicenseData File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = askSaveChanges();
            if (dialogResult == MessageBoxResult.Yes || dialogResult == MessageBoxResult.No)
            {
                newFile();
            }
        }

        /// <summary>
        /// Reset the Dataobject & the UI Elements to create a new File
        /// </summary>
        private void newFile()
        {
            manager = new Manager();
            LicensesTreeView.ItemsSource = manager.Groups;
            LicensesTreeView_SelectedItemChanged(null, null);
            updateAppTitle();
        }

        /// <summary>
        /// Ask the user if he want to save the changes into a file
        /// </summary>
        /// <returns>Result of the Messagebox (YES if there is nothing to save)</returns>
        private MessageBoxResult askSaveChanges()
        {
            if (manager.HasChange)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes first?", "Save changes?", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveMenuItem_Click(null, null);
                }
                return result;
            }
            return MessageBoxResult.Yes;
        }

        /// <summary>
        /// Creates a new Group at the toplevel of the TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewRootGroupButton_Click(object sender, RoutedEventArgs e)
        {
            Group newGroup = manager.newGroup(null);
            // Select new group in the treeview
            newGroup.IsSelected = true;
        }

        /// <summary>
        /// Create a new license under the selected group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLicenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (LicensesTreeView.SelectedItem is Group) {
                Group selectedParentGroup = (Group)LicensesTreeView.SelectedItem;
                License newLicense = manager.newLicense(selectedParentGroup);
                newLicense.select();
            }
        }

        /// <summary>
        /// Remove selected license/group of the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LicensesTreeView.SelectedItem != null)
            {
                if (LicensesTreeView.SelectedItem is Group)
                {
                    Group selectedGroup = (Group)LicensesTreeView.SelectedItem;
                    manager.removeGroup(selectedGroup);
                    // Change selection
                    if (selectedGroup.Parent != null)
                    {
                        selectedGroup.Parent.select();
                    }
                    else
                    {
                        if (manager.Groups.Count != 0)
                        {
                            manager.Groups.First().select();
                        }
                    }
                    LicensesTreeView_SelectedItemChanged(null, null);
                }
                if (LicensesTreeView.SelectedItem is License)
                {
                    License selectedLicense = (License)LicensesTreeView.SelectedItem;
                    selectedLicense.remove();
                    LicensesTreeView_SelectedItemChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Creates a new subgroup under the selected group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewSubGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (LicensesTreeView.SelectedItem != null && LicensesTreeView.SelectedItem is Group)
            {
                Group selectedGroup = (Group)LicensesTreeView.SelectedItem;
                Group newGroup = manager.newGroup(selectedGroup);
                newGroup.select();
            }
        }

        /// <summary>
        /// Copy button pressed: Copy the license key of the currently selected item into the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyCopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(((License)LicensesTreeView.SelectedItem).Key);
        }

        /// <summary>
        /// Open the default mailprogram to send a mail to the developer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactByMailRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Util.devEmail);
        }

        /// <summary>
        /// Open the default browser to browse to the homepage of the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHomepageRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Util.devWeb);
        }

        /// <summary>
        /// Show the about window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow();
            aw.ShowDialog();
        }

        /// <summary>
        /// Called if the application will be closed. Checks first if the user want to save pending changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult dialogResult = askSaveChanges();
            if (dialogResult == MessageBoxResult.Cancel)
            {
                if (e != null)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Exit button pressed (Close the program with askSaveChanges dialog)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Update the title of the window (add open filename if a file is open)
        /// </summary>
        private void updateAppTitle()
        {
            if (manager.Path != null)
            {
                this.Title = windowTitle + " - " + manager.Filename;
            }
            else
            {
                this.Title = windowTitle;
            }
        }

        /// <summary>
        /// On selected item of the treeview changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LicensesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (LicensesTreeView.SelectedItem == null)
            {
                RibbonButtonNewGroup.IsEnabled = false;
                RibbonButtonNewLicense.IsEnabled = false;
                LicenseDetailGrid.Visibility = System.Windows.Visibility.Collapsed;
                GroupDetailGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (LicensesTreeView.SelectedItem is Group)
            {
                RibbonButtonNewLicense.IsEnabled = true;
                RibbonButtonNewGroup.IsEnabled = true;
                if (!LicensesTreeViewContextMenu.Items.Contains(contextMenuItemAddLicense))
                {
                    LicensesTreeViewContextMenu.Items.Add(contextMenuItemAddLicense);
                }
                LicenseDetailGrid.Visibility = System.Windows.Visibility.Collapsed;
                GroupDetailGrid.Visibility = System.Windows.Visibility.Visible;
            }
            if (LicensesTreeView.SelectedItem is License)
            {
                RibbonButtonNewLicense.IsEnabled = false;
                RibbonButtonNewGroup.IsEnabled = false;
                LicensesTreeViewContextMenu.Items.Remove(contextMenuItemAddLicense);
                LicenseDetailGrid.Visibility = System.Windows.Visibility.Visible;
                GroupDetailGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Workaround for rightclick on a treeview element: With this event the element got selected first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LicensesTreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Search the object that is under the cursor
        /// </summary>
        /// <param name="depObj">MouseEventArgs</param>
        /// <returns>Currently hovered object</returns>
        private static TreeViewItem VisualUpwardSearch(DependencyObject depObj)
        {
            while (depObj != null && !(depObj is TreeViewItem))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
            }
            return (TreeViewItem)depObj;
        }
    }
}
