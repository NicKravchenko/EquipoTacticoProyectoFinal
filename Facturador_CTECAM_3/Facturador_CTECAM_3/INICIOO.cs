using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Resources;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Reflection;
using System.Globalization;

using System.Diagnostics;

namespace Facturador_CTECAM_3
{
    public partial class INICIOO : Form
    {
        public INICIOO()
        {
            PCname = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            TestForServer(@"localhost\SQLEXPRESS", 1433);
            Get_Script();
            InitializeComponent();
        }

        public static string TABLE_FACTURA = (@"CREATE TABLE [dbo].[FACTURAS] (
    [ID]                             INT           IDENTITY (1, 1) NOT NULL,
    [USUARIO_CREADOR_FACTURA]        VARCHAR (MAX) NOT NULL,
    [FORMATO_FACTURA]                VARCHAR (MAX) NOT NULL,
    [NUMERO_FACTURA]                 VARCHAR (MAX) NOT NULL,
    [TIPO_FACTURA]                   VARCHAR (MAX) NOT NULL,
    [NCF_FACTURA]                    VARCHAR (MAX) NULL,
    [FECHA_FACTURA]                  DATETIME      NOT NULL,
    [COMPANIA_RECEPTOR]              VARCHAR (MAX) NULL,
    [RNC_RECEPTOR]                   BIGINT        NULL,
    [PERSONA_ESPECIFICA_RECEPTOR]    VARCHAR (MAX) NULL,
    [ASUNTO_FACTURA]                 VARCHAR (MAX) NULL,
    [DESCRIPCION_GENERAL_FACTURA]    VARCHAR (MAX) NULL,
    [DESCRIPCION_DESGLOZADA_FACTURA] VARCHAR (MAX) NULL,
    [SUBTOTAL_FACTURA]               FLOAT (53)    NULL,
    [ITBIS_FACTURA]                  FLOAT (53)    NULL,
    [ITBIS_MIN30_FACTURA]            FLOAT (53)    NULL,
    [TOTAL_FACTURA]                  FLOAT (53)    NULL,
    [TOTAL_FINAL]                    FLOAT (53)    NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);");
        public static string TABLE_USER = (@"CREATE TABLE [dbo].[USER_REGISTER] ([ID] INT IDENTITY (1,1) NOT NULL, [Username] VARCHAR(MAX) NOT NULL, [Password] VARCHAR(MAX) NOT NULL, PRIMARY KEY CLUSTERED ([ID] ASC))");
        public static string TABLE_NCF = (@"CREATE TABLE [dbo].[NCF] (
        [Id]                  INT           IDENTITY (1, 1) NOT NULL,
        [NCF_TYPENUMBER]      VARCHAR (10)  NOT NULL,
        [NCF_TYPEDESCRIPTION] VARCHAR (MAX) NOT NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC));");
        public static string INSERT_NCF = (@"SET IDENTITY_INSERT [dbo].[NCF] ON
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (1, N'01', N'Crédito Fiscal')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (2, N'02', N'Consumidor Final')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (3, N'03', N'Nota de Debito')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (4, N'04', N'Nota de Credito')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (5, N'11', N'Comprobante de Compras')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (6, N'12', N'Registro Unico de Ingresos')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (7, N'13', N'Gastos Menores')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (8, N'14', N'Regimen Especial')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (9, N'15', N'Gubernamental')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (10, N'16', N'Exportaciones')
        INSERT INTO [dbo].[NCF] ([Id], [NCF_TYPENUMBER], [NCF_TYPEDESCRIPTION]) VALUES (11, N'17', N'Pagos al Exterior')
        SET IDENTITY_INSERT [dbo].[NCF] OFF");
        public static string conStr = @"packet size=4096;integrated security=SSPI;" + @"Server=localhost\SQLEXPRESS;persist security info=False;" + "initial catalog=FACTURADOR_DB";
        public static string conStr2 = @"Server=localhost\SQLEXPRESS;" +
                                        "Trusted_Connection=yes;" +
                                        "Database=master;" +
                                        "persist security info=False;" +
                                        "Connection timeout=30";//"packet size=4096;integrated security=SSPI;" + "data source=\"(local)\";" + "initial catalog=master";
        public static string PCname;
        public static string username;
        public static string password;
        public static string resourceName;
        public static string newusername;
        public static string newpassword;

        public static List<string> lstDBName = new List<string>();

        public static bool TestForServer(string address, int port)
        {

            int timeout = 500;

            if (ConfigurationManager.AppSettings["RemoteTestTimeout"] != null)
                timeout = int.Parse(ConfigurationManager.AppSettings["RemoteTestTimeout"]);

            var result = false;

            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IAsyncResult asyncResult = socket.BeginConnect(address, port, null, null);
                    result = asyncResult.AsyncWaitHandle.WaitOne(timeout, true);
                    socket.Close();
                }
                return result;
            }
            catch { return false; }
        }

        public static string GetDbCreationQuery()
        {
            string dbName = "FACTURADOR_DB";
            string query = "CREATE DATABASE " + dbName + ";";
            return query;
        }

        public static List<string> GetListOfDBNames1(string connection)
        {
            using (SqlConnection sqlConn = new SqlConnection(connection))
            {
                sqlConn.Open();
                DataTable tblDatabases = sqlConn.GetSchema("Databases");
                sqlConn.Close();

                foreach (DataRow row in tblDatabases.Rows)
                {
                    lstDBName.Add(row["database_name"].ToString());
                }
            }
            return lstDBName;
        }

        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        public static void User_LOGIN()
        {
            System.Data.SqlClient.SqlConnection sqlConnection2 = new System.Data.SqlClient.SqlConnection(INICIOO.conStr);

            SqlCommand cmd2 = new SqlCommand();

            string v = $"SELECT * FROM user_register WHERE [Username]='" + username + "' AND [Password]='" + password + "'";

            cmd2.CommandType = System.Data.CommandType.Text;
            cmd2.CommandText = v;
            cmd2.Connection = sqlConnection2;

            SqlDataAdapter GeoDrillLogin = new SqlDataAdapter(cmd2);
            DataSet result = new DataSet();
            sqlConnection2.Open();
            GeoDrillLogin.Fill(result, "Login");
            sqlConnection2.Close();

            if (result.Tables["Login"].Rows.Count > 0)
            {
                PROGRAMAA settingsForm = new PROGRAMAA();
                settingsForm.Show();
            }

            else
            {
                MessageBox.Show("Credenciales invalidas");
            }
        }

        public static void USER_REGISTER()
        {
            System.Data.SqlClient.SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection(INICIOO.conStr);

            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string v = $"INSERT INTO[dbo].[USER_REGISTER] ([Username], [Password]) VALUES ('" + newusername + "', '" + newpassword + "')";

            cmd.CommandText = v;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();

            MessageBox.Show("Usuario agregado!");
        }

        public static void Get_Script()
        {
            if (!TestForServer(@"localhost\SQLEXPRESS", 1433))
            {

                string appFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string resourcesFolderPath = Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, @"Resources\SQL2019-SSEI-Expr.exe");
                resourceName = "SQL2019_SSEI_Expr.exe";
                System.Diagnostics.Process.Start(resourcesFolderPath);

                while (!TestForServer(@"localhost\SQLEXPRESS", 1433))
                {
                    MessageBox.Show("Instalando...");
                }
                MessageBox.Show("Install COMPLETED!");
            }
            string connectionString = conStr;
            string connectionString2 = conStr2;

            var conn2 = new SqlConnection(connectionString2);
            var conn = new SqlConnection(connectionString);

            var query = GetDbCreationQuery();

            var command = new SqlCommand(query, conn2);
            var command2 = new SqlCommand(TABLE_FACTURA, conn);
            var command3 = new SqlCommand(TABLE_USER, conn);
            var command4 = new SqlCommand(TABLE_NCF, conn);
            var command5 = new SqlCommand(INSERT_NCF, conn);

            GetListOfDBNames1(connectionString2);

            try
            {
                if (!lstDBName.Contains("FACTURADOR_DB"))
                {
                    conn2.Open();
                    command.ExecuteNonQuery();
                    conn2.Close();

                    conn.Open();

                    command2.ExecuteNonQuery();
                    command3.ExecuteNonQuery();
                    command4.ExecuteNonQuery();
                    command5.ExecuteNonQuery();

                    MessageBox.Show("SE HA CREADO LA BASE DE DATOS", "MyProgram",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            finally
            {
                if ((conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
        }


        private void INCIARbutton_Click(object sender, EventArgs e)
        {
            username = UsuariotextBox.Text;
            password = PassWordtextBox.Text;
            User_LOGIN();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!CreateAccountContainer.Visible)
            {
                CreateAccountContainer.Visible = true;
                UserLoginContainer.Visible = false;
                INCIARbutton.Visible = false;
                button2.Text = "REGRESAR";
            }
            else
            {
                CreateAccountContainer.Visible = false;
                UserLoginContainer.Visible = true;
                INCIARbutton.Visible = true;
                button2.Text = "REGISTRAR USUARIO";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newusername = NewUsernameTextBox.Text;
            newpassword = NewPassWordTextBox.Text;
            if (NewPassWordTextBox.Text == ConfirmNewPWtextBox.Text)
                USER_REGISTER();
            else
                MessageBox.Show("La contraseña no coincide.");
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}