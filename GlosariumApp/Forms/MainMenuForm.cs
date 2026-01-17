using System;
using System.Drawing;
using System.Windows.Forms;
using GlosariumApp.Data;

namespace GlosariumApp.Forms
{
    // Form menu utama
    public class MainMenuForm : Form
    {
        private Label lblActivityHint;

        public MainMenuForm()
        {
            InitializeComponentModern();
            TermRepository.SeedData();

            // PERSYARATAN: Direktori - Buat folder backup saat startup
            TermRepository.CreateBackupDirectory();

            // Update activity hint
            UpdateActivityHint();
        }

        private void InitializeComponentModern()
        {
            this.Text = "Aplikasi Glosarium IT";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // Header dengan judul
            var lblTitle = new Label
            {
                Text = "Glosarium & Kuis IT",
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };

            // PERSYARATAN: Date, Time - Label waktu real-time
            var lblDateTime = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd MMMM yyyy - HH:mm:ss"),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray
            };

            // Timer untuk update jam
            var clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) =>
            {
                // PERSYARATAN: Date, Time - Update setiap detik
                lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy - HH:mm:ss");
                UpdateActivityHint(); // Update activity count juga
            };
            clockTimer.Start();

            var pnlCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(80, 20, 80, 20) };

            // Tombol Glosarium (UTAMA)
            var btnGlosarium = new Button
            {
                Text = "📖  Kelola Glosarium",
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGlosarium.FlatAppearance.BorderSize = 0;
            btnGlosarium.Click += (s, e) => { new GlosariumForm().ShowDialog(); UpdateActivityHint(); };

            var spacer1 = new Panel { Dock = DockStyle.Top, Height = 15 };

            // Tombol Kuis (UTAMA - HIGHLIGHT)
            var btnQuiz = new Button
            {
                Text = "🎮  MULAI KUIS SEKARANG",
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnQuiz.FlatAppearance.BorderSize = 0;
            btnQuiz.Click += (s, e) => { new QuizForm().ShowDialog(); UpdateActivityHint(); };

            var spacer2 = new Panel { Dock = DockStyle.Top, Height = 15 };

            // Tombol Import/Export (UTAMA)
            var btnFileOps = new Button
            {
                Text = "💾  Import / Export Data",
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFileOps.FlatAppearance.BorderSize = 0;
            btnFileOps.Click += (s, e) => { new FileOperationsForm().ShowDialog(); UpdateActivityHint(); };

            var spacer3 = new Panel { Dock = DockStyle.Top, Height = 20 };

            // === PANEL FITUR SEKUNDER (Statistik) ===
            var pnlSecondary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                Padding = new Padding(0, 0, 0, 0)
            };

            // Tombol Statistik (KECIL - Secondary)
            var btnStats = new Button
            {
                Text = "📊 Statistik",
                Left = 0,
                Top = 0,
                Width = 540,
                Height = 40,
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.FromArgb(127, 140, 141),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };
            btnStats.FlatAppearance.BorderSize = 1;
            btnStats.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);

            // Hover effect
            btnStats.MouseEnter += (s, e) => {
                btnStats.BackColor = Color.FromArgb(236, 240, 241);
                btnStats.ForeColor = Color.FromArgb(52, 73, 94);
            };
            btnStats.MouseLeave += (s, e) => {
                btnStats.BackColor = Color.FromArgb(245, 245, 245);
                btnStats.ForeColor = Color.FromArgb(127, 140, 141);
            };

            btnStats.Click += (s, e) => { new StatisticsForm().ShowDialog(); };

            pnlSecondary.Controls.Add(btnStats);

            // Add controls to center panel (reverse order for Dock.Top)
            pnlCenter.Controls.Add(pnlSecondary);
            pnlCenter.Controls.Add(spacer3);
            pnlCenter.Controls.Add(btnFileOps);
            pnlCenter.Controls.Add(spacer2);
            pnlCenter.Controls.Add(btnQuiz);
            pnlCenter.Controls.Add(spacer1);
            pnlCenter.Controls.Add(btnGlosarium);

            // === FOOTER KECIL - ACTIVITY LOG HINT ===
            var pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(15, 8, 15, 8)
            };

            lblActivityHint = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Text = "💡 Loading activity log...",
                Cursor = Cursors.Hand
            };

            // Klik untuk buka activity log
            lblActivityHint.Click += (s, e) => { new ActivityLogForm().ShowDialog(); UpdateActivityHint(); };

            // Hover effect
            lblActivityHint.MouseEnter += (s, e) => {
                lblActivityHint.ForeColor = Color.FromArgb(52, 152, 219);
                lblActivityHint.Font = new Font("Segoe UI", 8, FontStyle.Underline);
            };
            lblActivityHint.MouseLeave += (s, e) => {
                lblActivityHint.ForeColor = Color.Gray;
                lblActivityHint.Font = new Font("Segoe UI", 8);
            };

            pnlFooter.Controls.Add(lblActivityHint);

            this.Controls.Add(pnlCenter);
            this.Controls.Add(lblDateTime);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlFooter);
        }

        // Update activity hint dengan info terbaru
        private void UpdateActivityHint()
        {
            try
            {
                // Ambil jumlah log
                string[] logs = TermRepository.ReadLogFile();
                int logCount = logs.Length;

                // Ambil log terakhir jika ada
                string lastActivity = "Belum ada aktivitas";
                if (logCount > 0)
                {
                    string lastLog = logs[logs.Length - 1];
                    // Ambil hanya bagian action (setelah timestamp)
                    int actionStart = lastLog.IndexOf(']') + 1;
                    if (actionStart > 0 && actionStart < lastLog.Length)
                    {
                        lastActivity = lastLog.Substring(actionStart).Trim();
                        // Batasi panjang
                        if (lastActivity.Length > 60)
                        {
                            lastActivity = lastActivity.Substring(0, 57) + "...";
                        }
                    }
                }

                lblActivityHint.Text = $"📝 Activity Log ({logCount} total) • Last: {lastActivity} • Klik untuk detail";
            }
            catch
            {
                lblActivityHint.Text = "📝 Activity Log • Klik untuk melihat detail";
            }
        }
    }
}