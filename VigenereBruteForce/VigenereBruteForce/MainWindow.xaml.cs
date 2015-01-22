using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace VigenereBruteForce
{
    /// <summary>
    /// Quick and dirty program to guess a Vigenere Cipher Key given the suspected length (it's not really a brute forcer)
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoButton_Click(object sender, RoutedEventArgs evtArgs)
        {
            GoButton.IsEnabled = false;

            try
            {
                OutputBox.Text = Brute(int.Parse(SuspectedKeyLength.Text), CipherText.Text);
            }
            catch (Exception e)
            {
                OutputBox.Text = "There was an error:" + Environment.NewLine + e.ToString();
            }
            finally
            {
                GoButton.IsEnabled = true;
            }
        }

        private static string Brute(int keyLength, string cipherText)
        {
            // Buckets will store the different character sets by key
            var buckets = new List<char>[keyLength];
            for (var i = 0; i < keyLength; i++)
            {
                buckets[i] = new List<char>();
            }

            // Keep only alpha
            var regex = new Regex("[^A-Z]");
            var cTextArray = regex.Replace(cipherText.ToUpper(), "").ToCharArray();

            // Put all the text in their buckets
            for (var i = 0; i < cTextArray.Length; i++)
            {
                buckets[i%keyLength].Add(cTextArray[i]);
            }

            // Build the Vigenere table
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            var table = new Dictionary<char, Dictionary<char, char>>();

            // Fill
            for (var x = 0; x < alpha.Length; x++)
            {
                var c = alpha[x];
                table[c] = new Dictionary<char, char>();

                for (var y = 0; y < alpha.Length; y++)
                {
                    var b = alpha[y];
                    table[c][b] = alpha[(x + y)%alpha.Length];
                }
            }

            // Frequency analysis
            var lettersByFrequency = new char[] {'E', 'T', 'A', 'O', 'I', 'N'};

            // Create lists of the most popular letter
            var mostPopular = new char[keyLength];
            for (var i = 0; i < keyLength; i++)
            {
                mostPopular[i] = buckets[i].GroupBy(x => x).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
            }

            // Solve for each letter
            var possibleKeys = new List<string>();
            foreach (var popularLetter in lettersByFrequency)
            {
                var key = popularLetter + ": ";
                foreach (var lookingFor in mostPopular)
                {
                    key += GetLetter(table[popularLetter], lookingFor);
                }
                possibleKeys.Add(key);
            }

            return string.Join(Environment.NewLine, possibleKeys);
        }

        private static char GetLetter(Dictionary<char, char> table, char lookingFor)
        {
            foreach (var key in table.Keys)
            {
                if (table[key] == lookingFor)
                {
                    return key;
                }
            }

            return '?';
        }
    }
}