/*
 * File:            Player.cs
 * Creator:         Tyler Bartnick
 * Last Updated:    7/27/2019
 */
using System.Collections.Generic;

namespace WarCardGame.Models {

    /// <summary>
    /// The Player class contains the cards for each player in the game.
    /// </summary>
    public class Player {

        /// <value>Gets and sets the Player's display name.</value>
        public string Name { get; set; }

        /// <value>Gets and sets the Player's deck of cards.</value>
        public Queue<Card> Deck { get; set; }

        /// <summary>
        /// Player constructor - expects a single string argument to represent the name of the given player.
        /// </summary>
        /// <param name="name">A string representing the name of the player.</param>
        public Player(string name) {
            Name = name;
            Deck = new Queue<Card>();
        }

        /// <summary>
        /// Adds a collection of cards to the player's deck. Used when winning rounds/war.
        /// </summary>
        /// <param name="cards">An ICollection of type Card</param>
        public void AddCardsToDeck(ICollection<Card> cards) {
            foreach (Card card in cards) {
                Deck.Enqueue(card);
            }
        }
    }
}
