
## 📁 Struktur Project GlosariumApp

```text
GlosariumApp/
├── Program.cs                          # Entry point aplikasi
│
├── Models/
│   ├── Term.cs                         # Model data istilah + DateTime
│   ├── QuizSettings.cs                 # Model pengaturan kuis (BARU)
│   └── ActivityLog.cs                  # Model log aktivitas (BARU)
│
├── Data/
│   ├── TermRepository.cs               # Repository data + File Operations
│   └── StatisticsCalculator.cs         # Kalkulator statistik (BARU)
│
└── Forms/
    ├── MainMenuForm.cs                 # Menu utama + Real-time clock
    ├── GlosariumForm.cs                # CRUD Glosarium + Print System
    ├── QuizForm.cs                     # Kuis interaktif + Timer
    ├── TermDialog.cs                   # Dialog input/edit term
    ├── StatisticsForm.cs               # Statistik & Analisis (BARU)
    ├── ActivityLogForm.cs              # Riwayat aktivitas (BARU)
    └── FileOperationsForm.cs           # Import/Export CSV (BARU)



## **📊 TABEL KOMPONEN**

| No | Persyaratan | Status | File Utama | Line / Method |
|----|-------------|--------|------------|---------------|
| 1 | Watch Variables | ✅ | Semua file | Dapat di-debug dengan breakpoint |
| 2 | Breakpoints | ✅ | Semua file | Dapat set breakpoint di semua method |
| 3 | Exception Handling | ✅ | TermRepository.cs<br>QuizForm.cs<br>TermDialog.cs<br>FileOperationsForm.cs | Line 44, 60, 73, 121, 156<br>Constructor validation<br>BtnSave_Click()<br>Export / Import methods |
| 4 | Error Messages | ✅ | Same as above | MessageBox.Show() |
| 5 | String Identification | ✅ | Term.cs<br>ActivityLog.cs | Word, Definition, Id<br>Action, Details, User |
| 6 | String Methods | ✅ | TermDialog.cs<br>TermRepository.cs | .Trim(), .IsNullOrWhiteSpace() (73–79)<br>.Replace(), .Split(), .Substring() |
| 7 | Date, Time, TimeSpan | ✅ | Term.cs<br>QuizSettings.cs<br>ActivityLog.cs<br>MainMenuForm.cs<br>QuizForm.cs | CreatedDate, LastModified (15–16)<br>TimeLimit<br>Timestamp (12)<br>Real-time clock (48–56)<br>Timer countdown |
| 8 | Array | ✅ | TermRepository.cs<br>StatisticsCalculator.cs | ReadLogFile() → string[] (86)<br>ExportToCSV() → Term[] (125)<br>GetCategoryDistribution() → int[] |
| 9 | Collection | ✅ | TermRepository.cs | List<Term>, List<ActivityLog> (12–13) |
|10 | Array + Collection | ✅ | Berbagai file | Array dan List digunakan bersamaan |
|11 | If Statements | ✅ | GlosariumForm.cs<br>QuizForm.cs<br>TermDialog.cs | Filter logic (136–142)<br>Answer validation, timeout<br>Input validation (73–79) |
|12 | Switch-Case | ✅ | QuizSettings.cs<br>GlosariumForm.cs | SetDifficulty() (28–54)<br>Category color mapping |
|13 | For Loop | ✅ | TermRepository.cs<br>StatisticsCalculator.cs | Export CSV (131–139)<br>Import CSV (160–173)<br>Array iteration (20–25) |
|14 | Foreach Loop | ✅ | GlosariumForm.cs<br>StatisticsCalculator.cs | Enum iteration (30)<br>Enum values, TimeSpan collection |
|15 | While Loop | ✅ | TermRepository.cs<br>StatisticsCalculator.cs | Delete old backups (193–198)<br>Find longest definition (51–61) |
|16 | Do-While Loop | ✅ | StatisticsCalculator.cs | ValidateScore() (68–80) |
|17 | Functions & Methods | ✅ | Semua file | Banyak custom methods |
|18 | ByVal & ByRef | ✅ | StatisticsCalculator.cs | ref int score (64)<br>out string[], out int[] (96) |
|19 | Passing Arrays | ✅ | StatisticsCalculator.cs | GenerateStatisticsReport(int[]) (29) |
|20 | Parameters | ✅ | TermDialog.cs<br>Semua file | Constructor optional parameter<br>Berbagai tipe parameter |
|21 | Files | ✅ | TermRepository.cs | ReadLogFile(), ExportToCSV(), ImportFromCSV() |
|22 | File Dates / Times | ✅ | TermRepository.cs | GetLogFileInfo() – FileInfo |
|23 | Directory | ✅ | TermRepository.cs | CreateBackupDirectory() (175–201) |
|24 | Namespace | ✅ | Semua file | GlosariumApp.* hierarchy |
|25 | Commenting | ✅ | Semua file | Komentar persyaratan di setiap fitur |
|26 | Formatted Text | ✅ | Semua file | String interpolation $"{var}" |
|27 | Calculate | ✅ | StatisticsCalculator.cs<br>QuizForm.cs | Percentage & average<br>Score calculation |
|28 | Print | ✅ | GlosariumForm.cs | PrintDocument, PrintDialog, PrintPreview |
