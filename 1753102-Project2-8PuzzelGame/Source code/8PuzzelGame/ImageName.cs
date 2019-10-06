using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _8PuzzelGame
{
    /// <summary>
    /// class contains all image names in particular folder
    /// </summary>
    public class ImageName
    {
        private string foldername;

        public ImageName(string folder = @"Icons\")
        {
            foldername = folder;
            LoadImageName();
        }

        public void LoadImageName()
        {
            // open folder, get all filenames
            string path = AppDomain.CurrentDomain.BaseDirectory + foldername;
            string[] filenames = Directory.GetFiles(path);


            // check filename to assign to variables
            foreach (var fullname in filenames)
            {
                string filename = Path.GetFileName(fullname);

                string[] tokens = filename.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens[0].ToLower() == "Application".ToLower())
                {
                    ApplicationIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Option".ToLower())
                {
                    OptionIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Remove".ToLower())
                {
                    RemoveIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "RemoveAll".ToLower())
                {
                    RemoveAllIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Add".ToLower())
                {
                    AddIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Preview".ToLower())
                {
                    PreviewIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Start".ToLower())
                {
                    StartIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Refresh".ToLower())
                {
                    RefreshIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Help".ToLower())
                {
                    HelpIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Open".ToLower())
                {
                    OpenIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Save".ToLower())
                {
                    SaveIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Information".ToLower())
                {
                    InformationIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "File".ToLower())
                {
                    FileIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Folder".ToLower())
                {
                    FolderIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "Action".ToLower())
                {
                    ActionIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "List".ToLower())
                {
                    ListIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "UpDirection".ToLower())
                {
                    UpDirectionIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "RightDirection".ToLower())
                {
                    RightDirectionIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "DownDirection".ToLower())
                {
                    DownDirectionIcon.Add
                    (
                        path + filename
                    );
                }
                else if (tokens[0].ToLower() == "LeftDirection".ToLower())
                {
                    LeftDirectionIcon.Add
                    (
                        path + filename
                    );
                }
                else
                {
                    Images.Add
                    (
                        path + filename
                    );
                }


            }

        }

        public BindingList<string> ApplicationIcon { get; set; } = new BindingList<string>();
        public BindingList<string> OptionIcon { get; set; } = new BindingList<string>();
        public BindingList<string> RemoveIcon { get; set; } = new BindingList<string>();
        public BindingList<string> RemoveAllIcon { get; set; } = new BindingList<string>();
        public BindingList<string> AddIcon { get; set; } = new BindingList<string>();
        public BindingList<string> PreviewIcon { get; set; } = new BindingList<string>();
        public BindingList<string> StartIcon { get; set; } = new BindingList<string>();
        public BindingList<string> RefreshIcon { get; set; } = new BindingList<string>();
        public BindingList<string> HelpIcon { get; set; } = new BindingList<string>();
        public BindingList<string> OpenIcon { get; set; } = new BindingList<string>();
        public BindingList<string> SaveIcon { get; set; } = new BindingList<string>();
        public BindingList<string> InformationIcon { get; set; } = new BindingList<string>();
        public BindingList<string> FileIcon { get; set; } = new BindingList<string>();
        public BindingList<string> FolderIcon { get; set; } = new BindingList<string>();
        public BindingList<string> ActionIcon { get; set; } = new BindingList<string>();
        public BindingList<string> ListIcon { get; set; } = new BindingList<string>();
        public BindingList<string> UpDirectionIcon { get; set; } = new BindingList<string>();
        public BindingList<string> RightDirectionIcon { get; set; } = new BindingList<string>();
        public BindingList<string> DownDirectionIcon { get; set; } = new BindingList<string>();
        public BindingList<string> LeftDirectionIcon { get; set; } = new BindingList<string>();


        public BindingList<string> Images { get; set; } = new BindingList<string>();


    }
}


