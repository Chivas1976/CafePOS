using System;
using System.IO;

namespace CafePOS.Domain;

public static class AppPaths
{
    public static string Root =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CafePOS");

    public static string Belege => Directory.CreateDirectory(Path.Combine(Root, "Belege")).ToString();
    public static string Config => Directory.CreateDirectory(Path.Combine(Root, "Config")).ToString();

    //private static string (string p) { Directory.CreateDirectory(p); return p; }
}