using System;
using System.Timers;
using Vision20.Commons.NodeManagement;

/*
Tu napisz co węzeł robi i jak mu ustawić piny (ile, jaki typ)

Ten przykładowy węzeł ma dwie główne funkcje: odliczanie w dół od podanej wartości i sumowanie dwóch składników.
Zaprojektowany do podawania datakwantów typu Num
Po podaniu wartości na IN1, zaczyna w osobnym wątku odliczać od niej do zera i wystawiać te liczby na OUT1
Po dostarczeniu składników na IN2 i IN3, wyprowadza na OUT2 ich sumę. Obliczenia są uruchamiane podaniem drugiego składnika - na IN3
*/

public class VisionDynamic : CodedNodeBase
{
    private double _argOne;
    private double _argTwo;
    private int _countDown;
    private Timer _countDownTicker;

    public override void Consume(string inputName, object parameter)
    {
        //To jest starsza metoda, nie wskazująca na pochodzenie datakwantu
        //Jeżeli to nie jest potrzebne, to można ją stosować
        //Musi być pokryta, przynajmniej na razie, więc nie należy jej usuwać
    }

    public override void Consume(CodedQuantConsumeData consumeData, object parameter)
    {
        switch (consumeData.DestinationName)
        {
            case "IN1": //uruchomienie odliczania w dół
                {
                    _countDown = Convert.ToInt32(parameter);
                    if (!_countDownTicker.Enabled)
                    {
                        _countDownTicker.Start();
                    }
                }

                break;

            case "IN2": //przyjęcie pierwszego składnika sumowania
                {
                    _argOne = Convert.ToDouble(parameter);
                }

                break;

            case "IN3": //przyjęcie drugiego składnika i przeprowadzenie operacji sumowania
                {
                    _argTwo = Convert.ToDouble(parameter);
                    OnProduce("OUT2", (_argOne + _argTwo));
                    WriteLog($"Operation Log from {Environment.NodeName}");
                }

                break;
        }
    }

    public override void Init()
    {
        //tutaj można przygotować zasoby potrzebne już w trakcie działania węzła
        // WriteLog("Wywołano Init() na węźle");
        _countDown = 0;
        _argOne = 0;
        _argTwo = 0;
        _countDownTicker = new Timer
        {
            Interval = 200
        };
        _countDownTicker.Elapsed += DqGenerator;
    }

    protected override void Cleanup()
    {
        //tutaj poczyść zasoby, jeżeli jakiś używałeś
        //  WriteLog("Wywołano Cleanup() na węźle");
        _countDownTicker.Stop();
        _countDownTicker.Dispose();
    }

    private void DqGenerator(object oSource, ElapsedEventArgs args)
    {
        OnProduce("OUT1", _countDown--);
        WriteLog("Countdown tick");

        if (_countDown == 0)
        {
            _countDownTicker.Enabled = false;
        }
    }
}