using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using System.Data.SqlClient;

namespace Facturador_CTECAM_3
{
    public partial class PROGRAMAA : Form
    {
        public static SaveFileDialog SaveWindow;
        public static string[] numfacturaincremento = new string[2];
        public static string[] ncfincremento = new string[2];
        public static int TypeFacturaString;
        public static string Formatfactura;
        public static string Typefactura;
        public static string[] TIPOfacTura;
        public static string[] columnasGridView = { "ID", "CREADOR", "FORMATO", "NUMERO", "TIPO", "NCF", "FECHA", "COMPAÑIA", "RNC COMPAÑIA", "PERSONA", "ASUNTO", "GENERAL", "DESGLOZADA", "SUBTOTAL", "ITBIS", "ITBIS -30%", "TOTAL", "TOTAL FINAL" };
        public static FACTURA factura;
        public static FACTURACTECAM_Entities db = new FACTURACTECAM_Entities();

        public PROGRAMAA()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today;
            getComboBoxTipo();
        }

        public void GetNUMandNCF()
        {
            if (CTECAMcheckBox.Checked)
                Formatfactura = CTECAMcheckBox.Text;

            else
                Formatfactura = MarcoPerezcheckBox.Text;

            TypeFacturaString = (TipoFacturacomboBox1.SelectedIndex);
            TIPOfacTura = db.NCFs.Where(a => a.Id == TypeFacturaString + 1).Select(a => a.NCF_TYPEDESCRIPTION).ToArray();
            Typefactura = Convert.ToString(TipoFacturacomboBox1.SelectedValue);

            var numberfactura = db.FACTURAS.Where(a => a.FORMATO_FACTURA == Formatfactura & a.TIPO_FACTURA == Typefactura).Select(a => a.NUMERO_FACTURA).ToList();
            var numberfactura2 = db.FACTURAS.Where(a => a.FORMATO_FACTURA == Formatfactura).Select(a => a.NUMERO_FACTURA).ToList();
            
            if (numberfactura2.Count > 0)
            {
                var numeroFacturaRetrieve = numberfactura2.Last();
                numfacturaincremento = numeroFacturaRetrieve.Split('-');
                int increaseone = Convert.ToInt32(numfacturaincremento[0]) + 1;
                numfacturaincremento[0] = Convert.ToString(increaseone);

                if (increaseone < 10)
                    FacturaNumberlabel.Text = "0" + numfacturaincremento[0] + "-" + numfacturaincremento[1];

                else
                    FacturaNumberlabel.Text = numfacturaincremento[0] + "-" + (DateTime.Today.Year.ToString()).Substring(2);
            }

            else
                FacturaNumberlabel.Text = "01-" + (DateTime.Today.Year.ToString()).Substring(2);
            if (numberfactura.Count > 0)
            {
                var ncfNCF = db.FACTURAS.Where(a => a.FORMATO_FACTURA == Formatfactura & a.TIPO_FACTURA == Typefactura).Select(a => a.NCF_FACTURA).ToList();
                var NCFRetrieve = ncfNCF.Last();
                string ncf = Convert.ToString(NCFRetrieve).Insert(3, "-");
                ncfincremento = ncf.Split('-');
                int ncfincrease = Convert.ToInt32(ncfincremento[1]);
                int ncfincreaseoneplus = ncfincrease + 1;
                string ncf2 = Convert.ToString(ncfincreaseoneplus);
                string finalncf3 = ncfincremento[0] + (new string('0', ncfincremento[1].Length - ncf2.Length) + ncf2);
                NCFlabel.Text = finalncf3;
            }

            else
            {
               
                NCFlabel.Text = "B" + TipoFacturacomboBox1.SelectedValue + "00000001";
                MessageBox.Show("No hay registros");
            }

            TableFacturasPerFormat();
        }
       
        public void getComboBoxTipo()
        {
            var fillcombobox = db.NCFs.ToList();
            TipoFacturacomboBox1.ValueMember = "NCF_TYPENUMBER";
            TipoFacturacomboBox1.DisplayMember = "NCF_TYPEDESCRIPTION";
            TipoFacturacomboBox1.DataSource = fillcombobox;
        }
        
        public void TableFacturasPerFormat()
        {
            var FacturasToList = db.FACTURAS.Where(a => a.FORMATO_FACTURA == Formatfactura && a.TIPO_FACTURA == Typefactura).ToList();

            dataGridView1.DataSource = FacturasToList;

            for (int i = 0; i < columnasGridView.Length; i++)
            {
                dataGridView1.Columns[i].HeaderText = columnasGridView[i];
            }

            dataGridView1.AutoResizeColumns();
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
        }

        public static void OpenDocument()
        {
            string appFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string resourcesFolderPath = Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, @"Resources\FACTURA CTECAM EJEMPLO.docx");
            
            Document WordFactura = new Microsoft.Office.Interop.Word.Application().Documents.Open(resourcesFolderPath);

