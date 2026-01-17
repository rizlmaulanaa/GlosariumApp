using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using GlosariumApp.Data;
using GlosariumApp.Models;

namespace GlosariumApp.Forms
{
    public class GlosariumForm : Form
    {
        // Inisialisasi untuk mengatasi Non-nullable field errors
        private DataGridView dgv = new DataGridView();
        private ComboBox cmbFilter = new ComboBox();
        private Label lblCount = new Label();
        private Panel pnlFooter = new Panel();
        private PrintDocument printDocument;
        private int currentPrintPage = 0;
        private int currentItemIndex = 0;
        private System.Collections.Generic.List<Term> dataToPrint = new System.Collections.Generic.List<Term>();

        public GlosariumForm()
        {
            // *** SOLUSI AKHIR MASALAH DATA KOSONG ***
            TermRepository.SeedData(); // JAMIN DATA SUDAH ADA SEBELUM MENCARI
            // ****************************************

            // 1. ISI ITEM FILTER DULU (Mengatasi ArgumentOutOfRangeException)
            cmbFilter.Items.Add("Semua Kategori");
            foreach (var c in Enum.GetValues(typeof(TermCategory)))
            {
                cmbFilter.Items.Add(c);
            }

            // 2. INISIALISASI UI
            InitializeUI();

            // 3. SET SELECTED INDEX HANYA JIKA ADA ITEM
            if (cmbFilter.Items.Count > 0)
            {
                cmbFilter.SelectedIndex = 0;
            }

            // 4. PANGGIL LOADDATA (Sekarang Count pasti > 0)
            LoadData();

            // PERSYARATAN: Print - Inisialisasi print document
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void InitializeUI()
        {
            this.Text = "Manajemen Glosarium";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.BackColor = Color.WhiteSmoke;

            // Panel Header (Filter dan Tombol)
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White, Padding = new Padding(20) };

            var lblTitle = new Label { Text = "Filter Kategori:", Top = 25, Left = 20, AutoSize = true };

            cmbFilter.Top = 22; cmbFilter.Left = 120; cmbFilter.Width = 200; cmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadData();

            var btnAdd = CreateButton("Tambah", 350, Color.SeaGreen);
            btnAdd.Click += BtnAdd_Click;

            var btnEdit = CreateButton("Edit", 455, Color.Orange);
            btnEdit.Click += BtnEdit_Click;

            var btnDel = CreateButton("Hapus", 560, Color.IndianRed);
            btnDel.Click += BtnDel_Click;

            // TOMBOL: Print Preview
            var btnPreview = CreateButton("👁️ Preview", 665, Color.DodgerBlue);
            btnPreview.Click += BtnPreview_Click;

            // TOMBOL: Print
            var btnPrint = CreateButton("🖨️ Print", 770, Color.MediumSeaGreen);
            btnPrint.Click += BtnPrint_Click;

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, cmbFilter, btnAdd, btnEdit, btnDel, btnPreview, btnPrint });

            // Panel Footer (Counter) - DIPERBAIKI
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 50;
            pnlFooter.BackColor = Color.White;
            pnlFooter.Padding = new Padding(20, 10, 20, 10);

            lblCount.Dock = DockStyle.Fill;
            lblCount.TextAlign = ContentAlignment.MiddleLeft;
            lblCount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCount.ForeColor = Color.FromArgb(52, 73, 94);
            lblCount.Text = "Total Istilah: 0";

            pnlFooter.Controls.Add(lblCount);

            pnlFooter.Controls.Add(lblCount);

            // DataGridView
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.WhiteSmoke;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;

