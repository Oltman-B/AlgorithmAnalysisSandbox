using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Oltman_Lab0x01
{
    class SearchSandbox
    {
        private readonly List<int> _sortedValues = new List<int>();

        private const int MaxSecondsPerAlgorithm = 1;
        private const long MaxMicroSecondsPerAlg = MaxSecondsPerAlgorithm * 1000000;

        private const long NMin = 1;
        private const long NMax = int.MaxValue;
        private long _n = NMin;

        private readonly Random _rand = new Random();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private double _sequentialPrevTimeMicro;
        private double _sequentialTimeMicro;
        private double _sequentialExpectedDoublingRatio;
        private double _sequentialActualDoublingRatio;

        private double _binaryPrevTimeMicro;
        private double _binaryTimeMicro;
        private double _binaryExpectedDoublingRatio;
        private double _binaryActualDoublingRatio;


        private void GenerateSortedList(long length, int min = int.MinValue, int max = int.MaxValue)
        {
            for (int i = 0; i < Math.Max(length - _sortedValues.Count, length); i++)
            {
                _sortedValues.Add(_rand.Next(min, max));
            }
        }

        private void PrintSortedList()
        {
            foreach (var num in _sortedValues)
            {
                Console.Write($"{num}, ");
            }
        }

        private int SequentialSearch(int key)
        {
            return SequentialSearch(this._sortedValues, key);
        }

        private static int SequentialSearch(List<int> listToSearch, int key)
        {
            for (int i = 0; i < listToSearch.Count; i++)
            {
                if (listToSearch[i] == key) return i;
            }

            return -1;
        }

        private int BinarySearch(int key)
        {
            return BinarySearch(this._sortedValues, key);
        }

        private static int BinarySearch(List<int> sortedList, int key)
        {
            int min = 0;
            int max = sortedList.Count - 1;

            while (min <= max)
            {
                int mid = (max + min) / 2;
                if (sortedList[mid] == key) return mid;
                if (key < sortedList[mid])
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return -1;
        }

        public bool VerificationTests()
        {
            // Sequential Test 1
            List<int> unsortedList1 = new List<int> {2, 44, 20, 90, 44, 5, 2, -4, -2, 0};
            if (SequentialSearch(unsortedList1, 5) != 5) return false;
            if (SequentialSearch(unsortedList1, 100) != -1) return false;

            List<int> sortedListEvenCount = new List<int> {-100, -50, 0, 1, 33, 55, 66, 99, 1000, 1100, 2000, 3000};
            List<int> sortedListOddCount = new List<int> {-100, -50, 0, 1, 33, 55, 66, 99, 1000, 1100, 2000};
            if (BinarySearch(sortedListEvenCount, 55) != 5) return false;
            if (BinarySearch(sortedListOddCount, 55) != 5) return false;
            if (BinarySearch(sortedListEvenCount, 3000) != 11) return false;
            if (BinarySearch(sortedListEvenCount, -100) != 0) return false;
            if (BinarySearch(sortedListEvenCount, 30000) != -1) return false;


            // All tests pass
            return true;
        }


        public void RunTimeTests()
        {
            PrintHeader();

            for (; (_n * 2) <= NMax; _n *= 2)
            {
                int key = _rand.Next();
                GenerateSortedList(_n);
                if (_sequentialPrevTimeMicro < MaxMicroSecondsPerAlg)
                {
                    _stopwatch.Restart();
                    SequentialSearch(key);
                    _stopwatch.Stop();

                    _sequentialPrevTimeMicro = _sequentialTimeMicro;
                    _sequentialTimeMicro = TicksToMicroseconds(_stopwatch.ElapsedTicks);
                    CalculateSequentialDoublingRatios();
                }

                if (_binaryPrevTimeMicro < MaxMicroSecondsPerAlg)
                {
                    _stopwatch.Restart();
                    BinarySearch(key);
                    _stopwatch.Stop();

                    _binaryPrevTimeMicro = _binaryTimeMicro;
                    _binaryTimeMicro = TicksToMicroseconds(_stopwatch.ElapsedTicks);
                    CalculateBinaryDoublingRatios();
                }


                PrintDataRow();
            }

        }

        private void CalculateBinaryDoublingRatios()
        {
            if (_n <= 2)
            {
                _binaryExpectedDoublingRatio = -1;
                _binaryActualDoublingRatio = -1;
                return;
            }
                
            _binaryExpectedDoublingRatio = Math.Log2(_n) / Math.Log2((double)_n / 2);
            _binaryActualDoublingRatio = _binaryTimeMicro / _binaryPrevTimeMicro;
        }

        private void CalculateSequentialDoublingRatios()
        {
            // T(N)~a*N  -->  N / (N / 2) == 2
            _sequentialExpectedDoublingRatio = 2;
            _sequentialActualDoublingRatio = _sequentialPrevTimeMicro > 0 ?
                (_sequentialTimeMicro / _sequentialPrevTimeMicro) : -1;
        }

        private void PrintHeader()
        {
            Console.WriteLine($"N\t\t|Sequential Srch||Sequential Srch||Expected Seq Srch||Binary Srch||Binary Search ||Expected Bin Srch|");
            Console.WriteLine($" \t\t|Times Micro Sec||Doubling Ratio ||Doubling Ratio   ||Times      ||Doubling Ratio||Doubling Ratio   |");
        }

        private void PrintDataRow()
        {
            string binaryActualDoubleFormatted = _binaryActualDoublingRatio < 0 ? "na".PadLeft(14):
                _binaryActualDoublingRatio.ToString("F2").PadLeft(14);
            string binaryExpectDoubleFormatted = _binaryExpectedDoublingRatio < 0 ? "na".PadLeft(17):
                _binaryExpectedDoublingRatio.ToString("F2").PadLeft(17);
            string sequentialActualDoubleFormatted = _sequentialActualDoublingRatio < 0 ? "na".PadLeft(14):
                _sequentialActualDoublingRatio.ToString("F2").PadLeft(14);
            string sequentialExpectDoubleFormatted = _sequentialExpectedDoublingRatio < 0 ? "na".PadLeft(13):
                _sequentialExpectedDoublingRatio.ToString("F2").PadLeft(19);
            
            Console.WriteLine($"{_n,-15}{_sequentialTimeMicro,13:F2}{sequentialActualDoubleFormatted} {sequentialExpectDoubleFormatted} " +
                              $"{_binaryTimeMicro,15:F2} {binaryActualDoubleFormatted} {binaryExpectDoubleFormatted}");
        }

        private static double TicksToMicroseconds(long ticks)
        {
            return (double)ticks / Stopwatch.Frequency * 1000000;
        }
    }
}
