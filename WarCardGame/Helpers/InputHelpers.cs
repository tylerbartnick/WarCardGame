/*
 * File:            InputHelpers.cs
 * Creator:         Tyler Bartnick
 * Last Updated:    7/27/2019
 */

using System;
using System.IO;

namespace WarCardGame.Helpers {

    /// <summary>
    /// The InputHelpers class is a static class that provides utility methods for accepting and validating user input.
    /// </summary>
    public static class InputHelpers {

        /// <value>A static string array containing all non-alpha-numeric characters.</value>
        public static readonly string[] DefaultInvalidCharacters = { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+", "-", "[", "]", "{", "}", "\\", "\'", "\"", ";", ":", "/", "?", ".", "<", ">", ",", "|", " " };

        /// <value>A readonly integer value denoting the min length of a given user input.</value>
        public static readonly int MIN_LENGTH = 1;

        /// <value>A readonly integer value denoting the max length of a given user input.</value>
        public static readonly int MAX_LENGTH = 15;

        /// <summary>
        /// Reads the next line of characters from STDIN and returns them
        /// </summary>
        /// <param name="msg">The message to display to the user before expecting input.</param>
        /// <param name="invalidChars">An array of all characters to disallow.</param>
        /// <returns>A string of characters.</returns>
        public static string GetUserInput(string msg, string[] invalidChars = null) {

            // workaround that allows an array to be specified as an optional argument
            // if null, initialize local variable to be an array of type string of length 0
            invalidChars = invalidChars ?? new string[0];

            string userInput = "";
            while (true) {
                Console.Write(msg);
                try {
                    userInput = Console.ReadLine().Trim();
                } catch (IOException e) {
                    // Write error message to STDERR and exit the program with an exit code of -1
                    // to notify the OS that something went wrong.
                    TextWriter err = Console.Error;
                    err.WriteLine(e.Message);
                    Environment.Exit(-1);
                }

                // test for invalid characters and force user to enter a new value if they entered any of the
                // invalid/illegal characters
                if (!ContainsInvalidCharacters(userInput, invalidChars)) {
                    Console.WriteLine("Whitespace and the following characters are not allowed: ");
                    Console.WriteLine(string.Join(' ', invalidChars));
                    Console.WriteLine("Please try again");
                    continue;
                }

                // Ensure the value entered by the user is of an appropriate length and is not null
                if ((userInput.Length >= MIN_LENGTH && userInput.Length <= MAX_LENGTH) || !string.IsNullOrWhiteSpace(userInput))
                    break;
                Console.WriteLine($"Please provide a string between {MIN_LENGTH} and {MAX_LENGTH} characters.");

            }

            return userInput;
        }

        /// <summary>
        /// Tests whether or not the input string contains any of specified invalid characters.
        /// </summary>
        /// <param name="input">The string to test.</param>
        /// <param name="invalidChars">An array of invalid/disallowed characters.</param>
        /// <returns>boolean</returns>
        public static bool ContainsInvalidCharacters(string input, string[] invalidChars) {
            bool isValid = true;
            foreach (var c in invalidChars) {
                if (input.ToLower().Contains(c)) {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Reads the next character from STDIN.
        /// </summary>
        /// <returns>The next character in the stream.</returns>
        public static char GetUserChoice() {
            char userInput = (char)Console.Read();
            Console.ReadLine();
            return userInput;
        }

        /// <summary>
        /// Reads input from the user, but only considers pressing the ENTER key to be 
        /// valid input.
        /// </summary>
        public static void ForceUserToPressEnterKey() {
            ConsoleKeyInfo keyInfo;
            do {
                keyInfo = Console.ReadKey();
            } while (keyInfo.Key != ConsoleKey.Enter);
        }
    }
}
