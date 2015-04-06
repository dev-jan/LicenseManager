using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LicenseManager.Logic
{
    /// <summary>
    /// Represent a group of licenses
    /// </summary>
    public class Group : INotifyPropertyChanged
    {
        /// <summary>
        /// Instantiate a new Group with a parent group
        /// </summary>
        /// <param name="parent">Parental group</param>
        public Group(Group parent)
        {
            Parent = parent;

            // Register changeevent of the subitems
            Subgroups.CollectionChanged += Subgroups_CollectionChanged;
            Licenses.CollectionChanged += Licenses_CollectionChanged;
        }

        /// <summary>
        /// Instantiate a new Group without a parent group (root group)
        /// </summary>
        public Group()
        {
        }

        /// <summary>
        /// Notify property change of the subgroup of licenses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Licenses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Licenses");
        }

        /// <summary>
        /// Notify property change of the subgroup of groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Subgroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Subgroups");
        }

        private String name;
        /// <summary>
        /// The Name of the group 
        /// </summary>
        public String Name
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<License> licenses = new ObservableCollection<License>();
        /// <summary>
        /// List of license of the group
        /// </summary>
        public ObservableCollection<License> Licenses
        {
            get { return licenses; }
        }

        private ObservableCollection<Group> subgroups = new ObservableCollection<Group>();
        /// <summary>
        /// List of subgroups of this group (this groups may also contains licenses)
        /// </summary>
        public ObservableCollection<Group> Subgroups
        {
            get { return subgroups; }
            set 
            { 
                subgroups = value;
                OnPropertyChanged("Subgroups");
            }
        }

        private Group parent;
        /// <summary>
        /// The parent group of the group (NULL if the group is a root group)
        /// </summary>
        public Group Parent
        {
            get { return parent; }
            set 
            { 
                parent = value;
                if (parent != null)
                {
                    parent.Subgroups.Add(this);
                }
                OnPropertyChanged("Parent");
            }
        }

        private Boolean isSelected;
        /// <summary>
        /// UI Property: Group is selected in the treeview
        /// </summary>
        public Boolean IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Select the group in the UI
        /// </summary>
        public void select()
        {
            IsSelected = true;
            IsExpanded = true;
        }

        private Boolean isExpanded;
        /// <summary>
        /// UI Property: Group (and parent groups) are expanded in the treeview
        /// </summary>
        public Boolean IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// List of all subitems (licenses & groups)
        /// </summary>
        public IEnumerable Items
        {
            get
            {
                CompositeCollection items = new CompositeCollection();
                items.Add(new CollectionContainer { Collection = Licenses });
                items.Add(new CollectionContainer { Collection = Subgroups});
                return items;
            }
        }

        /// <summary>
        /// Serialize the group and its subitems (subgroups & licenses)
        ///   Format:  GROUP;[NAME OF GROUP];
        ///            [SUBGROUPS & LICENSES]
        ///            ENDGROUP;
        /// </summary>
        /// <returns>Serializsed String with all subitems (multi-lined)</returns>
        override
        public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("GROUP;{0};", Util.escape(Name)));
            foreach (Group oneGroup in Subgroups)
            {
                sb.Append(oneGroup.ToString());
            }
            foreach (License oneLicense in licenses) 
            {
                sb.AppendLine(oneLicense.ToString());
            }
            sb.AppendLine("ENDGROUP;");
            return sb.ToString();
        }

        /// <summary>
        /// Instantiate a group from the serialized string of a group.
        /// Important: The data must only include the definition of the group, without subitems!
        ///            For adding licenses to the group use <see cref="LicenseManager.Logic.Group.addLicense"/>
        /// </summary>
        /// <param name="data">Definition String of a group (single line)</param>
        /// <returns></returns>
        public static Group FromString(String data)
        {
            Group newGroup = new Group(null); 
            String[] fields = data.Split(';');
            newGroup.Name = fields[1];
            return newGroup;
        }

        /// <summary>
        /// Add a new license to the group from its serialized representation string
        /// </summary>
        /// <param name="data">Serialized String of license</param>
        public void addLicense(String data)
        {
            License.FromString(data, this);
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
    }
}
