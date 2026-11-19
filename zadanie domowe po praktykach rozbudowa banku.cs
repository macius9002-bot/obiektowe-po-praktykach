
using System;
using System.Collections.Generic;

class HistoriaOperacji
{
    private List<string> operacje = new List<string>();

    public void DodajOperacje(string opis)
    {
        operacje.Add(opis);
    }

    public void PokazHistorie()
    {
        foreach (string operacja in operacje)
        {
            Console.WriteLine(operacja);
        }
    }
}

class KontoBankowe
{
    private string wlasciciel;
    private string numerKonta;
    protected decimal saldo;
    protected HistoriaOperacji historia;

    public KontoBankowe(string wlasciciel, string numerKonta, decimal saldoPoczatkowe)
    {
        this.wlasciciel = wlasciciel;
        this.numerKonta = numerKonta;
        saldo = saldoPoczatkowe >= 0 ? saldoPoczatkowe : 0;
        historia = new HistoriaOperacji();
    }

    public string PobierzNumerKonta()
    {
        return numerKonta;
    }

    public decimal PobierzSaldo()
    {
        return saldo;
    }

    public virtual void PokazTyp()
    {
        Console.WriteLine("Konto bankowe");
    }

    public virtual void PokazDane()
    {
        Console.WriteLine($"Właściciel: {wlasciciel} | Numer konta: {numerKonta} | Saldo: {saldo} zł");
        PokazTyp();
    }

    public void PokazSaldo()
    {
        Console.WriteLine($"{wlasciciel}: {saldo} zł");
    }

    public void Wplac(decimal kwota)
    {
        if (kwota <= 0)
        {
            Console.WriteLine("Nieprawidłowa kwota");
            return;
        }

        saldo += kwota;
        historia.DodajOperacje($"Wpłata: +{kwota} zł");
        Console.WriteLine($"Wpłacono: {kwota} zł");
    }

    public void Wyplac(decimal kwota)
    {
        if (kwota <= 0 || kwota > saldo)
        {
            Console.WriteLine("Nie można wypłacić tej kwoty");
            return;
        }

        saldo -= kwota;
        historia.DodajOperacje($"Wypłata: -{kwota} zł");
        Console.WriteLine($"Wypłacono: {kwota} zł");
    }

    public void Przelew(KontoBankowe odbiorca, decimal kwota)
    {
        if (odbiorca == null || odbiorca == this || kwota <= 0 || kwota > saldo)
        {
            Console.WriteLine("Nie można wykonać przelewu");
            return;
        }

        saldo -= kwota;
        odbiorca.saldo += kwota;

        historia.DodajOperacje($"Przelew wychodzący: -{kwota} zł do {odbiorca.numerKonta}");
        odbiorca.historia.DodajOperacje($"Przelew przychodzący: +{kwota} zł od {numerKonta}");

        Console.WriteLine($"Wykonano przelew: {kwota} zł");
    }

    public void PokazHistorie()
    {
        historia.PokazHistorie();
    }
}

class KontoOszczednosciowe : KontoBankowe
{
    private decimal oprocentowanie;

    public KontoOszczednosciowe(string wlasciciel, string numerKonta, decimal saldoPoczatkowe, decimal oprocentowanie)
        : base(wlasciciel, numerKonta, saldoPoczatkowe)
    {
        this.oprocentowanie = oprocentowanie >= 0 ? oprocentowanie : 0;
    }

    public override void PokazTyp()
    {
        Console.WriteLine("Konto oszczędnościowe");
    }

    public void NaliczOdsetki()
    {
        decimal odsetki = saldo * oprocentowanie / 100;
        saldo += odsetki;

        historia.DodajOperacje($"Naliczono odsetki: +{odsetki} zł");
        Console.WriteLine($"Naliczono odsetki: {odsetki} zł");
    }
}

class KontoFirmowe : KontoBankowe
{
    private string nazwaFirmy;
    private string nip;

    public KontoFirmowe(string wlasciciel, string numerKonta, decimal saldoPoczatkowe, string nazwaFirmy, string nip)
        : base(wlasciciel, numerKonta, saldoPoczatkowe)
    {
        this.nazwaFirmy = nazwaFirmy;
        this.nip = nip;
    }

    public override void PokazTyp()
    {
        Console.WriteLine("Konto firmowe");
    }

    public void PokazDaneFirmy()
    {
        Console.WriteLine($"Firma: {nazwaFirmy} | NIP: {nip}");
    }

    public void PobierzOplate(decimal kwota)
    {
        if (kwota <= 0 || kwota > saldo)
        {
            Console.WriteLine("Nie można pobrać opłaty");
            return;
        }

        saldo -= kwota;
        historia.DodajOperacje($"Pobrana opłata: -{kwota} zł");
        Console.WriteLine($"Pobrano opłatę: {kwota} zł");
    }
}

class Program
{
    static void Main()
    {
        KontoBankowe konto1 = new KontoBankowe("Jan", "001", 1000);
        KontoBankowe konto2 = new KontoBankowe("Anna", "002", 2000);
        KontoBankowe konto3 = new KontoBankowe("Piotr", "003", 500);

        KontoOszczednosciowe konto4 = new KontoOszczednosciowe("Kasia", "004", 3000, 5);

        KontoFirmowe konto5 = new KontoFirmowe("Marek", "005", 5000, "ABC Firma", "1234567890");

        KontoBankowe[] konta = { konto1, konto2, konto3, konto4, konto5 };

        Console.WriteLine("=== DANE KONT ===");

        foreach (KontoBankowe konto in konta)
        {
            konto.PokazDane();
            Console.WriteLine();
        }

        Console.WriteLine("=== WPŁATY ===");

        konto1.Wplac(500);
        konto2.Wplac(200);
        konto3.Wplac(100);

        Console.WriteLine();

        Console.WriteLine("=== WYPŁATY ===");

        konto1.Wyplac(200);
        konto2.Wyplac(300);

        Console.WriteLine();

        Console.WriteLine("=== PRZELEWY ===");

        konto1.Przelew(konto3, 100);
        konto2.Przelew(konto4, 250);

        Console.WriteLine();

        Console.WriteLine("=== ODSETKI ===");

        konto4.NaliczOdsetki();

        Console.WriteLine();

        Console.WriteLine("=== OPŁATA FIRMOWA ===");

        konto5.PokazDaneFirmy();
        konto5.PobierzOplate(100);

        Console.WriteLine();

        Console.WriteLine("=== HISTORIA KONTA 1 ===");

        konto1.PokazHistorie();

        Console.WriteLine();

        Console.WriteLine("=== HISTORIA KONTA 4 ===");

        konto4.PokazHistorie();
    }
}