            WordFactura.Bookmarks[8].Range.Text = factura.NUMERO_FACTURA; //numero   
            WordFactura.Bookmarks[7].Range.Text = factura.NCF_FACTURA; //ncf
            WordFactura.Bookmarks[4].Range.Text = (factura.FECHA_FACTURA.ToShortDateString()); //fecha
            WordFactura.Bookmarks[2].Range.Text = factura.COMPANIA_RECEPTOR;//para
            WordFactura.Bookmarks[11].Range.Text = Convert.ToString(factura.RNC_RECEPTOR);//RNC para
            WordFactura.Bookmarks[9].Range.Text = factura.PERSONA_ESPECIFICA_RECEPTOR;//persona directa
            WordFactura.Bookmarks[1].Range.Text = factura.ASUNTO_FACTURA;//Asunto
            WordFactura.Bookmarks[14].Range.Text = factura.DESCRIPCION_GENERAL_FACTURA;//Titulo Descripcion
            WordFactura.Bookmarks[3].Range.Text = factura.DESCRIPCION_DESGLOZADA_FACTURA;//Desglose
            WordFactura.Bookmarks[12].Range.Text = ($"{(factura.SUBTOTAL_FACTURA):C2}");//Subtotal
            WordFactura.Bookmarks[13].Range.Text = ($"{(factura.SUBTOTAL_FACTURA):C2}");//Subtotal
            WordFactura.Bookmarks[5].Range.Text = Convert.ToString(factura.ITBIS_FACTURA);//ITBIS
            WordFactura.Bookmarks[15].Range.Text = Convert.ToString(factura.TOTAL_FACTURA);//TOTAL
            WordFactura.Bookmarks[6].Range.Text = Convert.ToString(factura.ITBIS_MIN30_FACTURA);//-30%ITBIS
            WordFactura.Bookmarks[16].Range.Text = Convert.ToString(factura.TOTAL_FINAL);//TOTAL FINAL


            SaveWindow = new SaveFileDialog();
            SaveWindow.InitialDirectory = @"C:\Users\\USER\\Documents\" + SaveWindow.FileName;
            SaveWindow.FileName = ($"Factura CTECAM {factura.NUMERO_FACTURA}-{factura.COMPANIA_RECEPTOR}");
            SaveWindow.ShowDialog();
            SaveWindow.DefaultExt = ".docx";

            WordFactura.SaveAs2(SaveWindow.FileName);
            WordFactura.Close();

            SaveWindow.FileOk += SaveWindow_FileOk;

            MessageBox.Show($"EXITO! Factura {factura.NUMERO_FACTURA} de {TIPOfacTura[0]} guardada.");
        }

        static void SaveWindow_FileOk(object sender, CancelEventArgs e)
        {
            OpenDocument();
            SaveWindow.AddExtension = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool subtotaltrue = double.TryParse(textBox1.Text, out double subtotal);

            ITBISlabel.Text = Convert.ToString(subtotal * .18);
            TOTALlabel.Text = Convert.ToString(subtotal + (Convert.ToDouble(ITBISlabel.Text)));
            ITBISmin30label.Text = Convert.ToString((Convert.ToDouble(ITBISlabel.Text) * .70));
            TotalFinallabel.Text = Convert.ToString(subtotal + (Convert.ToDouble(ITBISmin30label.Text)));
        }
        public void GetValuesFactura()
        {
            factura = new FACTURA
            {
                USUARIO_CREADOR_FACTURA = INICIOO.username,
                FORMATO_FACTURA = Formatfactura,
                NUMERO_FACTURA = FacturaNumberlabel.Text,
                TIPO_FACTURA = TipoFacturacomboBox1.SelectedValue.ToString(),
                NCF_FACTURA = NCFlabel.Text,
                FECHA_FACTURA = dateTimePicker1.Value,
                COMPANIA_RECEPTOR = CompaniatextBox.Text,
                RNC_RECEPTOR = Convert.ToInt64(RNCtextBox.Text),
                PERSONA_ESPECIFICA_RECEPTOR = PersonFromCompanytextBox.Text,
                ASUNTO_FACTURA = SubjecttextBox.Text,
                DESCRIPCION_GENERAL_FACTURA = GeneralDescriptiontextBox.Text,
                DESCRIPCION_DESGLOZADA_FACTURA = DetailsDescriptionTextBox1.Text,
                SUBTOTAL_FACTURA = Convert.ToDouble(textBox1.Text),
                ITBIS_FACTURA = Convert.ToDouble(ITBISlabel.Text),
                ITBIS_MIN30_FACTURA = Convert.ToDouble(ITBISmin30label.Text),
                TOTAL_FACTURA = Convert.ToDouble(TOTALlabel.Text),
                TOTAL_FINAL = Convert.ToDouble(TotalFinallabel.Text)
            };

            db.FACTURAS.Add(factura);
            db.SaveChanges();
        }

        private void SelectTypebutton_Click(object sender, EventArgs e)
        {
            if (CTECAMcheckBox.Checked)
                Formatfactura = CTECAMcheckBox.Text;

            else
                Formatfactura = MarcoPerezcheckBox.Text;

            Typefactura = Convert.ToString(TipoFacturacomboBox1.SelectedItem);

            TableFacturasPerFormat();
            GetNUMandNCF();
            FacturatoolStripContainer2.Enabled = true;
            dataGridView1.Enabled = true;

        }

        private void CTECAMcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CTECAMcheckBox.Checked)
                MarcoPerezcheckBox.Checked = false;
        }

        private void MarcoPerezcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MarcoPerezcheckBox.Checked)
                CTECAMcheckBox.Checked = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GetValuesFactura();

            OpenDocument();

            TableFacturasPerFormat();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            bool subtotaltrue = double.TryParse(textBox1.Text, out double subtotal);

            ITBISlabel.Text = Convert.ToString(subtotal * .18);
            TOTALlabel.Text = Convert.ToString(subtotal + (Convert.ToDouble(ITBISlabel.Text)));
            ITBISmin30label.Text = Convert.ToString((Convert.ToDouble(ITBISlabel.Text) * .70));
            TotalFinallabel.Text = Convert.ToString(subtotal + (Convert.ToDouble(ITBISmin30label.Text)));
        }
    }
}

