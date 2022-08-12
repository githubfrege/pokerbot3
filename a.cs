using System;
using System.Collections.Generic;
using System.Linq;

namespace pokerbot3
{
 
    class Program
    {
        public static ulong Deck = 0b_11111000_11111000_11111000_11111000_11111000_11111000_11111000_11111000;
        private static int compareKvp(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
        {
            return a.Key.CompareTo(b.Key);
        }
        public static ulong parseAsBitField(List<KeyValuePair<int, int>> cards)
        {
            ulong bf = 0;
            foreach (var card in cards)
            {
                bf |= (1 << car)
            }
            
          
        }
        public static int getScore(ulong bf)
        {

        }
        public static long getSolo(ulong bf)
        {

        }
        public static long getRanksField(long solo)
        {

        }
        static void Main(string[] args)
        {
            
        }
    }
}
