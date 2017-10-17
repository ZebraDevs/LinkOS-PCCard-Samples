using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ZXP7_ZXP8_CSharp_SetErrorRecoveryMode_SA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
