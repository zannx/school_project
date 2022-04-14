using DevComponents.DotNetBar;
using DevComponents.Schedule.Model;
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
    public partial class Dogodek : Office2007Form
    {
        public Dogodek()
        {
            InitializeComponent();
        }

        private void Dogodek_Load(object sender, EventArgs e)
        {
           Form1.con.Open();
            string sql = "SELECT ime_stranke FROM stranke";
            var cmd = new NpgsqlCommand(sql, Form1.con);
            comboBox1.Items.Clear();
            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                comboBox1.Items.Add( rdr.GetString(0));

            }
            Form1.con.Close();
        }
        public Appointment AddNewAppointment(DateTime startDate, DateTime endDate, string projekt)
        {

            Appointment appointment = new Appointment();
            dateTimeInput1.Value  = startDate;
            dateTimeInput2.Value = endDate ;
            comboBox1.Text = projekt;
            this.ShowDialog();

            appointment.StartTime = startDate;
            appointment.EndTime = endDate;

            appointment.Subject = textBoxX1.Text ;

            appointment.Description = textBoxX2.Text;
            appointment.Tooltip = textBoxX3.Text;



            // calendarView1.CalendarModel.Appointments.Add(appointment);

            return (appointment);
        }
        public Appointment EditAppointment(DateTime startDate, DateTime endDate,string subj,string desc,string tool,string projekt,int idd)
        {
            labelX8.Text = idd.ToString();
            Appointment appointment = new Appointment();
            dateTimeInput1.Value = startDate;
            dateTimeInput2.Value = endDate;
            textBoxX1.Text = subj;
            textBoxX2.Text = desc;
            textBoxX3.Text = tool;
            comboBox1.Text = projekt;
            this.ShowDialog();

            appointment.StartTime = startDate;
            appointment.EndTime = endDate;

            appointment.Subject = textBoxX1.Text;
            
            appointment.Description = textBoxX2.Text;
            appointment.Tooltip = textBoxX3.Text;



            // calendarView1.CalendarModel.Appointments.Add(appointment);

            return (appointment);
        }
        private void buttonX2_Click(object sender, EventArgs e)
        {
            textBoxX1.Text = "";
            this.Close() ;


        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (textBoxX1.Text == "") {
                MessageBox.Show("Zadeva je obvezen podatek!");
                    return; 
            }

            Form1.con.Open();
            var cmda = new NpgsqlCommand("select stranka_id('"+comboBox1.Text.ToString()+"')", Form1.con);
            string ids=cmda.ExecuteScalar().ToString();
            Form1.con.Close();
            if (Program.uporabnik == null) { Program.uporabnik = "1"; }
                string sql = "select posodobi_projekt('" + dateTimeInput1.Value.ToString("yyyy-MM-dd HH:mm") + "','" + dateTimeInput2.Value.ToString("yyyy-MM-dd HH:mm") + "','" + textBoxX1.Text + "','" + textBoxX3.Text + "','" + textBoxX2.Text + "'," + Program.uporabnik  + ","+ ids + ","+labelX8.Text +")";
            if (labelX8.Text == "")
            {
                sql = "select dodaj_projekt('" + dateTimeInput1.Value.ToString("yyyy-MM-dd HH:mm") + "','" + dateTimeInput2.Value.ToString("yyyy-MM-dd HH:mm") + "','" + textBoxX1.Text + "','" + textBoxX3.Text + "','" + textBoxX2.Text + "'," + Program.uporabnik + "," + ids + ")";
            }
            Form1.con.Open();
            var cmd = new NpgsqlCommand(sql, Form1.con);
            cmd.ExecuteNonQuery();
            Form1.con.Close();

            this.Close();
        }
    }
}
