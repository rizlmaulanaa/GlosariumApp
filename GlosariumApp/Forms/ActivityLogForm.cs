using System;
using System.Drawing;
using System.Windows.Forms;
using GlosariumApp.Data;

namespace GlosariumApp.Forms
{
    // FORM: Menampilkan riwayat aktivitas (SIMPLIFIED)
    // PERSYARATAN: Files, Array, Date/Time
    public class ActivityLogForm : Form
    {
        private ListBox lstLogs;
        private Label lblStatus;

        public ActivityLogForm()
        {
            InitializeUI();
            LoadLogs();
        }

        private void InitializeUI()
        {
            this.Text = "Riwayat Aktivitas";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            var lblTitle = new Label
            {
                Text = "📝 ACTIVITY LOG",
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(230, 126, 34)
            };

            lstLogs = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(10)
            };

            // SIMPLIFIED FOOTER - Hanya hint kecil
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(15, 10, 15, 10)
            };

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Text = "💡 Tip: Log otomatis mencatat semua aktivitas (CRUD, Quiz, Import/Export)"
            };

            // Small buttons di kanan bawah
            var btnRefresh = new Button
            {
                Text = "↻",
                Width = 35,
                Height = 35,
                Left = 750,
                Top = 10,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 14)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadLogs();

            var btnClear = new Button
            {
                Text = "🗑",
                Width = 35,
                Height = 35,
                Left = 795,
                Top = 10,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12)
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += BtnClear_Click;

            var btnClose = new Button
            {
                Text = "✕",
                Width = 35,
                Height = 35,
                Left = 840,
                Top = 10,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            pnlBottom.Controls.Add(lblStatus);
            pnlBottom.Controls.Add(btnRefresh);
            pnlBottom.Controls.Add(btnClear);
            pnlBottom.Controls.Add(btnClose);

            this.Controls.Add(lstLogs);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlBottom);
        }

        private void LoadLogs()
        {
            try
            {
                // PERSYARATAN: Files, Array - Baca log dari file
                string[] logs = TermRepository.ReadLogFile();

                lstLogs.Items.Clear();

                // PERSYARATAN: For loop dengan array
                for (int i = 0; i < logs.Length; i++)
                {
                    lstLogs.Items.Add(logs[i]);
                }

                if (logs.Length == 0)
                {
                    lstLogs.Items.Add("--- Belum ada aktivitas tercatat ---");
                    lblStatus.Text = "💡 Log kosong";
                }
                else
                {
                    lblStatus.Text = $"💡 Total {logs.Length} aktivitas tercatat";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Hapus semua log aktivitas?",
                "Konfirmasi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    System.IO.File.Delete("activity_log.txt");
                    LoadLogs();
                    MessageBox.Show("Log berhasil dihapus!", "Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}