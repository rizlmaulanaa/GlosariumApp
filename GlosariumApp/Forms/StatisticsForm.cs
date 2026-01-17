using GlosariumApp.Data;
using GlosariumApp.Models;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace GlosariumApp.Forms
{
    // FORM: Menampilkan statistik glosarium (TAMPILAN DIPERBAIKI)
    // PERSYARATAN: Calculate text, Array, For-loop, PRINT
    public class StatisticsForm : Form
    {
        private RichTextBox txtReport;
        private PrintDocument printDocument;
        private string contentToPrint = string.Empty;

        public StatisticsForm()
        {
            InitializeUI();
            LoadStatistics();

            // PERSYARATAN: Print - Inisialisasi print document
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void InitializeUI()
        {
            this.Text = "Statistik Glosarium";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            var lblTitle = new Label
            {
                Text = "📊 STATISTIK & ANALISIS",
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(46, 204, 113)
            };

            // PERBAIKAN: Gunakan RichTextBox untuk formatting lebih baik
            txtReport = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(30),
                WordWrap = true
            };

            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(20, 15, 20, 15)
            };

            var btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Left = 20,
                Top = 15,
                Width = 130,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadStatistics();

            var btnPrint = new Button
            {
                Text = "🖨️ Print",
                Left = 160,
                Top = 15,
                Width = 130,
                Height = 40,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += BtnPrint_Click;

            var btnPreview = new Button
            {
                Text = "👁️ Preview",
                Left = 300,
                Top = 15,
                Width = 130,
                Height = 40,
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.Click += BtnPreview_Click;

            var btnClose = new Button
            {
                Text = "Tutup",
                Left = 650,
                Top = 15,
                Width = 130,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            pnlBottom.Controls.Add(btnRefresh);
            pnlBottom.Controls.Add(btnPrint);
            pnlBottom.Controls.Add(btnPreview);
            pnlBottom.Controls.Add(btnClose);

            this.Controls.Add(txtReport);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlBottom);
        }

        private void LoadStatistics()
        {
            try
            {
                txtReport.Clear();

                // PERSYARATAN: Array - Mengambil distribusi kategori sebagai array
                int[] categoryData = StatisticsCalculator.GetCategoryDistribution();

                // Build formatted report
                txtReport.SelectionFont = new Font("Segoe UI", 16, FontStyle.Bold);
                txtReport.SelectionColor = Color.FromArgb(41, 128, 185);
                txtReport.AppendText("═══════════════════════════════════════════════\n");
                txtReport.AppendText("        LAPORAN STATISTIK GLOSARIUM\n");
                txtReport.AppendText("═══════════════════════════════════════════════\n\n");

                // PERSYARATAN: Calculate - Total
                int total = 0;
                for (int i = 0; i < categoryData.Length; i++)
                {
                    total += categoryData[i];
                }

                txtReport.SelectionFont = new Font("Segoe UI", 13, FontStyle.Bold);
                txtReport.SelectionColor = Color.Black;
                txtReport.AppendText($"📚 Total Istilah: {total}\n\n");

                // PERSYARATAN: Date, Time
                txtReport.SelectionFont = new Font("Segoe UI", 10);
                txtReport.SelectionColor = Color.Gray;
                txtReport.AppendText($"Dibuat pada: {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}\n\n");

                txtReport.SelectionFont = new Font("Segoe UI", 12, FontStyle.Bold);
                txtReport.SelectionColor = Color.FromArgb(46, 204, 113);
                txtReport.AppendText("─────────────────────────────────────────────\n");
                txtReport.AppendText("📊 DISTRIBUSI PER KATEGORI\n");
                txtReport.AppendText("─────────────────────────────────────────────\n\n");

                // PERSYARATAN: Foreach loop dengan enum
                foreach (TermCategory cat in Enum.GetValues(typeof(TermCategory)))
                {
                    int index = (int)cat;
                    int count = categoryData[index];

                    // PERSYARATAN: Calculate text - Persentase
                    double percentage = total > 0 ? (double)count / total * 100 : 0;

                    txtReport.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold);
                    txtReport.SelectionColor = Color.FromArgb(52, 73, 94);
                    txtReport.AppendText($"  • {cat,-15}");

                    txtReport.SelectionFont = new Font("Segoe UI", 11);
                    txtReport.SelectionColor = Color.Black;
                    txtReport.AppendText($": {count,3} istilah ");

                    txtReport.SelectionFont = new Font("Segoe UI", 10, FontStyle.Italic);
                    txtReport.SelectionColor = Color.FromArgb(127, 140, 141);
                    txtReport.AppendText($"({percentage:F1}%)\n");
                }

                // PERSYARATAN: While loop - Find longest definition
                var longest = StatisticsCalculator.FindLongestDefinition();
                if (longest != null)
                {
                    txtReport.AppendText("\n");
                    txtReport.SelectionFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    txtReport.SelectionColor = Color.FromArgb(230, 126, 34);
                    txtReport.AppendText("─────────────────────────────────────────────\n");
                    txtReport.AppendText("📏 DEFINISI TERPANJANG\n");
                    txtReport.AppendText("─────────────────────────────────────────────\n\n");

                    txtReport.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold);
                    txtReport.SelectionColor = Color.Black;
                    txtReport.AppendText($"  Istilah: {longest.Word}\n");

                    txtReport.SelectionFont = new Font("Segoe UI", 10);
                    txtReport.SelectionColor = Color.DimGray;
                    txtReport.AppendText($"  Panjang: {longest.Definition.Length} karakter\n\n");
                }

                // PERSYARATAN: ByRef (out parameters) - Get top categories
                StatisticsCalculator.GetTopCategories(out string[] topCats, out int[] topCounts);

                txtReport.SelectionFont = new Font("Segoe UI", 12, FontStyle.Bold);
                txtReport.SelectionColor = Color.FromArgb(155, 89, 182);
                txtReport.AppendText("─────────────────────────────────────────────\n");
                txtReport.AppendText("🏆 TOP 3 KATEGORI\n");
                txtReport.AppendText("─────────────────────────────────────────────\n\n");

                // PERSYARATAN: For loop dengan array
                for (int i = 0; i < topCats.Length && i < 3; i++)
                {
                    if (!string.IsNullOrEmpty(topCats[i]))
                    {
                        txtReport.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold);
                        txtReport.SelectionColor = Color.FromArgb(52, 73, 94);

                        string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : "🥉";
                        txtReport.AppendText($"  {medal} {i + 1}. {topCats[i],-15}");

                        txtReport.SelectionFont = new Font("Segoe UI", 11);
                        txtReport.SelectionColor = Color.Black;
                        txtReport.AppendText($": {topCounts[i]} istilah\n");
                    }
                }

                txtReport.AppendText("\n═══════════════════════════════════════════════\n");

                // Save untuk print
                contentToPrint = txtReport.Text;
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                    MessageBox.Show("Dokumen berhasil dicetak!", "Print Sukses",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saat print: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPreview_Click(object? sender, EventArgs e)
        {
            try
            {
                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = printDocument;
                previewDialog.Width = 800;
                previewDialog.Height = 600;
                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saat preview: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font printFont = new Font("Segoe UI", 10);
            float yPos = 100;
            float leftMargin = 50;
            float topMargin = 50;

            Font headerFont = new Font("Arial", 16, FontStyle.Bold);
            e.Graphics!.DrawString("LAPORAN STATISTIK GLOSARIUM", headerFont, Brushes.Black, leftMargin, topMargin);

            yPos = topMargin + 40;

            string[] lines = contentToPrint.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (yPos > e.PageBounds.Height - 100)
                {
                    e.HasMorePages = true;
                    break;
                }

                e.Graphics.DrawString(lines[i], printFont, Brushes.Black, leftMargin, yPos);
                yPos += printFont.GetHeight(e.Graphics);
            }
        }
    }
}