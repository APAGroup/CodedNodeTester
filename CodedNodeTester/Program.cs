using System;
using System.Reflection;
using Vision20.Commons.NodeManagement;

namespace CodedNodeTester
{
    internal class Program
    {
        public void OnProduce(object o, CodedNodeProductEventArgs arg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{arg.Value} (on {arg.PinName})");
            Console.ResetColor();
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Ładowanie assembly...");
            var cnc = Assembly.LoadFrom(@"..\..\..\CodedNode\bin\debug\CodedNode.dll");
            //Przed uruchomieniem tego porojektu NIE ZAPOMNIJ skompilować ręcznie CodedNode,
            //bo wiązania między projektami są całkowicie dynamiczne i VS tego za Ciebie nie zrobi

            //Tu zaczyna się testowanie węzła
            Console.WriteLine("Test węzła dynamicznie ładowanego...");
            var demo = PrepareCodedNode(cnc, 1, 1);
            demo?.Consume(new CodedQuantConsumeData("IN1", CodedDataQuantSource.Link), "23");
            demo?.Consume(new CodedQuantConsumeData("IN1", CodedDataQuantSource.Link), "to jest tester");

            Console.ReadKey();
        }

        private static CodedNodeBase PrepareCodedNode(Assembly codedNodeAssembly, int inpPins, int outPins)
        {
            var testNode = codedNodeAssembly.CreateInstance("VisionDynamic", false);
            if (!(testNode is CodedNodeBase demoNode)) return null;

            demoNode.ValueProduced += delegate (object o, CodedNodeProductEventArgs arg)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{arg.Value} (on {arg.PinName})");
                    Console.ResetColor();
                };

            demoNode.LogProduced += delegate (object o, LogEventArgs arg)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Log: {arg.LogText}");
                    Console.ResetColor();
                };

            demoNode.Environment = new CodedNodeEnvironment
            {
                InputCount = inpPins,
                OutputCount = outPins,
                TraceId = "[CodedNode: NodeUnderConstruction]",
                NodeName = "Node name",
                Flavor = "TEXT"
            };
            demoNode.Init();

            return demoNode;
        }
    }
}