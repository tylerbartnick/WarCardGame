/*
 * File:            Program.cs
 * Creator:         Tyler Bartnick
 * Last Updated:    7/27/2019
 */
using System;

using WarCardGame.Models;
using WarCardGame.Helpers;

namespace WarCardGame {
    class Program {

        static void Main(string[] args) {
            Console.WriteLine("+-------------------------------------------+");
            Console.WriteLine("|                    War                    |");
            Console.WriteLine("+-------------------------------------------+");

            Game game = CreateNewGame();
            game.Play();
        }

        /// <summary>
        /// Creates a new Game object and returns it.
        /// It prompts users for the names of the players before
        /// instantiating said Game object.
        /// </summary>
        /// <returns>A Game object</returns>
        static Game CreateNewGame() {
            string player1Name, player2Name;

            while (true) {
                // get player names and make sure they contain only alpha-numeric characters without whitespace
                player1Name = InputHelpers.GetUserInput("Enter name for Player 1:\n> ", InputHelpers.DefaultInvalidCharacters);
                player2Name = InputHelpers.GetUserInput("Enter name for Player 2:\n> ", InputHelpers.DefaultInvalidCharacters);

                // ensure player names are different, case-insensitive
                if (player1Name.ToLower() == player2Name.ToLower()) {
                    Console.WriteLine("Player names cannot be the same. Try again.");
                    continue;
                }
                break;
            }

            return new Game(player1Name, player2Name);
        }
    }
}
