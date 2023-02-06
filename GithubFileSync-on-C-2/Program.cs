using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubFileSync
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("获取目标Github仓库Release状态");
            Console.WriteLine("例如：https://github.com/[Author]/[Repository]");
            Console.Write("请输入Github Author：");
            var author = Console.ReadLine();
            Console.Write("请输入Github Repository：");
            var repository = Console.ReadLine();
            var url = $"https://api.github.com/repos/{author}/{repository}/releases/latest";
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var fileName = "latest.tmp";

            //var url = "https://api.github.com/repos/OlivOS-Team/OlivaDiceCore/releases/latest";
            var FetchAPIStatus = Download2Path(url, path, fileName);
            
            if(FetchAPIStatus == true && File.Exists(fileName))
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
            var str = File.ReadAllText($"{filePath}{fileName}");
            var obj = JObject.Parse(str);
            //Console.WriteLine($"{"名称",-46}    下载链接");
            //obj["assets"].ToList().ForEach.["name"];
            obj["assets"].ToList().ForEach(t =>
            {
                var name = t["name"].ToString();
                var url = t["browser_download_url"].ToString();

            });
            return true;
        }
    }
}