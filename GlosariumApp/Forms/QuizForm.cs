using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GlosariumApp.Data;
using GlosariumApp.Models;

namespace GlosariumApp.Forms
{
    public class QuizForm : Form
    {
        private const int MAX_QUESTIONS = 5;
        private List<Term>? sessionQuestions;
        private int qIndex = 0;
        private int score = 0;
        private Random rnd = new Random();
        private Term? currentTerm;

        // INISIALISASI untuk mengatasi Non-nullable field errors
        private Label lblQNum = new Label();
        private Label lblScore = new Label();
        private Label lblTimer = new Label(); // BARU: Label countdown
        private Label lblQuestion = new Label();
        private Button btnA = new Button();
        private Button btnB = new Button();
        private Button btnC = new Button();
        private Button btnD = new Button();

        // PERSYARATAN: TimeSpan, Timer untuk countdown
        private System.Windows.Forms.Timer countdownTimer = new System.Windows.Forms.Timer();
        private int timeRemaining = 30; // 30 detik per soal
        private DateTime quizStartTime; // Waktu mulai quiz
        private bool isQuizActive = true; // Status quiz

        public QuizForm()
        {
            InitializeUI();

            // Event handler untuk form closing
            this.FormClosing += QuizForm_FormClosing;

            if (TermRepository.Terms.Count < 4)
            {
                MessageBox.Show("Data terlalu sedikit untuk kuis! (Min. 4 Istilah)", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // PERSYARATAN: Date, Time - Record start time
            quizStartTime = DateTime.Now;

            // Setup countdown timer
            countdownTimer.Interval = 1000; // 1 detik
            countdownTimer.Tick += CountdownTimer_Tick;

            StartNewSession();
        }

        private void InitializeUI()
        {
            this.Text = "Mode Kuis Pengetahuan";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 12);

            // Panel Top - Header dengan info quiz
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(40, 40, 40) };

            lblQNum.Text = "Q 1/5";
            lblQNum.Left = 20;
            lblQNum.Top = 15;
            lblQNum.ForeColor = Color.White;
            lblQNum.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblQNum.AutoSize = true;

            // BARU: Timer Countdown (di tengah)
            lblTimer.Text = "⏱️ 00:30";
            lblTimer.Left = 320;
            lblTimer.Top = 15;
            lblTimer.Width = 160;
            lblTimer.ForeColor = Color.LightGreen;
            lblTimer.Font = new Font("Consolas", 18, FontStyle.Bold);
            lblTimer.TextAlign = ContentAlignment.MiddleCenter;

            lblScore.Text = "Skor: 0";
            lblScore.Left = 650;
            lblScore.Top = 15;
            lblScore.ForeColor = Color.Yellow;
            lblScore.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblScore.AutoSize = true;

            pnlTop.Controls.Add(lblQNum);
            pnlTop.Controls.Add(lblTimer);
            pnlTop.Controls.Add(lblScore);

            // Label Question
            lblQuestion.Text = "Loading...";
            lblQuestion.Dock = DockStyle.Top;
            lblQuestion.Height = 150;
            lblQuestion.TextAlign = ContentAlignment.MiddleCenter;
            lblQuestion.Font = new Font("Segoe UI", 16);
            lblQuestion.Padding = new Padding(20);

            // Panel Answers
            Panel pnlAnswers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(50, 20, 50, 50) };

            btnA = CreateAnswerBtn(0);
            btnB = CreateAnswerBtn(1);
            btnC = CreateAnswerBtn(2);
            btnD = CreateAnswerBtn(3);

            btnA.Click += Answer_Click;
            btnB.Click += Answer_Click;
            btnC.Click += Answer_Click;
            btnD.Click += Answer_Click;

            pnlAnswers.Controls.AddRange(new Control[] { btnA, btnB, btnC, btnD });

            this.Controls.Add(pnlAnswers);
            this.Controls.Add(lblQuestion);
            this.Controls.Add(pnlTop);
        }

