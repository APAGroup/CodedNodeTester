using System;
using System.Reflection;
using Vision20.Commons.NodeManagement;

namespace NazcaMock
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("MOCK Systemu Nazca - użycie wezłów dynamicznie ładowanych");
            Console.WriteLine("Ładowanie CodedNoda...");
            //Przygotowanie instancji węzła
            var cnc = Assembly.LoadFrom(@"..\\..\\..\\CodedNode\\bin\\Debug\\CodedNode.dll");
            var demo = PrepareCodedNode(cnc, 3, 1);

            //Tu zaczyna się testowanie węzła
            Console.WriteLine("Start odliczania w dół od 10");
            demo?.Consume(new CodedQuantConsumeData("IN1", CodedDataQuantSource.Link), 10d);

            Console.WriteLine("Wysłano wartosc 23 do sumowania");
            demo?.Consume(new CodedQuantConsumeData("IN2", CodedDataQuantSource.Link), 23d);
            Console.WriteLine("Wysłano wartosc 33 jako drugi argument");
            demo?.Consume(new CodedQuantConsumeData("IN3", CodedDataQuantSource.Link), 33d);

            Console.ReadKey();
        }

        private static CodedNodeBase PrepareCodedNode(Assembly codedNodeAssembly, int inpPins, int outPins)
        {
            var testNode = codedNodeAssembly.CreateInstance("VisionDynamic", false);
            if (!(testNode is CodedNodeBase demoNode)) return null;

            demoNode.ValueProduced += delegate (object o, CodedNodeProductEventArgs arg)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Otrzymano wartość {arg.Value} na pinie {arg.PinName}");
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
                Flavor = "NUM"
            };
            demoNode.Init();

            return demoNode;
        }
    }
}