            this.Controls.Add(dgv);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);

            pnlFooter.BringToFront();
        }

        private Button CreateButton(string text, int x, Color color)
        {
            return new Button { Text = text, Top = 17, Left = x, Width = 100, Height = 35, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        }

        private void LoadData()
        {
            string filter;

            if (cmbFilter.SelectedItem != null)
            {
                filter = cmbFilter.SelectedItem.ToString()!;
            }
            else
            {
                filter = "Semua Kategori";
            }

            // PERSYARATAN: If statement, Collection filtering
            var source = filter == "Semua Kategori"
                ? TermRepository.Terms
                : TermRepository.Terms.Where(t => t.Category.ToString() == filter).ToList();

            dgv.DataSource = null;
            dgv.DataSource = source;

            // PERSYARATAN: If statements - Hide kolom yang tidak perlu
            if (dgv.Columns.Contains("Id")) dgv.Columns["Id"]!.Visible = false;
            if (dgv.Columns.Contains("CreatedDate")) dgv.Columns["CreatedDate"]!.Visible = false;
            if (dgv.Columns.Contains("LastModified")) dgv.Columns["LastModified"]!.Visible = false;

            lblCount.Text = $"Total Istilah: {source.Count}";
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using (var dlg = new TermDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // PERSYARATAN: Method dengan parameter
                    TermRepository.AddTerm(dlg.TermData);
                    LoadData();
                }
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            Term selected = (Term)dgv.CurrentRow.DataBoundItem;

            using (var dlg = new TermDialog(selected))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // PERSYARATAN: Method dengan parameter
                    TermRepository.UpdateTerm(selected);
                    LoadData();
                }
            }
        }

        private void BtnDel_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            Term selected = (Term)dgv.CurrentRow.DataBoundItem;

            if (MessageBox.Show($"Hapus istilah '{selected.Word}'?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var termToDelete = TermRepository.Terms.FirstOrDefault(t => t.Id == selected.Id);

                if (termToDelete != null)
                {
                    // PERSYARATAN: Method dengan parameter
                    TermRepository.DeleteTerm(termToDelete);
                    LoadData();
                }
            }
        }

        // PERSYARATAN: Print - Method untuk menampilkan preview
        private void BtnPreview_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validasi data
                if (TermRepository.Terms.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk ditampilkan!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // PERSYARATAN: Print - Siapkan data sesuai filter yang aktif
                PreparePrintData();

                // PERSYARATAN: Print Preview - Tampilkan preview dialog
                using (PrintPreviewDialog previewDialog = new PrintPreviewDialog())
                {
                    previewDialog.Document = printDocument;
                    previewDialog.Width = 1000;
                    previewDialog.Height = 700;
                    previewDialog.StartPosition = FormStartPosition.CenterScreen;
                    previewDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                MessageBox.Show($"Error saat preview: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PERSYARATAN: Print - Method untuk langsung print
        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validasi data
                if (TermRepository.Terms.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk dicetak!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // PERSYARATAN: Print - Siapkan data sesuai filter yang aktif
                PreparePrintData();

                // PERSYARATAN: Print Dialog - Tampilkan dialog pengaturan print
                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.Document = printDocument;

                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        printDocument.Print();
                        MessageBox.Show("Daftar glosarium berhasil dicetak!", "Print Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                MessageBox.Show($"Error saat print: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PERSYARATAN: Method - Menyiapkan data untuk print sesuai filter
        private void PreparePrintData()
        {
            // Reset pagination
            currentPrintPage = 0;
            currentItemIndex = 0;

            // PERSYARATAN: String methods - Get current filter
            string currentFilter = cmbFilter.SelectedItem?.ToString() ?? "Semua Kategori";

            // PERSYARATAN: If statement, Collection - Filter data sesuai kategori
            if (currentFilter == "Semua Kategori")
            {
                dataToPrint = TermRepository.Terms.ToList();
            }
            else
            {
                dataToPrint = TermRepository.Terms
                    .Where(t => t.Category.ToString() == currentFilter)
                    .ToList();
            }
        }

        // PERSYARATAN: Print - Event handler untuk PrintPage (DESAIN FORMAL/PRESENTATION)
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // PERSYARATAN: String - Get current filter
            string currentFilter = cmbFilter.SelectedItem?.ToString() ?? "Semua Kategori";

            // Page dimensions
            int pageWidth = e.PageBounds.Width;
            int pageHeight = e.PageBounds.Height;

            // === DRAW DECORATIVE BORDERS ===
            // PERSYARATAN: Print - Gambar border dekoratif
            using (Pen borderPen = new Pen(Color.FromArgb(41, 128, 185), 12))
            {
                // Top border
                e.Graphics!.DrawLine(borderPen, 0, 0, pageWidth, 0);

                // Left corner design
                Point[] trianglePoints = {
                    new Point(0, 0),
                    new Point(250, 0),
                    new Point(0, 250)
                };
                e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(52, 152, 219)), trianglePoints);
            }

            // Right bottom corner design
            Point[] rightTriangle = {
                new Point(pageWidth, pageHeight),
                new Point(pageWidth - 200, pageHeight),
                new Point(pageWidth, pageHeight - 200)
            };
            e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(41, 128, 185)), rightTriangle);

            // PERSYARATAN: If statement - Halaman pertama (Cover Page)
            if (currentPrintPage == 0)
            {
                // === COVER PAGE ===
                Font titleFont = new Font("Arial", 36, FontStyle.Bold);
                Font subtitleFont = new Font("Arial", 24, FontStyle.Regular);
                Font infoFont = new Font("Arial", 14, FontStyle.Italic);
                Font smallFont = new Font("Arial", 11);

                // Logo/Header area (simulasi - bisa diganti dengan image)
                string appTitle = "GLOSARIUM APP";
                SizeF titleSize = e.Graphics.MeasureString(appTitle, new Font("Arial", 18, FontStyle.Bold));
                float titleX = (pageWidth - titleSize.Width) / 2;
                e.Graphics.DrawString(appTitle, new Font("Arial", 18, FontStyle.Bold),
                    Brushes.White, titleX, 80);

                // Main Title (centered)
                string mainTitle = "DAFTAR GLOSARIUM";
                SizeF mainTitleSize = e.Graphics.MeasureString(mainTitle, titleFont);
                float mainTitleX = (pageWidth - mainTitleSize.Width) / 2;
                float mainTitleY = 300;
                e.Graphics.DrawString(mainTitle, titleFont, new SolidBrush(Color.FromArgb(44, 62, 80)),
                    mainTitleX, mainTitleY);

                // Subtitle
                string subtitle = "Teknologi Informasi";
                SizeF subtitleSize = e.Graphics.MeasureString(subtitle, subtitleFont);
                float subtitleX = (pageWidth - subtitleSize.Width) / 2;
                e.Graphics.DrawString(subtitle, subtitleFont, new SolidBrush(Color.FromArgb(52, 73, 94)),
                    subtitleX, mainTitleY + 60);

                // Decorative line
                float lineY = mainTitleY + 130;
                e.Graphics.DrawLine(new Pen(Color.FromArgb(52, 152, 219), 3),
                    pageWidth / 2 - 150, lineY, pageWidth / 2 + 150, lineY);

                // Category filter info box
                float boxY = mainTitleY + 180;
                float boxWidth = 400;
                float boxHeight = 60;
                float boxX = (pageWidth - boxWidth) / 2;

                RectangleF categoryBox = new RectangleF(boxX, boxY, boxWidth, boxHeight);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(236, 240, 241)), categoryBox);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(52, 152, 219), 2), Rectangle.Round(categoryBox));

                string categoryText = $"Kategori: {currentFilter}";
                SizeF catTextSize = e.Graphics.MeasureString(categoryText, subtitleFont);
                e.Graphics.DrawString(categoryText, new Font("Arial", 20, FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(41, 128, 185)),
                    boxX + (boxWidth - catTextSize.Width) / 2, boxY + 15);

                // Info section
                float infoY = boxY + 100;
                string totalInfo = $"Total : {dataToPrint.Count}";
                SizeF infoSize = e.Graphics.MeasureString(totalInfo, infoFont);
                e.Graphics.DrawString(totalInfo, infoFont, Brushes.DimGray,
                    (pageWidth - infoSize.Width) / 2, infoY);

                // PERSYARATAN: Date, Time - Tanggal cetak
                string dateInfo = $"Dicetak pada: {DateTime.Now:dddd, dd MMMM yyyy}";
                SizeF dateSize = e.Graphics.MeasureString(dateInfo, smallFont);
                e.Graphics.DrawString(dateInfo, smallFont, Brushes.Gray,
                    (pageWidth - dateSize.Width) / 2, infoY + 30);

                string timeInfo = $"Pukul: {DateTime.Now:HH:mm:ss}";
                SizeF timeSize = e.Graphics.MeasureString(timeInfo, smallFont);
                e.Graphics.DrawString(timeInfo, smallFont, Brushes.Gray,
                    (pageWidth - timeSize.Width) / 2, infoY + 50);

                // Footer
                string footer = "Aplikasi Glosarium IT - Manajemen Istilah Teknologi";
                SizeF footerSize = e.Graphics.MeasureString(footer, new Font("Arial", 10, FontStyle.Italic));
                e.Graphics.DrawString(footer, new Font("Arial", 10, FontStyle.Italic),
                    Brushes.Gray, (pageWidth - footerSize.Width) / 2, pageHeight - 100);

                currentPrintPage++;
                e.HasMorePages = true;
                return;
            }

            // === CONTENT PAGES ===
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font contentFont = new Font("Arial", 10);
            Font labelFont = new Font("Arial", 9, FontStyle.Bold);

            float yPos = 100;
            float leftMargin = 80;
            float rightMargin = pageWidth - 80;

            // Page header
            e.Graphics.DrawString("DAFTAR GLOSARIUM IT", new Font("Arial", 16, FontStyle.Bold),
                new SolidBrush(Color.FromArgb(41, 128, 185)), leftMargin, 50);
            e.Graphics.DrawString($"Kategori: {currentFilter}", new Font("Arial", 11),
                Brushes.DimGray, leftMargin, 75);

            // PERSYARATAN: For loop - Print items (4 per page untuk layout rapi)
            int itemsPerPage = 4;
            int itemsPrinted = 0;

            // PERSYARATAN: While loop - Print hingga halaman penuh atau data habis
            while (currentItemIndex < dataToPrint.Count && itemsPrinted < itemsPerPage)
            {
                Term term = dataToPrint[currentItemIndex];

                // Item box dengan border
                float boxHeight = 140;
                RectangleF itemBox = new RectangleF(leftMargin, yPos, rightMargin - leftMargin, boxHeight);

                // Shadow effect
                RectangleF shadowBox = new RectangleF(leftMargin + 3, yPos + 3,
                    rightMargin - leftMargin, boxHeight);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, 200, 200)), shadowBox);

                // Main box
                e.Graphics.FillRectangle(Brushes.White, itemBox);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(189, 195, 199), 2), Rectangle.Round(itemBox));

                float contentX = leftMargin + 15;
                float contentY = yPos + 15;

                // Number badge
                RectangleF numberBadge = new RectangleF(contentX, contentY, 35, 35);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(52, 152, 219)), numberBadge);

                // PERSYARATAN: Calculate - Item numbering
                string itemNumber = (currentItemIndex + 1).ToString();
                SizeF numSize = e.Graphics.MeasureString(itemNumber, new Font("Arial", 12, FontStyle.Bold));
                e.Graphics.DrawString(itemNumber, new Font("Arial", 12, FontStyle.Bold),
                    Brushes.White, contentX + (35 - numSize.Width) / 2, contentY + (35 - numSize.Height) / 2);

                // Word (Title)
                e.Graphics.DrawString(term.Word, new Font("Arial", 14, FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(44, 62, 80)), contentX + 50, contentY + 5);

                // Category badge
                RectangleF catBadge = new RectangleF(rightMargin - 150, contentY + 5, 120, 25);

                // PERSYARATAN: Switch-case - Warna per kategori
                Color categoryColor;
                switch (term.Category)
                {
                    case TermCategory.Pemrograman:
                        categoryColor = Color.FromArgb(52, 152, 219);
                        break;
                    case TermCategory.Jaringan:
                        categoryColor = Color.FromArgb(46, 204, 113);
                        break;
                    case TermCategory.Algoritma:
                        categoryColor = Color.FromArgb(230, 126, 34);
                        break;
                    case TermCategory.Database:
                        categoryColor = Color.FromArgb(155, 89, 182);
                        break;
                    case TermCategory.Hardware:
                        categoryColor = Color.FromArgb(231, 76, 60);
                        break;
                    default:
                        categoryColor = Color.FromArgb(149, 165, 166);
                        break;
                }

                e.Graphics.FillRectangle(new SolidBrush(categoryColor), catBadge);
                SizeF catSize = e.Graphics.MeasureString(term.Category.ToString(), new Font("Arial", 9, FontStyle.Bold));
                e.Graphics.DrawString(term.Category.ToString(), new Font("Arial", 9, FontStyle.Bold),
                    Brushes.White, rightMargin - 150 + (120 - catSize.Width) / 2, contentY + 10);

                contentY += 45;

                // Separator
                e.Graphics.DrawLine(new Pen(Color.FromArgb(189, 195, 199)),
                    contentX, contentY, rightMargin - 15, contentY);
                contentY += 10;

                // Definition label
                e.Graphics.DrawString("Definisi:", labelFont, Brushes.DimGray, contentX, contentY);
                contentY += 18;

                // Definition text (word wrap)
                // PERSYARATAN: String methods - Text handling
                string definition = term.Definition;
                RectangleF defRect = new RectangleF(contentX, contentY, rightMargin - contentX - 30, 50);
                e.Graphics.DrawString(definition, contentFont, Brushes.Black, defRect);

                yPos += boxHeight + 20;
                currentItemIndex++;
                itemsPrinted++;
            }

            // Footer dengan page number
            e.Graphics.DrawLine(new Pen(Color.LightGray, 1), leftMargin, pageHeight - 60,
                rightMargin, pageHeight - 60);

            // PERSYARATAN: Calculate - Page numbering
            int pageNum = currentPrintPage;
            string pageText = $"Halaman {pageNum}";
            e.Graphics.DrawString(pageText, new Font("Arial", 9), Brushes.Gray, leftMargin, pageHeight - 50);
            e.Graphics.DrawString("Glosarium App © 2024", new Font("Arial", 9), Brushes.Gray,
                rightMargin - 150, pageHeight - 50);

            // PERSYARATAN: If statement - Check if there are more pages
            if (currentItemIndex < dataToPrint.Count)
            {
                currentPrintPage++;
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                currentPrintPage = 0;
                currentItemIndex = 0;
            }
        }
    }
}