        private Button CreateAnswerBtn(int index)
        {
            int gap = 10;
            int height = 60;
            int topPos = index * (height + gap);

            Button btn = new Button
            {
                Top = topPos,
                Left = 50,
                Width = 680,
                Height = height,
                BackColor = Color.AliceBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            return btn;
        }

        private void StartNewSession()
        {
            sessionQuestions = TermRepository.Terms.OrderBy(x => rnd.Next()).Take(MAX_QUESTIONS).ToList();
            qIndex = 0;
            score = 0;
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (sessionQuestions == null || qIndex >= sessionQuestions.Count)
            {
                // PERSYARATAN: TimeSpan - Calculate quiz duration
                TimeSpan quizDuration = DateTime.Now - quizStartTime;

                // Stop timer
                countdownTimer.Stop();
                isQuizActive = false;

                // PERSYARATAN: Log activity - Record quiz completion
                string quizResult = $"Quiz selesai - Skor: {score}/100 - Waktu: {quizDuration:mm\\:ss}";
                TermRepository.LogActivity("QUIZ", quizResult);

                MessageBox.Show($"Permainan Selesai!\n\nSkor Akhir: {score} / 100\nWaktu Total: {quizDuration:mm\\:ss}",
                    "Hasil", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.Close();
                return;
            }

            currentTerm = sessionQuestions[qIndex];

            lblQNum.Text = $"Pertanyaan {qIndex + 1} / {MAX_QUESTIONS}";
            lblScore.Text = $"Skor: {score}";
            lblQuestion.Text = $"Apa istilah untuk:\n\"{currentTerm!.Definition}\"?";

            // RESET TIMER - 30 detik per soal
            timeRemaining = 30;
            UpdateTimerDisplay();
            countdownTimer.Start();

            var distractors = TermRepository.Terms
                .Where(t => t.Id != currentTerm.Id)
                .OrderBy(x => rnd.Next())
                .Take(3)
                .Select(t => t.Word).ToList();

            var options = new List<string>(distractors) { currentTerm.Word };
            options = options.OrderBy(x => rnd.Next()).ToList();

            btnA.Text = $"A.  {options[0]}"; btnA.Tag = options[0];
            btnB.Text = $"B.  {options[1]}"; btnB.Tag = options[1];
            btnC.Text = $"C.  {options[2]}"; btnC.Tag = options[2];
            btnD.Text = $"D.  {options[3]}"; btnD.Tag = options[3];

            // Enable all buttons
            btnA.Enabled = btnB.Enabled = btnC.Enabled = btnD.Enabled = true;

            // Reset button colors
            btnA.BackColor = btnB.BackColor = btnC.BackColor = btnD.BackColor = Color.AliceBlue;
            btnA.ForeColor = btnB.ForeColor = btnC.ForeColor = btnD.ForeColor = Color.Black;
        }

        // PERSYARATAN: Timer - Countdown tick event
        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (!isQuizActive)
                return;

            timeRemaining--;
            UpdateTimerDisplay();

            // PERSYARATAN: If statement - Check timeout
            if (timeRemaining <= 0)
            {
                countdownTimer.Stop();
                MessageBox.Show($"Waktu Habis!\n\nJawaban yang benar: {currentTerm?.Word}",
                    "Time's Up!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                qIndex++;
                LoadQuestion();
            }
        }

        private void UpdateTimerDisplay()
        {
            lblTimer.Text = $"⏱️ 00:{timeRemaining:D2}";

            // PERSYARATAN: If statement - Color based on time
            if (timeRemaining > 10)
                lblTimer.ForeColor = Color.LightGreen;
            else if (timeRemaining > 5)
                lblTimer.ForeColor = Color.Orange;
            else
                lblTimer.ForeColor = Color.Red;
        }

        private void Answer_Click(object? sender, EventArgs e)
        {
            // Stop timer
            countdownTimer.Stop();

            Button clickedBtn = (Button)sender!;
            if (currentTerm == null) return;

            string answer = clickedBtn.Tag?.ToString() ?? string.Empty;

            // Disable all buttons
            btnA.Enabled = btnB.Enabled = btnC.Enabled = btnD.Enabled = false;

            if (answer == currentTerm.Word)
            {
                score += 20;
                MessageBox.Show("Benar! +20 Poin", "Tepat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Salah! Jawaban yang benar adalah:\n\n{currentTerm.Word}",
                    "Ups!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            qIndex++;
            LoadQuestion();
        }

        // EVENT: Konfirmasi saat window ditutup
        private void QuizForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isQuizActive)
            {
                var confirm = MessageBox.Show(
                    "Quiz masih berlangsung.\n\nApakah Anda yakin ingin keluar?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Cleanup timer
            countdownTimer.Stop();
            countdownTimer.Tick -= CountdownTimer_Tick;
            countdownTimer.Dispose();
            isQuizActive = false;
        }
    }
}