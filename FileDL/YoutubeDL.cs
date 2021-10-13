using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyFunTimes.FileDL
{
    class YoutubeDL
    {
        public void download(string song)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            p.StartInfo = info;
            p.Start();
            //it always starts in the debug folder
            using (StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("cd ../../FileDL/DLDestination");
                    //sw.WriteLine("youtube-dl " + song);
                    sw.WriteLine("youtube-dl.exe --audio-format mp3 https://www.youtube.com/watch?v=0iX735YRws0&list=RD0iX735YRws0&start_radio=1");
                }
            }
        }
    }
}
