using System;
using System.IO;
using System.Collections.Generic;

namespace Twist
{
    class Program
    {
        // Dieser Path muss an den Dateipfad der Wörterliste angepasst werden!
        const string NAME_OF_WORTSCHATZ_FILE = "C:/Users/Nils/source/repos/Twist/Twist/woerterliste.txt";

        static string[] wortschatz;
        static bool isTwistMode;
        static String[] eingabe;

        static void Main(string[] args)
        {
            // Den Wortschatz der Textdatei in ein Array laden 
            loadWortschatz();

            // Programm starten
            start();
        }

        /// <summary>
        /// Startet das Programm
        /// </summary>
        static void start()
        {
            // 2-Dimensionale Liste, falls es zu einem Wort mehrere enttwistete Wörter (Lösungen) gibt. So werden alle möglichen Lösungen ausgegeben
            var ergebnis = new List<String[]>();

            /* EINGABE */
            // Dem Benutzer den Modus (Wort twisten oder enttwisten) auswählen lassen und eingegebene Zeichenkette (nach Leerzeichen getrennt) einlesen
            choseMode();
            eingabe = getEingabe();

            /* VERARBEITUNG */
            // Wort für Wort anhand jeweiliger Hilfsmethoden twisten bzw. enttwisten
            foreach (String wort in eingabe)
            {
                String[] einzelergebnis;
                if (isTwistMode)
                {
                    einzelergebnis = twist(wort);
                } else
                {
                    einzelergebnis = enttwist(wort.ToLower());
                }
                ergebnis.Add(einzelergebnis);
            }

            /* AUSGABE */
            // Ergebnis ausgeben
            String ergebnisStr = generateErgebnisStr(ergebnis);
            Console.WriteLine(ergebnisStr);

            /* Eine neue Runde / Eingabe wird nach zwei Leerzeilen gestartet */
            Console.WriteLine("");
            Console.WriteLine("");
            start();
        }

        /// <summary>
        /// Diese Methode lädt den Wortschatz aus der Textdatei in ein Array
        /// </summary>
        static void loadWortschatz()
        {
            wortschatz = File.ReadAllLines(PATH_TO_WORTSCHATZ_FILE);
        }

        /// <summary>
        /// Diese Methode gibt den Benutzer die Möglichkeit, den Modus (Twisten oder Enttwisten) auszuwählen
        /// </summary>
        static void choseMode()
        {
            Console.Write("Möchtest du eine Zeichenkette twisten oder enttwisten? Gebe 0 für Twisten, 1 für Enttwisten oder 2 für ein Programmende ein: ");
            switch (Console.ReadLine())
            {
                case "0":
                    Console.WriteLine("--- AUSGEWÄHLTER MODUS: Twisten ---");
                    isTwistMode = true;
                    break;
                case "1":
                    Console.WriteLine("--- AUSGEWÄHLTER MODUS: Enttwisten ---");
                    isTwistMode = false;
                    break;
                case "2":
                    Console.WriteLine("--- PROGRAMMENDE ---");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Fehlerhafte Eingabe. Bitte erneut eingeben!");
                    choseMode();
                    break;
            }
        }

        /// <summary>
        /// Diese Methode gibt den Benutzer die Möglichkeit, eine Zeichenkette einzugeben, welche auf Richtigkeit überprüft wird
        /// </summary>
        /// <returns> Array mit den eingegebenen Wörtern </returns>
        static String[] getEingabe()
        {
            Console.Write("Bitte ein oder mehrere Wörter (ohne Satzzeichen getrennt!) eingeben, welche " + (isTwistMode ? "getwistet" : "enttwistet") + " werden sollen (Für Modusauswahl '<' eingeben): ");

            String eingabeCurr = Console.ReadLine();
            String[] eingabeArr;

            switch (eingabeCurr)
            {
                case "":
                    Console.WriteLine("Fehlerhafte Eingabe. Bitte erneut eingeben!");
                    eingabeArr = getEingabe();
                    break;
                case "<":
                    // Zurück zur Modusauswahl
                    Console.WriteLine("");
                    Console.WriteLine("");
                    choseMode();
                    eingabeArr = getEingabe();
                    break;
                default:
                    // Ist die Eingabe richtig, wird die Eingabe nach Leerzeichen getrennt und die einzelnen Wörter in ein Array gepackt, welches zurückgegeben wird
                    eingabeArr = eingabeCurr.Split(' ');
                    break;
            }

            return eingabeArr;          
        }

