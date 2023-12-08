
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

class Day7
{
    static int Main()
    {
        DoPart1("sample.txt");
        DoPart1("input.txt");
        DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }

    class Hand
    {
        public char[] Cards;
        public int Bid;
        public int NumberMatching;
        public int CardRank;
        public int SecondCardRank;
    };

static void DoPart1(string inputFile)
    {
        Dictionary<char, int> ranks = new Dictionary<char, int>()
        {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'J', 11 },
            { 'T', 10 },
            { '9', 9 },
            { '8', 8 },
            { '7', 7 },
            { '6', 6 },
            { '5', 5 },
            { '4', 4 },
            { '3', 3 },
            { '2', 2 }

        };

        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<Hand> hands = new List<Hand>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var data = line.Split(" ");
            var hand = new Hand()
            {
                Cards = data[0].ToCharArray(),
                Bid = int.Parse(data[1]),
                NumberMatching = 0,
                SecondCardRank = 0
            };
            foreach (var kvp in ranks)
            {
                var count = data[0].Count(c => c == kvp.Key);
                if (count > hand.NumberMatching)
                {
                    hand.NumberMatching = count;
                    hand.CardRank = kvp.Value;

                }
            }

            if (hand.NumberMatching > 3)
            {
                hand.NumberMatching += 2;

            }
            if (hand.NumberMatching == 3)
            {
                hand.NumberMatching = 4;
                foreach (var kvp in ranks)
                {
                    if (hand.CardRank == kvp.Value) continue;
                    var count = data[0].Count(c => c == kvp.Key);
                    if (count == 2)
                    {
                        hand.NumberMatching = 5;
                        if (hand.SecondCardRank < kvp.Value)
                        {
                            hand.SecondCardRank = kvp.Value;
                        }
                    }
                }

            }

            if (hand.NumberMatching == 2)
            {
                foreach (var kvp in ranks)
                {
                    if (hand.CardRank == kvp.Value)
                        continue;
                    var count = data[0].Count(c => c == kvp.Key);
                    if (count == 2)
                    {
                        hand.NumberMatching = 3;
                        if (hand.SecondCardRank < kvp.Value)
                        {
                            hand.SecondCardRank = kvp.Value;
                        }
                    }
                }
            }

            hands.Add(hand);
        }

        hands.Sort((a,b) =>
        {
            if (a.NumberMatching == b.NumberMatching)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (a.Cards[i] != b.Cards[i])
                    {
                        return ranks[a.Cards[i]].CompareTo(ranks[b.Cards[i]]);
                    }
                }
            } 
            return a.NumberMatching.CompareTo(b.NumberMatching);
        });
        for (int i = 0; i < hands.Count; i++)
        {
            Console.WriteLine($"Hand {hands[i].Cards[0]}, {hands[i].Cards[1]}, {hands[i].Cards[2]}, {hands[i].Cards[3]}, {hands[i].Cards[4]}, {hands[i].CardRank}");
            solution += hands[i].Bid * (i+ 1);
        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part 1 ({inputFile})- {solution}");

        Console.WriteLine($"Completed part 1 in {end - start}");
    }



    static void DoPart2(string inputFile)
    {
        Dictionary<char, int> ranks = new Dictionary<char, int>()
        {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'T', 10 },
            { '9', 9 },
            { '8', 8 },
            { '7', 7 },
            { '6', 6 },
            { '5', 5 },
            { '4', 4 },
            { '3', 3 },
            { '2', 2 },
            { 'J', 1 },

        };

        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<Hand> hands = new List<Hand>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var data = line.Split(" ");
            var hand = new Hand()
            {
                Cards = data[0].ToCharArray(),
                Bid = int.Parse(data[1]),
                NumberMatching = 0,
                SecondCardRank = 0
            };
            foreach (var kvp in ranks)
            {
                if (kvp.Key == 'J') continue;
                var cardsToMatch = data[0].Select(c => c == 'J' ? kvp.Key : c );
                var count = cardsToMatch.Count(c => c == kvp.Key);
                if (count > hand.NumberMatching)
                {
                    hand.NumberMatching = count;
                    hand.CardRank = kvp.Value;

                }
            }

            if (hand.NumberMatching > 3)
            {
                hand.NumberMatching += 2;

            }
            if (hand.NumberMatching == 3)
            {
                hand.NumberMatching = 4;
                foreach (var kvp in ranks)
                {
                    if (kvp.Key == 'J')
                        continue;
                    if (hand.CardRank == kvp.Value)
                        continue;
                    var count = data[0].Count(c => c == kvp.Key);
                    if (count == 2)
                    {
                        hand.NumberMatching = 5;
                        if (hand.SecondCardRank < kvp.Value)
                        {
                            hand.SecondCardRank = kvp.Value;
                        }
                    }
                }

            }

            if (hand.NumberMatching == 2)
            {
                foreach (var kvp in ranks)
                {
                    if (kvp.Key == 'J')
                        continue;
                    if (hand.CardRank == kvp.Value)
                        continue;
                    var count = data[0].Count(c => c == kvp.Key);
                    if (count == 2)
                    {
                        hand.NumberMatching = 3;
                        if (hand.SecondCardRank < kvp.Value)
                        {
                            hand.SecondCardRank = kvp.Value;
                        }
                    }
                }
            }

            hands.Add(hand);
        }

        hands.Sort((a, b) =>
        {
            if (a.NumberMatching == b.NumberMatching)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (a.Cards[i] != b.Cards[i])
                    {
                        return ranks[a.Cards[i]].CompareTo(ranks[b.Cards[i]]);
                    }
                }
            }
            return a.NumberMatching.CompareTo(b.NumberMatching);
        });
        for (int i = 0; i < hands.Count; i++)
        {
            Console.WriteLine($"Hand {hands[i].Cards[0]}, {hands[i].Cards[1]}, {hands[i].Cards[2]}, {hands[i].Cards[3]}, {hands[i].Cards[4]}, {hands[i].CardRank}");
            solution += hands[i].Bid * (i + 1);
        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part 2 ({inputFile})- {solution}");

        Console.WriteLine($"Completed part 2 in {end - start}");
    }

  
}