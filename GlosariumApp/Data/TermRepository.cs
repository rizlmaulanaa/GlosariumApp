using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlosariumApp.Models;

namespace GlosariumApp.Data
{
    // CLASS STATIC: Penyimpanan data sementara
    // PERSYARATAN: Collection (List<Term>)
    public static class TermRepository
    {
        // PERBAIKAN: Gunakan property bukan field
        public static List<Term> Terms { get; set; } = new List<Term>();

        // PERSYARATAN: Collection untuk activity logs
        public static List<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

        // PERSYARATAN: String untuk path file
        private static readonly string LogFilePath = "activity_log.txt";

        public static void SeedData()
        {
            if (Terms.Count == 0)
            {
                Terms.Add(new Term { Word = "Variable", Definition = "Wadah penyimpanan nilai yang dapat berubah dalam program.", Category = TermCategory.Pemrograman });
                Terms.Add(new Term { Word = "Bandwidth", Definition = "Kapasitas maksimum transfer data dalam jaringan.", Category = TermCategory.Jaringan });
                Terms.Add(new Term { Word = "Recursion", Definition = "Fungsi yang memanggil dirinya sendiri.", Category = TermCategory.Algoritma });
                Terms.Add(new Term { Word = "Foreign Key", Definition = "Kunci tamu untuk menghubungkan dua tabel database.", Category = TermCategory.Database });
                Terms.Add(new Term { Word = "SSD", Definition = "Media penyimpanan flash memory, lebih cepat dari HDD.", Category = TermCategory.Hardware });
                Terms.Add(new Term { Word = "Algorithm", Definition = "Langkah-langkah logis untuk menyelesaikan masalah.", Category = TermCategory.Algoritma });
                Terms.Add(new Term { Word = "Router", Definition = "Perangkat yang menghubungkan beberapa jaringan.", Category = TermCategory.Jaringan });
                Terms.Add(new Term { Word = "Array", Definition = "Struktur data yang menyimpan elemen sejenis.", Category = TermCategory.Pemrograman });
                Terms.Add(new Term { Word = "SQL", Definition = "Bahasa query untuk mengelola database relasional.", Category = TermCategory.Database });
                Terms.Add(new Term { Word = "CPU", Definition = "Central Processing Unit, otak komputer.", Category = TermCategory.Hardware });
            }
        }

        // PERSYARATAN: Method dengan parameter
        public static void AddTerm(Term term)
        {
            Terms.Add(term);

            // PERSYARATAN: Date, Time - Log dengan timestamp
            LogActivity("ADD", $"Menambahkan istilah: {term.Word}");
        }

        // PERSYARATAN: Method dengan parameter
        public static void UpdateTerm(Term term)
        {
            term.LastModified = DateTime.Now;
            LogActivity("EDIT", $"Mengubah istilah: {term.Word}");
        }

        // PERSYARATAN: Method dengan parameter
        public static void DeleteTerm(Term term)
        {
            Terms.Remove(term);
            LogActivity("DELETE", $"Menghapus istilah: {term.Word}");
        }

