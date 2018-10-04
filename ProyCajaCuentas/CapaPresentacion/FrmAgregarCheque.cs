﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Windows.Forms;
using CapaLogicaNegocios;

namespace CapaPresentacion
{
    public partial class FrmAgregarCheque : Form
    {

        //----------------constructor
        public FrmAgregarCheque()
        {
            InitializeComponent();
            CrearColumnasParaDataGridViewConceptosEnCheque();
        }


        //------------------Methods
        private string ChequeInfoBasico_createController(string numCheque, string beneficiario, decimal cantidad,
            DateTime fechaDeCheque, DateTime fechaDeCobro, DataGridView dataGrid, int idUsuarioOperador)
        {
            string mensaje = "";

            ClsChequeInfoBasico clsChequeInfoBasico = new ClsChequeInfoBasico();
            clsChequeInfoBasico.NumCheque = numCheque;
            clsChequeInfoBasico.Beneficiario = beneficiario;
            clsChequeInfoBasico.Cantidad = cantidad;
            clsChequeInfoBasico.FechaDeCheque = fechaDeCheque;
            clsChequeInfoBasico.FechaDeCobro = fechaDeCobro;
            clsChequeInfoBasico.IdUsuarioOperador = idUsuarioOperador;

            IEnumerable<DataGridViewRow> filas = dataGrid.Rows.Cast<DataGridViewRow>();
            filas.ToList().ForEach(item =>
            clsChequeInfoBasico.AddConceptoAListaConceptosEnCheque(item.Cells[1].EditedFormattedValue.ToString(),
            item.Cells[2].EditedFormattedValue.ToString(), item.Cells[4].EditedFormattedValue.ToString(),
            Decimal.Parse(item.Cells[3].EditedFormattedValue.ToString())
            ));

            mensaje = clsChequeInfoBasico.Cheque_DescripcionDeCheque_ConceptoEnCheque_create();

            return (mensaje);
            //https://stackoverflow.com/questions/15098071/how-can-i-use-linq-to-find-a-datagridview-row
            //https://stackoverflow.com/questions/1883920/call-a-function-for-each-value-in-a-generic-c-sharp-collection
        }



        //------------------Utils
        private void CrearColumnasParaDataGridViewConceptosEnCheque()
        {
            DataGridViewButtonColumn columnaBotonesEliminar = new DataGridViewButtonColumn();
            columnaBotonesEliminar.Name = "columnaBotonesEliminar";
            columnaBotonesEliminar.HeaderText = "";
            columnaBotonesEliminar.Text = "X";
            columnaBotonesEliminar.UseColumnTextForButtonValue = true;

            

            dataGridView1.Columns.Add(columnaBotonesEliminar);
            dataGridView1.Columns.Add("Proveedor", "Título");
            dataGridView1.Columns.Add("Concepto", "Detalles");
            dataGridView1.Columns.Add("Importe", "Importe");
            dataGridView1.Columns.Add("Factura", "Factura");


            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
        }

        private void LimpiarGroupBoxAniadirConceptos()
        {
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        private void LimpiarGroupBoxDatosDeCheque()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            radioButton2.Checked = true;
            dateTimePicker2.Value = DateTime.Now;
        }

        private void LimpiarGroupBoxDescripcion()
        {
            dataGridView1.Rows.Clear();
        }

        private bool TieneAlgoMasQueEspaciosEnBlanco(string texto)
        {
            string cad = texto.Trim();

            bool res = cad.Length > 0 ? true : false;
            return (res);
        }

