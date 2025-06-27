using System;
using System.Diagnostics;
using System.Linq;
using Gwen.Net.Components;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;
using Gwen.Net.Platform;
using static Gwen.Net.Platform.GwenPlatform;

namespace Gwen.Net.CommonDialog
{
    /// <summary>
    ///     Base class for a file or directory dialog.
    /// </summary>
    public abstract class FileDialog : Component
    {
        private String currentFilter;

        private String currentFolder;
        private readonly Label fileNameLabel;
        private readonly ComboBox filters;

        private readonly TreeControl folders;

        private Boolean foldersOnly;
        private readonly ListBox items;
        private readonly VerticalSplitter nameFilterSplitter;
        private readonly Button newFolder;
        private readonly Button ok;

        private Boolean onClosing;
        private readonly TextBox path;
        private readonly TextBox selectedName;
        private readonly Window window;

        /// <summary>
        ///     Constructor for the base class. Implementing classes must call this.
        /// </summary>
        /// <param name="parent">Parent.</param>
        protected FileDialog(ControlBase parent) : base(new Window(parent))
        {
            window = (Window) View;
            window.Size = new Size(width: 400, height: 300);
            window.StartPosition = StartPosition.CenterCanvas;
            window.Closed += OnWindowClosed;

            DockLayout layout = new(window)
            {
                Margin = Margin.Two
            };
            
            DockLayout top = new(layout)
            {
                Dock = Dock.Top
            };

            _(new Label(top)
            {
                Dock = Dock.Left,
                Margin = Margin.Two,
                Alignment = Alignment.CenterV | Alignment.Left,
                Text = "Path:"
            });
            
            path = new TextBox(top)
            {
                Dock = Dock.Fill,
                Margin = Margin.Two
            };
            path.SubmitPressed += OnPathSubmitted;
            
            newFolder = new Button(top)
            {
                Dock = Dock.Right,
                Margin = Margin.Two,
                Padding = new Padding(left: 10, top: 0, right: 10, bottom: 0),
                Text = "New"
            };
            newFolder.Clicked += OnNewFolderClicked;
            
            var up = new Button(top)
            {
                Dock = Dock.Right,
                Margin = Margin.Two,
                Padding = new Padding(left: 10, top: 0, right: 10, bottom: 0),
                Text = "Up"
            };
            up.Clicked += OnUpClicked;
            
            var center = new VerticalSplitter(layout)
            {
                Dock = Dock.Fill,
                Value = 0.3f,
                SplitterSize = 2
            };
            
            folders = new TreeControl(center)
            {
                Margin = Margin.Two
            };
            folders.Selected += OnFolderSelected;
            
            items = new ListBox(center)
            {
                Margin = Margin.Two,
                ColumnCount = 3
            };
            items.RowSelected += OnItemSelected;
            items.RowDoubleClicked += OnItemDoubleClicked;
            
            DockLayout bottom = new(layout)
            {
                Dock = Dock.Bottom
            };
            
            var cancel = new Button(bottom)
            {
                Dock = Dock.Right,
                Margin = Margin.Two,
                Padding = new Padding(left: 10, top: 0, right: 10, bottom: 0),
                Width = 100,
                Text = "Cancel"
            };
            cancel.Clicked += OnCancelClicked;
            
            ok = new Button(bottom)
            {
                Dock = Dock.Right,
                Margin = Margin.Two,
                Padding = new Padding(left: 10, top: 0, right: 10, bottom: 0),
                Width = 100,
                Text = "Ok"
            };
            ok.Clicked += OnOkClicked;
            
            nameFilterSplitter = new VerticalSplitter(layout)
            {
                Dock = Dock.Bottom,
                Value = 0.7f,
                SplitterSize = 2
            };
            
            DockLayout name = new(nameFilterSplitter);
            
            fileNameLabel = new Label(name)
            {
                Dock = Dock.Left,
                Margin = Margin.Two,
                Alignment = Alignment.CenterV | Alignment.Left,
                Text = "File name:"
            };
            
            selectedName = new TextBox(name)
            {
                Dock = Dock.Fill,
                Margin = Margin.Two
            };
            selectedName.SubmitPressed += OnNameSubmitted;
            
            filters = new ComboBox(nameFilterSplitter)
            {
                Margin = Margin.Two
            };
            filters.ItemSelected += OnFilterSelected;
        }

        /// <summary>
        /// Suppress the warning that the label is unused - the parent uses it.
        /// </summary>
        private static void _(ControlBase control)
        {
            Debug.Assert(control != null);
        }
        
        /// <summary>
        ///     Initial folder for the dialog.
        /// </summary>
        public String InitialFolder
        {
            set => SetPath(value);
        }

