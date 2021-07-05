using System;
using System.Collections.Generic;
using System.IO;

namespace Hangman
{
    class HangmanGame
    {
        public int Lives { get; set; }

        private DateTime _startGameDateTime;
        private DateTime _endGameDateTime;
        private TimeSpan _guessingTime;

        private int _lives;
        private string _hiddenWord;
        private string _guessedCapital;
        private string _country;
        private List<char> _lettersWhichPlayerEnteredIncorrect;
        private int _guessingTries = 0;
        private string _playerName;

        private string GetDataSetToGuessFromFile()
        {
            string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"RequiredFiles\countries_and_capitals.txt");

            StreamReader fileWithCauntriesAndCapitols = File.OpenText(path);
            List<string> readedLines = new List<string>();
            string readedLine;
            while ((readedLine = fileWithCauntriesAndCapitols.ReadLine()) != null)
            {
                readedLines.Add(readedLine);
            }
            Random generator = new Random();
            int randomNumber = generator.Next(readedLines.Count);
            string countryAndCapitalSet = readedLines[randomNumber];
            return countryAndCapitalSet;
        }

        private void SetBaseGameVariables()
        {
            _lives = Lives;
            string[] words = GetDataSetToGuessFromFile().Split(" | ");
            _lettersWhichPlayerEnteredIncorrect = new List<char>();
            _guessedCapital = words[1].ToUpper();
            _country = words[0];
        }

        private string HideWord()
        {
            _hiddenWord = "";
            foreach (char sign in _guessedCapital)
            {
                if (sign == ' ')
                    _hiddenWord += sign;
                else
                    _hiddenWord += "_";
            }
            return _hiddenWord;
        }

        private void ShowStartScreen()
        {
            Console.WriteLine("\n__________\n\nThis is game HANGMAN, try to guess European capitals.\n__________\n");
        }

        private void ShowWordInCorrectStateAndLives()
        {
            Console.WriteLine();
            Console.WriteLine("Secret word: " + _hiddenWord);
            Console.WriteLine("Your lives: " + _lives);
            Console.Write("Not in word: ");
            foreach (char letter in _lettersWhichPlayerEnteredIncorrect)
            {
                Console.Write(letter + ", ");
            }
            Console.WriteLine();
            if (_lives == 1)
            {
                Console.WriteLine("The capital of " + _country);
            }
            Console.WriteLine();
        }

        private void GetPlayerInput()
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
            _guessingTries++;
        }
        private void GetUserLetter()
        {
            string playerInput;
            do
            {
                Console.WriteLine("Enter one letter");
                playerInput = Console.ReadLine().ToUpper();
            } while (playerInput.Length != 1 || playerInput == "_");

            char letter = Convert.ToChar(playerInput);
            CheckIsLetterInGuessingWord(letter);
        }

        private void GetUserPassword()
        {
            Console.WriteLine("Enter the password");
            string playerInput = Console.ReadLine().ToUpper();
            CheckPassword(playerInput);
        }

        private void CheckIsLetterInGuessingWord(char playerInput)
        {
            bool isLetterInWord = false;
            char[] hiddenWordAsChars = _hiddenWord.ToCharArray();
            int index = 0;
            int letterOccuranceCounter = 0;
            foreach (char letter in _guessedCapital)
            {
                if (playerInput == letter)
                {
                    letterOccuranceCounter++;
                    if (letterOccuranceCounter == 1)
                    {
                        Console.WriteLine("That was lucky.");
                        if (hiddenWordAsChars[index] == playerInput || _lettersWhichPlayerEnteredIncorrect.Contains(playerInput))
                        {
                            Console.WriteLine("This letter you already tried. I'll take one of your live.");
                            _lives--;
                        }
                    }
                    isLetterInWord = true;
                    hiddenWordAsChars[index] = playerInput;
                }
                index++;
            }
            if (!isLetterInWord)
            {
                Console.WriteLine("You are closer to death.");
                if (!_lettersWhichPlayerEnteredIncorrect.Contains(playerInput))
                    _lettersWhichPlayerEnteredIncorrect.Add(playerInput);
                _lives--;
            }
            _hiddenWord = new string(hiddenWordAsChars);
        }

        private void CheckPassword(string userInput)
        {
            if (userInput == _guessedCapital)
            {
                _hiddenWord = _guessedCapital;
                Console.WriteLine("That was lucky.");
                IsGameEnded();
            }
            else if (userInput != _guessedCapital)
            {
                Console.WriteLine("You are closer to death.");
                _lives -= 2;
            }
        }

        private bool IsGameEnded()
        {
            if (_lives < 1 || _hiddenWord.IndexOf("_") == -1)
                return true;
            else
                return false;
        }

        private void InitGame()
        {
            SetBaseGameVariables();
            HideWord();
            ShowStartScreen();
            _startGameDateTime = DateTime.Now;
        }

        private void Gameplay()
        {
            while (!IsGameEnded())
            {
                ShowWordInCorrectStateAndLives();
                GetPlayerInput();
            }
        }

        private void ShowResult()
        {
            if (_lives < 1)
            {
                Console.WriteLine();
                Console.WriteLine("YOU DIED");
            }
            else
            {
                _endGameDateTime = DateTime.Now;
                _guessingTime = _endGameDateTime - _startGameDateTime;
                Console.WriteLine();
                Console.WriteLine("You guessed the capital after " + _guessingTries + " letters. It took you " + _guessingTime);
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
                Play();
            }
            else if (userAction == "n")
            {
                Console.WriteLine("Good bye");
            }
        }

        private void SaveHighScore()
        {
            Console.WriteLine("Whats's your name?");
            _playerName = Console.ReadLine();
            string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"RequiredFiles\high_scores.txt");
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
                sw.WriteLine(_playerName + " | " + thisDay + " | " + _guessingTime + " | " + _guessingTries + " | " + _guessedCapital);
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            HangmanGame hangmanGame = new HangmanGame();
            hangmanGame.Lives = 5;
            hangmanGame.Play();
        }
    }
}
