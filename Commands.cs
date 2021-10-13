using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HappyFunTimes.FileDL;
using HappyFunTimes.BDO;
using HappyFunTimes;
using System.Collections.Generic;
using C5;
using System.Text;
using System.IO;
using System.Threading;

namespace Commands
{
    [Group("bns")]
    public class bnsCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Ping", RunMode = RunMode.Async)]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

    }
    [Group("bdo")]
    public class bdoCommands : ModuleBase<SocketCommandContext>
    {
        [Command("meal", RunMode = RunMode.Async)]
        public async Task meal()
        {
            BDOapi api = new BDOapi("A2", "F");
            IPriorityQueue<MealNode> meal = api.CheapestImperialMeals();
            var sb = new StringBuilder();
            sb.Append(">>> ");
            sb.AppendLine("**Meal — Cost To Package**");
            MealNode max = meal.FindMax();
            MealNode currentNode = meal.FindMin();
            while (max.packageCost > currentNode.packageCost)
            {
                currentNode = meal.DeleteMin();
                sb.AppendLine(currentNode.meal + " — " + String.Format("{0:n0}", currentNode.packageCost));
            }
            await ReplyAsync(sb.ToString());
        }
    }
    [Group("fun")]
    public class funCommands : ModuleBase<SocketCommandContext>
    {
        [Command("dl")]
        public Task downloadSong(string link)
        {   
            YoutubeDL test = new YoutubeDL();
            test.download(link);
            ReplyAsync("Downloading...");
            Thread.Sleep(3000); //temp fix. need to wait for the file to finish downloading before the file checks are done
            DirectoryInfo di = new DirectoryInfo(@"..\..\FileDL\DLDestination");
            FileInfo[] fi = di.GetFiles();
            string name = string.Empty;
            foreach (FileInfo fiTemp in fi)
            {
                if (fiTemp.Name != "youtube-dl.exe")
                {
                    name = fiTemp.Name;
                }
            }
            string file = "../../FileDL/DLDestination/" + name;
            Context.Channel.SendFileAsync(file, "done");
            Thread.Sleep(7000); // also a temp fix
            
            if(File.Exists(@file)) {
                File.Delete(@file);
            }
            
            return ReplyAsync("");
        }
    }
}

