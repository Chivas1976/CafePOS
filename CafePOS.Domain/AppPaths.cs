using System;
using System.IO;

namespace CafePOS.Domain;

public static class AppPaths
{
    public static string Root =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CafePOS");

    public static string Belege => Ensure(Path.Combine(Root, "Belege"));
    public static string Config => Ensure(Path.Combine(Root, "Config"));

    private static string Ensure(string p) { Directory.CreateDirectory(p); return p; }
}