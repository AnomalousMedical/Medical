using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical
{
    /// <summary>
    /// This class keeps track of the current filenames for a series of
    /// open, save, and save as operations.
    /// </summary>
    public class FileTracker
    {
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private SaveFileDialog saveFileDialog = new SaveFileDialog();
        private String currentFile = null;
        private DialogResult lastResult;

        /// <summary>
        /// Constructor, takes a filter for the save and open dialogs.
        /// </summary>
        /// <param name="filter"></param>
        public FileTracker(String filter)
        {
            saveFileDialog.Filter = filter;
            openFileDialog.Filter = filter;
        }

        public void setInitialDirectory(String path)
        {
            saveFileDialog.InitialDirectory = path;
            openFileDialog.InitialDirectory = path;
        }

        /// <summary>
        /// Show the open file dialog and return the chosen file.  
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <returns>The current file or null if the user canceled the dialog.</returns>
        public String openFile(IWin32Window parent)
        {
            lastResult = openFileDialog.ShowDialog(parent);
            if (openFileDialog.FileName != null && openFileDialog.FileName != "")
            {
                currentFile = openFileDialog.FileName;
            }
            else
            {
                currentFile = null;
            }
            return currentFile;
        }

        /// <summary>
        /// Gets the name of the current file for a save operation.  If no file is
        /// defined the save dialog box will be shown and that result will be returned.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <returns>The current file or null if the user canceled the dialog.</returns>
        public String saveFile(IWin32Window parent)
        {
            if (currentFile == null)
            {
                saveFileAs(parent);
            }
            else
            {
                lastResult = DialogResult.OK;
            }
            return currentFile;
        }

        /// <summary>
        /// This will show the save file dialog and will return the newly chosen file.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <returns>The current file or null if the user canceled the dialog.</returns>
        public String saveFileAs(IWin32Window parent)
        {
            lastResult = saveFileDialog.ShowDialog(parent);
            if (saveFileDialog.FileName != null && saveFileDialog.FileName != "")
            {
                currentFile = saveFileDialog.FileName;
            }
            else
            {
                currentFile = null;
            }
            return currentFile;
        }

        /// <summary>
        /// Call this function to clear the current file.  This will force the save dialog
        /// to open when save() is called.  Useful if a new thing to be saved was made.
        /// </summary>
        public void clearCurrentFile()
        {
            setCurrentFile(null);
        }

        /// <summary>
        /// Invalidates the current file.  Call this to force calls to saveFile to show
        /// the dialog.
        /// </summary>
        public void invalidateCurrentFile()
        {
            setCurrentFile(null);
        }

        /// <summary>
        /// Get the currently selected filename.
        /// </summary>
        /// <returns>The current file.</returns>
        public String getCurrentFile()
        {
            return currentFile;
        }

        /// <summary>
        /// Set the currently selected filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void setCurrentFile(String filename)
        {
            currentFile = filename;
            saveFileDialog.FileName = currentFile;
            openFileDialog.FileName = currentFile;
        }

        /// <summary>
        /// Check to see if the user really decided to save the file.
        /// </summary>
        /// <returns>True if the use clicked Ok.  False if they clicked Cancel.</returns>
        public bool lastDialogAccepted()
        {
            return lastResult == DialogResult.OK;
        }
    }
}
