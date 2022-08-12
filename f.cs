using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
namespace pokerbot3
{
 
    static class Program
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
        private static int getHighestRank(long allCards)
        {
            return 63 - BitOperations.LeadingZeroCount((ulong)allCards | 1);
        }

        private static long getTieBreaker(long ranksField)
        {
            int first = getHighestRank(ranksField);
            int tiebreaker = first << 16;
            for (int i = 0; i < 4; i++)
            {
                first = getHighestRank(ranksField ^ (1 << first));
                tiebreaker |= first << 16 - ((i + 1) * 4);

            }
            return tiebreaker;
        }
        public static int compareScore((int score ,long tiebreaker) score1, (int score,long tiebreaker) score2)
        {
            
        }
        public static (int mainScore, long tieBreaker) getScore(ulong bf)
        {
            getFields(bf, out int solo, out long ranksField, out bool flush);
            bool straight = isStraight(solo);
            int score = 0;
            if (straight && flush)
            {
                if (solo == 31744)
                {
                    score =  10;
                }
                else
                {
                    score = 9;
                }
                
            }
            switch (ranksField % 15)
            {
                case 1:
                    score = 8;
                    break;
                case 10:
                    score =  7;
                    break;
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
                score = 6;
            }
            else if (straight || solo == 16444)
            {

                score =  5;
            }
            return (score, getTieBreaker(ranksField));
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
        public static IEnumerable<ulong> ToIEnum(this ulong num)
        {
            for (int i = 2; i <= 60; i++)
            {
                if ((num & (1UL << i)) > 0)
                {
                    yield return 1UL << i;
                }
            }
        }
        private static IEnumerable<ulong> cardCombos(IEnumerable<ulong> cards, int count)
        {
            int i = 0;
            foreach (var card in cards)
            {
                if (count == 1)
                {
                    yield return card;
                }

                else
                {
                    foreach (var result in cardCombos(cards.Skip(i + 1), count - 1))
                    {

                        yield return result | card;
                    }
                }

                ++i;
            }
        }
        public static float GetOdds(List<KeyValuePair<int, int>> holeCardsKvpList, List<KeyValuePair<int, int>> tableKvpList)
        {
            float heroWins = 0; //if player wins
            float villainWins = 0; //if opponent wins
            //float villainCardSets = 0;
            ulong holes = parseAsBitField(holeCardsKvpList);
            ulong communityCards = parseAsBitField(tableKvpList);
            ulong availableCards = Deck ^ (holes | communityCards);
            foreach (ulong villainHoles in cardCombos(availableCards.ToIEnum(), 2)) //every combination of cards our opponent may have
            {
                /*villainCardSets++;
                Console.WriteLine(villainCardSets);*/
                //List<ulong> validHands = new List<ulong>(); //all 5-card sets that may potentially appear on the table
                ulong currentAvailableCards = availableCards ^ villainHoles;
                foreach (ulong cardAdditions in cardCombos(currentAvailableCards.ToIEnum(),5 - tableKvpList.Count)) //all combinations of cards that may be added to the existing table
                {
                    ulong currentHand = communityCards | cardAdditions;
                    (float heroPoints, float villainPoints) scoreUpdate = updateScore(holeCardsKvpList, villainCardSet, currentHand);
                    heroWins += scoreUpdate.heroPoints;
                    villainWins += scoreUpdate.villainPoints;

                }

            }
            return heroWins / (heroWins + villainWins);

        }
        private static ulong handToPlay(ulong holes, ulong cardsOnTable)
        {

            (int mainScore, long tieBreaker) max = (-100000, -100000);
            ulong maxHand = 0;
            foreach (ulong combo in cardCombos(cardsOnTable.ToIEnum(), 3))
            {
                (int mainScore, long tieBreaker) currentScore = getScore(combo | holes);
                if  ((currentScore.mainScore.CompareTo(max.mainScore) != 0 ? currentScore.mainScore.CompareTo(max.mainScore) : currentScore.tieBreaker.CompareTo(max.tieBreaker)) > 0)
                {
                    max = currentScore;
                    maxHand = combo | holes;
                }
)
            }
            allCombos.AddRange(cardCombos(tableCards, 4));
           /* foreach (var combosList in allCombos)
            {

                if (combosList.Count == 3)
                {
                    List<KeyValuePair<int, int>> myHand = new List<KeyValuePair<int, int>>(combosList);
                    myHand.AddRange(holeCards);
                    Hand hand = ParseAsHand(myHand);
                    if (hand > maxHand)
                    {
                        maxHand = hand;
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        List<KeyValuePair<int, int>> myHand = new List<KeyValuePair<int, int>>(combosList);
                        myHand.Add(holeCards[i]);
                        Hand hand = ParseAsHand(myHand);

                        if (hand > maxHand)
                        {
                            maxHand = hand;
                        }
                    }
                }


            }*/
            return maxHand;
        }
        /*private static IEnumerable<ulong> cardCombos(ulong cards, int count, int discarded)
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
        }*/

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
