using System;
using System.Windows.Forms;

namespace provaXNAGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Form1 prova = new Form1())
            {
                Application.Run(prova);
            }
            */
            using (Game1 game = new Game1())
            {
                Form frm = (Form)Form.FromHandle(game.Window.Handle);
                frm.FormBorderStyle = FormBorderStyle.None;
                game.Run();
            }
        }
    }
}
