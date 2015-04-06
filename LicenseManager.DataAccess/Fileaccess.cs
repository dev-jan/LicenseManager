using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LicenseManager.Logic;

namespace LicenseManager.DataAccess
{
    /// <summary>
    /// This class offers methods to access the filesystem
    /// </summary>
    public class Fileaccess
    {
        /// <summary>
        /// Fileaccess cannot be instantiated, all methods are static
        /// </summary>
        private Fileaccess()
        {
        }

        /// <summary>
        /// Save the data into a file
        /// </summary>
        /// <param name="data">Data to save</param>
        /// <param name="path">Full path to the file</param>
        /// <returns></returns>
        public static Boolean save(Manager data, String path)
        {
            // Checks if the directory exists first
            if (!Directory.Exists(Path.GetPathRoot(path)))
            {
                Directory.CreateDirectory(Path.GetPathRoot(path));
            }
            // Write the data into the file
            using (StreamWriter fileWriter = new StreamWriter(path, false) )
            {
                fileWriter.Write(data.ToString());
            }
            data.HasChange = false;
            return true;
        }

        /// <summary>
        /// Load the data from a previously saved file
        /// </summary>
        /// <param name="path">Full path to the file</param>
        /// <returns>The data that are saved in the file</returns>
        public static Manager load(String path)
        {
            Manager manager = new Manager();
            manager.Path = path;
            if (File.Exists(path))
            {
                using (StreamReader fileReader = new StreamReader(path, false))
                {
                    Group currentGroup = null;
                    while (!fileReader.EndOfStream)
                    {
                        String rawLine = fileReader.ReadLine();
                        currentGroup = interpretLine(rawLine, manager, currentGroup);
                    }
                }
            }
            return manager;
        }

        /// <summary>
        /// Interpret one line of the file
        /// </summary>
        /// <param name="line">readed line of the file</param>
        /// <param name="manager">manager to save the interpreted data</param>
        /// <param name="currentGroup">current group of the file</param>
        /// <returns></returns>
        public static Group interpretLine(String line, Manager manager, Group currentGroup)
        {
            String[] oneLine = line.Split(';');
            if (oneLine[0] == "GROUP")
            {
                Group parentgroup = currentGroup;
                currentGroup = Group.FromString(line);
                if (parentgroup != null)
                {
                    // Group is a subgroup -> add to parent group
                    currentGroup.Parent = parentgroup;
                }
                else
                {
                    // Group is a rootgroup -> add directly into the manager
                    manager.Groups.Add(currentGroup);
                }
            }
            else if (oneLine[0] == "LIC")
            {
                if (currentGroup != null)
                {
                    // add license to the current group
                    currentGroup.addLicense(line);
                }
            }
            else if (oneLine[0] == "ENDGROUP")
            {
                // current group is finished
                if (currentGroup.Parent != null)
                {
                    // set the parent group of the current group as current group
                    return currentGroup.Parent;
                }
                else
                {
                    // current group was a rootgroup, now the currentgroup is null
                    return null;
                }
            }
            else
            {
                // Line doesn't contain usefull data...
            }
            return currentGroup;
        }
    }
}