        /// <summary>
        /// Diese Methode baut einen String anhand der Ergebnisse zusammen
        /// </summary>
        /// <param name="ergebnis"> Liste bestehend aus String Arrays </param>
        /// <returns> Zusammengesetzter String aus den Ergebnissen </returns>
        static String generateErgebnisStr(List<String[]> ergebnis)
        {
            // Vorne an den Ergebnis String 'Lösung:' schreiben
            String ergebnisStr = "Lösung:";

            // Jedes Ergebnis durchlaufen...
            int index = 0;
            foreach (String[] einzelergebnis in ergebnis)
            {
                // ... und je nach Modus Ergebnis String zusammensetzen
                if (isTwistMode)
                {
                    // Wenn Twist Modus: Einfach nur den getwisteten String an der ersten Stelle des Arrays an den String hinzufügen und zurückgeben
                    ergebnisStr = ergebnisStr + " " + einzelergebnis[0];
                }
                else
                {
                    // Wenn Enttwist Modus: Je nach Anzahl der Ergebnisse...
                    if (einzelergebnis.Length == 0)
                    {
                        // ... hinzufügen, dass keine Lösung gefunden wurde
                        ergebnisStr = ergebnisStr + " (Zu der Zeichenkette '" + eingabe[index] + "' wurde keine Lösung gefunden)";
                    }
                    else if (einzelergebnis.Length == 1)
                    {
                        // ... eine Lösung hinzufügen
                        ergebnisStr = ergebnisStr + " " + einzelergebnis[0];
                    }
                    else
                    {
                        // ... mehrere Lösungen mit '/' getrennt hinzufügen
                        String loesungStr = "";
                        for (int i = 0; i < einzelergebnis.Length; i++)
                        {
                            String loesung = einzelergebnis[i];
                            loesungStr = loesungStr + loesung;
                            if (i < einzelergebnis.Length - 1)
                            { 
                                loesungStr = loesungStr + "/";
                            }
                        }
                        ergebnisStr = ergebnisStr + " (" + loesungStr + ")";
                    }
                }
                index++;
            }

            return ergebnisStr;
        }

        /// <summary>
        /// Diese Methode twistet ein gegebenen String
        /// </summary>
        /// <param name="str"> Zu twistenden String </param>
        /// <returns> Getwisteter String </returns>
        static String[] twist(string str)
        {
            String[] twistArr = new String[1];
            // Wenn String nur aus Nummern besteht oder weniger als 4 Buchstaben enthält, dann kann es nicht getwistet werden 
            // (da nur ein Buchstabe im mittleren Teil) und wird einfach als Lösung direkt hinzugefügt
            if (isDigitsOnly(str) || str.Length <= 3)
            {
                twistArr[0] = str;
            } 
            else
            {
                // Den mittleren Teil ohne Anfangs- und Endbuchstabe abtrennen und shufflen / vermischen
                String midStr = str.Substring(1, str.Length - 2);
                String midShuffleStr = shuffle(midStr);

                // Den Anfangs- und Endbuchstaben an den vermischten String wieder anhängen.
                char[] enttwistArr = str.ToCharArray();
                String twist = enttwistArr[0] + midShuffleStr + enttwistArr[enttwistArr.Length - 1];

                // Ergebnis in ein Array schreiben, um in der Main Methode ein einheitliches Ergebnis der Twist und Enttwist Methode zurückgegeben wird.
                twistArr[0] = twist;
            }

            return twistArr;
        }

        /// <summary>
        /// Diese Methode vermischt (shuffelt) zufällig ein gegebenen String
        /// </summary>
        /// <param name="str"> Zu vermischenden String </param>
        /// <returns> Vermischter String </returns>
        static String shuffle(String str)
        {
            char[] wortArr = str.ToCharArray();
            Random rng = new Random();
            int index = wortArr.Length;
            while (index > 1)
            {
                index--;
                int newNo = rng.Next(index + 1);
                var value = wortArr[newNo];
                wortArr[newNo] = wortArr[index];
                wortArr[index] = value;
            }
            return new string(wortArr);
        }

