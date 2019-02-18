using System;
using CodedNode;
using Vision20.Commons.NodeManagement;

/*
Tu napisz co węzeł robi i jak mu ustawić piny (ile, jaki typ)
*/

public class VisionDynamic : CodedNodeBase
{
    public override void Consume(string inputName, object parameter)
    {
        //To jest starsza metoda, nie wskazująca na pochodzenie datakwantu
        //Jeżeli to nie potrzebne, to można ją stosować
        //Musi być pokryta, przynajmniej na razie
    }

    public override void Consume(CodedQuantConsumeData consumeData, object parameter)
    {
        switch (consumeData.DestinationName)
        {
            case "IN1":
                {
                    var prod = Convert.ToString(parameter); //na początku nie zapomnij konwertować z object
                    var output = Helper.GetHelperInfo();
                    output += String.Format(", to jest wprowadzone na pin: {0}...", prod);
                    OnProduce("OUT1", output); //tego używaj do wysłania datakwantu z węzła na zewnątrz
                    WriteLog("done...");
                }

                break;

            default:
                {
                }
                break;
        }
    }

    public override void Init()
    {
        //tutaj można przygotować zasoby potrzebne już w trakcie działania węzła
        WriteLog("Wywołano Init() na węźle");
    }

    protected override void Cleanup()
    {
        //tutaj poczyść zasoby, jeżeli jakiś używałeś
        WriteLog("Wywołano Cleanup() na węźle");

    }
}