        /// <summary>
        ///     Set initial folder and selected item.
        /// </summary>
        public String CurrentItem
        {
            set
            {
                SetPath(GetDirectoryName(value));
                SetCurrentItem(GetFileName(value));
            }
        }

        /// <summary>
        ///     Window title.
        /// </summary>
        public String Title
        {
            get => window.Title;
            set => window.Title = value;
        }

        /// <summary>
        ///     File filters. See <see cref="SetFilters(string, int)" />.
        /// </summary>
        public String Filters
        {
            set => SetFilters(value);
        }

        /// <summary>
        ///     Text shown in the ok button.
        /// </summary>
        public String OkButtonText
        {
            get => ok.Text;
            set => ok.Text = value;
        }

        /// <summary>
        ///     Function that is called when dialog is closed. If ok is pressed, parameter is the selected file / directory.
        ///     If cancel is pressed or window closed, parameter is null.
        /// </summary>
        public Action<String> Callback { get; set; }

        /// <summary>
        ///     Hide or show new folder button.
        /// </summary>
        public Boolean EnableNewFolder
        {
            get => !newFolder.IsCollapsed;
            set => newFolder.IsCollapsed = !value;
        }

        /// <summary>
        ///     Show only directories.
        /// </summary>
        protected Boolean FoldersOnly
        {
            get => foldersOnly;
            set
            {
                foldersOnly = value;
                filters.IsCollapsed = value;
                fileNameLabel.Text = "Folder name:";

                if (value)
                {
                    nameFilterSplitter.Zoom(section: 0);
                }
                else
                {
                    nameFilterSplitter.UnZoom();
                }
            }
        }

        protected override void OnCreated()
        {
            UpdateFolders();

            onClosing = false;

            currentFolder = CurrentDirectory;

            currentFilter = "*.*";
            filters.AddItem("All files (*.*)", "All files (*.*)", "*.*");
        }

