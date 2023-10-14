using System;
using System.Windows;

namespace JeepDiag.PassThroughWPF
{
    class Program
    {
        [STAThread]
        private static void Main()
        {
            var app = new Application();
            app.Run(new MainWindow());
        }
    }
}