        /// <summary>
        /// Diese Methode enttwistet ein String
        /// </summary>
        /// <param name="str"> Zu enttwisteten String </param>
        /// <returns> Array mit möglichen Lösungen (Wenn Array leer, dann wurde keine Lösung gefunden) </returns>
        static String[] enttwist(string str)
        {
            var listOfEntwisteteStrings = new List<string>();
            // Wenn das Wort
            // ... keine Buchstaben enthält also der String leer ist: Leeres Array zurückgeben (Keine Lösung gefunden)
            // ... nur Nummern enthält: Direkt dem Ergebnis hinzufügen (Nummer bleibt Nummer)
            // ... mindestens einen Buchstaben enthält: Wort weiter enttwisten
            if (str.Length >= 1)
            {
                if (isDigitsOnly(str))
                {
                    listOfEntwisteteStrings.Add(str);
                }
                else
                {
                    // Zu Enttwistetes Wort in ein Chararray konvertieren und eine neue Liste deklarieren
                    char[] twistArr = str.ToCharArray();

                    // Mitteleren Teil abschneiden, wenn Wort mindestens 3 Buchstaben enthält
                    String midTwistStr = "";
                    if (str.Length >= 3)
                    {
                        midTwistStr = str.Substring(1, str.Length - 2);
                    }
                    // Alle möglichen Wörter aus dem Wortschatz (anhand Anfangs- & Endbuchstabe sowie Länge) filtern und in eine neue Liste packen
                    foreach (string wort in wortschatz)
                    {
                        String wortLower = wort.ToLower();
                        bool isTwistWort = false;
                        // Wort auf Anfangsbuchstabe, Endbuchstabe und Länge vergleichen
                        if (wortLower.StartsWith(twistArr[0]) && wortLower.EndsWith(twistArr[twistArr.Length - 1]) && wortLower.Length == twistArr.Length)
                        {
                            // Wenn Wort weniger als 2 Buchstaben enthält, isTwistWort auf True setzen, da beide Wörter gleich sind
                            if (str.Length <= 2)
                            {
                                isTwistWort = true;
                            }
                            else
                            {
                                // Wenn mind. 3 Buchstaben und Anfangs-, Endbuchstabe und Länge stimmt: Überprüfen, ob die Buchstaben in dem mittleren 
                                // Teil beider Strings übereinstimmen mittels einer Hilfsmethode
                                String midWort = wort.Substring(1, wort.Length - 2);
                                isTwistWort = areSameLetters(midTwistStr, midWort);
                            }
                        }

                        if (isTwistWort)
                        {
                            listOfEntwisteteStrings.Add(wort);
                        }
                    }
                }
            }

            // Liste der möglichen Lösungen in ein Array umwandeln und zurückgeben
            return listOfEntwisteteStrings.ToArray();
        }

        /// <summary>
        /// Diese Methode überprüft, ob ein String nur aus Nummern besteht
        /// </summary>
        /// <param name="str">Zu überprüfender String</param>
        /// <returns> Boolean Wert, ob String nur aus Buchstaben besteht oder nicht. </returns>
        static bool isDigitsOnly(String str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Diese Methode vergleicht, ob zwei Strings die gleichen Buchstaben enthalten, egal an welcher Position diese sich im String befinden.
        /// </summary>
        /// <param name="strOne">erster String</param>
        /// <param name="strTwo">zweiter String</param>
        /// <returns> Boolean Wert, ob die Strings die gleichen Buchstaben enthalten oder nicht. </returns>
        static bool areSameLetters(String strOne, String strTwo)
        {
            bool ergebnis = true;
            char[] oneArr = strOne.ToCharArray();
            char[] twoArr = strTwo.ToCharArray();
            // Gehe jeden Buchstaben des einen Strings durch und überprüfe, ob dieser im anderen String vorhanden ist
            foreach (char buchstabeOne in oneArr)
            {
                // Für jeden Buchstaben, jeden Buchstaben des anderen Strings durchgehen und vergleichen
                bool contains = false;
                int index = 0;
                foreach (char buchstabeTwo in twoArr)
                {
                    // Wenn der gleiche Buchstabe gefunden wurde, Buchstabe leer setzen, damit nicht zweimal der gleiche verglichen wird, und contains auf true setzen.
                    if (buchstabeOne == buchstabeTwo)
                    {
                        twoArr[index] = ' ';
                        contains = true;
                        break;
                    }
                    index++;
                }

                if (!contains)
                {
                    ergebnis = false;
                    break;
                }
            }
            return ergebnis;
        }
    }
}
