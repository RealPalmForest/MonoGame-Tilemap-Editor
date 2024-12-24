using System;
using System.Runtime.InteropServices;

namespace PalmMapEditor.Files;

public static class SaveFile
{
    // Importing the GetSaveFileName function
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName(ref SaveFileName ofn);

    /// <summary>
    /// Shows a save file dialog and returns the chosen file path.
    /// </summary>
    /// <param name="fileType">The file types (e.g., "Text Files (*.txt)\0*.txt\0All Files (*.*)\0*.*\0").</param>
    /// <param name="boxTitle">The title of the dialog box.</param>
    /// <param name="defaultExt">The default file extension (e.g., "txt").</param>
    /// <returns>The file path chosen by the user or an empty string if canceled.</returns>
    public static string SaveDialog(string fileType, string boxTitle, string defaultExt = "txt")
    {
        var ofn = new SaveFileName();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = fileType;                  // Filter for file types
        ofn.lpstrFile = new string(new char[256]);   // Buffer to store the file name
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrFileTitle = new string(new char[64]); // Buffer to store the file title
        ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
        ofn.lpstrTitle = boxTitle;                  // Dialog box title
        ofn.lpstrDefExt = defaultExt;               // Default file extension

        if (GetSaveFileName(ref ofn))               // Show the save file dialog
            return ofn.lpstrFile;

        return string.Empty; // Return empty if the user cancels
    }
}