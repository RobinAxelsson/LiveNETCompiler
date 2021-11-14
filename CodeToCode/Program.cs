using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace CodeToCode
{
    class Program
    {
        static string TextCode = @"C:\Users\axels\Google Drive\Code\VStudio_source\repos\CodeToCode\CodeToCode\TextCode.txt";
        static string ConsoleCode = @"C:\Users\axels\Google Drive\Code\VStudio_source\repos\CodeToCode\CodeToCode\ConsoleCode.txt";
        static Action<string> Write = Console.WriteLine;

        static void Main(string[] args)
        {
            //string codeToCompile;            

            //while (true)
            //{
            //    codeToCompile = File.ReadAllText(TextCode);
            //    Console.WriteLine("Compiling File...");
            //    CompileToRoslynDLL(codeToCompile, "Write");
            //    Console.WriteLine("Change code:");
            //    Recode(Console.ReadLine());
            //    Console.WriteLine();
            //}
            CompileToRoslynConsoleApp(File.ReadAllText(ConsoleCode));
        }
        public static void CompileToRoslynConsoleApp(string codeToCompile)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            string assemblyName = Path.GetRandomFileName();
            string ref1 = typeof(System.Object).GetTypeInfo().Assembly.Location;
            string ref2 = typeof(Console).GetTypeInfo().Assembly.Location;
            string ref3 = Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll");

            var refPaths = new[] { ref1, ref2, ref3 };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                //options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Write("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    Write("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

                    //var type = assembly.GetType("CodeToCode.Writer");
                    //var instance = assembly.CreateInstance("CodeToCode.Writer");
                    //var methodInfo = type.GetMember("Main").First() as MethodInfo;
                    //methodInfo.Invoke(instance, parameters);
                }
            }
        }
        public static void CompileToRoslynDLL(string codeToCompile, string method, params string[] parameters)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
            
            string assemblyName = Path.GetRandomFileName();
            string ref1 = typeof(System.Object).GetTypeInfo().Assembly.Location;
            string ref2 = typeof(Console).GetTypeInfo().Assembly.Location;
            string ref3 = Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll");

            var refPaths = new[] { ref1, ref2, ref3 };
            MetadataReference[] references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                //options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Write("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    Write("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);
                    
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType("CodeToCode.Writer");
                    var instance = assembly.CreateInstance("CodeToCode.Writer");
                    var methodInfo = type.GetMember(method).First() as MethodInfo;
                    methodInfo.Invoke(instance, parameters);
                }
            }
        }
        public static void Recode(string newString)
        {
            string filePath = TextCode;
            string code = File.ReadAllText(filePath);
            int start = code.IndexOf('"');
            int end = code.LastIndexOf('"');
            int length = end - start - 1;

            if (length == 0)
            {
            }
            else
            {
                code = code.Remove(start + 1, length);                
            }
            code = code.Insert(start + 1, newString);
            File.WriteAllText(filePath, code);
        }
    }
}
