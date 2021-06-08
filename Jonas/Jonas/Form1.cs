using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

    

namespace Jonas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            VerificaBanco();
            GetTodos();
        
         }
        DialogResult result;
        bool opcaoInserir = true;
        private static SQLiteConnection DbConnection()
        {
            var userName = Environment.UserName;
            var sqliteConnection = new SQLiteConnection("Data Source=C:\\Users\\"+ userName + "\\Downloads\\SistemaGerenciamentoVOOS\\Jonas\\Jonas\\bin\\Debug\\acme.sqlite; Version=3;");
            sqliteConnection.Open();
            return sqliteConnection;
        }
         public void VerificaBanco()
         {
            var userName = Environment.UserName;
            var banco = "Data Source=C:\\Users\\"+ userName + "\\Downloads\\SistemaGerenciamentoVOOS\\Jonas\\Jonas\\bin\\Debug\\acme.sqlite; Version=3";
            var sqliteConnection = new SQLiteConnection(banco);
            if(!File.Exists (banco))
            {
                CriarBancoSQLite();
                CriarTabelaSQlite();
            }
        }
        public void CriarBancoSQLite()
        {
            try
            {
                var userName = Environment.UserName;
                SQLiteConnection.CreateFile(@"C:\\Users\\" + userName + "\\Downloads\\SistemaGerenciamentoVOOS\\Jonas\\Jonas\\bin\\Debug\\acme.sqlite");
            }
            catch
            {
                throw;
            }
        }
        public static void CriarTabelaSQlite()
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS TB_VOO (
                                           ID_VOO INTEGER,
                                           DATA_VOO Datetime,
                                           CUSTO numeric(10, 2),
                                           DISTANCIA int,
                                           CAPTURA Char(1),
                                           NIVEL_DOR int,
                                           PRIMARY KEY(ID_VOO AUTOINCREMENT))";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable GetTodos()
        {
            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "SELECT DATA_VOO, CAPTURA, NIVEL_DOR FROM TB_VOO";
                    da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    cmd.Connection.Close();
                    cmd.Dispose();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetIdByVOO(DateTime DATA_VOO, string CAPTURA, int NIVEL_DOR)
        {
            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();
            try
            {

                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM TB_VOO WHERE DATA_VOO = '{DATA_VOO.ToString("yyyy-MM-dd HH:mm:ss")}' AND CAPTURA = '{CAPTURA}' AND NIVEL_DOR = {NIVEL_DOR}";
                    da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    var idVoo = Convert.ToInt32(dt.Rows[0]["ID_VOO"]);
                    cmd.Connection.Close();
                    cmd.Dispose();
                    return idVoo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InserirVoo()
        {
            try
            {

                var data = DateTime.Parse(dateTimePicker1.Text);
                var captura = chkSim.Checked == true ? "S" : chkNao.Checked == true ? "N" : "N";
                var nivelDor = Int32.Parse(txtDor.Text);
                var custo = Int32.Parse(txtCusto.Text);
                var distancia = Int32.Parse(txtDistancia.Text);
                if (string.IsNullOrEmpty(data.ToString()) || string.IsNullOrEmpty(captura.ToString())
                    || string.IsNullOrEmpty(nivelDor.ToString()) || string.IsNullOrEmpty(custo.ToString())
                    || string.IsNullOrEmpty(distancia.ToString()))
                {
                    MessageBox.Show("Preencha todos os campos corretamente");
                }
                else
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO TB_VOO
                                            (DATA_VOO,
                                             CUSTO,
                                             DISTANCIA,
                                             CAPTURA,
                                             NIVEL_DOR)
                                             values
                                             (
                                             @DATA_VOO,
                                             @CUSTO,
                                             @DISTANCIA,
                                             @CAPTURA,
                                             @NIVEL_DOR
                                             )";
                        cmd.Parameters.AddWithValue("@DATA_VOO", data);
                        cmd.Parameters.AddWithValue("@CUSTO", custo);
                        cmd.Parameters.AddWithValue("@DISTANCIA", distancia);
                        cmd.Parameters.AddWithValue("@CAPTURA", captura);
                        cmd.Parameters.AddWithValue("@NIVEL_DOR", nivelDor);
                        cmd.ExecuteNonQuery();
                        GetTodos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AtualizarVoo(DateTime DATA_VOO, string CAPTURA, int NIVEL_DOR)
        {
            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {

                    var data = DateTime.Parse(dateTimePicker1.Text);
                    var captura = chkSim.Checked == true ? "S" : chkNao.Checked == true ? "N" : "N";
                    var nivelDor = Int32.Parse(txtDor.Text);
                    var idVoo = GetIdByVOO(DATA_VOO, CAPTURA, NIVEL_DOR);
                    var custo = Int32.Parse(txtCusto.Text);
                    var distancia = Int32.Parse(txtDistancia.Text);

                    if (idVoo <= 0)
                    {
                        MessageBox.Show("Dados não encontrados no Sistema");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(data.ToString()) || string.IsNullOrEmpty(captura.ToString())
                        || string.IsNullOrEmpty(nivelDor.ToString()) || string.IsNullOrEmpty(custo.ToString())
                        || string.IsNullOrEmpty(distancia.ToString()))
                        {
                            MessageBox.Show("Preencha todos os campos corretamente");
                        }
                        else
                        {
                            cmd.CommandText = $@"UPDATE TB_VOO SET
                                             DATA_VOO = '{data.ToString("yyyy-MM-dd HH:mm:ss")}',
                                             CUSTO = {custo},
                                             DISTANCIA = {distancia},
                                             CAPTURA = '{captura}',
                                             NIVEL_DOR = {nivelDor}
                                             where ID_VOO = {idVoo}
                                                ";
                            cmd.ExecuteNonQuery();
                            GetTodos();
                        }
                    }

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletarVoo()
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    var data = DateTime.Parse(dataGridView1.SelectedCells[0].Value.ToString());
                    var captura = dataGridView1.SelectedCells[1].Value.ToString();
                    var nivel = Int32.Parse(dataGridView1.SelectedCells[2].Value.ToString());

                    if (string.IsNullOrWhiteSpace(data.ToString()) || string.IsNullOrWhiteSpace(captura.ToString()) || string.IsNullOrWhiteSpace(nivel.ToString()))
                    {
                        MessageBox.Show("Selecione uma linha corretamente");
                    }
                    else
                    {
                        var idVoo = GetIdByVOO(data, captura, nivel);
                        cmd.CommandText = $"DELETE FROM TB_VOO Where ID_VOO= {idVoo}";
                        cmd.ExecuteNonQuery();
                        GetTodos();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // BOTÃO DE INCLUIR 
        private void button1_Click(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = " ";
            txtCusto.Clear();
            txtDistancia.Clear();
            txtDor.Clear();
            chkNao.Checked = false;
            chkSim.Checked = false;

        }
        // BOTÃO DE EXCLUIR
        private void button2_Click(object sender, EventArgs e)
        {
            DeletarVoo();
        }
        // TEXBOX PARA A ENTRADA DA DATA
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        // TEXBOX PARA A ENTRADA DO CUSTO
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        // TEXBOX PARA A ENTRADA DA DISTANCIA
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        // CHECKBOX SIM 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        // CHECKBOX NAO
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
        // TEXTBOX PARA A ENTRADA DO NIVEL DE DOR 
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        // BOTAO DE SALVAR
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 3)
            {
                var data = DateTime.Parse(dataGridView1.SelectedCells[0].Value.ToString());
                var captura = dataGridView1.SelectedCells[1].Value.ToString();
                var nivel = Int32.Parse(dataGridView1.SelectedCells[2].Value.ToString());
                AtualizarVoo(data, captura, nivel);
            }
            else
            {
                string message = "Deseja inserir os dados do Voo:";
                string title = "Atention";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.OK)
                {
                    InserirVoo();
                }
                else if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("Opção cancelado");
                }

            }

        }
        // BOTAO DE CANCELAR
        private void button4_Click(object sender, EventArgs e)
        {

        }
        // TEXTO DATA
        private void label1_Click(object sender, EventArgs e)
        {

        }
        // TEXTO CUSTO
        private void label2_Click(object sender, EventArgs e)
        {

        }
        // TEXTO CUSTO
        private void label3_Click(object sender, EventArgs e)
        {

        }
        // TEXTO CAPTURA
        private void label4_Click(object sender, EventArgs e)
        {

        }
        // TEXTO NIVEL DOR 
        private void label5_Click(object sender, EventArgs e)
        {

        }
        // DATAGRIDVIEW ONDE SERÃO EXIBIDOS OS DADOS 
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
                btnSalvar.Enabled = true;
        }

        private void ClickData(object sender, EventArgs e)
        {
            btnSalvar.Enabled = true;
        }

        private void ClickCusto(object sender, EventArgs e)
        {
            btnSalvar.Enabled = true;
        }

        private void ClickDist(object sender, EventArgs e)
        {
            btnSalvar.Enabled = true;
        }

        private void ClickChk(object sender, EventArgs e)
        {
            btnSalvar.Enabled = true;
        }

        private void chkNai(object sender, EventArgs e)
        {
            btnSalvar.Enabled = true;
        }

        private void nivelDorClick(object sender, EventArgs e)
        {
            btnSalvar.Enabled = true;
        }
    }
}
