using System;

namespace GlosariumApp.Models
{
    // ENUM: Daftar pilihan kategori
    // PERSYARATAN: Identifikasi enum untuk select case
    public enum TermCategory
    {
        Pemrograman,
        Jaringan,
        Algoritma,
        Database,
        Hardware,
        Lainnya
    }

    // CLASS: Cetakan dasar untuk satu istilah
    // PERSYARATAN: Identifikasi string (Word, Definition, Id)
    public class Term
    {
        public string Id { get; set; }

        // PERBAIKAN: Tambah required atau inisialisasi default
        public string Word { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public TermCategory Category { get; set; }

        // PERSYARATAN: Date, Time - Menambahkan tracking waktu
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        public Term()
        {
            Id = Guid.NewGuid().ToString();
            // PERSYARATAN: Date, Time - Inisialisasi tanggal
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
    }
}