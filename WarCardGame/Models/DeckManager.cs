/*
 * File:            DeckManager.cs
 * Creator:         Tyler Bartnick
 * Last Updated:    7/27/2019
 */

using System;
using System.Collections.Generic;

namespace WarCardGame.Models {
    /// <summary>
    /// The Deck Manager class contains definitions for generating and manipulating a standard deck of 52 playing cards.
    /// </summary>
    public static class DeckManager {

        /// <summary>
        /// Creates, shuffles, and equally distributes a deck of 52 playing cards between two players.
        /// </summary>
        /// <param name="p1">Player 1</param>
        /// <param name="p2">Player 2</param>
        public static void CreateDecks(Player p1, Player p2) {
            // Ensure decks are completely empty first
            p1.Deck.Clear();
            p2.Deck.Clear();

            IList<Card> cards = GenerateCards();

            Shuffle(cards);
            Deal(cards, p1, p2);
        }

        /// <summary>
        /// Generates a standard deck of 52 playing cards with 4 suits.
        /// </summary>
        /// <returns>A List of type Card containing all 52 playing cards, in order.</returns>
        public static IList<Card> GenerateCards() {
            IList<Card> cards = new List<Card>();
            // start lower bound at lowest value in a deck of cards, face card values are just the next whole numbers after the cardinal numbers
            for (int i = 2; i < 15; i++) {
                foreach (string suit in Card.Suits) {
                    cards.Add(new Card {
                        Suit = suit,
                        Value = i
                    });
                }
            }

            return cards;
        }

        /// <summary>
        /// Implements an in-place Fisher-Yates card shuffling algorithm.
        /// </summary>
        /// <remarks>Pseudo-code for the algorithm can be found here: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm </remarks>
        /// <param name="cards">A List of type Card to be shuffled in place.</param>
        public static void Shuffle(IList<Card> cards) {
            Random random = new Random();

            int n = cards.Count;
            for (int i = 0; i < n; i++) {
                // swap the current index with a random index
                int r = i + random.Next(n - i);
                Swap(cards, r, i);
            }
        }

        /// <summary>
        /// Swaps the values of a list at the specified indices.
        /// </summary>
        /// <param name="cards">A List of type Card.</param>
        /// <param name="index1">First index.</param>
        /// <param name="index2">Second index.</param>
        private static void Swap(IList<Card> cards, int index1, int index2) {
            Card temp = cards[index1];
            cards[index1] = cards[index2];
            cards[index2] = temp;
        }

        /// <summary>
        /// Distributes a deck of cards between two players.
        /// </summary>
        /// <param name="cards">A List of type Card</param>
        /// <param name="p1">A reference to Player 1</param>
        /// <param name="p2">A reference to Player 2</param>
        private static void Deal(IList<Card> cards, Player p1, Player p2) {
            for (int i = 0; i < cards.Count; i++) {
                if (i % 2 == 0) {
                    p1.Deck.Enqueue(cards[i]);
                } else {
                    p2.Deck.Enqueue(cards[i]);
                }
            }
        }
    }
}
