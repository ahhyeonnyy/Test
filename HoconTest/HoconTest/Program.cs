using Akka.Configuration;
using Akka.Configuration.Hocon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HoconTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            var hocon = @"akka.cluster
{
    collector
    {
        sample-interval = ""test hocon""
        gossip-interval = ""1234""
        service = ""SERVICE""
        user = ""user""
        password = ""0000""
    }

    periodic-tasks-initial-delay = 5s

    moving-average-half-life = 100

    dispatcher = 
        [
            ""akka.tcp://mls-cluster@123456789""
        ]
}";
            var config = ConfigurationFactory.ParseString(hocon);
            var hoconPath = "C:\\Workspace\\Test\\test.hocon";

            var logLevel = "DEBUG";
            config = config.WithFallback(ConfigurationFactory.ParseString($"akka.cluster = {logLevel}"));

            // 변경된 설정 저장
            var updatedConfigText = config.ToString(true);

            // JSON 형식을 원하는 형식으로 변환
            updatedConfigText = ConvertToJsonLike(updatedConfigText);

            // 변환된 설정 저장
            File.WriteAllText(hoconPath, updatedConfigText);

        }
        private static string ConvertToJsonLike(string input)
        {
            // 콜론(:)을 등호(=)로 변경하고 공백을 제거합니다.
            input = Regex.Replace(input, @"\s*:\s*(?=[^{])", " = ");

            // 줄 바꿈 처리
            string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // 중괄호를 줄 바꿈으로 처리하고 공백을 추가합니다.
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                if (lines[i].EndsWith("{"))
                {
                    lines[i] = lines[i].Substring(0, lines[i].Length - 1).Trim() + "\n{";
                }
                else if (lines[i].EndsWith("}"))
                {
                    lines[i] = "}\n" + lines[i].Substring(1).Trim();
                }
            }

            return string.Join("\n", lines);
        }
    }
}