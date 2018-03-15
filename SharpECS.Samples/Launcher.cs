﻿using System;

namespace SharpECS.Samples
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Launcher
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game())
                game.Run();
        }
    }
#endif
}
