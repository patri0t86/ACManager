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
                if (!characterName.Equals(""))
                {
                    using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + "Crash_Errors.txt", true))
                    {
                        writer.WriteLine(DateTime.Now.ToString() + " - " + characterName);
                        writer.Close();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
