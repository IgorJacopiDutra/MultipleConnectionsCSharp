using MultipleConnectionsCSharp.DAL;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace MultipleConnectionsCSharp
{
    public partial class frmPrincipal : Form
    {
        private DataBase dataBase;
        private Query query;

        public frmPrincipal()
        {
            InitializeComponent();            
            dataBase = new DataBase("User=SYSDBA;Password=masterkey;Database=C:\\geiewin\\SubProjetos\\Curso\\Git\\MultipleConnectionsCSharp\\data\\DATA.FDB;DataSource=localhost;Port=3051");
            query = new Query(dataBase);
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                query.Close(); 
                query.SQL.Clear();
                query.SQL.Add("INSERT INTO clientes (id, nome) VALUES (@id, @nome)");
                query.Prepare();

                // Validate input parameters
                string codigo = "1";
                string nome = "MICROSOFT";
                ValidateInputParameters(codigo, nome);

                query.ParamByName("id", codigo);
                query.ParamByName("nome", nome);

                query.ExecuteNonQuery();

                if (String.IsNullOrEmpty(query.Error))
                {
                    MessageBox.Show("Inclusão realizada com sucesso");
                }
                else
                {
                    MessageBox.Show($"Problema na inclusão: {query.Error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inserir dados: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                query.Close();
                query.SQL.Clear();
                query.SQL.Add("UPDATE clientes SET nome = @nome WHERE id = @id");
                query.Prepare();

                // Validate input parameters
                string codigo = "1";
                string nome = "CSHARP";
                ValidateInputParameters(codigo, nome);

                query.ParamByName("id", codigo);
                query.ParamByName("nome", nome);

                query.ExecuteNonQuery();

                if (String.IsNullOrEmpty(query.Error))
                {
                    MessageBox.Show("Alteração realizada com sucesso");
                }
                else
                {
                    MessageBox.Show($"Problema na alteração: {query.Error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar dados: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                query.Close();
                query.SQL.Clear();
                query.SQL.Add("DELETE FROM clientes WHERE id = @id");
                query.Prepare();

                // Validate input parameters
                string codigo = "1";
                ValidateInputParameters(codigo);

                query.ParamByName("id", codigo);

                query.ExecuteNonQuery();

                if (String.IsNullOrEmpty(query.Error))
                {
                    MessageBox.Show("Exclusão realizada com sucesso");
                }
                else
                {
                    MessageBox.Show($"Problema na exclusão: {query.Error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir dados: {ex.Message}");
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                query.Close();
                query.SQL.Clear();
                query.SQL.Add("SELECT * FROM clientes");
                query.Prepare();

                query.ExecuteReader();

                if (String.IsNullOrEmpty(query.Error))
                {
                    while (query.Reader.Read())
                    {
                        MessageBox.Show($"Cliente: {query.Reader["nome"].ToString()}");
                    }

                    query.Reader.Close();
                }
                else
                {
                    MessageBox.Show($"Problema na busca: {query.Error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar dados: {ex.Message}");
            }
        }

        // Validate input parameters
        private void ValidateInputParameters(params string[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    throw new ArgumentException("Parâmetros não podem ser nulos ou vazios.");
                }
            }
        }
    }
}
