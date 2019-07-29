/*
 * File:            Game.cs
 * Creator:         Tyler Bartnick
 * Last Updated:    7/27/2019
 */

using System;
using System.Collections.Generic;
using System.Linq;

using WarCardGame.Helpers;

namespace WarCardGame.Models {

    /// <summary>
    /// The Game class represents a game of War and contains all logic required to play the game.
    /// </summary>
    public class Game {

        /// <value>Constant denoting the minimum cards a player needs on hand to engage opponent in war.</value>
        public const int MIN_CARDS_FOR_WAR = 2;

        /// <value>Constant denoting the interval at which each player shuffles their decks to reduce total game time.</value>
        private const int SHUFFLE_INTERVAL = 25;

        /// <value>Constant string inserted before all new output to help keep a visual hierarchy.</value>
        public const string SEPERATOR = "---------------------------------------------";

        /// <value>A readonly reference to the first player.</value>
        public Player Player1 { get; }

        /// <value>A readonly reference to the second player.</value>
        public Player Player2 { get; }

        /// <value>A counter that keeps track of the total number of turns taken</value>
        private int totalTurns;

        /// <summary>
        /// Instantiates a Game object.
        /// </summary>
        /// <param name="p1Name">A string representing the name of Player 1</param>
        /// <param name="p2Name">A string representing the name of Player 2</param>
        public Game(string p1Name, string p2Name) {
            Player1 = new Player(p1Name);
            Player2 = new Player(p2Name);
            DeckManager.CreateDecks(Player1, Player2);
            totalTurns = 0;
        }

        /// <summary>
        /// Main game loop. Processes user input for the menu system and then executes the required function(s) as specified.
        /// </summary>
        public void Play() {
            do {
                DisplayInGameMenu();
                char userInput = InputHelpers.GetUserChoice();
                switch (userInput) {
                    case 'N':
                    case 'n':
                        NextTurn();
                        break;
                    case 'V':
                    case 'v':
                        DisplayScore();
                        break;
                    case 'C':
                    case 'c':
                        Console.Clear();
                        break;
                    case 'F':
                    case 'f':
                        RunInBatchMode();
                        break;
                    case 'H':
                    case 'h':
                        DisplayGameInstructions();
                        break;
                    case 'Q':
                    case 'q':
                        ForceGameOver();
                        break;
                    default:
                        Console.WriteLine("Invalid input, please enter \"n\", \"v\", \"f\", \"c\", \"h\", or \"q\".");
                        break;
                }
            } while (!GameOver());
            DisplayGameResults();
        }

        /// <summary>
        /// Plays the next turn in the game.
        /// </summary>
        public void NextTurn() {
            totalTurns++;

            // the play area represents the area on the "table" where all cards would
            // normally be in play. An ICollection is used to serve as a type of card
            // pool since the winner of the round takes all cards.
            ICollection<Card> playArea = new List<Card>();

            // each player plays the card on top of their deck
            Card p1Card = Player1.Deck.Dequeue();
            Card p2Card = Player2.Deck.Dequeue();

            // the cards are then added to the play area
            playArea.Add(p1Card);
            playArea.Add(p2Card);

            // check to see if war has been triggered
            // this comparison is a while loop because the war condition can be triggered
            // a number of times in sequence. Doing it this way demands that the final
            // cards drawn for each player be different before doing the final comparison
            // to see who won the round.
            while (p1Card.Value == p2Card.Value) {
                Console.WriteLine(SEPERATOR);
                Console.WriteLine("WAR!!!");
                // make sure each player has enough cards for war.
                // if you don't have enough cards, you can't go to war
                // and you automatically lose.
                // similar enough to real life to make sense - you can't go to war without proper supplies.
                // Since the GameOver() method checks to see if any of the players is out of cards,
                // the player without enough cards for war has their deck discarded which forces an end to the game.
                if (!PlayerHasEnoughCardsForWar(Player1)) {
                    Player1.Deck.Clear();
                    Console.WriteLine($"{Player1.Name} doesn't have enough cards for war.\n{Player2.Name} wins!");
                    break;
                }

                if (!PlayerHasEnoughCardsForWar(Player2)) {
                    Player2.Deck.Clear();
                    Console.WriteLine($"{Player2.Name} doesn't have enough cards for war.\n{Player1.Name} wins!");
                    break;
                }

                // the rules of war dictate that each user must add one card face down
                // and then add another face up and the second card drawn in that sequence
                // is the one used for determining the winner of war and the round
                // this operates the same way as beginning a turn, including adding each card
                // drawn to the play area for collection by the winner
                playArea.Add(Player1.Deck.Dequeue());
                playArea.Add(Player2.Deck.Dequeue());
                p1Card = Player1.Deck.Dequeue();
                p2Card = Player2.Deck.Dequeue();

                playArea.Add(p1Card);
                playArea.Add(p2Card);
            }

            // determine winner and assign cards in play area to winner
            if (p1Card.Value > p2Card.Value) {
                Console.WriteLine(SEPERATOR);
                Console.WriteLine($"{Player1.Name} wins the round!");
                Player1.AddCardsToDeck(playArea);
            } else if (p1Card.Value < p2Card.Value) {
                Console.WriteLine(SEPERATOR);
                Console.WriteLine($"{Player2.Name} wins the round!");
                Player2.AddCardsToDeck(playArea);
            }

            Console.WriteLine($"{Player1.Name} played: " + p1Card);
            Console.WriteLine($"{Player2.Name} played: " + p2Card);
            Console.WriteLine($"Turns taken: {totalTurns}");
            playArea.Clear();

            // A game of War without shuffling each player's deck every so often can run indefinitely (or seem to).
            // Some variants of the classic game suggest that a player must shuffle their deck at various
            // intervals or when they have used all of their cards before using the cards they have won
            // since the last time they shuffled.
            // Since a previously run simulation has taken upwards of 3.2 million turns, a good middle ground
            // is to shuffle on a specified interval. This greatly improves the speed of a game.
            // Most of the games that use this logic end well before 1000 turns.
            if (totalTurns % SHUFFLE_INTERVAL == 0) {
                ShufflePlayerDeck(Player1);
                ShufflePlayerDeck(Player2);
            }

        }

