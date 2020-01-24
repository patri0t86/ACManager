using System;
using System.IO;

namespace FellowshipManager
{
    public static class Crash
    {
        public static void Notify(string characterName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + "Crash_Errors.txt", true))
                {
                    writer.WriteLine(DateTime.Now.ToString() + " - " + characterName + Environment.NewLine);
                    writer.Close();
                }
            }
            catch
            {
            }
        }
    }
}
