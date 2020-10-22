using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Client.Common
{
    public class Quotation
    {
        //private Quotation()
        //{
        //}
        const int intMax = Int32.MaxValue;

        public List<List<int>> BigList { get; set; } = new List<List<int>> { new List<int>() };




        double Average { get; set; }
        double Median { get; set; }
        int Mode { get; set; }
        double StandardDeviation { get; set; }

        ulong LostItems { get; set; }
        ulong ItemsCount { get; set; }

        int ListIndex { get; set; } = 0;


        public void Init(List<int> items, ulong packageNumber)
        {
            items.Sort();

            if ((BigList[ListIndex].Count + items.Count) > intMax)
            {
                if (ListIndex < intMax - 1)
                {
                    ListIndex++;
                    BigList[ListIndex] = items;
                }
            }
            else
            {
                BigList[ListIndex].AddRange(items);
                BigList[ListIndex].Sort();
            }

            ItemsCount += Convert.ToUInt64(items.Count);
            LostItems += packageNumber - ItemsCount;
        }



        public void CalculateStatistics()
        {
            //GetAverage();
            //GetMedian();
            //GetMode();
            //GetStdDev();

            Task[] tasks = new Task[]
            {
                Task.Run(() => GetAverage()),
                Task.Run(() => GetMedian()),
                Task.Run(() => GetMode()),
                Task.Run(() => GetStdDev())
            };

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"\n Среднее значение: {Average:N2}\n Медиана: {Median:G3}\n Мода: {Mode}\n Стандартное отклонение: {StandardDeviation:N2}\n Количество потерянных пакетов: {LostItems}\n");
        }

        void GetAverage()
        {
            List<double> avList = new List<double>();

            List<Task<double>> tasks = new List<Task<double>>();

            Parallel.ForEach(BigList, i =>
            {
                tasks.Add(Task.Run(() => i.Average()));
            });

            Task.WaitAll(tasks.ToArray());

            foreach (var it in tasks)
            {
                avList.Add(it.Result);
            }

            Average = avList.Average();
        }

        void GetMedian()
        {
            List<double> tempList = new List<double>();

            List<Task<double>> tasks = new List<Task<double>>();

            Parallel.ForEach(BigList, i =>
            {
                tasks.Add(Task.Run(() => MedianCalc(i)));
            });

            Task.WaitAll(tasks.ToArray());

            foreach (var it in tasks)
            {
                tempList.Add(it.Result);
            }
            var MedianTask = Task.Run(() => MedianCalcDouble(tempList));
            Task.WaitAll(MedianTask);
            Median = MedianTask.Result;
        }



        void GetMode()
        {

            Dictionary<int, int> dict = new Dictionary<int, int>();

            List<Task<KeyValuePair<int, int>>> tasks = new List<Task<KeyValuePair<int, int>>>();

            Parallel.ForEach(BigList, i =>
            {
                tasks.Add(Task.Run(() => ModeCalcStepOne(i)));
            });

            Task.WaitAll(tasks.ToArray());

            foreach (var it in tasks)
            {
                dict.Add(it.Result.Key, it.Result.Value);
            }

            var ModeTask = Task.Run(() => ModeCalcStepTwo(dict));
            Task.WaitAll(ModeTask);
            Mode = ModeTask.Result;
        }



        void GetStdDev()
        {
            List<double> tempList = new List<double>();

            List<Task<double>> tasks = new List<Task<double>>();

            Parallel.ForEach(BigList, i =>
            {
                tasks.Add(Task.Run(() => GetStdDevCalc(i)));
            });

            Task.WaitAll(tasks.ToArray());

            foreach (var it in tasks)
            {
                tempList.Add(it.Result);
            }


            var meanOfSquaredDifferences = tempList
                .Average();

            StandardDeviation = Math.Sqrt(meanOfSquaredDifferences);

        }










        double GetStdDevCalc(List<int> Items)
        {
            var meanOfNumbers = Items.Average();


            var squaredDifferences = new List<double>(Items.Count);
            foreach (var number in Items)
            {
                var difference = number - meanOfNumbers;
                var squaredDifference = Math.Pow(difference, 2);
                squaredDifferences.Add(squaredDifference);
            }

            var meanOfSquaredDifferences = squaredDifferences
                .Average();


            //var standardDeviation = Math.Sqrt(meanOfSquaredDifferences);

            return meanOfSquaredDifferences;
        }




        double MedianCalc(List<int> Items)
        {
            double returnValue = 0;
            int midle = Items.Count / 2;
            if (Items.Count % 2 == 0)
            {
                returnValue = (Items[midle - 1] + Items[midle]) / 2;
            }
            else
            {
                returnValue = Items[midle];
            }
            return returnValue;
        }

        double MedianCalcDouble(List<double> Items)
        {
            double returnValue = 0;
            int midle = Items.Count / 2;
            if (Items.Count % 2 == 0)
            {
                returnValue = (Items[midle - 1] + Items[midle]) / 2;
            }
            else
            {
                returnValue = Items[midle];
            }
            return returnValue;
        }



        KeyValuePair<int, int> ModeCalcStepOne(List<int> Items)
        {
            int returnValue = 0;

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (int elem in Items)
            {
                if (dict.ContainsKey(elem))
                    dict[elem]++;
                else
                    dict[elem] = 1;
            }

            int maxCount = 0;
            foreach (int elem in dict.Keys)
            {
                if (dict[elem] >= maxCount)
                {
                    maxCount = dict[elem];
                    returnValue = elem;
                }
            }

            if (maxCount == 1)
            {
                maxCount = 0;
                returnValue = 0;
            }
            return new KeyValuePair<int, int>(returnValue, maxCount);
        }


        int ModeCalcStepTwo(Dictionary<int, int> dict)
        {
            int returnValue = 0;

            int maxCount = 0;
            foreach (int elem in dict.Keys)
            {
                if (dict[elem] >= maxCount)
                {
                    maxCount = dict[elem];
                    returnValue = elem;
                }
            }

            if (maxCount == 1)
                returnValue = 0;

            return returnValue;
        }



    }
}
