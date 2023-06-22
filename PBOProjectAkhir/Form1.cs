using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql; // untuk mengenali class Npgsqlconnection =>>> Dbconnection

namespace PBOProjectAkhir
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection conn; // untuk menghubungkan dengan database
        //connectionstring => server (alamat database server), port (port database server),
        //user id (username database yang digunakan untuk terhubung ke database), password (password dari username),
        //database (nama database yang akan dipakai)
        string connstring = String.Format("Server = {0}; Port={1};" +
                        "User Id={2}; Password={3}; Database={4};", "localhost", "5432", "postgres", 
                        "@postgre16teai", "database_projectakhir_gaterelibrary");
        private DataTable dt; // untuk menyimpan data dari database dalam bentuk tabel
        private NpgsqlCommand cmd; // untuk menjalankan query dalam database
        private string sql = null;

        private DataGridViewRow r;

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection(connstring); // ConnectionString ke dalam Constructor class Npgsqlconnection
            btnSelect.PerformClick();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                dgvData.DataSource = null;
                conn.Open(); // membuka koneksi ke database server
                sql = "select * from bk_select()";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvData.DataSource = dt;
                conn.Close(); // menutup koneksi ke database server
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Fail!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close(); // menutup koneksi ke database server
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from bk_insert(:_buku, :_pengarang, :_kategori)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_buku", txtBuku.Text);
                cmd.Parameters.AddWithValue("_pengarang", txtPengarang.Text);
                cmd.Parameters.AddWithValue("_kategori", txtKategori.Text);
                if ((int) cmd.ExecuteScalar() == 1)
                {
                    conn.Close();
                    MessageBox.Show("Data buku yang dimasukkan berhasil!", "Sukses!", MessageBoxButtons.OK, 
                                    MessageBoxIcon.Information);
                    btnSelect.PerformClick();
                    txtBuku.Text = txtKategori.Text = txtPengarang.Text = null;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Dgagal dimasukkan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close(); // menutup koneksi ke database server
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(r == null)
            {
                MessageBox.Show("tolong pilih murid yang akan diupdate", "opppp", MessageBoxButtons.OK, 
                                   MessageBoxIcon.Warning);
                return;
            }
            try
            {
                conn.Open();
                sql = @"select * from bk_update(:_id, :_buku, :_pengarang, :_kategori)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_id", r.Cells["_id"].Value.ToString());
                cmd.Parameters.AddWithValue("_buku", txtBuku.Text);
                cmd.Parameters.AddWithValue("_pengarang", txtPengarang.Text);
                cmd.Parameters.AddWithValue("_kategori", txtKategori.Text);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    conn.Close(); // menutup koneksi ke database server
                    MessageBox.Show("update berhasil", "bagus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSelect.PerformClick();
                    txtBuku.Text = txtKategori.Text = txtPengarang.Text = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "update gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close(); // menutup koneksi ke database server
            }
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                r = dgvData.Rows[e.RowIndex];
                txtBuku.Text = r.Cells["_buku"].Value.ToString();
                txtPengarang.Text = r.Cells["_pengarang"].Value.ToString();
                txtKategori.Text = r.Cells["_kategori"].Value.ToString();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("pilih yang mo diapus", "opps", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(MessageBox.Show("kamu yakin mo apus? ["+r.Cells["_kategori"].Value.ToString()+"]??", "konfirmasi hapus", 
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    conn.Open();
                    sql = @"select * from bk_delete(:_id)";
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("_id", r.Cells["_id"].Value.ToString());
                    if((int)cmd.ExecuteScalar() == 1)
                    {
                        conn.Close();
                        MessageBox.Show("delete berhasil", "oke berhasil", MessageBoxButtons.OK, 
                                        MessageBoxIcon.Information);
                        btnSelect.PerformClick();
                        txtBuku.Text = txtKategori.Text = txtPengarang.Text = null;
                        r = null;
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("error: " + ex.Message, "delete gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