        private void MostrarMensajeSiNoSeCapturo(string nombreDelCampo, bool tieneCapturaElCampo)
        {
            if (tieneCapturaElCampo == false)
                MessageBox.Show(nombreDelCampo + " no capturado, se requiere", "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private bool ExisteFacturaEnGrid(string facturaCapturada)
        {
            IEnumerable<DataGridViewRow> filas = dataGridView1.Rows.Cast<DataGridViewRow>();

            var res = from s in filas
                      where s.Cells[4].EditedFormattedValue.ToString().Equals(facturaCapturada)
                      select s;

            if (res.Count<DataGridViewRow>() >= 1)
                return true;
            else
                return false;
        }

        //------------------Events


        private void button1_Click(object sender, EventArgs e)
        {
            bool seCapturoTitulo = TieneAlgoMasQueEspaciosEnBlanco(textBox4.Text);
            bool seCapturoDetalles = TieneAlgoMasQueEspaciosEnBlanco(textBox5.Text);
            bool seCapturoImporte = TieneAlgoMasQueEspaciosEnBlanco(textBox6.Text);
            bool seCapturoFactura = TieneAlgoMasQueEspaciosEnBlanco(textBox7.Text);

            if(seCapturoTitulo && seCapturoDetalles && seCapturoImporte && seCapturoFactura)
            {
                decimal importeDecimal;
                bool importeEstaEnFormatoValido = Decimal.TryParse(textBox6.Text, out importeDecimal);

                if(importeEstaEnFormatoValido)
                {
                    if (ExisteFacturaEnGrid(textBox7.Text))
                    {
                        MessageBox.Show("Ya capturo la factura " + textBox7.Text, "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    else
                    {
                        int n = dataGridView1.Rows.Add();
                        dataGridView1.Rows[n].Cells[1].Value = textBox4.Text;
                        dataGridView1.Rows[n].Cells[2].Value = textBox5.Text;
                        dataGridView1.Rows[n].Cells[3].Value = textBox6.Text;
                        dataGridView1.Rows[n].Cells[4].Value = textBox7.Text;

                        LimpiarGroupBoxAniadirConceptos();
                    }
                }

                else
                {
                    MessageBox.Show("Introduzca un valor adecuado para el importe", "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            else
            {
                MostrarMensajeSiNoSeCapturo("Titulo", seCapturoTitulo);
                MostrarMensajeSiNoSeCapturo("Detalles", seCapturoDetalles);
                MostrarMensajeSiNoSeCapturo("Importe", seCapturoImporte);
                MostrarMensajeSiNoSeCapturo("Factura", seCapturoFactura);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if(dataGridView1.Rows.Count >= 1)
                {
                    DialogResult res = MessageBox.Show("¿Esta usted seguro que desea continuar?", "Guardar cambios", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        bool seCapturoNumDeCheque = TieneAlgoMasQueEspaciosEnBlanco(textBox1.Text);
                        bool seCapturoBeneficiario = TieneAlgoMasQueEspaciosEnBlanco(textBox2.Text);
                        bool seCapturoCantidad = TieneAlgoMasQueEspaciosEnBlanco(textBox3.Text);


                        if (seCapturoNumDeCheque && seCapturoBeneficiario && seCapturoCantidad)
                        {
                            decimal cantidadDecimal;
                            bool cantidadEstaEnFormatoValido = Decimal.TryParse(textBox3.Text, out cantidadDecimal);

                            if(cantidadEstaEnFormatoValido)
                            {
                                DateTime fechaDeCobroParam = (radioButton2.Checked == true) ? SqlDateTime.MinValue.Value : dateTimePicker2.Value;

                                string mensaje = ChequeInfoBasico_createController(textBox1.Text, textBox2.Text, cantidadDecimal, dateTimePicker1.Value,
                                fechaDeCobroParam, dataGridView1, ClsLogin.Id);
                                
                                
                                if(mensaje.Contains("ok") )
                                {
                                    MessageBox.Show("Registros guardados exitosamente", "Resultado de operación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    LimpiarGroupBoxDatosDeCheque();
                                    LimpiarGroupBoxAniadirConceptos();
                                    LimpiarGroupBoxDescripcion();
                                }

                                else
                                {
                                    MessageBox.Show(mensaje, "Resultado de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }

                            else
                            {
                                MessageBox.Show("Introduzca un valor adecuado para la cantidad del cheque", "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }

                        else
                        {
                            MostrarMensajeSiNoSeCapturo("Número de cheque", seCapturoNumDeCheque);
                            MostrarMensajeSiNoSeCapturo("Beneficiario", seCapturoBeneficiario);
                            MostrarMensajeSiNoSeCapturo("Cantidad de cheque", seCapturoCantidad);
                        }

                    }
                }

                else
                {
                    MessageBox.Show("Necesitas capturar algún concepto", "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked == false)
            {
                dateTimePicker2.Enabled = true;
            }

            else
            {
                dateTimePicker2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmBuscarYSeleccionarNombreProveedor frmBuscarYSeleccionarNombreProveedor = new FrmBuscarYSeleccionarNombreProveedor();
            frmBuscarYSeleccionarNombreProveedor.ShowDialog(this);

            textBox4.Text = frmBuscarYSeleccionarNombreProveedor.NombreProveeedorSeleccionado;

            frmBuscarYSeleccionarNombreProveedor.Dispose();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }
    }
}