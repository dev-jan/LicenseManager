using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager.Logic
{
    /// <summary>
    /// Represent a single License
    /// </summary>
    public class License : INotifyPropertyChanged
    {
        /// <summary>
        /// Instantiate a new License under a parent group
        /// </summary>
        /// <param name="parent">Parent group (NULL if license not have a parent)</param>
        public License(Group parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Instantiate a new License without a parent group
        /// </summary>
        public License()
        {
        }

        private String product;
        /// <summary>
        /// The productname of the license
        /// </summary>
        public String Product
        {
            get { return product; }
            set 
            { 
                product = value;
                OnPropertyChanged("Product");
            }
        }

        private String key;
        /// <summary>
        /// The licensekey of the license
        /// </summary>
        public String Key
        {
            get { return key; }
            set 
            {
                key = value;
                OnPropertyChanged("Key");
            }
        }

        private String notice;
        /// <summary>
        /// The notice of the license (optional)
        /// </summary>
        public String Notice
        {
            get { return notice; }
            set 
            { 
                notice = value;
                OnPropertyChanged("Notice");
            }
        }

        private Group parent;
        /// <summary>
        /// Parent group of the license
        /// </summary>
        public Group Parent
        {
            get { return parent; }
            set 
            { 
                parent = value;
                if (parent != null)
                {
                    parent.Licenses.Add(this);
                }
                OnPropertyChanged("Parent");
            }
        }

        private Boolean isSelected;
        /// <summary>
        /// UI Property: License selected in the treeview
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
        /// Select this license (in the UI)
        /// </summary>
        public void select()
        {
            IsSelected = true;
            IsExpanded = true;
        }

        private Boolean isExpanded;
        /// <summary>
        /// UI Property: License & parent groups expended
        /// </summary>
        public Boolean IsExpanded
        {
            get { return isExpanded; }
            set 
            { 
                isExpanded = value;
                if (parent != null)
                {
                    Parent.IsExpanded = value;
                }
                OnPropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// Serialize the license into a single string (one line)
        ///   Format: LIC;[PRODUCTNAME];[KEY];[NOTICE];
        /// </summary>
        /// <returns>Serialized String</returns>
        override
        public String ToString()
        {
            return String.Format("LIC;{0};{1};{2};", Util.escape(Product), Util.escape(Key), Util.escape(Notice));
        }
        
        /// <summary>
        /// Instantiate a license from a serialized String of the license
        ///   Format: <see cref="LicenseManager.Logic.License.ToString()" />
        /// </summary>
        /// <param name="data">Serialised License String</param>
        /// <param name="parent">Parent group of the license</param>
        /// <returns>The instantiated license</returns>
        public static License FromString(String data, Group parent) {
            License newLicense = new License(parent);
            String[] fields = data.Split(';');
            newLicense.Product = fields[1];
            newLicense.Key = fields[2];
            newLicense.Notice = Util.unescape(fields[3]);
            return newLicense;
        }

        /// <summary>
        /// Remove the license
        /// </summary>
        public void remove()
        {
            if (Parent != null)
            {
                Parent.Licenses.Remove(this);
            }
            else
            {
                // License cannot be removed...
            }
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