        // PERSYARATAN: File operations - Menyimpan log ke file (PUBLIC)
        public static void LogActivity(string action, string details)
        {
            var log = new ActivityLog(action, details);
            ActivityLogs.Add(log);

            try
            {
                // PERSYARATAN: Files - Menulis ke file text
                File.AppendAllText(LogFilePath, log.ToString() + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                System.Windows.Forms.MessageBox.Show($"Error saat menyimpan log: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // PERSYARATAN: Files - Membaca log dari file
        public static string[] ReadLogFile()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    // PERSYARATAN: Array - Return array of strings
                    return File.ReadAllLines(LogFilePath);
                }
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Error handling
                System.Windows.Forms.MessageBox.Show($"Error membaca log: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            // PERBAIKAN: Gunakan Array.Empty
            return Array.Empty<string>();
        }

        // PERSYARATAN: Files, File dates and times - Info file log
        public static string GetLogFileInfo()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    FileInfo fileInfo = new FileInfo(LogFilePath);

                    // PERSYARATAN: File dates and times, Formatted text
                    return $"File: {fileInfo.Name}\n" +
                           $"Size: {fileInfo.Length} bytes\n" +
                           $"Created: {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}\n" +
                           $"Last Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}";
                }
                else
                {
                    return "File log belum ada.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // PERSYARATAN: Files, Array - Export ke CSV (FORMAT SIMPLE - NO DATES)
        public static bool ExportToCSV(string filePath)
        {
            try
            {
                // PERSYARATAN: Array - Konversi List ke Array
                Term[] termsArray = Terms.ToArray();

                var lines = new List<string>
                {
                    // HEADER SIMPLE: Word, Definition, Category
                    "Word,Definition,Category"
                };

                // PERSYARATAN: For loop dengan array
                for (int i = 0; i < termsArray.Length; i++)
                {
                    Term t = termsArray[i];

                    // PERSYARATAN: String methods - Replace untuk handle koma dan quotes
                    string word = t.Word.Replace(",", ";").Replace("\"", "'");
                    string def = t.Definition.Replace(",", ";").Replace("\"", "'");

                    // FORMAT SIMPLE: hanya 3 kolom
                    lines.Add($"{word},{def},{t.Category}");
                }

                // PERSYARATAN: Files - Menulis ke file CSV
                File.WriteAllLines(filePath, lines);

                LogActivity("EXPORT", $"Export {termsArray.Length} istilah ke {Path.GetFileName(filePath)}");
                return true;
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Handling exceptions
                System.Windows.Forms.MessageBox.Show($"Error export: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        // PERSYARATAN: Files, Array - Import dari CSV (NO DUPLICATE)
        public static bool ImportFromCSV(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File tidak ditemukan!");
                }

                // PERSYARATAN: Files, Array - Baca sebagai array
                string[] lines = File.ReadAllLines(filePath);

                int importCount = 0;
                int duplicateCount = 0;
                int errorCount = 0;

                // PERSYARATAN: For loop - Skip header (start from 1)
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        // PERSYARATAN: String methods - Trim whitespace
                        string line = lines[i].Trim();

                        // Skip empty lines
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length >= 3)
                        {
                            // PERSYARATAN: String methods - Trim each part
                            string word = parts[0].Trim();
                            string definition = parts[1].Trim();
                            string categoryStr = parts[2].Trim();

                            // PERSYARATAN: If statement - Check duplicate (case-insensitive)
                            bool isDuplicate = Terms.Any(t =>
                                t.Word.Equals(word, StringComparison.OrdinalIgnoreCase));

                            if (isDuplicate)
                            {
                                duplicateCount++;
                                continue; // Skip duplicate
                            }

                            // PERSYARATAN: String methods, If statement - Parse category
                            TermCategory category;
                            if (!Enum.TryParse<TermCategory>(categoryStr, true, out category))
                            {
                                category = TermCategory.Lainnya;
                            }

                            var newTerm = new Term
                            {
                                Word = word,
                                Definition = definition,
                                Category = category
                            };

                            Terms.Add(newTerm);
                            importCount++;
                        }
                        else
                        {
                            errorCount++;
                        }
                    }
                    catch
                    {
                        errorCount++;
                    }
                }

                // Log hasil import
                string logDetails = $"Import dari {Path.GetFileName(filePath)} - " +
                                  $"Berhasil: {importCount}, Duplikat: {duplicateCount}, Error: {errorCount}";
                LogActivity("IMPORT", logDetails);

                // PERSYARATAN: If statement - Show detailed result
                if (importCount > 0 || duplicateCount > 0 || errorCount > 0)
                {
                    string resultMessage = $"Import selesai!\n\n" +
                                         $"✅ Berhasil ditambahkan: {importCount}\n" +
                                         $"⚠️  Duplikat (dilewati): {duplicateCount}\n" +
                                         $"❌ Error (dilewati): {errorCount}";

                    System.Windows.Forms.MessageBox.Show(resultMessage, "Hasil Import",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                }

                return true;
            }
            catch (Exception ex)
            {
                // PERSYARATAN: Error handling
                System.Windows.Forms.MessageBox.Show($"Error import: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        // PERSYARATAN: Direktori - Cek dan buat direktori backup
        public static void CreateBackupDirectory()
        {
            try
            {
                string backupDir = "Backups";

                // PERSYARATAN: Direktori - Cek eksistensi direktori
                if (!Directory.Exists(backupDir))
                {
                    // PERSYARATAN: Direktori - Membuat direktori baru
                    Directory.CreateDirectory(backupDir);
                    LogActivity("SYSTEM", "Membuat folder backup");
                }

                // PERSYARATAN: Direktori - List files dalam direktori
                string[] backupFiles = Directory.GetFiles(backupDir, "*.csv");

                if (backupFiles.Length > 5)
                {
                    // PERSYARATAN: While loop - Hapus file lama jika lebih dari 5
                    int index = 0;
                    while (index < backupFiles.Length - 5)
                    {
                        File.Delete(backupFiles[index]);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error backup: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}