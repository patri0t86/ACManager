using Decal.Adapter;
using System;
using System.IO;
using System.Text;

namespace ACManager
{
    /// <summary>
    /// General debugging class.
    /// Can write to debug file, or just output to chat.
    /// </summary>
    internal static class Debug
    {
        private static string DebugFileName { get { return "acm_debug.txt"; } }
        private static string DebugFilePath { get; set; }
        private static string ErrorFileName { get { return "acm_errors.txt"; } }
        private static string ErrorFilePath { get; set; }

        /// <summary>
        /// Sets the file paths to print debug/exceptions statements to.
        /// </summary>
        /// <param name="path"></param>
        internal static void Init(string path)
        {
            ErrorFilePath = Path.Combine(path, ErrorFileName);
            DebugFilePath = Path.Combine(path, DebugFileName);
        }

        /// <summary>
        /// Function to debug errors to a file.
        /// </summary>
        /// <param name="e"></param>
        internal static void LogException(Exception e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString());
                sb.Append(" --- ");
                sb.Append(e.StackTrace);
                sb.Append(Environment.NewLine);
                File.AppendAllText(ErrorFilePath, sb.ToString());
            }
            catch (Exception ex)
            {
                CoreManager.Current.Actions.AddChatText(ex.Message, 5, 1);
            }
        }

        /// <summary>
        /// Function to write in-line debug statements to a file.
        /// </summary>
        /// <param name="message"></param>
        internal static void ToFile(string message)
        {
            try
            {
                string text = $"{DateTime.Now} --- {message}{Environment.NewLine}";
                File.AppendAllText(DebugFilePath, text);
            }
            catch (Exception e)
            {
                CoreManager.Current.Actions.AddChatText(e.Message, 5, 1);
            }
        }

        /// <summary>
        /// Sends a chat message in-game. This is visible only to the client running this bot.
        /// This is used for debugging or sending info to the user.
        /// </summary>
        /// <param name="text"></param>
        internal static void ToChat(string text)
        {
            CoreManager.Current.Actions.AddChatText(text, 5, 1);
        }
    }
}
