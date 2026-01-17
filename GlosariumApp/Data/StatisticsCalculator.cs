using System;
using System.Collections.Generic;
using System.Linq;
using GlosariumApp.Models;

namespace GlosariumApp.Data
{
    // CLASS BARU: Kalkulator statistik
    // PERSYARATAN: Calculate text, For-loop, While, Do-while
    public static class StatisticsCalculator
    {
        // PERSYARATAN: Array, For-loop - Hitung distribusi kategori
        public static int[] GetCategoryDistribution()
        {
            // PERSYARATAN: Array - Fixed size array untuk 6 kategori
            int[] distribution = new int[6];

            // PERSYARATAN: For-loop dengan collection
            for (int i = 0; i < TermRepository.Terms.Count; i++)
            {
                Term term = TermRepository.Terms[i];
                int catIndex = (int)term.Category;
                distribution[catIndex]++;
            }

            return distribution;
        }

        // PERSYARATAN: Calculate text, Array, Passing arrays sebagai parameter
        public static string GenerateStatisticsReport(int[] categoryData)
        {
            string report = "=== LAPORAN STATISTIK GLOSARIUM ===\n\n";

            // PERSYARATAN: Calculate - Total
            int total = 0;

            // PERSYARATAN: For-loop dengan array
            for (int i = 0; i < categoryData.Length; i++)
            {
                total += categoryData[i];
            }

            report += $"Total Istilah: {total}\n\n";
            report += "Distribusi per Kategori:\n";

            // PERSYARATAN: Foreach loop dengan enum
            foreach (TermCategory cat in Enum.GetValues(typeof(TermCategory)))
            {
                int index = (int)cat;
                int count = categoryData[index];

                // PERSYARATAN: Calculate text - Persentase
                double percentage = total > 0 ? (double)count / total * 100 : 0;

                report += $"  {cat}: {count} ({percentage:F1}%)\n";
            }

            // PERSYARATAN: Date, Time
            report += $"\nDibuat pada: {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}";

            return report;
        }

        // PERSYARATAN: While loop - Cari istilah terpanjang
        public static Term? FindLongestDefinition()
        {
            if (TermRepository.Terms.Count == 0) return null;

            Term longest = TermRepository.Terms[0];
            int index = 1;

            // PERSYARATAN: While loop
            while (index < TermRepository.Terms.Count)
            {
                Term current = TermRepository.Terms[index];

                // PERSYARATAN: String methods - Length
                if (current.Definition.Length > longest.Definition.Length)
                {
                    longest = current;
                }

                index++;
            }

            return longest;
        }

        // PERSYARATAN: Do-while loop - Validasi input score
        public static bool ValidateScore(ref int score, int maxScore)
        {
            // PERSYARATAN: ByRef - Menggunakan ref parameter
            int attempts = 0;

            // PERSYARATAN: Do-while loop
            do
            {
                if (score >= 0 && score <= maxScore)
                {
                    return true;
                }

                // Normalize score jika invalid
                score = Math.Max(0, Math.Min(score, maxScore));
                attempts++;

            } while (attempts < 3);

            return false;
        }

        // PERSYARATAN: Calculate, TimeSpan
        public static TimeSpan CalculateAverageQuizTime(List<TimeSpan> completionTimes)
        {
            if (completionTimes.Count == 0)
                return TimeSpan.Zero;

            // PERSYARATAN: Calculate - Total time
            TimeSpan totalTime = TimeSpan.Zero;

            // PERSYARATAN: Foreach loop
            foreach (var time in completionTimes)
            {
                totalTime += time;
            }

            // PERSYARATAN: Calculate - Average
            double averageTicks = totalTime.Ticks / (double)completionTimes.Count;
            return TimeSpan.FromTicks((long)averageTicks);
        }

        // PERSYARATAN: Array, Passing arrays, ByRef (out parameter)
        public static void GetTopCategories(out string[] topCategories, out int[] counts)
        {
            // PERSYARATAN: ByRef - Menggunakan out parameter
            var distribution = GetCategoryDistribution();

            // PERSYARATAN: Array creation
            topCategories = new string[3];
            counts = new int[3];

            // Create pairs for sorting
            var pairs = new List<Tuple<string, int>>();

            for (int i = 0; i < distribution.Length; i++)
            {
                pairs.Add(new Tuple<string, int>(
                    ((TermCategory)i).ToString(),
                    distribution[i]
                ));
            }

            // Sort descending
            var sorted = pairs.OrderByDescending(p => p.Item2).Take(3).ToList();

            // PERSYARATAN: For loop - Fill arrays
            for (int i = 0; i < sorted.Count && i < 3; i++)
            {
                topCategories[i] = sorted[i].Item1;
                counts[i] = sorted[i].Item2;
            }
        }
    }
}