using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;

//https://mikecodes.net/2020/05/11/in-app-scripts-with-c-roslyn/

namespace RoslynScripting
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Write some C#");
            var evaluate = true;

            while (evaluate)
            {
                var input = Console.ReadLine();

                if (input == "exit")
                {
                    evaluate = false;
                    break;
                }

                if (input == "clear")
                {
                    Console.Clear();
                    Console.WriteLine("Write some C#");
                    continue;
                }

                object result = await CSharpScript.EvaluateAsync(input);
                Console.WriteLine(result.ToString());
            }
        }
    }
}
