using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Schedule;
using DevComponents.Schedule.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Windows.Forms.DataVisualization.Charting;
using System.Web;

namespace Vodenje_evidence
{
    public partial class Form1 : Office2007Form
    {

        public static NpgsqlConnection con;
        private string server;
        private string database;
        private string uid;
        private string password;
        DataTable tablex = new DataTable("Customers");
        public Form1()
        {
            InitializeComponent();
        }
        private void narediizpis()
        {




            FastReport.Report r = new FastReport.Report();
            r.Preview = previewControl1;

            r.RegisterData(tablex, "eee");
            r.GetDataSource("eee").Enabled = true;

            r.Load(Application.StartupPath + @"\pregled_dobav.frx");

            r.Show();
            previewControl1.ZoomPageWidth();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.chart1.Titles.Add("Pregled dela ur za mesec po uporabnikih!");

            styleManager1.ManagerStyle = eStyle.Metro;
            DateTime Datumdo = DateTime.Now;
            DateTime Datumod = new DateTime(Datumdo.Year, Datumdo.Month, 1);

            dateTimeInput1.Value = Datumod;
            dateTimeInput2.Value = Datumdo;
            dataGridViewX1.Rows.Add("Testni projekt");
            dataGridViewX2.Rows.Add("Testni uporabnik");
            dataGridViewX1.CurrentCell = dataGridViewX1.Rows[0].Cells[0];
            server = "ella.db.elephantsql.com";
            database = "deijsesw";
            uid = "deijsesw";
            password = "Y8_xx2e3IyvkUvdMxjTVuYW8tvRMv6RS";
            var cs = "Host=" + server + ";Username=" + uid + ";Password=" + password + ";Database=" + database + "";
            con = new NpgsqlConnection(cs);
            napolnichart();
            login lo = new login();
            lo.ShowDialog();
            labelX5.Text = "Pozdravljen " + Program.uporabnik_ime;
            napolniprojekte(0);
            napolnistranke();
            napolniuporabnike();
            narediizpis();
        }

