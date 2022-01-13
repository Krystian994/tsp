using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace TSP
{
    class Program
    {                             // główne ustawienia programu
        static int ile_os = 10;   // liczba osobników (tras)
        static int czas = 10;      // czas w sekundach ile ma się wykonywać program
        
        static void Main(string[] args)
        {
            odczyt144();        // wywołanie funkcji dla pliku pr144
            odczyt127();        // wywołanie funkcji dla pliku bier127
        }

        static void plik(int[] lp, int[] x, int[] y, string typpliku)
        {
            List<string> lines = File.ReadAllLines(typpliku + ".tsp").ToList(); // typpliku nazwa pliku
            bool czyliczby = false;
            int i = 0;

            foreach (string line in lines)
            {
                if (line == "NODE_COORD_SECTION")
                {
                    czyliczby = true;
                    continue;
                }

                if (line == "EOF")
                {
                    czyliczby = false;
                }

                if (czyliczby == true)
                {
                    string[] tmp = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    lp[i] = int.Parse(tmp[0]);
                    x[i] = int.Parse(tmp[1]);
                    y[i] = int.Parse(tmp[2]);
                    i++;
                }
            }
        }

        static void macierz(int[] x, int[] y, int[,] matrix)
        {
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < y.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = 0;
                    }
                    else
                    {
                        matrix[i, j] = (Math.Abs(x[i] - x[j]) + Math.Abs(y[i] - y[j]));
                    }
                }
            }
        }

        static void losowanie(int[] lp, int[,] per)
        {
            List<int> liczby = new List<int>();
            Random rand = new Random();
            int liczba;

            for (int i = 0; i < ile_os; i++)
            {
                liczby.Clear();
                for (int j = 0; j < lp.Length; j++)
                {
                    do
                    {
                        liczba = rand.Next(0, lp.Length);

                    } while (liczby.Contains(liczba));
                    per[i, j] = liczba;
                    liczby.Add(liczba);
                }
            }
        }

        static void krzyzowanie(int[,] per, int[] lp)
        {
            List<int> pary = new List<int>();
            List<int> punkty = new List<int>();
            Random rand = new Random();

            int[] temp = new int[lp.Length];
            int temposoba;

            int rodzicjeden;
            int rodzicdwa;

            int punktpierwszy;
            int punktdrugi;
            int liczmiejsce;

            int dzieci = 10;

            pary.Clear();

            for (int i = 0; i < 5; i++)
            {
                punkty.Clear();
                liczmiejsce = 0;

                do
                {
                    rodzicjeden = rand.Next(10);

                } while (pary.Contains(rodzicjeden));

                pary.Add(rodzicjeden);

                do
                {
                    rodzicdwa = rand.Next(10);
                } while (pary.Contains(rodzicdwa));

                pary.Add(rodzicdwa);

                punktpierwszy = 0;
                punktdrugi = rand.Next(60, 90);

                for (int j = punktpierwszy; j < punktdrugi; j++)
                {
                    temp[liczmiejsce] = per[rodzicjeden, j];
                    punkty.Add(temp[liczmiejsce]);
                    liczmiejsce++;
                }

                for (int k = liczmiejsce; k < lp.Length; k++) // sprawdzanie czy dany punkt już nie wystąpił
                {
                    for (int l = 0; l < lp.Length; l++)
                    {
                        temposoba = per[rodzicdwa, l];
                        if (!punkty.Contains(temposoba))
                        {
                            temp[k] = temposoba;
                            punkty.Add(temposoba);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                for (int m = 0; m < lp.Length; m++)
                {
                    per[dzieci, m] = temp[m];
                }
                dzieci++;
            }
        }
        static void mutacja(int[,] per, int[] lp)
        {
            List<int> wybrane = new List<int>();
            Random rand = new Random();

            int x;
            int y;
            int tempx;
            int tempy;

            for (int i = 0; i < 15; i++)
            {
                wybrane.Clear();
                double czymutacja = rand.NextDouble();
                if (czymutacja <= 0.15)
                {
                    x = rand.Next(0, lp.Length);
                    wybrane.Add(x);
                    do
                    {
                        y = rand.Next(0, lp.Length);
                    } while (wybrane.Contains(y));

                    tempx = per[i, x];
                    tempy = per[i, y];

                    per[i, x] = tempy;
                    per[i, y] = tempx;
                }
            }
        }
        static void selekcja(int[,] per, int[,] matrix, int[] lp, int[] wyniki)
        {
            int suma;
            int[] temp = new int[lp.Length];

            for (int i = 0; i < wyniki.Length; i++)
            {
                suma = 0;
                for (int j = 0; j < lp.Length - 1; j++)
                {
                    int x = per[i, j];
                    int y = per[i, j + 1];
                    suma += matrix[x, y];
                }
                suma += matrix[per[i, lp.Length-1], per[i, 0]];
                wyniki[i] = suma;
            }


            for (int n = 0; n < wyniki.Length - 1; n++)
            {
                if (wyniki[n] > wyniki[n + 1])
                {
                    int tempwynik = wyniki[n];
                    wyniki[n] = wyniki[n + 1];
                    wyniki[n + 1] = tempwynik;

                    for (int m = 0; m < temp.Length; m++)
                    {
                        temp[m] = per[n, m];
                        per[n, m] = per[n + 1, m];
                        per[n + 1, m] = temp[m];
                    }
                    n = -1;
                }
            }

            for (int a = 10; a < wyniki.Length; a++)
            {
                wyniki[a] = 0;
                for (int b = 0; b < lp.Length; b++)
                {
                    per[a, b] = 0;
                }
            }
        }

        static void zapis(int[,] per, int[] wyniki,int[] lp, string typpliku)
        {
            using (StreamWriter sw = File.AppendText("wynikiKSzewczyk"+ typpliku + ".txt"))
            {
                for (int i = 0; i < lp.Length; i++)
                {
                    sw.Write(per[0, i] + 1 + " ");
                }
                sw.Write(wyniki[0]);
                sw.WriteLine();
            }
        }

        static void odczyt144()
        {
            int[] lp = new int[144];
            int[] x = new int[144];
            int[] y = new int[144];
            int[] wyniki = new int[15];

            int[,] matrix = new int[144, 144];
            int[,] per = new int[15, 144];

            string typpliku = "pr144";

            TimeSpan lczas = new TimeSpan(0, czas, 0);

            plik(lp, x, y, typpliku);
            macierz(x, y, matrix);
            for(int n = 0; n < ile_os; n++) 
            { 
                losowanie(lp, per);
                Stopwatch licz = new Stopwatch();
                licz.Start();
                while (licz.Elapsed < lczas)
                    {
                        krzyzowanie(per, lp);
                        mutacja(per, lp);
                        selekcja(per, matrix, lp, wyniki);
                    }

            zapis(per, wyniki, lp, typpliku);
            }
        }
        static void odczyt127()
        {
            int[] lp = new int[127];
            int[] x = new int[127];
            int[] y = new int[127];
            int[] wyniki = new int[15];

            int[,] matrix = new int[127, 127];
            int[,] per = new int[15, 127];

            string typpliku = "bier127";

            TimeSpan lczas = new TimeSpan(0, czas, 0);

            plik(lp, x, y, typpliku);
            macierz(x, y, matrix);
            for (int n = 0; n < ile_os; n++)
            {
                losowanie(lp, per);

                Stopwatch licz = new Stopwatch();
                licz.Start();
                while (licz.Elapsed < lczas )
                {
                    krzyzowanie(per, lp);
                    mutacja(per, lp);
                    selekcja(per, matrix, lp, wyniki);
                }

                zapis(per, wyniki, lp, typpliku);
            }
        }
    }
}
