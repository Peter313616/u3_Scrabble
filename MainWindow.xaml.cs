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
        ///global variables
        int BlankTileCounter = 0;
        string currentWord;
        string badLetters = "ETAOINSHRDLCUMWFGYPBVKJXQZ";
        string BestWord = "";
        int BestScore = 0;

        public MainWindow()
        {
            InitializeComponent();
            ///takes the dictionary and puts into a stream reader to be used
            System.Net.WebClient wc = new System.Net.WebClient();
            Stream download = wc.OpenRead("http://darcy.rsgc.on.ca/ACES/ICS4U/SourceCode/Words.txt");
            StreamReader sr = new StreamReader(download);
            ///picks random letters, and finds and removes blanks
            ScrabbleGame sg = new ScrabbleGame();
            string letters = sg.drawInitialTiles().ToUpper();//E C K S O I H = testing letters
            while (letters.IndexOf(" ") != -1)
            {
                letters = letters.Remove(letters.IndexOf(" "), 1);
                BlankTileCounter++;
            }
            ///finds the tiles in the alphabet and removes them, leaving all other letters
            for (int i = 0; i < letters.Length; i++)
            {
                int letterLocation = badLetters.IndexOf(letters.Substring(i, 1));
                if (letterLocation != -1)
                {
                    badLetters = badLetters.Remove(letterLocation, 1);
                }
            }

            string lastWord = "";
            lblOutput.Content += "\t\t" + Environment.NewLine; ///formating

            while (!sr.EndOfStream)
            {
                int counter = 0;
                currentWord = sr.ReadLine().ToUpper();
                string CheckWord = currentWord;///
                if (currentWord.Length < 8 && currentWord != lastWord)///only have 7 tiles, can't be longer
                { 
                    for (int i = 0; i < badLetters.Length; i++)
                    {
                        if (currentWord.Contains(badLetters.Substring(i,1)))
                        {///checks for any letters that aren't tiles
                            counter++;
                            if (counter > BlankTileCounter)
                            {///if there are more wrong letters than blank tiles, exit for loop
                                i = 26;
                            }
                        }
                    }//end for loop
                    if (counter <= BlankTileCounter)
                    {///checks if the word has only letters available
                        int currentPoints = 0;
                        for (int i = 0; i < letters.Length; i++)
                        {
                            string currentLetter = letters.Substring(i, 1);///finds specific letter in current word
                            int RemoveLetter = CheckWord.IndexOf(currentLetter);///finds index of that letter
                            if (RemoveLetter != -1)
                            {///removes thre letter form the temp word, and gets the points for the letter
                                CheckWord = CheckWord.Remove(RemoveLetter, 1);
                                ScrabbleLetter sl = new ScrabbleLetter(currentLetter.ToCharArray()[0]);
                                currentPoints += sl.Points;
                            }
                        }
                        if (CheckWord.Length <= BlankTileCounter)
                        {/// if the word can be made with the tiles, writes it to output
                            lastWord = currentWord;
                            lblOutput.Content += Environment.NewLine + currentWord.Substring(0,1)
                                + currentWord.Substring(1);

                            if (currentPoints > BestScore)
                            {///checks if the current word has a better score than previous best
                                BestWord = currentWord;
                                BestScore = currentPoints;
                            }                           
                        }//end check for usable word
                    }//end if for potential word
                }//end length check if
            }//end while loop
            sr.Close();
            //writes best wrod and c=score to output
            lblBest.Content = "Your letters were: " + letters
                                + Environment.NewLine +"Your best possible word is: "
                                + BestWord.Substring(0, 1) + BestWord.Substring(1).ToLower()
                                + ", which gets you " + BestScore + " points";
        }
    }
}
