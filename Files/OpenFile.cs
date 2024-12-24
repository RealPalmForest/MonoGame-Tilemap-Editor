using System;
using System.Runtime.InteropServices;

namespace PalmMapEditor.Files;

// From https://www.pinvoke.net/default.aspx/Structures/OPENFILENAME.html
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct SaveFileName
{
    public int lStructSize;
    public IntPtr hwndOwner;
    public IntPtr hInstance;
    public string lpstrFilter;
    public string lpstrCustomFilter;
    public int nMaxCustFilter;
    public int nFilterIndex;
    public string lpstrFile;
    public int nMaxFile;
    public string lpstrFileTitle;
    public int nMaxFileTitle;
    public string lpstrInitialDir;
    public string lpstrTitle;
    public int Flags;
    public short nFileOffset;
    public short nFileExtension;
    public string lpstrDefExt;
    public IntPtr lCustData;
    public IntPtr lpfnHook;
    public string lpTemplateName;
    public IntPtr pvReserved;
    public int dwReserved;
    public int flagsEx;
}

public static class OpenFile
{
    // From https://www.pinvoke.net/default.aspx/comdlg32/GetOpenFileName.html
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref SaveFileName ofn);

    /// <summary>
    /// Shows a prompt to select a file, then returns its path.
    /// </summary>
    /// <param name="fileType">The file types. Eg: "Excel Files (*.xlsx)\0*.xlsx\0All Files (*.*)\0*.*\0PNG\0*.png\0"</param>
    /// <param name="boxTitle">The title of the prompt.</param>
    /// <returns></returns>
    public static string OpenDialog(string fileType, string boxTitle)
    {
        var ofn = new SaveFileName();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        // Define Filter for your extensions (Excel, ...)
        ofn.lpstrFilter = fileType;
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrFileTitle = new string(new char[64]);
        ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
        ofn.lpstrTitle = boxTitle;

        if (GetOpenFileName(ref ofn))
            return ofn.lpstrFile;
        return string.Empty;
    }
}