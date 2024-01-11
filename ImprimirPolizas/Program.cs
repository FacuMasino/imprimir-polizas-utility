using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AutoUpdaterDotNET;

namespace ImprimirPolizas
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setear idioma español
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture =
                CultureInfo.CreateSpecificCulture("es");
            // Verificar actualizaciones
            AutoUpdater.Start(
                "https://github.com/FacuMasino/imprimir-polizas-utility/raw/main/autoupdater.xml"
            );
            string path = Directory.GetCurrentDirectory();
            Directory.CreateDirectory($"{path}\\descargas");
            TryLoadNativeLibrary("\\");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        // Intenta cargar pdfium.dll para evitar error "Unable to load DLL 'pdfium.dll'"
        // El archivo .dll debe estar dentro de las correspondientes carpetas en el directorio raiz
        private static bool TryLoadNativeLibrary(string path)
        {
            if (path == null)
                return false;

            path = Path.Combine(path, IntPtr.Size == 4 ? "x86" : "x64");

            path = Path.Combine(path, "pdfium.dll");

            return File.Exists(path) && LoadLibrary(path) != IntPtr.Zero;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)] string lpFileName
        );
    }
}
