using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Unicode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubFileSync
{
    internal class Program
    {

        // static string auther { get;  set; }
        static object args1 { get; set; }
        static object args2 { get; set; }
        static object author { get; set; }
        static object repo { get; set; }
        static string url { get; set; }
        static string AssetsName { get; set; }
        static string ActualDownloadURL { get; set; }
        static object obj { get; set; }
        static object JsonAssets { get; set; }
        public static string str { get; set; }
        public static string path { get; private set; }

        public static void Main(string[] args)
        {
            //Console.WriteLine(args.GetValue(0));
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            try {
                Program.args1 = args.GetValue(0);
                Program.args2 = args.GetValue(1);
                Console.WriteLine(args1);
                Console.WriteLine(args2);
            }
            catch(Exception ex)
            {
                Console.WriteLine("在启动时您并未指定作者或Github Repo");
                Console.WriteLine($"出现错误 Unhandled exception. System.IndexOutOfRangeException: {ex.Message}");
            }
            
            Console.WriteLine("获取目标Github仓库Release状态");
            Console.WriteLine("例如：https://github.com/[Author]/[Repository]");
            if(args1 == null || args2 == null)
            {
                Console.Write("请输入Github Author：");
                var author = Console.ReadLine();
                Program.author = author;
                Console.Write("请输入Github Repository：");
                var repo = Console.ReadLine();
                Program.repo = repo;
                /*if(author == null && repo == null)
                {
                    Console.WriteLine("Unhandled exception.");
                    Process.GetCurrentProcess().Kill();
                }
                else if (author == null)
                {
                    Console.WriteLine("Unhandled exception.");
                    Process.GetCurrentProcess().Kill();
                }
                else if (repo == null)
                {
                    Console.WriteLine("Unhandled exception.");
                    Process.GetCurrentProcess().Kill();
                }*/
                    url = $"https://api.github.com/repos/{author}/{repo}/releases/latest";
            }
            else
            {
                var author = args1;
                var repo = args2;
                url = $"https://api.github.com/repos/{author}/{repo}/releases/latest";
            }
            //Console.Write("请输入Github Author：");
            //var author = Console.ReadLine();
            //Console.Write("请输入Github Repository：");
            //var repository = Console.ReadLine();
            Program.path = AppDomain.CurrentDomain.BaseDirectory;
            var fileName = "latest.tmp";
            if(File.Exists($"{path}{fileName}"))
            {
                File.Delete($"{path}{fileName}");
            }
            //var url = "https://api.github.com/repos/OlivOS-Team/OlivaDiceCore/releases/latest";
            var FetchAPIStatus = Download2Path(url, path, fileName);
            //var FetchAPIStatus = true;
            if (FetchAPIStatus == true && File.Exists(fileName))
            {
                ReadJsonFile(path, fileName);
            }

        }


        /// <summary>
        /// 下载Api数据到指定路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Download2Path(string url, string filePath, string fileName)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // 超时时间
                    // httpClient.Timeout
                    // httpClient.DefaultRequestHeaders.Add("", "");

                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36 Edg/109.0.1518.78");

                    var response = httpClient.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // 写入文件
                        var fileStream = response.Content.ReadAsStreamAsync().Result;
                        var info = new DirectoryInfo(filePath);
                        if (!info.Exists)
                        {
                            info.Create();
                        }

                        var path = Path.Combine(filePath, fileName);

                        using (var outputFileStream = new FileStream(path, FileMode.Create))
                        {
                            fileStream.CopyTo(outputFileStream);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public static bool ReadJsonFile(string filePath, string fileName)
        {
            
            //Console.WriteLine($"{"名称",-46}    下载链接");
            //obj["assets"].ToList().ForEach.["name"];
            if (File.Exists(filePath + fileName))
            {
                Program.str = File.ReadAllText($"{filePath}{fileName}");
            }
            else
            {
                Console.WriteLine("Unhandled exception.");
                Process.GetCurrentProcess().Kill();
            }
                string str = Program.str;
            if (str == null)
            {
                Console.WriteLine("Unhandled exception.");
                Process.GetCurrentProcess().Kill();
            }
                var obj = JObject.Parse(str);
                var JsonAssets = obj["assets"];

            if (!Directory.Exists($"{path}{author}"))
            {
                Directory.CreateDirectory($"{path}{author}");
            }
            if (!Directory.Exists($"{path}{author}\\{repo}"))
            {
                Directory.CreateDirectory($"{path}{author}\\{repo}");
            }
            //JArray json2 = (JArray)JsonConvert.DeserializeObject(obj2);
            var SavePath = $"{path}{author}\\{repo}\\";
            try
            {
                var k = 0;
                var i = 0;
                var JsonAssetsTest = JsonAssets[i]["url"];
                //Console.WriteLine(JsonAssetsTest);
                for (i = 0;k == 0;i++)
                {
                    try
                    {
                        Program.AssetsName = JsonAssets[i]["name"].ToString();
                        Program.ActualDownloadURL = JsonAssets[i]["browser_download_url"].ToString();
                        Console.WriteLine($"准备下载{ActualDownloadURL}");
                        Download2Path(ActualDownloadURL, SavePath, AssetsName);

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Debug: "+e.Message);
                        k--;
                        break;
                    }
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                var releaseTag = obj["tag_name"];
                Program.ActualDownloadURL = $"https://github.com/{author}/{repo}/archive/refs/tags/{releaseTag}.zip";
                Console.WriteLine($"准备下载{ActualDownloadURL}");
                Download2Path(ActualDownloadURL, $"{path}{author}\\{repo}\\", $"{releaseTag}.zip");
            }

            
            
            
            
            /*obj["assets"].ToList().ForEach(t =>
            {
                var name = t["name"].ToString();
                var url = t["browser_download_url"].ToString();

            });*/
            return true;
        }
    }
}