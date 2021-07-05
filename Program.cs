using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hangman
{
    class HangmanGame
    {
        public int Lives { get; set; }
        private int guessing_tries = 0;
        private string country { get; set; }
        private string guessed_word { get; set; }
        private List<char> notInWord = new List<char>();
        private string hiddenWord = "";
        private DateTime data1;
        private DateTime data2;
        private TimeSpan guessing_time;
        private string name = "";

        public HangmanGame(string word)
        {
            string[] words = word.Split(" | ");
            guessed_word = words[1].ToUpper();
            country = words[0];
        }

        private string HideWord()
        {
            foreach (char sign in guessed_word)
            {
                if (sign == ' ')
                    hiddenWord += sign;
                else
                    hiddenWord += "_";
            }
            return hiddenWord;
        }

        private void ShowStartScreen()
        {
            Console.WriteLine("\n__________\n\nThis is game HANGMAN, try to guess European capitals.\n__________\n");
        }

        private void ShowWordInCorrectStateAndLives()
        {
            Console.WriteLine();
            Console.WriteLine("Secret word: " + hiddenWord);
            Console.WriteLine("Your lives: " + Lives);
            Console.Write("Not in word: ");
            foreach (char letter in notInWord)
            {
                Console.Write(letter + ", ");
            }
            Console.WriteLine();
            if (Lives == 1)
            {
                Console.WriteLine("The capital of " + country);
            }
            Console.WriteLine();
        }

        private void GetUserInput()
        {
            string userAction;
            do
            {
                Console.WriteLine("Do you guess the letter or the password? 1-letter, 2-password");
                userAction = Console.ReadLine();
            } while (userAction != "1" && userAction != "2");
            if (userAction == "1")
            {
                GetUserLetter();
            }
            else if (userAction == "2")
            {
                GetUserPassword();
            }
            guessing_tries++;
        }
        private void GetUserLetter()
        {
            string userInput;
            do
            {
                Console.WriteLine("Enter one letter");
                userInput = Console.ReadLine();
            } while (userInput.Length != 1 || userInput == "_");
            string inputUpper = userInput.ToUpper();
            char letter = Convert.ToChar(inputUpper);
            CheckLetter(letter);
        }

        private void GetUserPassword()
        {
            string userInput;
            Console.WriteLine("Enter the password");
            userInput = Console.ReadLine();
            string inputUpper = userInput.ToUpper();
            CheckPassword(inputUpper);
        }
        private void CheckLetter(char userInput)
        {
            bool isLetterInWord = false;
            char[] hiddenWordAsChars = hiddenWord.ToCharArray();
            int index = 0;
            foreach (char letter in guessed_word)
            {
                if (userInput == letter)
                {
                    Console.WriteLine("That was lucky.");
                    if (hiddenWordAsChars[index] == userInput)
                    {
                        Console.WriteLine("This letter you already guessed. I'll take one of your live.");
                        Lives--;
                    }
                    isLetterInWord = true;
                    hiddenWordAsChars[index] = userInput;
                }
                index++;
            }
            hiddenWord = new string(hiddenWordAsChars);
            if (!isLetterInWord)
            {
                Console.WriteLine("You are closer to death.");
                Lives--;
                notInWord.Add(userInput);
            }
        }

        private void CheckPassword(string userInput)
        {
            if (userInput == guessed_word)
            {
                hiddenWord = guessed_word;
                Console.WriteLine("That was lucky.");
                IsGameEnded();
            }
            else if (userInput != guessed_word)
            {
                Console.WriteLine("You are closer to death.");
                Lives -= 2;
            }
        }

        private bool IsGameEnded()
        {
            if (Lives < 1 || hiddenWord.IndexOf("_") == -1)
                return true;
            else
                return false;
        }

        private void InitGame()
        {
            HideWord();
            ShowStartScreen();
            data1 = GetTimestamp(DateTime.Now);
        }

        private DateTime GetTimestamp(DateTime value)
        {
            return value;
        }

        private void Gameplay()
        {
            while (!IsGameEnded())
            {
                ShowWordInCorrectStateAndLives();
                GetUserInput();
            }
        }

        private void ShowResult()
        {
            if (Lives < 1)
            {
                Console.WriteLine();
                Console.WriteLine("YOU DIED");
            }
            else
            {
                data2 = GetTimestamp(DateTime.Now);
                guessing_time = data2 - data1;
                Console.WriteLine();
                Console.WriteLine("You guessed the capital after " + guessing_tries + " letters. It took you " + guessing_time);
                SaveHighScore();
            }
            string userAction;
            do
            {
                Console.WriteLine("Retry? y-yes, n-no");
                userAction = Console.ReadLine();
            }
            while (userAction != "y" && userAction != "n");
            if (userAction == "y")
            {
                StartGame();
            }
            else if (userAction == "n")
            {
                Console.WriteLine("Good bye");
            }
        }

        private void SaveHighScore()
        {
            Console.WriteLine("Whats's your name?");
            name = Console.ReadLine();
            string path = @"high_scores.txt";
            StreamWriter sw;
            DateTime thisDay = DateTime.Today;
            try
            {
                if (!File.Exists(path))
                {
                    sw = File.CreateText(path);
                }
                else
                {
                    sw = new StreamWriter(path, true);
                }
                sw.WriteLine(name + " | " + thisDay + " | " + guessing_time + " | " + guessing_tries + " | " + guessed_word);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public void Play()
        {
            InitGame();
            Gameplay();
            ShowResult();
        }

        private void StartGame()
        {
            string path = @"countries_and_capitals.txt";
            StreamReader sr = File.OpenText(path);
            List<string> passwords = new List<string>();
            string password = "";
            while ((password = sr.ReadLine()) != null)
            {
                passwords.Add(password);
            }
            Random generator = new Random();
            int a = generator.Next(passwords.Count);
            string word = passwords[a];
            HangmanGame hangmanGame = new HangmanGame(word);
            hangmanGame.Lives = 5;
            hangmanGame.Play();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string path = @"countries_and_capitals.txt";
            StreamReader sr = File.OpenText(path);
            List<string> passwords = new List<string>();
            string password = "";
            while ((password = sr.ReadLine()) != null)
            {
                passwords.Add(password);
            }
            Random generator = new Random();
            int a = generator.Next(passwords.Count);
            string word = passwords[a];
            HangmanGame hangmanGame = new HangmanGame(word);
            hangmanGame.Lives = 5;
            hangmanGame.Play();
        }
    }
}
