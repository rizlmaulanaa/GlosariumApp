using System;
using System.Drawing;
using System.Windows.Forms;
using GlosariumApp.Models;

namespace GlosariumApp.Forms
{
    // Hapus 'partial'
    public class TermDialog : Form
    {
        public Term TermData { get; private set; } = new Term(); // Inisialisasi TermData

        // INISIALISASI DI SINI: Mengatasi error 'Non-nullable field X must contain a non-null value'
        private TextBox txtWord = new TextBox();
        private TextBox txtDef = new TextBox();
        private ComboBox cmbCat = new ComboBox();
        private Button btnSave = new Button();
        private Button btnCancel = new Button();

        public TermDialog(Term existingTerm = null)
        {
            InitializeComponentModern();

            cmbCat.DataSource = Enum.GetValues(typeof(TermCategory));

            if (existingTerm != null)
            {
                TermData = existingTerm;
                txtWord.Text = TermData.Word;
                txtDef.Text = TermData.Definition;
                cmbCat.SelectedItem = TermData.Category;
                this.Text = "Edit Istilah";
            }
            else
            {
                // TermData sudah diinisialisasi di deklarasi
                this.Text = "Tambah Istilah Baru";
                if (cmbCat.Items.Count > 0)
                {
                    cmbCat.SelectedIndex = 0;
                }
            }
            btnSave.Click += BtnSave_Click;
        }

        private void InitializeComponentModern()
        {
            // ... (Kode UI sama seperti sebelumnya) ...
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            Label lblWord = new Label { Text = "Istilah:", Top = 20, Left = 30, AutoSize = true };

            txtWord.Top = 45; txtWord.Left = 30; txtWord.Width = 370; txtWord.Font = new Font("Segoe UI", 11);

            Label lblCat = new Label { Text = "Kategori:", Top = 90, Left = 30, AutoSize = true };

            cmbCat.Top = 115; cmbCat.Left = 30; cmbCat.Width = 370; cmbCat.DropDownStyle = ComboBoxStyle.DropDownList; cmbCat.Font = new Font("Segoe UI", 11);

            Label lblDef = new Label { Text = "Definisi:", Top = 160, Left = 30, AutoSize = true };

            txtDef.Top = 185; txtDef.Left = 30; txtDef.Width = 370; txtDef.Height = 100; txtDef.Multiline = true; txtDef.ScrollBars = ScrollBars.Vertical; txtDef.Font = new Font("Segoe UI", 11);

            btnSave.Text = "SIMPAN"; btnSave.Top = 310; btnSave.Left = 230; btnSave.Width = 80; btnSave.Height = 35; btnSave.BackColor = Color.DodgerBlue; btnSave.ForeColor = Color.White; btnSave.FlatStyle = FlatStyle.Flat; btnSave.DialogResult = DialogResult.OK; btnSave.Cursor = Cursors.Hand;
            btnCancel.Text = "BATAL"; btnCancel.Top = 310; btnCancel.Left = 320; btnCancel.Width = 80; btnCancel.Height = 35; btnCancel.BackColor = Color.LightGray; btnCancel.FlatStyle = FlatStyle.Flat; btnCancel.DialogResult = DialogResult.Cancel; btnCancel.Cursor = Cursors.Hand;

            this.Controls.AddRange(new Control[] { lblWord, txtWord, lblCat, cmbCat, lblDef, txtDef, btnSave, btnCancel });
            this.AcceptButton = btnSave;
        }

        private void BtnSave_Click(object? sender, EventArgs e) // Ubah object sender menjadi object?
        {
            if (string.IsNullOrWhiteSpace(txtWord.Text) || string.IsNullOrWhiteSpace(txtDef.Text))
            {
                MessageBox.Show("Mohon lengkapi semua isian.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            TermData.Word = txtWord.Text.Trim();
            TermData.Definition = txtDef.Text.Trim();
            TermData.Category = (TermCategory)cmbCat.SelectedItem!; // Tambah '!' karena pasti ada item
        }
    }
}