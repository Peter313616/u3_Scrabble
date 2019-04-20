/*
 * Peter McEwen
 * April 23, 2019
 * selects random letters, then can find possible words & best option
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace U3_ScrabbleHome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int BlankTileCounter = 0;
        string currentWord;
        string badLetters = "ETAOINSHRDLCUMWFGYPBVKJXQZ";
        string BestWord = "";
        int BestScore = 0;

        public MainWindow()
        {
            InitializeComponent();

            System.Net.WebClient wc = new System.Net.WebClient();
            Stream download = wc.OpenRead("http://darcy.rsgc.on.ca/ACES/ICS4U/SourceCode/Words.txt");
            StreamReader sr = new StreamReader(download);

            ScrabbleGame sg = new ScrabbleGame();
            string letters = sg.drawInitialTiles().ToUpper();//E C K S O I H = testing letters
            while (letters.IndexOf(" ") != -1)
            {
                letters = letters.Remove(letters.IndexOf(" "), 1);
                BlankTileCounter++;
            }

            for (int i = 0; i < letters.Length; i++)
            {
                int letterLocation = badLetters.IndexOf(letters.Substring(i, 1));
                if (letterLocation != -1)
                {
                    badLetters = badLetters.Remove(letterLocation, 1);
                }
            }

            string lastWord = "";
            lblOutput.Content += "\t\t" + Environment.NewLine; //ease of reading/known commands in cs

            while (!sr.EndOfStream)
            {
                int counter = 0;
                currentWord = sr.ReadLine().ToUpper();
                string CheckWord = currentWord;
                if (currentWord.Length < 8 && currentWord != lastWord)
                { 
                    for (int i = 0; i < badLetters.Length; i++)
                    {
                        if (currentWord.Contains(badLetters.Substring(i,1)))
                        {
                            //MessageBox.Show(currentWord);
                            counter++;
                            if (counter > BlankTileCounter)
                            {
                                i = 26;
                            }
                        }
                    }
                    if (counter <= BlankTileCounter)
                    {
                        int currentPoints = 0;
                        for (int i = 0; i < letters.Length; i++)
                        {
                            string currentLetter = letters.Substring(i, 1);
                            int RemoveLetter = CheckWord.IndexOf(currentLetter);
                            if (RemoveLetter != -1)
                            {
                                CheckWord = CheckWord.Remove(RemoveLetter, 1);
                                ScrabbleLetter sl = new ScrabbleLetter(currentLetter.ToCharArray()[0]);
                                currentPoints += sl.Points;

                            }
                        }
                        if (CheckWord.Length <= BlankTileCounter)
                        {
                            lastWord = currentWord;
                            lblOutput.Content += Environment.NewLine + currentWord.Substring(0,1)
                                + currentWord.Substring(1);

                            
                            for (int i = 0; i < currentWord.Length; i++)
                            {
                                char[] temp = currentWord.ToCharArray();
                                
                            }

                            if (currentPoints > BestScore)
                            {
                                BestWord = currentWord;
                                BestScore = currentPoints;
                            }
                            
                        }
                    }
                }
            }
            sr.Close();
            lblBest.Content = "Your letters were: " + letters
                                + Environment.NewLine +"Your best possible word is: "
                                + BestWord.Substring(0, 1) + BestWord.Substring(1).ToLower()
                                + ", which gets you " + BestScore + " points";
        }
    }
}
