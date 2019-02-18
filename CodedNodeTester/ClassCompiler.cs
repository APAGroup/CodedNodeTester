using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Vision20.Commons.NodeManagement;

namespace CodedNodeTester
{
    public static class ClassCompiler
    {
        private const string CodesDirectory = "Codes";
        private const string CompiledDirectory = "Compiled";
        private const string AssemblyInfoCode = "using System.Reflection;\n[assembly: AssemblyVersion(\"{0}\")]\n[assembly: AssemblyFileVersion(\"{0}\")]\n\n";

        public static string[] SetAssemblyVersionInCode(string code)
        {
            var version = Assembly.GetAssembly(typeof(CodedNodeBase)).GetName().Version;

            var codes = new string[2];
            codes[0] = code;
            codes[1] = string.Format(AssemblyInfoCode, version);
            return codes;
        }

        private static bool CheckParams(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "-c")
                {
                    return true;
                }
            }
            else
            {
                if (args.Length == 2)
                {
                    var input = (args[1]);
                    if (input.Length > 0)
                    {
                        return true;
                    }
                }
            }
            PrintRequeiredInput();
            return false;
        }

        public static bool MakeCompile(string[] args)
        {
            if (args.Length > 0)
            {
                if (CheckParams(args))
                {
                    if (args.Length == 1)
                    {
                        Compile();
                    }
                    else
                    {
                        Compile(args[1]);
                    }

                    Console.ResetColor();
                }
                return true;
            }
            return false;
        }

        private static void Compile()
        {
            if (!Directory.Exists(CodesDirectory))
            {
                return;
            }
            var files = Directory.GetFiles(CodesDirectory).Select(Path.GetFileNameWithoutExtension);
            foreach (var file in files)
            {
                Compile(file);
            }
        }

        private static void Compile(string inputFile)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Procedura generowania biblioteki: {inputFile}...\n\n");
            Console.ForegroundColor = ConsoleColor.Red;
            try
            {
                if (!Directory.Exists(CompiledDirectory))
                {
                    Directory.CreateDirectory(CompiledDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nie można utworzyć katalogu {CompiledDirectory} dla kodu wynikowego. Powód: \n{ex}");
                return;
            }
            var compilerParameters = new CompilerParameters()
            {
                GenerateInMemory = false,
                GenerateExecutable = false,
                OutputAssembly = $"{CompiledDirectory}\\{inputFile}.dll",
            };
            var filePath = $"{CodesDirectory}\\{inputFile}.cs";
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Linq.dll");
            compilerParameters.ReferencedAssemblies.Add("Nazca.CodedNodeBase.dll");
            compilerParameters.ReferencedAssemblies.Add("Ical.Net.dll");
            compilerParameters.ReferencedAssemblies.Add("ical.net.collections.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Net.Http.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Xml.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            compilerParameters.ReferencedAssemblies.Add("System.ServiceModel.dll");
            try
            {
                var fileLines = File.ReadAllLines(filePath).Aggregate((a, b) => a + "\n" + b);
                var cSharpCodeProvider = new CSharpCodeProvider();
                var codes = SetAssemblyVersionInCode(fileLines);
                var results = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, codes);
                if (!results.Errors.HasErrors)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Kompilacja zakończyła się sukcesem. Plik wynikowy: {inputFile}.dll");
                    return;
                }
                var errors = results.Errors.OfType<CompilerError>().Select(a => "(" + a.Line + ") " + a.ErrorText);
                Console.WriteLine($"Wystąpiły błądy kompilacji: {errors.Aggregate((a, b) => a + "\n" + b)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex}");
            }
        }

        private static void PrintRequeiredInput()
        {
            Console.WriteLine("Nieprawidłowe parametry wejściowe.\n\n");
            Console.WriteLine("    W celu wygenerowania biblioteki należy użyć składni parametrów:\n");
            Console.WriteLine("    CodeNodeTester -o [nazwapliku]");
            Console.WriteLine("       gdzie [nazwapliku] - to nazwa pliku z napisaną klasą, np: ExampleCodedNode (bez rozszerzenia).");
            Console.WriteLine("                            Jeśli ten parametr nie zostanie podany, program skompiluje wszystkie pliki znajdujące się w Katalogu Coded.");
        }
    }
}