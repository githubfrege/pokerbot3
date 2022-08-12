using System;
using System.Collections.Generic;
using System.Linq;

namespace pokerbot3
{
 
    class Program
    {
        public static ulong Deck = 0b_11111111_1111100_11111111_1111100_11111111_1111100_11111111_1111100;
        private static int compareKvp(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
        {
            return a.Key.CompareTo(b.Key);
        }
        public static ulong parseAsBitField(List<KeyValuePair<int, int>> cards)
        {
            ulong bf = 0;
            foreach (var card in cards)
            {
                bf |= 1UL << (card.Key + (15 * card.Value));
            }
            return bf;
          
        }
        private static bool isStraight(long solo)
        {
            long lsb = solo &= -solo;
            long normalized = solo / lsb;
            return normalized == 31;

        }
        public static int getScore(ulong bf)
        {
            getFields(bf, out int solo, out long ranksField, out bool flush);
            bool straight = isStraight(solo);
            if (straight && flush)
            {
                if (solo == 31744)
                {
                    return 10;
                }
                return 9;
            }
            int score = 0;
            switch (ranksField % 15)
            {
                case 1:
                    return 8;
                case 10:
                    return 7;
                case 9:
                    score = 4;
                    break;
                case 7:
                    score = 3;
                    break;
                case 6:
                    score = 2;
                    break;
                case 5:
                    score = 1;
                    break;
                default:
                    break;
            }
            if (flush)
            {
                return 6;
            }
            else if (straight || solo == 16444)
            {

                return 5;
            }
            return score;
        }
        public static void getFields(ulong bf, out int solo, out long ranksField, out bool flush)
        {
            solo = 0;
            ranksField = 0;
            flush = false;
            Dictionary<int, int> instances = new Dictionary<int, int>();
            for (int i = 0; i < 3; i++)
            {
                int flushIdx = 0;
                for (int j = 2; j <= 14;j++)
                {
                    flushIdx++;
                    if (flushIdx == 4)
                    {
                        flush = true;
                    }
                    int offset = 0;
                    if ((bf & (1UL <<(j + (15 * i)))) != 0){
                        solo |= 1 << j;
                        if (!instances.ContainsKey(j))
                        {
                            instances.Add(j, 1);
                        }
                        else
                        {
                            instances[j] = instances[j] + 1;
                        }
                        offset = instances[j];

                    }
                    long addition = 1 << (j << 2);
                    addition = addition << offset;
                    ranksField |= addition;
                }
            }
            ranksField = ranksField >> 1;

        }
        private static IEnumerable<ulong> cardCombos(ulong cards, int count, int discarded)
        {
            for (int idx = discarded; idx <= 60; idx++)
            {
                ulong card = cards & (1UL << idx);
                if (card > 0)
                {
                    if (count == 1)
                    {
                        yield return card;
                    }
                    foreach (ulong result in cardCombos(cards ^ card, count - 1, idx))
                    {
                        yield return card |= result;
                    }
                }
            }
        }
    
        /*public static long getRanksField(ulong bf)
        {
            long ranksField = 0;
            Dictionary<int, int> instances = new Dictionary<int, int>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 2; j <= 14; j++)
                {
                    int offset = 0;
                    if ((bf & (1UL << (j + (16 * i)))) != 0)
                    {

                        if (!instances.ContainsKey(j))
                        {
                            instances.Add(j, 1);
                        }
                        else
                        {
                            instances[j] = instances[j] + 1;
                        }
                        offset = instances[j];

                    }
                    long addition = 1 << (j << 2);
                    addition = addition << offset;
                    ranksField |= addition;
                        
                }
                
            }
        }*/
        static void Main(string[] args)
        {
            
        }
    }
}
