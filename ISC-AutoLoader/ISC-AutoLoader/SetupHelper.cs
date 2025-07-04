

namespace ISC_AutoLoader
{ 

    public class SetupHelper
    {
        /// <summary>
        /// Attempt to make things easer for windows users
        /// </summary>
        public SetupHelper() { }
        public void runCommand(String command)
        {
            string run = $"/C {command}";
            //--headless
            System.Diagnostics.Process.Start("cmd.exe", run);
            Console.WriteLine("Added a windows Env Variable you will need to restart command line");

        }
    }
}