        private void napolniprojekte(int kva) { 
        
            if(kva==0){
            
          
            calendarView1.CalendarModel.Appointments.Clear();
            try { 
                tablex.Columns.Add("id", typeof(int));
            tablex.Columns.Add("Zacetek", typeof(DateTime));
            tablex.Columns.Add("Konec", typeof(DateTime));
            tablex.Columns.Add("descr", typeof(string));
            tablex.Columns.Add("subj", typeof(string));
            tablex.Columns.Add("toolt", typeof(string));
            tablex.Columns.Add("uporabnik", typeof(int));
            tablex.Columns.Add("stranka_id", typeof(int));
            tablex.Columns.Add("ime_stranke", typeof(string));
                    tablex.Columns.Add("Ime_up", typeof(string));
                }
                catch{}

            }
            tablex.Rows.Clear();
            con.Open();
            string sql = "select id,zacetek,konec,descr,subj,toolt,uporabnik_id,stranka_id,(select ime_stranke from stranke where id=stranka_id) as im_stranke ,(select rtrim(priimek) from "+(char)(34)+"Uporabniki"+(char)(34)+" where id=uporabnik_id) as im_u   from projekti";
             var cmd = new NpgsqlCommand(sql, con);
             //dataGridViewX1.Rows.Clear();
             NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                string subjec = "";
                try
                {
                    subjec = rdr.GetString(4).ToString().Trim();
                }
                catch{ 
                }
                


                if (subjec == "")
                {

                }else
                {
                   
                     DateTime startDate = rdr.GetDateTime(1);
                        DateTime endDate = rdr.GetDateTime(2);
                        string descr = rdr.GetString(3);
                        string subj = rdr.GetString(4);
                        string tool = rdr.GetString(5);
                        string projekt = rdr.GetString(8);
                        int projekt_id = rdr.GetInt32(0);  
                    if (kva == 0)
                    {
                     
                        Appointment appointment = new Appointment();
                        //dateTimeInput1.Value = startDate;
                        //dateTimeInput2.Value = endDate;
                        //id,zacetek,konec,descr,subj,toolt,uporabnik_id,stranka_id,(s
                
                        appointment.StartTime = startDate;
                        appointment.EndTime = endDate;

                        appointment.Subject = subj;

                        appointment.Description = descr;
                        appointment.Tooltip = tool;
                        appointment.Id = projekt_id;
                        appointment.Tag = projekt;

                        calendarView1.CalendarModel.Appointments.Add(appointment);

                        calendarView1.EnsureVisible(appointment);
                    }
                    tablex.Rows.Add(rdr.GetInt32(0), startDate, endDate, descr, subj, tool, rdr.GetInt32(6), rdr.GetInt32(7), rdr.GetString(8).ToString().Trim(), rdr.GetString(9).ToString().Trim());

                }

            }
            con.Close();
        }
        private void napolnistranke()
        {
            con.Open();
            string sql = "SELECT id,ime_stranke FROM stranke";
            var cmd = new NpgsqlCommand(sql, con);
            dataGridViewX1.Rows.Clear();
            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                try
                {
  dataGridViewX1.Rows.Add(rdr.GetInt32(0), rdr.GetString(1));
                }catch{ }
              

            }
            con.Close();
        }
        private void napolniuporabnike()
        {
            con.Open();
            string sql = "SELECT id,email FROM "+ ((char)34).ToString() + "Uporabniki"+ ((char)34).ToString() + "";
            var cmd = new NpgsqlCommand(sql, con);
            dataGridViewX2.Rows.Clear();
            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                try
                {
                    dataGridViewX2.Rows.Add(rdr.GetInt32(0), rdr.GetString(1));
                }catch{ }
              

            }
            con.Close();
        }
        private void napolnichart() {


            this.chart1.Series.Clear();
            this.chart1.Series.Clear();

         
            Series series = this.chart1.Series.Add("ŽanLuka");
           // series.ChartType = SeriesChartType.Spline;
            con.Open();
            string sql = "select  DATE_PART('month', zacetek),sum(DATE_PART('hour', konec - zacetek )) from projekti group by DATE_PART('month', zacetek) order by DATE_PART('month', zacetek)";
            var cmd = new NpgsqlCommand(sql, con);
      
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            int ii = 0;
            double a2 = 0;
            double a1 = 0;
            while (rdr.Read())
            {
                try
                {
                    ii = ii + 1;
                    a1 = rdr.GetDouble(0);
                    a2= rdr.GetDouble(1);
                    if (ii == 1) {
                        for (int i = 1; i < a1; i++) {
                            series.Points.AddXY(i, 0);
                        }
                    }
                    series.Points.AddXY(a1, a2);
                }
                catch { }


            }
            for (double i = a1+1; i < 13; i++)
            {
                series.Points.AddXY(i, 0);
            }
            con.Close();

           

        }
        private void btnDay_Click(object sender, EventArgs e)
        {
            calendarView1.SelectedView = eCalendarView.Day;
        }

        private void btnWeek_Click(object sender, EventArgs e)
        {
            calendarView1.SelectedView = eCalendarView.Week ;
        }

        private void btnMonth_Click(object sender, EventArgs e)
        {
            calendarView1.SelectedView = eCalendarView.Month ;
        }

        private void btnYear_Click(object sender, EventArgs e)
        {
            calendarView1.SelectedView = eCalendarView.Year ;
        }

        private void btnTimeLine_Click(object sender, EventArgs e)
        {
            calendarView1.SelectedView = eCalendarView.TimeLine ;
            
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
           
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            collapsibleSplitContainer1.SplitterDistance = groupPanel2.Left + groupPanel2.Width + 20;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            styleManager1.ManagerStyle = eStyle.VisualStudio2010Blue;
            previewControl1.UIStyle = FastReport.Utils.UIStyle.Office2010Blue;
            dateTimeInput1.ForeColor = Color.Black ;
            dateTimeInput2.ForeColor = Color.Black ;
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            styleManager1.ManagerStyle = eStyle.VisualStudio2012Dark  ;
            previewControl1.UIStyle = FastReport.Utils.UIStyle.Office2010Black;
            dateTimeInput1.ForeColor = Color.White;
            dateTimeInput2.ForeColor = Color.White;
        }

        private void buttonX4_Click(object sender, EventArgs e)
        {
            styleManager1.ManagerStyle = eStyle.VisualStudio2012Light ;
            previewControl1.UIStyle = FastReport.Utils.UIStyle.VisualStudio2012Light;
            dateTimeInput1.ForeColor = Color.Black;
            dateTimeInput2.ForeColor = Color.Black;
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            styleManager1.ManagerStyle = eStyle.Metro ;
            previewControl1.UIStyle = FastReport.Utils.UIStyle.VistaGlass ;
            dateTimeInput1.ForeColor = Color.Black;
            dateTimeInput2.ForeColor = Color.Black;
        }

        private void buttonX13_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddHours(2.5);

            if (calendarView1.DateSelectionStart.HasValue &&
                calendarView1.DateSelectionEnd.HasValue)
            {
                startDate = calendarView1.DateSelectionStart.Value;
                endDate = calendarView1.DateSelectionEnd.Value;
            }
            var doc = new Dogodek();
            string pro = dataGridViewX1.Rows[dataGridViewX1.CurrentCell.RowIndex ].Cells[1].Value.ToString();
            Appointment ap = doc.AddNewAppointment(startDate, endDate, pro);
            if (ap.Subject == "")
            {
            }
            else {
                calendarView1.CalendarModel.Appointments.Clear();
                napolniprojekte(0);
                napolnichart();
             
                narediizpis();
            }
         
        }

        private void calendarView1_ItemClick(object sender, EventArgs e)
        {

        }

        private void calendarView1_ItemDoubleClick(object sender, MouseEventArgs e)
        {
            AppointmentView item = sender as AppointmentView;

            if (item != null)
            {
                Appointment ap = item.Appointment;

                string s = string.Format(
                    "Subject: {0}\nDescription: {1}\nToolTip: {2}\n\n" +
                    "StartTime: {3}\nEndTime: {4}\n\n" +
                    "CategoryColor: {5}\nTimeMarkedAs: {6}",
                    ap.Subject, ap.Description, ap.Tooltip,
                    ap.StartTime, ap.EndTime,
                    String.IsNullOrEmpty(ap.CategoryColor) ? "Default" : ap.CategoryColor,
                    String.IsNullOrEmpty(ap.TimeMarkedAs) ? "Default" : ap.TimeMarkedAs);
                var doc = new Dogodek();
                string pro = ap.Tag.ToString();//dataGridViewX1.Rows[0].Cells[1].Value.ToString();
                ap = doc.EditAppointment(ap.StartTime, ap.EndTime, ap.Subject, ap.Description, ap.Tooltip, pro , ap.Id);
                calendarView1.CalendarModel.Appointments.Clear();
                napolniprojekte(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
             
           
            
        }

        private void buttonX7_Click(object sender, EventArgs e)
        {
            var doc = new stranke();
            doc.odpri(dataGridViewX1.Rows[dataGridViewX1.CurrentCell.RowIndex ].Cells[0].Value.ToString());
            napolniprojekte(0);
        }

        private void buttonX6_Click(object sender, EventArgs e)
        {
            var doc = new stranke();
            doc.odpri("");
            napolniprojekte(0);
        }

        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewX1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            buttonX13.PerformClick();
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            if (textBoxX1.Text.Trim().Length > 1)
            {
                for (int i = 0; i < dataGridViewX1.Rows.Count ; i++)
                {
                    string aa = textBoxX1.Text.Trim().ToUpper();
                    string bb = dataGridViewX1.Rows[i].Cells[1].Value.ToString().ToUpper();
                    bb = bb.Trim();
                    if (bb.Contains(aa) )
                    {
                        dataGridViewX1.Rows[i].Visible = true;
                    }
                    else 
                    {
                        dataGridViewX1.Rows[i].Visible = false ;
                    }
                  
                }
            }
            else {
                for (int i = 0; i < dataGridViewX1.Rows.Count-1; i++)
                {
                    dataGridViewX1.Rows[i].Visible = true;
                }
            }
        }

        private void calendarView1_DateSelectionEndChanged(object sender, DateSelectionEventArgs e)
        {
          
        }

        private void calendarView1_ItemLayoutUpdated(object sender, EventArgs e)
        {
           
           
        }

        private void calendarView1_AppointmentViewChanged(object sender, AppointmentViewChangedEventArgs e)
        {
 try
            {
                CalendarView cal = (CalendarView)sender;
                Appointment ap = cal.SelectedAppointments[0].Appointment;
                DateTime zac = ap.StartTime;
                DateTime kon = ap.EndTime;

                string sql = "select posodobi_projekt_cas('" + zac.ToString("yyyy-MM-dd HH:mm") + "','" + kon.ToString("yyyy-MM-dd HH:mm") + "'," + ap.Id + ")";

                con.Open();
                var cmd = new NpgsqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                con.Close();
                napolnichart();
                napolniprojekte(1);
                narediizpis();
            }
            catch{ 
            }
        }

        private void buttonX5_Click(object sender, EventArgs e)
        {

            Form1.con.Open();
            var cmd = new NpgsqlCommand("select brisi_stranko("+ dataGridViewX1.Rows[dataGridViewX1.CurrentCell.RowIndex].Cells[0].Value.ToString() + ")", Form1.con);
            cmd.ExecuteNonQuery();
            Form1.con.Close();
            dataGridViewX1.Rows.RemoveAt(dataGridViewX1.CurrentCell.RowIndex);
        }

        private void textBoxX2_TextChanged(object sender, EventArgs e)
        {
            if (textBoxX2.Text.Trim().Length > 1)
            {
                for (int i = 0; i < dataGridViewX2.Rows.Count; i++)
                {
                    string aa = textBoxX2.Text.Trim().ToUpper();
                    string bb = dataGridViewX2.Rows[i].Cells[1].Value.ToString().ToUpper();
                    bb = bb.Trim();
                    if (bb.Contains(aa))
                    {
                        dataGridViewX2.Rows[i].Visible = true;
                    }
                    else
                    {
                        dataGridViewX2.Rows[i].Visible = false;
                    }

                }
            }
            else
            {
                for (int i = 0; i < dataGridViewX2.Rows.Count - 1; i++)
                {
                    dataGridViewX2.Rows[i].Visible = true;
                }
            }
        }

        private void buttonX11_Click(object sender, EventArgs e)
        {
            FastReport.Report r = new FastReport.Report();
            r.Preview = previewControl1;

            r.RegisterData(tablex, "eee");
            r.GetDataSource("eee").Enabled = true;

            r.Load(Application.StartupPath + @"\pregled_dobav.frx");
            r.Prepare();
            r.ShowPrepared();
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf";
            FastReport.Export.Pdf.PDFExport export = new FastReport.Export.Pdf.PDFExport();
            r.Export(export, fileName);
        
            System.Diagnostics.Process.Start(fileName);
        }

        private void buttonX12_Click(object sender, EventArgs e)
        {
            FastReport.Report r = new FastReport.Report();
            r.Preview = previewControl1;

            r.RegisterData(tablex, "eee");
            r.GetDataSource("eee").Enabled = true;

            r.Load(Application.StartupPath + @"\pregled_dobav.frx");
            r.Prepare();
            r.ShowPrepared();
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".xls";
            FastReport.Export.Csv.CSVExport export = new FastReport.Export.Csv.CSVExport ();
            r.Export(export, fileName);
        
            System.Diagnostics.Process.Start(fileName);
        }

        private void buttonX9_Click(object sender, EventArgs e)
        {
            var doc = new users();
            doc.odpri("");
            napolniuporabnike();
        }

        private void buttonX8_Click(object sender, EventArgs e)
        {
            var doc = new users();
            doc.odpri(dataGridViewX2.Rows[dataGridViewX2.CurrentCell.RowIndex].Cells[0].Value.ToString());
            napolniuporabnike();
        }

        private void buttonX10_Click(object sender, EventArgs e)
        {

            Form1.con.Open();
            var cmd = new NpgsqlCommand("select brisi_uporabnika(" + dataGridViewX2.Rows[dataGridViewX2.CurrentCell.RowIndex].Cells[0].Value.ToString() + ")", Form1.con);
            cmd.ExecuteNonQuery();
            Form1.con.Close();
            dataGridViewX2.Rows.RemoveAt(dataGridViewX2.CurrentCell.RowIndex);
        }

        private void labelX5_Click(object sender, EventArgs e)
        {
            login aa = new login();
            aa.ShowDialog();
            labelX5.Text = "Pozdravljen " + Program.uporabnik_ime;

        }
    }
}
