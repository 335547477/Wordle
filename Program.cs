//Author: Kinjal Padhiar
//File Name: Program.cs
//Project Name: PadhiarKPASS1
//Creation Date: February 16, 2022
//Modified Date: Febrary 23, 2022
/*Description: A simplfied, console version of our favourite educational game 
 * (that doesn't make you old, I promise)...DRUMROLL...WORDLE*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PadhiarKPASS1
{
    class Program
    { 
        //declaring variables to aid in file input/output
        static StreamReader inFile;
        static StreamWriter outFile;

        //declaring lists to store answers and extra words read from files 
        static List<string> wordleAns = new List<string>();
        static List<string> wordleEx = new List<string>();

        //String of letters In The alphabet
        static string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //Declaring lists to store letters relative to the color they correspond to
        static List<char> storeGreen = new List<char>();
        static List<char> storeYellow = new List<char>();
        static List<char> storeGrey = new List<char>();

        //specifically declaring list for yellow letters in the gameboard 
        static List<char> storeYellowAr = new List<char>();
        
        //Initialize variables for the guesses, answers and when the correct guess occured
        static string guess;
        static string answer;

        //Declaring variables to read from file, save to file, and reset statistics
        static int gamesPlayed;
        static int gamesWon;
        static int currGamesWonStreak;
        static int maxGamesWonStreak;
        static int[] winGD = new int[6];

        static void Main(string[] args)
        {
            //options chosen from end menu after user gets correct answer or wrong answer 
            int chosen;

            //Set Console Size
            Console.SetWindowSize(60, 35);

            //Create 2D Array for Game Board 
            char[,] gameboard = new char[6, 5];

            //reads in dictionaries and stores them in two lists (wordleAns and wordleEx)
            ReadData();

            //reads in User Statistics from file
            StatsRead();

            //Initial Menu and Display Rules To User
            CentreCursor("The goal of Wordle is to guess the 5 letter word.");
            Console.WriteLine("The goal of Wordle is to guess the 5 letter word.");
            CentreCursor("The green letters are in the word and in the correct spot!");
            Console.WriteLine("The green letters are in the word and in the correct spot!");
            CentreCursor("The yellow letters are in the word but in the wrong spot.");
            Console.WriteLine("The yellow letters are in the word but in the wrong spot.");
            CentreCursor("The grey letters are not in the word.");
            Console.WriteLine("The grey letters are not in the word.");
            CentreCursor("You have 6 guesses. Good Luck!");
            Console.WriteLine("You have 6 guesses. Good Luck!");

            //Retrieves key from user to start playing game 
            CentreCursor("Press Enter To Start Playing!");
            Console.WriteLine("Press Enter To Start Playing!");
            Console.ReadLine();
            Console.Clear();

            //Generates Randomized Word From Answer Dictionary
            answer = RandomAns();

            //Creates GameBoard and Displays Alphabet
            Console.WriteLine(alpha);
            GameBoard(gameboard);

            //Prompt user for a guess and accordingly presents feedback
            RepeatGuesses(gameboard);

            //while loops for when user failed to get the correct answer after 6 tries
            while (answer.Equals(guess) == false)
            {
                //displays correct answer and changes user statitics accordingly 
                //current max streak is lost and max win streak is updated if needed
                WrongAns();

                //presents end of the game options and retrieves chosen option from user 
                chosen = EndMenuOptions();

                //if statements to present feedback depending on the option chosen
                if (chosen == 1)
                {
                    //clears console, and all stored letters
                    Console.Clear();
                    storeGreen.Clear();
                    storeYellow.Clear();
                    storeGrey.Clear();

                    //generates new answer from answer dictionary
                    answer = RandomAns();

                    //displays alphabet and gameBoard
                    Console.WriteLine(alpha);
                    GameBoard(gameboard);

                    //allows user to make guesses
                    RepeatGuesses(gameboard);

                    //if all guessses are wrong then displays statistics and allows users to choose again
                    WrongAns();
                    chosen = EndMenuOptions();

                    //the while loops restarts from the top and continues based on new chosen value 
                    continue;
                }
                else if (chosen == 2)
                {
                    //resets all statistics and saves reset data
                    ResetStats();

                    //allows user to choose another option and continue loop
                    chosen = EndMenuOptions();
                    continue;
                }
                else if (chosen == 3)
                {
                    //breaks out of while loop to allow user to exit program
                    break;
                }
                else
                {
                    //states to user that invalid number was chosen and presents menu to try again
                    Console.WriteLine("Invalid Selection. Please Try Again.");
                    chosen = EndMenuOptions();
                    continue;
                }
            }

            //End of the program prompt
            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
        }

        //reads dictionaries in from file at the start of the game
        private static void ReadData()
        {
            //variable neccesary to read in data from the file 
            string line;

            //Try-Catch block to read in data from two dictionaries 
            try
            {
                //opens answers dictionary
                inFile = File.OpenText("WordleAnswers.txt");

                //while loop to read every line into answers list under end of file 
                while (!inFile.EndOfStream)
                {
                    line = inFile.ReadLine();
                    wordleAns.Add(line);
                }

                //while loop to read every line into extra words list under end of file 
                inFile = File.OpenText("WordleExtras.txt");
                while (!inFile.EndOfStream)
                {
                    line = inFile.ReadLine();
                    wordleEx.Add(line);
                }
            }
            catch (FileNotFoundException fnf)
            {
                Console.WriteLine("ERROR: " + fnf);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
            finally
            {
                //if statement to close the file if it is not null
                if (inFile != null)
                {
                    inFile.Close();
                }
            }
        }

        //reads user statitics from file at start of the game 
        private static void StatsRead()
        {
            //opens file to read from 
            inFile = File.OpenText("UserStats.txt");

            //if the file has user statistics then read them in 
            if (inFile != null)
            {
                //a try-catch block to read in user statitics
                try
                {
                    //variables to read in statistics data 
                    string line;
                    string[] data;
                    int count = 0;

                    //reads in line by line the saved user statistics
                    gamesPlayed = Convert.ToInt32(inFile.ReadLine());
                    gamesWon = Convert.ToInt32(inFile.ReadLine());
                    currGamesWonStreak = Convert.ToInt32(inFile.ReadLine());
                    maxGamesWonStreak = Convert.ToInt32(inFile.ReadLine());

                    //uses a while loop to read in the win distribution which is all on the same line
                    while (inFile.EndOfStream)
                    {
                        //reads line from file and splits it based on comma
                        line = inFile.ReadLine();
                        data = line.Split(',');

                        //uses count variable to identify which index to store the number in 
                        winGD[count] = Convert.ToInt32(data[count]);
                        count++;
                    }
                }
                catch (EndOfStreamException endstream)
                {
                    Console.WriteLine("ERROR: " + endstream.Message);
                }
                catch (FileNotFoundException fnf)
                {
                    Console.WriteLine("ERROR: " + fnf.Message);
                }
                finally
                {
                    //if statement to close file if is it not null
                    if (inFile != null)
                    {
                        inFile.Close();
                    }
                }
            }
            

        }

        //generates random answer from answers dictionary
        private static string RandomAns()
        {
            //declaring variables and creating a random generator
            Random r = new Random();
            int ansIndex;

            //generates a random number that is within the number of the words in the answers dictionary
            ansIndex = r.Next(wordleAns.Count);

            //answer is the word in the wordle dictionary that is at that random index 
            answer = wordleAns[ansIndex];

            //returns answer
            return answer;
        }

        //creates empty gameboard for start of the game (without guesses)
        private static void GameBoard(char [,] gameboard)
        {
            //Loop through and fill array with starting positions
            for (int row = 0; row < gameboard.GetLength(0); row++)
            {
                //writes each guess number 
                Console.WriteLine("Guess: " + (row + 1)); 

                //nested loops to go through each column for each row
                for (int col = 0; col < gameboard.GetLength(1); col++)
                {
                    //to start, assigns gameBoard with the value of a dash
                    gameboard[row, col] = '_';
                    Console.Write("|" + gameboard[row, col]  + "|");
                }

                //skips to the next line for the next row
                Console.WriteLine();
            }
        }

        //prompts user for a guess and stores value
        private static void Guess(int guessNum)
        {
            Console.Write("Enter Guess " + guessNum + " : ");
            guess = Console.ReadLine();
        }

        //allows user 6 guesses and provides feedback accordingly 
        private static void RepeatGuesses(char[,] gameBoard)
        {
            //declares variable to stores the guess that the correct answer was on
            int correctGuessNum;

            //for-loop to allow the user 6 guesses
            for (int i = 0; i < 6; i++)
            {
                //Asks user for a guess and retrives feedback
                Guess(i + 1);


                //if the guess is 5 letters and contains no special characters then continues program, otherwise provides feedback to user and asks them to try again
                if (guess.Length == 5 && guess.Contains(@"1234567890\| !#$%&/()=?»«@£§€{}.-;'<>_,") == false)
                {

                    //changes answer to all lower case to compare 
                    answer = answer.ToLower();

                    //if statement to check if the answer is correct
                    if (answer.Equals(guess))
                    {

                        //store correct guess number 
                        correctGuessNum = i;

                        //clears screen and displays that answer was correct
                        Console.Clear();
                        Console.WriteLine("Correct Answer! The word was indeed " + answer + " .");

                        //updates statistics (max streak is updated if larger than current streak, based on guess number win distribution is updated)
                        gamesPlayed++;
                        gamesWon++;
                        currGamesWonStreak++;
                        if (currGamesWonStreak > maxGamesWonStreak)
                        {
                            maxGamesWonStreak = currGamesWonStreak;
                        }
                        winGD[correctGuessNum] = winGD[correctGuessNum] + 1;

                        //saves updated statistics and writes them to screen for user 
                        GameFinish();

                        //allows user to play again, reset stats, or exit and stores chosen option
                        int chosen = EndMenuOptions();

                        //while the chosen number is not 3(exit), outputs feedback accordingly
                        while (chosen != 3)
                        {

                            //allows user to play again if chosen option is 1
                            if (chosen == 1)
                            {

                                //clears screen and each coloured list 
                                Console.Clear();
                                storeGreen.Clear();
                                storeYellow.Clear();
                                storeGrey.Clear();

                                //outputs the alphabet and game board 
                                Console.WriteLine(alpha);
                                GameBoard(gameBoard);

                                //makes i equal to -1, so the increment statement can make the guesses start at 0 again 
                                i = -1;

                                //generates new answer from answers dictionary 
                                answer = RandomAns();

                                //breaks out of while loop to continue on with for loop for guesses
                                break;
                            }
                            else if (chosen == 2)
                            {
                                //resets statistics 
                                ResetStats();

                                //allows user to choose what they would like to do and stores chosen number 
                                chosen = EndMenuOptions();

                                //based on chosen number repeats while loop
                                continue;
                            }
                        }

                        //if chosen number is 3(exit) breaks out of for loop, otherwise continues
                        if (chosen == 3)
                        {
                            break;
                        }
                        else
                        {
                            continue;
                        }

                    }

                    //if either dictionaries contain the guessed word, continues program
                    if (wordleAns.Contains(guess) == true || wordleEx.Contains(guess) == true)
                    {

                        //changes guess and answer to uppercase to compare and fill array with 
                        guess = guess.ToUpper();
                        answer = answer.ToUpper();

                        //for loop that loops through each letter in the word and decides which color it belongs to
                        for (int k = 0; k < guess.Length; k++)
                        {

                            //if statement for where the letter is placed in comparison to the answer and stores them in their designated color accordingly
                            if (answer.Contains(guess[k]) && answer[k] == guess[k])
                            {

                                //letter is in the correct spot in the answer, thus adds to green letter list
                                storeGreen.Add(guess[k]);

                                //alphabetically sorts list 
                                storeGreen.Sort();

                                //if the yellow list contains this letter, then removes it because it has been guessed correctly now
                                if (storeYellow.Contains(guess[k]))
                                {
                                    storeYellow.Remove(guess[k]);
                                }
                            }
                            else if (answer.Contains(guess[k]) && answer[k] != guess[k])
                            {

                                //if the green letters list doesn't contain this letter, then add it to the yellow list
                                if (storeGreen.Contains(guess[k]) == false)
                                {
                                    storeYellow.Add(guess[k]);
                                }

                                //for the gameboard array, add all yellow letters in the guess 
                                storeYellowAr.Add(guess[k]);

                                //sort both lists alphabetically 
                                storeYellowAr.Sort();
                                storeYellow.Sort();
                            }
                            else
                            {

                                //letter is not in the word so it is stored in the grey letter list
                                storeGrey.Add(guess[k]);

                                //sorts list alphabetically
                                storeGrey.Sort();
                            }

                        }

                        //clears screen
                        Console.Clear();
                        Console.WriteLine();

                        //colours the alphabet according to the list
                        AlphaColours();
                        Console.WriteLine();

                        //fills the gameboard with the guess and colours accordingly
                        FillGameBoard(gameBoard, i);
                        continue;

                    }
                    else
                    {

                        //states the word is not in the given lists and retrieves key to allow user to try again
                        Console.WriteLine("The word is not in the list. \nPress Enter to try again");
                        Console.ReadLine();
                        Console.Clear();
                        Console.WriteLine();

                        //colours the alphabet according to the list
                        AlphaColours();
                        Console.WriteLine();

                        //based on which guess number they are on, prints out gameboard
                        if (i != 0)
                        {
                            //changes guess value to dashes to print empty board, with previous guesses
                            guess = "_____";
                            FillGameBoard(gameBoard, i);
                        }
                        else
                        {
                            //prints empty gameboard 
                            GameBoard(gameBoard);
                        }

                        //the value of i decreases by one to default back to the guess user was on
                        i--;
                        continue;
                    }
                }
                else
                {
                    //states guess can only be 5 letters (no special chars) and retrieves key to allow user to try agan
                    Console.WriteLine("The word can only be 5 letters (no special characters). \nPress Enter to try again");
                    Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine();

                    //colours the alphabet accordingly 
                    AlphaColours();
                    Console.WriteLine();

                    //based on which guess number they are on, prints out gameboard
                    if (i != 0)
                    {
                        //changes guess value to dashes to print empty board, with previous guesses
                        guess = "_____";
                        FillGameBoard(gameBoard, i);
                    }
                    else
                    {
                        //prints empty gameboard 
                        GameBoard(gameBoard);
                    }

                    //the value of i decreases by one to default back to the guess user was on
                    i--;
                    continue;
                }

            }
        }

        //fills gameboard with the letters of each guess
        private static void FillGameBoard(char[,] gameboard, int i)
        {
            //for loops that loops through the 2D array
            for (int row = 0; row < gameboard.GetLength(0); row++)
            {
                for (int col = 0; col < gameboard.GetLength(1); col++)
                {
                    //if statement for when row is equal to the guess number 
                    if (row == i)
                    {
                        //changes values in each colomn to the letter in the same index in the guess
                        gameboard[row, col] = guess[col];
                    }
                }

            }

            //after filling loop in with guess, colours each letter according to rules
            ColorsGameBoard(gameboard);
        }

        //colours in the gameboard with the appopriate colours
        private static void ColorsGameBoard(char[,] gameboard)
        {
            //creating two lists that take the list the green and yellow letters and delete all repeating letters
            List<char> storeLetterG = storeGreen.Distinct().ToList();
            List<char> storeLetterY = storeYellowAr.Distinct().ToList();

            //loops through the 2D array 
            for (int row = 0; row < gameboard.GetLength(0); row++)
            {
                //writes the guess number for each row 
                Console.WriteLine("Guess: " + (row + 1));

                for (int col = 0; col < gameboard.GetLength(1); col++)
                {
                    //prints left border line
                    Console.Write("|");

                    //changes colour of letter based on which coloured list it is in 
                    if (storeLetterG.Contains(gameboard[row, col]) == true && answer[col] == gameboard[row,col])
                    {
                        //changes letter colour to green and prints it 
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(gameboard[row, col]);
                    }
                    else if (storeLetterY.Contains(gameboard[row, col]) == true)
                    {
                        //changes letter colour to yellow and prints it 
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(gameboard[row, col]);
                    }
                    else
                    {
                        //changes letter colour to grey and prints it 
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(gameboard[row, col]);
                    }

                    //resets colour back to white
                    Console.ResetColor();

                    //prints right side border
                    Console.Write("|");
                }

                //presses enter key to send cursor to next line for next row
                Console.WriteLine();
            }
        }

        //colours the string of alphabets after each guess 
        private static void AlphaColours()
        {
            //creating three lists that take the list of each colour and delete all repeating letters
            List<char> storeLetterG = storeGreen.Distinct().ToList();
            List<char> storeLetterY = storeYellow.Distinct().ToList();
            List<char> storeLetterGray = storeGrey.Distinct().ToList();

            //for loop that loops through every letter of alphabet 
            for (int f = 0; f < alpha.Length; f++)
            {
                //based on which coloured lists contains that alphabet letter, prints that letter in that corresponding colour
                if (storeLetterG.Contains(alpha[f]) == true)
                {
                    //changes colour to gray and writes the alphabet letter
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(alpha[f]);

                    //resets colour back to white and continues loop
                    Console.ResetColor();
                    continue;
                }
                else if (storeLetterY.Contains(alpha[f]) == true)
                {
                    //changes colour to gray and writes the alphabet letter
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(alpha[f]);

                    //resets colour back to white and continues loop
                    Console.ResetColor();
                    continue;
                }
                else if (storeLetterGray.Contains(alpha[f]) == true)
                {
                    //changes colour to gray and writes the alphabet letter
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(alpha[f]);

                    //resets colour back to white and continues loop
                    Console.ResetColor();
                    continue;
                }
                else
                {
                    //writes the letter of the alphabet coloured white and continues loop
                    Console.Write(alpha[f]);
                    continue;
                }
            }
        }

        //changes user statistics and outputs feedback when user gives wrong answer
        private static void WrongAns ()
        {
            //clears screen and displays correct answer
            Console.Clear();
            Console.WriteLine("The correct answer was " + answer + ".");

            //updates user statistics (games played, if the current game streak is more than the max game streak, then update max game streak and set current streak to 0)
            gamesPlayed = gamesPlayed + 1;
            if (currGamesWonStreak > maxGamesWonStreak)
            {
                maxGamesWonStreak = currGamesWonStreak;
            }
            currGamesWonStreak = 0;

            //updates and prints user statistics as well as saves data
            GameFinish();
        }

        //provides user with options at the end of the game and returns chosen option number
        private static int EndMenuOptions ()
        {
            //declares varibale to store users chosen option
            int chosen;

            //outputs menu for user to choosen from
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. Play Again");
            Console.WriteLine("2. Reset Statistics");
            Console.WriteLine("3. Exit");

            //retrieves chosen option and returns it 
            chosen = Convert.ToInt32(Console.ReadLine());
            return chosen;
        }

        //reset all user statistics and save them to user statistics file
        private static void ResetStats ()
        {
            //try-catch to reset statistics and save them back to file
            try
            {
                //opens file to write to
                outFile = File.CreateText("UserStats.txt");

                //sets all user stats to 0
                gamesPlayed = 0;
                gamesWon = 0;
                currGamesWonStreak = 0;
                maxGamesWonStreak = 0;
                
                //uses for loop to set all indices in win distribution array to 0
                for (int j = 0; j < winGD.Length; j++)
                {
                    winGD[j] = 0;
                }
                
                //writes the reset statitics back to file to save data
                outFile.WriteLine(gamesPlayed);
                outFile.WriteLine(gamesWon);
                outFile.WriteLine(currGamesWonStreak);
                outFile.WriteLine(maxGamesWonStreak);
                outFile.WriteLine(winGD[0] + "," + winGD[1] + "," + winGD[2] + "," + winGD[3] + "," + winGD[4] + "," + winGD[5]);
               
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
            finally
            {
                //if statement to close file if it is not null
                if (outFile == null)
                {
                    outFile.Close();
                }
            }
            
            //clears console and states that statitics have been reset
            Console.Clear();
            Console.WriteLine("Statistic Have Been Reset");

        }

        //when user finishes game(either wins or loses): caluclates win percentage, presents user statistics to player and saves data
        private static void GameFinish ()
        {
            //calculates win percentage
            int winPercentage;
            winPercentage = (gamesWon / gamesPlayed) * 100;

            //writes each of the user statistics to player
            Console.WriteLine("Games Played: " + gamesPlayed);
            Console.WriteLine("Games Won: " + gamesWon);
            Console.WriteLine("Current Win Streak: " + currGamesWonStreak);
            Console.WriteLine("Max Win Streak: " + maxGamesWonStreak);
            Console.WriteLine("Win Distribution: ");
            Console.WriteLine("Guess 1: " + winGD[0]);
            Console.WriteLine("Guess 2: " + winGD[1]);
            Console.WriteLine("Guess 3: " + winGD[2]);
            Console.WriteLine("Guess 4: " + winGD[3]);
            Console.WriteLine("Guess 5: " + winGD[4]);
            Console.WriteLine("Guess 6: " + winGD[5]);
            Console.WriteLine(winPercentage + "%");

            //try-catch-finally block to open file and save user statistics to 
            try
            {
                //opens file to write to
                outFile = File.CreateText("UserStats.txt");

                //writes and saves data to user stats file
                outFile.WriteLine(gamesPlayed);
                outFile.WriteLine(gamesWon);
                outFile.WriteLine(currGamesWonStreak);
                outFile.WriteLine(maxGamesWonStreak);
                outFile.WriteLine(winGD[0] + "," + winGD[1] + "," + winGD[2] + "," + winGD[3] + "," + winGD[4] + "," + winGD[5]);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
            finally
            {

                //if statement to close file if it is not null
                if (outFile != null)
                {
                    outFile.Close();
                } 
            }
        }

        //centres cursor in order to centre text on screen
        private static void CentreCursor (string text)
        {
            int positionCurs;

            //dividing console width by 2 and subtracting the length of string to get middle postion
            positionCurs = ((Console.WindowWidth / 2) - (text.Length / 2));

            //setting cursor to desired position
            Console.Write("".PadRight(positionCurs));
        }
    }
}
