using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GlosariumApp.Data;

namespace GlosariumApp.Forms
{
    // FORM: Import/Export operations (SIMPLE & CLEAN)
    // PERSYARATAN: Files, Direktori
    public class FileOperationsForm : Form
    {
        private TextBox txtFilePath;
        private Label lblStatus;
        private string defaultExportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "glosarium_backup.csv");

        public FileOperationsForm()
        {
            InitializeUI();
            txtFilePath.Text = defaultExportPath;
        }

        private void InitializeUI()
        {
            this.Text = "Import / Export Data";
            this.Size = new Size(650, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // === HEADER ===
            var lblTitle = new Label
            {
                Text = "💾 Kelola File Data",
                Top = 20,
                Left = 20,
                Width = 610,
                Height = 40,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // === FILE PATH SECTION ===
            var lblPath = new Label
            {
                Text = "Lokasi File:",
                Top = 80,
                Left = 20,
                Width = 100,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtFilePath = new TextBox
            {
                Top = 105,
                Left = 20,
                Width = 500,
                Height = 30,
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                BackColor = Color.FromArgb(236, 240, 241)
            };

            var btnBrowse = new Button
            {
                Text = "...",
                Top = 105,
                Left = 530,
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Click += BtnBrowse_Click;

            // === INFO BOX ===
            var pnlInfo = new Panel
            {
                Top = 150,
                Left = 20,
                Width = 590,
                Height = 80,
                BackColor = Color.FromArgb(241, 248, 233),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15, 10, 15, 10)
            };

            var lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "💡 Export: Simpan semua data istilah ke file CSV\n" +
                       "💡 Import: Tambahkan data dari file CSV ke database",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(85, 107, 47),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlInfo.Controls.Add(lblInfo);

            // === ACTION BUTTONS ===
            var btnExport = new Button
            {
                Text = "📤 Export Data",
                Top = 250,
                Left = 20,
                Width = 285,
                Height = 50,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;

            var btnImport = new Button
            {
                Text = "📥 Import Data",
                Top = 250,
                Left = 325,
                Width = 285,
                Height = 50,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.Click += BtnImport_Click;

            // === STATUS LABEL ===
            lblStatus = new Label
            {
                Top = 315,
                Left = 20,
                Width = 590,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = "Siap untuk export atau import data."
            };

            // === ADD ALL CONTROLS ===
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPath);
            this.Controls.Add(txtFilePath);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(pnlInfo);
            this.Controls.Add(btnExport);
            this.Controls.Add(btnImport);
            this.Controls.Add(lblStatus);
        }

        // PERSYARATAN: Files - Browse untuk pilih lokasi file
        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "CSV Files (*.csv)|*.csv";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "glosarium_backup.csv";
                    dialog.Title = "Pilih Lokasi File";
                    dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        txtFilePath.Text = dialog.FileName;
                        lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
                        lblStatus.Text = $"📂 Lokasi file diubah ke: {Path.GetFileName(dialog.FileName)}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PERSYARATAN: Files - Export data ke CSV
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                // PERSYARATAN: If statement - Validasi data
                if (TermRepository.Terms.Count == 0)
                {
                    MessageBox.Show("Tidak ada data untuk di-export!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string filePath = txtFilePath.Text;

                // PERSYARATAN: String methods - Validation
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    MessageBox.Show("Pilih lokasi file terlebih dahulu!", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Konfirmasi jika file sudah ada
                if (File.Exists(filePath))
                {
                    var result = MessageBox.Show(
                        $"File sudah ada:\n{filePath}\n\nTimpa file yang ada?",
                        "Konfirmasi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                        return;
                }

                // PERSYARATAN: Files, Array - Export menggunakan array
                bool success = TermRepository.ExportToCSV(filePath);

                if (success)
                {
                    lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
                    lblStatus.Text = $"✅ Export berhasil! {TermRepository.Terms.Count} istilah disimpan.";

                    var openFolder = MessageBox.Show(
                        $"Export berhasil!\n\n" +
                        $"File: {Path.GetFileName(filePath)}\n" +
                        $"Lokasi: {Path.GetDirectoryName(filePath)}\n" +
                        $"Total: {TermRepository.Terms.Count} istilah\n\n" +
                        $"Buka lokasi file?",
                        "Export Sukses",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (openFolder == DialogResult.Yes)
                    {
                        // PERSYARATAN: Files - Buka folder
                        System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"❌ Error: {ex.Message}";
                MessageBox.Show($"Error saat export:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PERSYARATAN: Files - Import data dari CSV
        private void BtnImport_Click(object? sender, EventArgs e)
        {
            try
            {
                // PERSYARATAN: Files - Open file dialog
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                    dialog.Title = "Pilih File untuk Import";
                    dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = dialog.FileName;

                        // PERSYARATAN: Files - Check file existence
                        if (!File.Exists(filePath))
                        {
                            MessageBox.Show("File tidak ditemukan!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // PERSYARATAN: File dates and times - Show file info
                        FileInfo fileInfo = new FileInfo(filePath);
                        long fileSize = fileInfo.Length;
                        DateTime lastModified = fileInfo.LastWriteTime;

                        var confirmResult = MessageBox.Show(
                            $"Import data dari file:\n\n" +
                            $"Nama: {Path.GetFileName(filePath)}\n" +
                            $"Ukuran: {fileSize:N0} bytes\n" +
                            $"Terakhir diubah: {lastModified:dd/MM/yyyy HH:mm}\n\n" +
                            $"Data akan DITAMBAHKAN ke database.\n" +
                            $"Duplikat akan otomatis dilewati.\n\n" +
                            $"Lanjutkan?",
                            "Konfirmasi Import",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (confirmResult == DialogResult.Yes)
                        {
                            int beforeCount = TermRepository.Terms.Count;

                            // PERSYARATAN: Files, Array - Import dari CSV (NO DUPLICATE)
                            bool success = TermRepository.ImportFromCSV(filePath);

                            if (success)
                            {
                                int afterCount = TermRepository.Terms.Count;
                                int imported = afterCount - beforeCount;

                                lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
                                lblStatus.Text = $"✅ Import berhasil! {imported} istilah baru ditambahkan.";

                                // Detail message sudah ditampilkan di TermRepository.ImportFromCSV
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"❌ Error: {ex.Message}";
                MessageBox.Show($"Error saat import:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}