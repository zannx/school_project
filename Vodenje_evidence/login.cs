using DevComponents.DotNetBar;
using Npgsql;
using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Vodenje_evidence
{
    public partial class login : Office2007Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {


            string sql = "select uporabnik_pass('" + textBoxX1.Text + "','" + textBoxX2.Text + "')";
           
            Form1.con.Open();
            var cmd = new NpgsqlCommand(sql, Form1.con);
            string pass = "";
            try { 
            pass= cmd.ExecuteScalar().ToString();
            }catch
            {
            }
                
            Form1.con.Close();
            if (pass.Trim() == "")
            {
                MessageBox.Show("NAPAČNO GESLO!"); 
            }else
            {
                Program.uporabnik = pass;
               this.Close();
                Program.uporabnik_ime =  textBoxX1.Text;
            
                 
            }
            
        }

        private void login_Load(object sender, EventArgs e)
        {
        
        }

        private void textBoxX2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxX2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { 
                buttonX1.PerformClick();
            }
        }

        private void textBoxX1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBoxX2.Focus();
                textBoxX2.SelectAll();
            }
            }
    }
}