        /// <summary>
        /// Determines whether or not the specified player has enough cards to engage in war.
        /// </summary>
        /// <param name="player">An instance of Player</param>
        /// <returns>boolean</returns>
        public bool PlayerHasEnoughCardsForWar(Player player) {
            if (player.Deck.Count < MIN_CARDS_FOR_WAR)
                return false;
            return true;
        }

        /// <summary>
        /// Determines the state of the game. Game is over once any player is out of cards.
        /// </summary>
        /// <returns>boolean</returns>
        private bool GameOver() {
            if (!Player1.Deck.Any() || !Player2.Deck.Any())
                return true;
            return false;
        }

        /// <summary>
        /// Forces a game to end by destroying all cards in play.
        /// </summary>
        private void ForceGameOver() {
            Player1.Deck.Clear();
            Player2.Deck.Clear();
        }

        /// <summary>
        /// Runs game in a non-interactive state, continually playing the next turn until a winner is decided.
        /// </summary>
        private void RunInBatchMode() {
            while (!GameOver()) {
                NextTurn();
            }
        }

        /// <summary>
        /// Shuffles the deck of a player.
        /// </summary>
        /// <param name="player">The player who's deck is to be shuffled.</param>
        private void ShufflePlayerDeck(Player player) {
            List<Card> pDeck = player.Deck.ToList();
            DeckManager.Shuffle(pDeck);
            player.Deck = new Queue<Card>();
            player.AddCardsToDeck(pDeck);
        }

        /// <summary>
        /// Displays the rules of the game.
        /// </summary>
        public static void DisplayGameInstructions() {
            Console.WriteLine(SEPERATOR);
            Console.WriteLine("War - Game Rules and Objectives");
            Console.WriteLine("Objective:");
            Console.WriteLine("\tBe the first player to obtain all 52 playing cards");
            Console.WriteLine("Game Rules:");
            Console.WriteLine("\tThe deck is to be divided evenly between two players.");
            Console.WriteLine("\tEvery turn, each player draws and plays the top card from his/her deck.");
            Console.WriteLine("\tThe player who dealt the card with the higher value (Aces are high) wins the round.");
            Console.WriteLine("\tThe winner collects all cards that have been played that round and places them on the bottom of his/her deck.");
            Console.WriteLine("\tIf the cards played by each player are of the same value, they are to be engaged in war!");
            Console.WriteLine("\tTo engage in war, each player must draw two cards from his/her deck and place them in the play area.");
            Console.WriteLine("\tThe values of the last card drawn for each player are to be compared. The higher card wins.");
            Console.WriteLine("\tThe sequence for war can be repeated as many times as necessary if war results in yet another war scenario.");
            Console.WriteLine("\tThe winner of war collects all cards that have been played and places them on the bottom of his/her deck.");
            Console.WriteLine("\tThere is one caveat to war, however.");
            Console.WriteLine("\tIf a player is to engage in war, they must have at least two cards in their deck.");
            Console.WriteLine("\tIf they do not have at least two cards in their deck, they cannot partake and war and automatically forfeit the game.");
            Console.WriteLine("IMPORTANT!");
            Console.WriteLine("Most actions will be taken care of for you. All you have to do is issue a command from the menu and the game will handle the rest.");
            Console.WriteLine("Examples of this are shuffling and dealing the initial deck and drawing the next card to begin the next round.");

        }

        /// <summary>
        /// Diplays the in-game menu options.
        /// </summary>
        private void DisplayInGameMenu() {
            Console.WriteLine(SEPERATOR);
            Console.WriteLine("Enter your choice: ");
            Console.WriteLine("N) Next turn");
            Console.WriteLine("V) View score");
            Console.WriteLine("F) Finish game in batch mode");
            Console.WriteLine("C) Clear screen");
            Console.WriteLine("H) Help");
            Console.WriteLine("Q) Quit");
            Console.Write("> ");
        }

        /// <summary>
        /// Displays the results from the game based off the number of cards in each player's deck.
        /// </summary>
        private void DisplayGameResults() {
            if (Player1.Deck.Count == Player2.Deck.Count) {
                Console.WriteLine(SEPERATOR);
                Console.WriteLine("GAME ENDED! IT'S A DRAW!");
            } else {
                Player winner = (Player1.Deck.Count > Player2.Deck.Count) ? Player1 : Player2;
                Console.WriteLine(SEPERATOR);
                Console.WriteLine($"{winner.Name} WINS!");
                Console.WriteLine($"{totalTurns} turns taken");
            }
        }

        /// <summary>
        /// Displays the relative score of a game, or more accurately, the number of cards each player possesses.
        /// </summary>
        private void DisplayScore() {
            Console.WriteLine(SEPERATOR);
            Console.WriteLine($"{Player1.Name} has {Player1.Deck.Count} cards.");
            Console.WriteLine($"{Player2.Name} has {Player2.Deck.Count} cards.");
        }
    }
}
