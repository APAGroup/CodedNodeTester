using System;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vision20.Commons.NodeManagement;

namespace CodedNodeTests
{
    [TestClass()]
    public class ExampleCodedNodeTest
    {
        public static CodedNodeBase CodedNodeInstance;

        [TestInitialize]
        public void AMethodToRunBeforeAllTest()
        {
            string NodePath = @"..\\..\\..\\CodedNode\\bin\\Debug\\CodedNode.dll";
            var hDynamicLib = Assembly.LoadFrom(NodePath);
            var hDynamicClass = hDynamicLib.CreateInstance("VisionDynamic", false);
            Assert.IsTrue(hDynamicClass is CodedNodeBase); // sprawdzenie, czy dynamiczna metoda załadowana z *.dll jest CodedNodeBase
            CodedNodeInstance = (CodedNodeBase)hDynamicClass;

            CodedNodeInstance.Environment = new CodedNodeEnvironment
            {
                InputCount = 3,
                OutputCount = 1,
                TraceId = "[CodedNode: NodeUnderConstruction]",
                NodeName = "ExampleCodedNode",
                Flavor = "NUM"
            };
            CodedNodeInstance.Init();
        }

        [TestCleanup]
        public void AMethodToRunOnEndOfAllTests()
        {
            CodedNodeInstance.Dispose();
        }

        [TestMethod]
        public void AsyncCountDownRunsProperlyTest()
        // Test węzła z odseparowanym asynchronicznym silnikiem
        // Asercja w wątku głównym na wynik operacji odbywającej
        // się w wątku timera wewnątrz silnika węzła CodedNode
        // Więcej wartościowych informacji: https://msdn.microsoft.com/en-us/magazine/dn818494.aspx
        {
            //Arrange
            int countDownCounter = 0;

            CodedNodeInstance.LogProduced += (object o, LogEventArgs arg) =>
            {
                Console.WriteLine(arg.LogText); //na potrzeby podglądu przebiegu testu
            };

            ManualResetEventSlim mre = new ManualResetEventSlim(false); //będzie używany do synchronizacji wątka testu i timera węzła
            CodedNodeInstance.ValueProduced += (object o, CodedNodeProductEventArgs arg) =>
            {
                if (arg.PinName == "OUT1")
                {
                    countDownCounter++; // Rejestrowanie pojedycznych odpowiedzi węzła na OUT1
                }
                if (countDownCounter == 5)
                {
                    mre.Set(); //Warunek wystarczający spełniony, odblokowanie
                }
            };

            //Act
            //Uruchomienie polecenia wygenerowania sekwencji pięciu wartości
            CodedNodeInstance.Consume(new CodedQuantConsumeData("IN1", CodedDataQuantSource.Link), 5d);

            //Assert
            //Oczekiwanie na spełnienie warunku maksymalnie przez zakładany czas (5*200ms)
            Assert.IsTrue(mre.Wait(1080), "Węzeł nie zrealizował sekwencji");
            //UWAGA - w ten sposób nie można sprawdzić, czy węzeł nie wygenerował nadmiarowych danych, tylko czy wykonał minimum
        }

        [TestMethod()]
        public void ProperLoggingTest()
        // Test sprawdza, czy silnik węzła generuje porządany log przy operacji sumowania
        // Jednocześnie jest sprawdzane przekazywanie Environment do jego silnika
        {
            //Arrange
            string logGenerated = null;
            CodedNodeInstance.LogProduced += (object o, LogEventArgs arg) =>
            {
                logGenerated = arg.LogText;
            };

            //Act
            CodedNodeInstance.Consume(new CodedQuantConsumeData("IN3", CodedDataQuantSource.Link), 0d);

            //Assert
            Assert.IsNotNull(logGenerated, "Węzeł nie wygenerował wpisu");
            Assert.AreEqual("[CodedNode: NodeUnderConstruction] Operation Log from ExampleCodedNode", logGenerated);
            //Taka nazwa węzła została pozadana w metodzie inicjującej testy
        }

        [DataTestMethod]
        [DataRow(2d, 2d, 4d)]
        [DataRow(34d, 54d, 88d)]
        [DataRow(-34d, 53d, 19d)]
        public void SumBatchedTest(double argOne, double argTwo, double desiredResult)
        // Test sprawdza funkcję sumowania węzła dla wielu argumentów i wyników jednocześnie
        {
            //Arrange + Assert
            CodedNodeInstance.ValueProduced += (object o, CodedNodeProductEventArgs arg) =>
            {
                if (arg.PinName == "OUT2")
                {
                    Assert.AreEqual(desiredResult, arg.Value, "Nieprawidłowy wynik");
                }
            };

            //Act
            CodedNodeInstance.Consume(new CodedQuantConsumeData("IN2", CodedDataQuantSource.Link), argOne);
            CodedNodeInstance.Consume(new CodedQuantConsumeData("IN3", CodedDataQuantSource.Link), argTwo);
        }

        [TestMethod()]
        public void SumSingleTest()
        // Test sprawdza funkcję sumowania węzła na jednym przypadku
        {
            //Arrange
            double? sumOutcome = null;
            CodedNodeInstance.ValueProduced += (object o, CodedNodeProductEventArgs arg) =>
            {
                if (arg.PinName == "OUT2")
                {
                    sumOutcome = (double)arg.Value;
                }
            };

            //Act
            CodedNodeInstance.Consume(new CodedQuantConsumeData("IN2", CodedDataQuantSource.Link), 5d);
            CodedNodeInstance.Consume(new CodedQuantConsumeData("IN3", CodedDataQuantSource.Link), 3d);

            //Assert
            Assert.IsNotNull(sumOutcome, "Operacja nie została wykonana");
            Assert.AreEqual(8d, sumOutcome, "Nieprawidłowy wynik");
        }
    }
}