        /// <summary>
        ///     Set current path.
        /// </summary>
        /// <param name="newPath">Path.</param>
        /// <returns>True if the path change was successful. False otherwise.</returns>
        public Boolean SetPath(String newPath)
        {
            if (DirectoryExists(newPath))
            {
                currentFolder = newPath;
                path.Text = currentFolder;
                UpdateItemList();

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Set filters.
        /// </summary>
        /// <param name="filterStr">Filter string. Format 'name|filter[|name|filter]...'</param>
        /// <param name="current">Set this index as a current filter.</param>
        public void SetFilters(String filterStr, Int32 current = 0)
        {
            String[] newFilters = filterStr.Split(separator: '|');

            if ((newFilters.Length & 0x1) == 0x1)
            {
                throw new Exception("Error in filter.");
            }

            filters.RemoveAll();

            for (var i = 0; i < newFilters.Length; i += 2)
            {
                filters.AddItem(newFilters[i], newFilters[i], newFilters[i + 1]);
            }

            filters.SelectedIndex = current;
        }

        /// <summary>
        ///     Set current file or directory.
        /// </summary>
        /// <param name="item">File or directory. This doesn't need to exists.</param>
        protected void SetCurrentItem(String item)
        {
            selectedName.Text = item;
        }

        /// <summary>
        ///     Close the dialog and call the call back function.
        /// </summary>
        /// <param name="callbackPath">Parameter for the call back function.</param>
        protected void Close(String callbackPath)
        {
            OnClosing(callbackPath, doClose: true);
        }

        /// <summary>
        ///     Called when the user selects a file or directory.
        /// </summary>
        /// <param name="selectedPath">Full path of selected file or directory.</param>
        protected virtual void OnItemSelected(String selectedPath)
        {
            if ((DirectoryExists(selectedPath) && foldersOnly) || (FileExists(selectedPath) && !foldersOnly))
            {
                SetCurrentItem(GetFileName(selectedPath));
            }
        }

        /// <summary>
        ///     Called to validate the file or directory name when the user enters it.
        /// </summary>
        /// <param name="submittedPath">Full path of the name.</param>
        /// <returns>Is the name valid.</returns>
        protected virtual Boolean IsSubmittedNameOk(String submittedPath)
        {
            if (DirectoryExists(submittedPath))
            {
                if (!foldersOnly)
                {
                    SetPath(submittedPath);
                }
            }
            else if (FileExists(submittedPath))
            {
                return true;
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Called to validate the path when the user presses the ok button.
        /// </summary>
        /// <param name="pathToValidate">Full path.</param>
        /// <returns>Is the path valid.</returns>
        protected virtual Boolean ValidateFileName(String pathToValidate)
        {
            return true;
        }

        /// <summary>
        ///     Called when the dialog is closing.
        /// </summary>
        /// <param name="callbackPath">Path for the call back function</param>
        /// <param name="doClose">True if the dialog needs to be closed.</param>
        protected virtual void OnClosing(String callbackPath, Boolean doClose)
        {
            if (onClosing)
            {
                return;
            }

            onClosing = true;

            if (doClose)
            {
                window.Close();
            }

            if (Callback != null)
            {
                Callback(callbackPath);
            }
        }

        private void OnPathSubmitted(ControlBase sender, EventArgs args)
        {
            if (!SetPath(path.Text))
            {
                path.Text = currentFolder;
            }
        }

        private void OnUpClicked(ControlBase sender, ClickedEventArgs args)
        {
            String newPath = GetDirectoryName(currentFolder);

            if (newPath != null)
            {
                SetPath(newPath);
            }
        }

        private void OnNewFolderClicked(ControlBase sender, ClickedEventArgs args)
        {
            String folderPath = path.Text;

            if (DirectoryExists(folderPath))
            {
                path.Focus();
            }
            else
            {
                try
                {
                    CreateDirectory(folderPath);
                    SetPath(folderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(View, ex.Message, Title);
                }
            }
        }

        private void OnFolderSelected(ControlBase sender, EventArgs args)
        {
            var node = sender as TreeNode;

            if (node != null && node.UserData != null)
            {
                SetPath(node.UserData as String);
            }
        }

        private void OnItemSelected(ControlBase sender, ItemSelectedEventArgs args)
        {
            if (args.SelectedItem.UserData is String selectedPath)
            {
                OnItemSelected(selectedPath);
            }
        }

        private void OnItemDoubleClicked(ControlBase sender, ItemSelectedEventArgs args)
        {
            if (args.SelectedItem.UserData is String selectedPath)
            {
                if (DirectoryExists(selectedPath))
                {
                    SetPath(selectedPath);
                }
                else
                {
                    OnOkClicked(sender: null, args: null);
                }
            }
        }

        private void OnNameSubmitted(ControlBase sender, EventArgs args)
        {
            String submittedPath = Combine(currentFolder, selectedName.Text);

            if (IsSubmittedNameOk(submittedPath))
            {
                OnOkClicked(sender: null, args: null);
            }
        }

        private void OnFilterSelected(ControlBase sender, ItemSelectedEventArgs args)
        {
            currentFilter = filters.SelectedItem.UserData as String;
            UpdateItemList();
        }

        private void OnOkClicked(ControlBase sender, ClickedEventArgs args)
        {
            String clickedPath = Combine(currentFolder, selectedName.Text);

            if (ValidateFileName(clickedPath))
            {
                OnClosing(clickedPath, doClose: true);
            }
        }

        private void OnCancelClicked(ControlBase sender, ClickedEventArgs args)
        {
            OnClosing(callbackPath: null, doClose: true);
        }

        private void OnWindowClosed(ControlBase sender, EventArgs args)
        {
            OnClosing(callbackPath: null, doClose: false);
        }

        private void UpdateItemList()
        {
            items.Clear();

            IOrderedEnumerable<IFileSystemDirectoryInfo> directories;
            IOrderedEnumerable<IFileSystemFileInfo> files = null;

            try
            {
                directories = GetDirectories(currentFolder).OrderBy(di => di.Name);

                if (!foldersOnly)
                {
                    files = GetFiles(currentFolder, currentFilter).OrderBy(fi => fi.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(View, ex.Message,  Title);

                return;
            }

            foreach (IFileSystemDirectoryInfo di in directories)
            {
                ListBoxRow row = items.AddRow(di.Name, name: null, di.FullName);
                row.SetCellText(columnIndex: 1, "<dir>");
                row.SetCellText(columnIndex: 2, di.FormattedLastWriteTime);
            }

            if (!foldersOnly)
            {
                foreach (IFileSystemFileInfo fi in files)
                {
                    ListBoxRow row = items.AddRow(fi.Name, name: null, fi.FullName);
                    row.SetCellText(columnIndex: 1, fi.FormattedFileLength);
                    row.SetCellText(columnIndex: 2, fi.FormattedFileLength);
                }
            }
        }

        private void UpdateFolders()
        {
            folders.RemoveAllNodes();

            foreach (ISpecialFolder folder in GetSpecialFolders())
            {
                TreeNode category = folders.FindNodeByName(folder.Category, recursive: false);

                if (category == null)
                {
                    category = folders.AddNode(folder.Category, folder.Category);
                }

                category.AddNode(folder.Name, folder.Name, folder.Path);
            }

            folders.ExpandAll();
        }
    }
}
