/*
 * File:            Card.cs
 * Creator:         Tyler Bartnick
 * Last Updated:    7/27/2019
 */

namespace WarCardGame.Models {

    /// <summary>
    /// The Card class represents a stanard playing card.
    /// </summary>
    public class Card {

        /// <value>A static string array representing all possible suits within a standard deck of cards.</value>
        public static readonly string[] Suits = { "Club", "Diamond", "Heart", "Spade" };

        /// <value>The card's integer value used in comparisons within the game.</value>
        public int Value { get; set; }

        /// <value>The card's suit.</value>
        public string Suit { get; set; }

        /// <summary>
        /// Creates and returns a string representation of a Card object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString() {
            string faceCardValue = "";

            if (Value >= 2 && Value <= 10) {
                return Value.ToString() + " " + Suit;
            } else if (Value == 11) {
                faceCardValue = "J";
            } else if (Value == 12) {
                faceCardValue = "Q";
            } else if (Value == 13) {
                faceCardValue = "K";
            } else if (Value == 14) {
                faceCardValue = "A";
            }

            return faceCardValue + " " + Suit;
        }
    }
}
