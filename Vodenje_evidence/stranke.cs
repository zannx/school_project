using DevComponents.DotNetBar;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Vodenje_evidence
{
    public partial class stranke : Office2007Form
    {
        public stranke()
        {
            InitializeComponent();
        }

        private void stranke_Load(object sender, EventArgs e)
        {

        }
        public void odpri(string id_s) {
            labelX6.Text = id_s;
            if (id_s == "")
            {
            }
            else
            {
                Form1.con.Open();
                string sql = "SELECT ime_stranke,naslov,kraj_id,(select naziv_poste from kraji where id=kraj_id) FROM stranke where id=" + id_s;
                var cmd = new NpgsqlCommand(sql, Form1.con);

                NpgsqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    textBoxX3.Text=rdr.GetString(0);
                    textBoxX1.Text = rdr.GetString(1);
                    textBoxX2.Text = rdr.GetInt32(2).ToString();
                    labelX3.Text = rdr.GetString(3);
                }
                Form1.con.Close();
            }
            this.ShowDialog();
        }
        private void buttonX2_Click(object sender, EventArgs e)
        {
            textBoxX1.Text = "";
            textBoxX2.Text = "";
            textBoxX3.Text = "";
            this.Close();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (textBoxX2.Text == "") { textBoxX2.Text = "42"; }
            string sql = "select posodobi_stranko('" + textBoxX3.Text + "','" + textBoxX1.Text + "'," + labelX6.Text + "," + textBoxX2.Text + ")";
            if (labelX6.Text == "") {
                sql = "select dodaj_stranko('" + textBoxX3.Text + "','" + textBoxX1.Text + "'," + textBoxX2.Text + ");";
            }
            Form1.con.Open();
            var cmd = new NpgsqlCommand(sql, Form1.con);
            cmd.ExecuteNonQuery();
            Form1.con.Close();
            this.Close();
            
        }
    }
}
