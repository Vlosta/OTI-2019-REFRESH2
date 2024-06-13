using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.IO;

namespace AplicatieFreeBook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\freebook.mdf;Integrated Security=True;Connect Timeout=30");
        string path = "";
        string emailUserLogat = "ion@yahoo.com";
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Email user: " + emailUserLogat; 
            path = System.IO.Directory.GetCurrentDirectory().ToString();
            path += "\\csarp\\";
            con.Open();
            SqlCommand cmdTrunc1 = new SqlCommand("Truncate table utilizatori", con);
            cmdTrunc1.ExecuteNonQuery();
            SqlCommand cmdTrunc2 = new SqlCommand("Truncate table carti", con);
            cmdTrunc2.ExecuteNonQuery();
            StreamReader sr = new StreamReader(path + "\\utilizatori.txt");
            SqlCommand cmdTrunc3 = new SqlCommand("Truncate table imprumut", con);
            cmdTrunc3.ExecuteNonQuery();
            string line="";
            while((line=sr.ReadLine()) != null)
            {
                string email = line.Split('*')[0];
                string parola = line.Split('*')[1];
                string nume = line.Split('*')[2];
                string prenume = line.Split('*')[3];
                SqlCommand cmd1 = new SqlCommand("Insert into utilizatori values(@p1,@p2,@p3,@p4)", con);
                cmd1.Parameters.Add("@p1", email);
                cmd1.Parameters.Add("@p2", parola);
                cmd1.Parameters.Add("@p3", nume);
                cmd1.Parameters.Add("@p4", prenume);
                cmd1.ExecuteNonQuery();

            }
            sr = new StreamReader(path + "\\carti.txt");
            line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string titlu = line.Split('*')[0];
                string autor = line.Split('*')[1]; 
                string gen = line.Split('*')[2];
                SqlCommand cmd1 = new SqlCommand("Insert into carti values(@p1,@p2,@p3)", con);
                cmd1.Parameters.Add("@p1", titlu);
                cmd1.Parameters.Add("@p2", autor);
                cmd1.Parameters.Add("@p3", gen);
                cmd1.ExecuteNonQuery();
            }
            sr = new StreamReader(path + "\\imprumuturi.txt");
            line = "";
            while((line=sr.ReadLine()) != null)
            {
                string titlu = line.Split('*')[0]; 
                string email = line.Split('*')[1];
                DateTime data = DateTime.Parse(line.Split('*')[2]);
                SqlCommand cmd1 = new SqlCommand("Select id_carte from carti where titlu=@p1",con);
                cmd1.Parameters.Add("@p1", titlu);
                SqlDataReader rdr=cmd1.ExecuteReader();
                rdr.Read();
                int id_carte = Convert.ToInt32(rdr[0]);
                rdr.Close();
                SqlCommand cmd2 = new SqlCommand("Insert into imprumut values(@p1,@p2,@p3)", con);
                cmd2.Parameters.Add("@p1", id_carte);
                cmd2.Parameters.Add("@p2", email);
                cmd2.Parameters.Add("@p3", data);
                cmd2.ExecuteNonQuery();

            }
            con.Close();

            SqlDataAdapter sda = new SqlDataAdapter("Select distinct carti.id_carte, carti.titlu,carti.autor,carti.gen from carti inner join imprumut on carti.id_carte=imprumut.id_carte and imprumut.email='"+emailUserLogat+"'", con);
            DataTable dt1=new DataTable();
            sda.Fill(dt1);
            dataGridView1.DataSource = dt1;
            DataGridViewButtonColumn coloana = new DataGridViewButtonColumn();
            coloana.Name = "Imprumuta carte";
            dataGridView1.Columns.Add(coloana);
            dataGridView1.Columns[0].Visible = false;

            DataTable dt2 = new DataTable();
            DataColumn c1 = new DataColumn();
            SqlDataAdapter sda1 = new SqlDataAdapter("Select carti.id_carte, carti.titlu, carti.autor, imprumut.data_imprumut from carti inner join imprumut on carti.id_carte=imprumut.id_carte and imprumut.email='" + emailUserLogat + "'", con);
            sda1.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
                dt2.Rows[i][0] = i + 1;
            dt2.Columns.Add("Data disponibilitate");
            for (int i = 0; i < dt2.Rows.Count; i++)
                dt2.Rows[i][4] =  Convert.ToDateTime(dt2.Rows[i][3]).AddDays(30).ToShortDateString();
             
            dataGridView2.DataSource = dt2;
             


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            label1.Text = e.ColumnIndex.ToString();
            //dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.BackColor = Color.Red;
        }

        private void dgv2(object sender, EventArgs e)
        {
            dataGridView2[1, 1].Style.BackColor = Color.Red;
        }

        private void click(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2[1, 1].Style.BackColor = Color.Red;

        }

        private void dgv2_source(object sender, EventArgs e)
        {
            DataTable dt2 = new DataTable();
            dt2 = (DataTable)dataGridView2.DataSource;
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = Color.Red;
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dataGridView2[1, i].Style = style;
            }
        }
    }
}
