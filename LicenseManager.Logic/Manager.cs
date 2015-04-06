using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager.Logic
{
    /// <summary>
    /// Represent the data of the LicenseManager
    /// 
    /// Contains the list of all rootgroups & its licenses
    /// Offers methods to manipulate the data
    /// </summary>
    public class Manager : INotifyPropertyChanged
    {
        /// <summary>
        /// Instantiate a new manager object (usually only one manager per application is needed!)
        /// </summary>
        public Manager() 
        {
            hasChange = false;

            // Listen to changes of the data
            groups.CollectionChanged += groups_CollectionChanged;
        }

        private String path;
        /// <summary>
        /// The path where the data is stored (NULL if the data is not stored in a file)
        /// </summary>
        public String Path
        {
            get { return path; }
            set 
            { 
                path = value;
                OnPropertyChanged("Path");
                OnPropertyChanged("Filename");
            }
        }
        
        /// <summary>
        /// The Filename of the path where the data is stored (read-only)
        /// </summary>
        public String Filename
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }

        private Boolean hasChange;
        /// <summary>
        /// Has the manager changes of the data (compare to the stored file)
        /// </summary>
        public Boolean HasChange
        {
            get { return hasChange; }
            set 
            { 
                hasChange = value;
                OnPropertyChanged("HasChange");
            }
        }

        private ObservableCollection<Group> groups = new ObservableCollection<Group>();
        /// <summary>
        /// List of all rootgroups (the contains all other groups + licenses)
        /// </summary>
        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set 
            { 
                groups = value;
                OnPropertyChanged("Groups");
            }
        }

        /// <summary>
        /// Serialize all groups, subgroups & licenses into a string
        /// </summary>
        /// <returns>Serialized representation of all data</returns>
        override
        public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# Info: LicenseManager Data File \n");
            sb.Append("# DO NOT CHANGE THE STRUCTURE OF THIS FILE!\n");

            foreach (Group oneGroup in Groups) 
            {
                sb.Append(oneGroup.ToString());
            }

            sb.Append("# End of File");
            return sb.ToString();
        }

        /// <summary>
        /// Notify property change of the rootgroups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void groups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            hasChange = true;
        }

        /// <summary>
        /// Called on property change (notify the UI)
        /// </summary>
        /// <param name="name">Changed Property (Name of the property, not of the private field!)</param>
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Public event that got called on property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Create a new Group and add it into the parentgroup/manager
        /// </summary>
        /// <param name="parentGroup">Parent group of group, NULL if new group is a rootgroup</param>
        /// <returns>created group</returns>
        public Group newGroup(Group parentGroup)
        {
            Group newGroup = new Group(parentGroup);
            newGroup.Name = "[Name...]";
            if (parentGroup == null)
            {
                this.Groups.Add(newGroup);
            }
            return newGroup;
        }

        /// <summary>
        /// Create a new License and add it into the parentgroup
        /// </summary>
        /// <param name="parentGroup">Parent group of the license (cannot be null!)</param>
        /// <returns>created license</returns>
        public License newLicense(Group parentGroup)
        {
            License newLic = new License(parentGroup);
            newLic.Product = "[Product...]";
            return newLic;
        }

        /// <summary>
        /// Remove a group
        /// </summary>
        /// <param name="groupToRemove">Group to remove</param>
        public void removeGroup(Group groupToRemove)
        {
            if (groupToRemove.Parent != null)
            {
                groupToRemove.Parent.Subgroups.Remove(groupToRemove);
            }
            else
            {
                this.Groups.Remove(groupToRemove);
            }
        }

    }
}
