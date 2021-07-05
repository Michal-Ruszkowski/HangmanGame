using System;
using System.Collections.Generic;
using System.IO;

namespace Hangman
{
    class HangmanGame
    {
        public int Lives { get; set; }
        public string Country { get; set; }
        public string Capital { get; set; }
        public List<char> notInWord = new List<char>();
        private string hiddenWord = "";

        public HangmanGame(string word)
        {
            string[] words = word.Split(" | ");
            Capital = words[1].ToUpper();
            Country = words[0];
        }

        private string hideWord()
        {
            foreach (char sign in Capital)
            {
                if (sign == ' ')
                    hiddenWord += sign;
                else
                    hiddenWord += "_";
            }
            return hiddenWord;
        }

        private void showStartScreen()
        {
            Console.WriteLine("\n__________\n\nThis is game HANGMAN, try to guess European capitals.\n__________\n");
        }

        private void showWordInCorrectStateAndLives()
        {
            Console.WriteLine();
            Console.WriteLine("Secret word: {0}", hiddenWord);
            Console.WriteLine("Your lives: {0}", Lives);
            Console.Write("Not in word: ");
            foreach (char letter in notInWord)
            {
                Console.Write(letter + ", ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private void getUserInput()
        {
            string userAction;
            do
            {
                Console.WriteLine("Do you guess the letter or the password? 1-letter, 2-password");
                userAction = Console.ReadLine();
            } while (userAction != "1" && userAction != "2");
            if (userAction == "1")
            {
                getUserLetter();
            }
            else if (userAction == "2")
            {
                getUserPassword();
            }
        }
        private void getUserLetter()
        {
            string userInput;
            do
            {
                Console.WriteLine("Enter one letter");
                userInput = Console.ReadLine();
            } while (userInput.Length != 1 || userInput == "_");
            string inputUpper = userInput.ToUpper();
            char letter = Convert.ToChar(inputUpper);
            checkLetter(letter);
        }

        private void getUserPassword()
        {
            string userInput;
            Console.WriteLine("Enter the password");
            userInput = Console.ReadLine();
            string inputUpper = userInput.ToUpper();
            checkPassword(inputUpper);
        }
        private void checkLetter(char userInput)
        {
            bool isLetterInWord = false;
            char[] hiddenWordAsChars = hiddenWord.ToCharArray();

            int index = 0;
            foreach (char letter in Capital)
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

        private void checkPassword(string userInput)
        {


            if (userInput == Capital)
            {
                hiddenWord = Capital;
                Console.WriteLine("That was lucky.");
                isGameEnded();
            }


            else if (userInput != Capital)
            {
                Console.WriteLine("You are closer to death.");
                Lives -= 2;
            }
        }

        private bool isGameEnded()
        {
            if (Lives < 1 || hiddenWord.IndexOf("_") == -1)
                return true;
            else
                return false;
        }

        private void initGame()
        {
            hideWord();
            showStartScreen();
        }

        private void gameplay()
        {
            while (!isGameEnded())
            {
                showWordInCorrectStateAndLives();
                getUserInput();
            }
        }

        private void showResult()
        {
            if(Lives == 1)
            {
                Console.WriteLine("The capital of " + Country);
            }
            if (Lives < 1)
            {
                Console.WriteLine("\n__________\n\nYOU DIED\n__________\n");
            }
            else
            {
                Console.WriteLine("\n__________\n\nYou guessed the capital after 12 letters. It took you 45 seconds\n__________\n");
                Console.WriteLine("Retry? y-yes, n-no");
                string userAction = Console.ReadLine();
                while (userAction != "y" && userAction != "n") ;
                if (userAction == "y")
                {
                    startGame();
                }
                else if (userAction == "n")
                {
                    Console.WriteLine("Good bye");
                }
            }                
        }

        public void play()
        {
            initGame();
            gameplay();
            showResult();
        }

        public void startGame()
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
            hangmanGame.play();
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
            hangmanGame.play();
        }
    }
}