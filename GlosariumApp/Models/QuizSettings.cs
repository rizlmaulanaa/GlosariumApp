using System;

namespace GlosariumApp.Models
{
    // PERSYARATAN: Enum untuk switch-case statement
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    // CLASS BARU: Pengaturan kuis
    public class QuizSettings
    {
        public DifficultyLevel Difficulty { get; set; }

        // PERSYARATAN: TimeSpan untuk durasi
        public TimeSpan TimeLimit { get; set; }

        public int QuestionCount { get; set; }
        public int PassingScore { get; set; }

        public QuizSettings()
        {
            Difficulty = DifficultyLevel.Medium;
            TimeLimit = TimeSpan.FromMinutes(5);
            QuestionCount = 5;
            PassingScore = 60;
        }

        // PERSYARATAN: Switch-case statement untuk mengatur difficulty
        public void SetDifficulty(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy:
                    TimeLimit = TimeSpan.FromMinutes(10);
                    QuestionCount = 5;
                    PassingScore = 40;
                    break;

                case DifficultyLevel.Medium:
                    TimeLimit = TimeSpan.FromMinutes(5);
                    QuestionCount = 7;
                    PassingScore = 60;
                    break;

                case DifficultyLevel.Hard:
                    TimeLimit = TimeSpan.FromMinutes(3);
                    QuestionCount = 10;
                    PassingScore = 80;
                    break;

                default:
                    TimeLimit = TimeSpan.FromMinutes(5);
                    QuestionCount = 5;
                    PassingScore = 60;
                    break;
            }

            Difficulty = level;
        }
    }
}