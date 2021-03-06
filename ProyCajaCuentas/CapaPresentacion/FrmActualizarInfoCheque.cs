﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaLogicaNegocios;
using System.Data.SqlTypes;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class FrmActualizarInfoCheque :  Form, IConectarBeneficiarioElegido, IConectarProveedorEnCheque
    {
        //--------------------Constructor
        public FrmActualizarInfoCheque()
        {
            InitializeComponent();
            CrearColumnasParaDataGridViewConceptosEnCheque();

            try
            {
                if (EsActivoModuloController(37))  { /*Se puede modificar cualquier cosa del cheque*/ }
                else { EstablecerReadOnlyEnAlgunosControles(); /* Se puede modificar solo la fecha de cobro*/}
            }

            catch (System.Data.SqlClient.SqlException ex)
            {
                ClsMyException clsMyException = new ClsMyException();
                string res = clsMyException.FormarTextoDeSqlException(ex);

                MessageBox.Show(res, "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
            }
        }

        //----------------Controllers
        public DataTable Cheque_BuscarDetallesChequeController(string numChequeBuscado)
        {
            ClsCheque clsCheque = new ClsCheque();
            clsCheque.NumCheque = numChequeBuscado;
            return (clsCheque.Cheque_BuscarDetallesCheque());
        }

        public string Cheque__DescripcionDeCheque_ConceptoEnCheque_Update(int idCheque, string numCheque, string beneficiario, decimal cantidad,
            DateTime fechaDeCheque, DateTime fechaDeCobro, DataGridView dataGrid,
            bool usarEnCalculosReporteEgresosIngresos, int idUsuarioOperador)
        {
            string mensaje = "";

            ClsChequeInfoBasico clsChequeInfoBasico = new ClsChequeInfoBasico();
            clsChequeInfoBasico.IdCheque = idCheque;
            clsChequeInfoBasico.NumCheque = numCheque;
            clsChequeInfoBasico.Beneficiario = beneficiario;
            clsChequeInfoBasico.Cantidad = cantidad;
            clsChequeInfoBasico.FechaDeCheque = fechaDeCheque;
            clsChequeInfoBasico.FechaDeCobro = fechaDeCobro;
            clsChequeInfoBasico.UsarEnCalculosReporteEgresosIngresos = usarEnCalculosReporteEgresosIngresos;
            clsChequeInfoBasico.IdUsuarioOperador = idUsuarioOperador;

            IEnumerable<DataGridViewRow> filas = dataGrid.Rows.Cast<DataGridViewRow>();
            filas.ToList().ForEach(item =>
            clsChequeInfoBasico.AddConceptoAListaConceptosEnCheque(item.Cells[1].EditedFormattedValue.ToString(),
            item.Cells[2].EditedFormattedValue.ToString(), item.Cells[4].EditedFormattedValue.ToString(),
            Decimal.Parse(item.Cells[3].EditedFormattedValue.ToString())
            ));

            mensaje = clsChequeInfoBasico.Cheque__DescripcionDeCheque_ConceptoEnCheque_Update();

            return (mensaje);
        }

        public DataTable Proveedor_BuscarElementosProveidosActivosController(int idProveedor)
        {
            ClsProveedor clsProveedor = new ClsProveedor();
            clsProveedor.Id = idProveedor;

            return (clsProveedor.Proveedor_BuscarElementosProveidosActivos());
        }

        public bool EsActivoModuloController(int idModuloBuscado)
        {
            var lista = ClsLogin.ModulosALosQueTieneAccesoUsuario.AsEnumerable();
            DataRow filaBuscada = lista.FirstOrDefault(s =>
            (s.Field<int>(1) == idModuloBuscado) && (s.Field<bool>(2) == true));

            if (filaBuscada != null)
            {
                return true;
            }

            else
                return false;
        }

        //--------------------Utils
        public void MostrarInfoEnPantalla(DataTable infoChequeTabla)
        {
            DataRow fila = (infoChequeTabla.AsEnumerable()).First<DataRow>();

            textBox5.Text = fila.Field<int>(0).ToString();
            textBox1.Text = fila.Field<string>("NumCheque");
            textBox2.Text = fila.Field<string>("Beneficiario");
            textBox3.Text = fila.Field<decimal>("Cantidad").ToString();
            dateTimePicker1.Value = fila.Field<DateTime>("FechaDeCheque");

            DateTime fechaDeCobro = fila.Field<DateTime>("FechadeCobro");
            if( (fechaDeCobro.Year == 1753) &&  (fechaDeCobro.Month == 1) && (fechaDeCobro.Day == 1)  )
            {
                radioButton2.Checked = true;
                dateTimePicker2.Value = DateTime.Now;
                dateTimePicker2.Enabled = false;
            }

            else
            {
                radioButton1.Checked = true;
                dateTimePicker2.Value = fechaDeCobro;
                dateTimePicker2.Enabled = true;
            }

            AniadirFilasADataGridViewDesde(infoChequeTabla);
            MostrarSiSePuedeUsarChequeEnCalculosReporteIngresosEgresos(fila.Field<bool>("UsarEnCalculosReporteEgresosIngresos"));
        }

        private void CrearColumnasParaDataGridViewConceptosEnCheque()
        {
            DataGridViewButtonColumn columnaBotonesEliminar = new DataGridViewButtonColumn();
            columnaBotonesEliminar.Name = "columnaBotonesEliminar";
            columnaBotonesEliminar.HeaderText = "";
            columnaBotonesEliminar.Text = "X";
            columnaBotonesEliminar.UseColumnTextForButtonValue = true;



            dataGridView1.Columns.Add(columnaBotonesEliminar);
            dataGridView1.Columns.Add("Proveedor", "Proveedor");
            dataGridView1.Columns.Add("Concepto", "Detalles");
            dataGridView1.Columns.Add("Importe", "Importe");
            dataGridView1.Columns.Add("Factura", "Factura");


            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
        }


        private void AniadirFilasADataGridViewDesde(DataTable infoChequeTabla)
        {
            List<DataRow> filasDataRow = infoChequeTabla.AsEnumerable().ToList<DataRow>();

            Action<DataRow> apuntadorAFuncionAgregarADataGrid = item =>
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[1].Value = item.Field<string>("NomProveedor");
                dataGridView1.Rows[n].Cells[2].Value = item.Field<string>("NomConcepto");
                dataGridView1.Rows[n].Cells[3].Value = item.Field<decimal>("Importe");
                dataGridView1.Rows[n].Cells[4].Value = item.Field<string>("Factura");
            };

            filasDataRow.ForEach(apuntadorAFuncionAgregarADataGrid);
        }

        private void LimpiarGroupBoxAniadirConceptos()
        {
            textBox4.Text = "";
            comboBox1.DataSource = null;
            comboBox1.Items.Clear();
            comboBox1.ResetText();
            textBox6.Text = "";
            textBox7.Text = "";
            radioButton3.Checked = true;
        }

        private void LimpiarGroupBoxDatosDeCheque()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            radioButton2.Checked = true;
            dateTimePicker2.Value = DateTime.Now;
            textBox5.Text = "";
        }

        private void LimpiarGroupBoxDescripcion()
        {
            dataGridView1.Rows.Clear();
        }

        private void LimpiarGroupBoxUsarEnCalculoReporteIngresosEgresos()
        {
            comboBox2.DataSource = null;
            comboBox2.Items.Clear();
            comboBox2.ResetText();
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
            facturaCapturada = facturaCapturada.Trim();
            IEnumerable<DataGridViewRow> filas = dataGridView1.Rows.Cast<DataGridViewRow>();

            var res = from s in filas
                      where s.Cells[4].EditedFormattedValue.ToString().Equals(facturaCapturada)
                      select s;

            if (res.Count<DataGridViewRow>() >= 1)
                return true;
            else
                return false;
        }

        private bool EsPositivo(string numero)
        {
            decimal valorReal = 0.0m;
            valorReal = Decimal.Parse(numero);

            bool res = valorReal >= 0 ? true : false;
            return (res);
        }

        private decimal SumarContenidoEnGrid()
        {
            IEnumerable<DataGridViewRow> filasGrid = dataGridView1.Rows.Cast<DataGridViewRow>();

            var x = (from s in filasGrid
                     select (Decimal.Parse(s.Cells[3].EditedFormattedValue.ToString()))
                    ).ToList();

            return (x.Sum());
        }


        private void CargarElementosProveidosEnComboBox(DataTable elementosProveidos)
        {
            var coleccion = elementosProveidos.AsEnumerable();
            List<DataRow> filas = coleccion.ToList<DataRow>();

            var x = from fila in filas
                    select fila.Field<string>("Nombre de elemento");

            comboBox1.DataSource = null;
            comboBox1.Items.Clear();
            comboBox1.ResetText();

            comboBox1.DataSource = x.ToList<string>();
        }

        private bool EstaSeleccionadoItemDeComboBox(ComboBox miComboBox)
        {
            if (miComboBox.SelectedIndex >= 0)
                return true;
            else
                return false;
        }


        private void EstablecerReadOnlyEnAlgunosControles()
        {
            textBox2.ReadOnly = true;
            button6.Enabled = false;
            dateTimePicker1.Enabled = false;
            textBox4.ReadOnly = true;
            button3.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            comboBox1.Enabled = false;
            textBox6.ReadOnly = true;
            textBox7.ReadOnly = true;
            button1.Enabled = false;
            dataGridView1.Columns[0].Visible = false;
            comboBox2.Enabled = false;
        }


        private void MostrarSiSePuedeUsarChequeEnCalculosReporteIngresosEgresos(bool UsarEnCalculosReporteEgresosIngresos)
        {
            List<string> opciones = new List<string>();
            opciones.Add("Si");
            opciones.Add("No");

            comboBox2.DataSource = opciones;

            if (UsarEnCalculosReporteEgresosIngresos)
                comboBox2.SelectedIndex = 0;
            else
                comboBox2.SelectedIndex = 1;
        }

        void IConectarBeneficiarioElegido.MostrarBeneficiarioElegido(int idBeneficiario, string nombreBeneficiario)
        {
            if(idBeneficiario > 0)
                textBox2.Text = nombreBeneficiario;
        }

        void IConectarProveedorEnCheque.MostrarNombreProveedorElegido(int idProveedorSeleccionado, string nombreProveedorSeleccionado)
        {           
            if(idProveedorSeleccionado > 0)
            {               
                try
                {
                    textBox4.Text = nombreProveedorSeleccionado;
                    DataTable elementosProveidos = Proveedor_BuscarElementosProveidosActivosController(idProveedorSeleccionado);
                    CargarElementosProveidosEnComboBox(elementosProveidos);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
                }
            }
        }
        //------------------Events
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable infoChequeTabla = Cheque_BuscarDetallesChequeController(textBox1.Text.Trim());

                if(infoChequeTabla.Rows.Count > 0)
                {
                    MessageBox.Show("Número de cheque encontrado","Resultado de búsqueda",MessageBoxButtons.OK,MessageBoxIcon.Information);

                    LimpiarGroupBoxDatosDeCheque();
                    LimpiarGroupBoxAniadirConceptos();
                    LimpiarGroupBoxDescripcion();
                    LimpiarGroupBoxUsarEnCalculoReporteIngresosEgresos();



                    MostrarInfoEnPantalla(infoChequeTabla);
                    textBox1.ReadOnly = true;
                    button2.Enabled = true;
                    button5.Enabled = true;
                    button4.Enabled = false;
                }

                else
                {
                    string aux = textBox1.Text;
                    LimpiarGroupBoxDatosDeCheque();
                    LimpiarGroupBoxAniadirConceptos();
                    LimpiarGroupBoxDescripcion();
                    textBox1.Text = aux;
                    MessageBox.Show("No se encontro el número de cheque " + textBox1.Text, "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            catch (System.Data.SqlClient.SqlException ex)
            {
                ClsMyException clsMyException = new ClsMyException();
                string res = clsMyException.FormarTextoDeSqlException(ex);

                MessageBox.Show(res, "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
            {
                dateTimePicker2.Enabled = true;
            }

            else
            {
                dateTimePicker2.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool seCapturoTitulo = TieneAlgoMasQueEspaciosEnBlanco(textBox4.Text);
            bool quiereElegirDetalles = radioButton3.Checked == true ? true : false;
            bool seSeleccionoElementoProveido = EstaSeleccionadoItemDeComboBox(comboBox1);
            bool seCapturoImporte = TieneAlgoMasQueEspaciosEnBlanco(textBox6.Text);
            bool seCapturoFactura = TieneAlgoMasQueEspaciosEnBlanco(textBox7.Text);

            bool filtro = false;
            if (quiereElegirDetalles)
            {
                filtro = seCapturoTitulo && seSeleccionoElementoProveido && seCapturoImporte;
            }

            else
            {
                filtro = seCapturoTitulo && seCapturoImporte;
            }

            if (filtro)
            {
                decimal importeDecimal;
                bool importeEstaEnFormatoValido = Decimal.TryParse(textBox6.Text, out importeDecimal);

                if (importeEstaEnFormatoValido && EsPositivo(textBox6.Text) )
                {
                    //Estas lineas comentadas permiten insertar una factura si no existe en el datagrid, se comento para evitar ello.
                    //if(seCapturoFactura && ExisteFacturaEnGrid(textBox7.Text))
                    //{
                    //    MessageBox.Show("Ya capturo la factura " + textBox7.Text, "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //}
                    //else
                    //{
                    //    int n = dataGridView1.Rows.Add();
                    //    dataGridView1.Rows[n].Cells[1].Value = textBox4.Text;
                    //    dataGridView1.Rows[n].Cells[2].Value = comboBox1.SelectedItem.ToString();
                    //    dataGridView1.Rows[n].Cells[3].Value = textBox6.Text;
                    //    dataGridView1.Rows[n].Cells[4].Value = (seCapturoFactura) ? textBox7.Text.Trim() : "";

                    //    LimpiarGroupBoxAniadirConceptos();
                    //    textBox3.Text = (SumarContenidoEnGrid()).ToString();
                    //}
                    int n = dataGridView1.Rows.Add();
                    dataGridView1.Rows[n].Cells[1].Value = textBox4.Text;
                    if(quiereElegirDetalles)
                    {
                        dataGridView1.Rows[n].Cells[2].Value = comboBox1.SelectedItem.ToString();
                    }

                    else
                    {
                        dataGridView1.Rows[n].Cells[2].Value = String.Empty;
                    }

                    dataGridView1.Rows[n].Cells[3].Value = textBox6.Text;
                    dataGridView1.Rows[n].Cells[4].Value = (seCapturoFactura) ? textBox7.Text.Trim() : "";

                    LimpiarGroupBoxAniadirConceptos();
                    textBox3.Text = (SumarContenidoEnGrid()).ToString();
                }

                else
                {
                    MessageBox.Show("Introduzca un valor adecuado para el importe", "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            else
            {
               MostrarMensajeSiNoSeCapturo("Proveedor", seCapturoTitulo);
               if(quiereElegirDetalles)
               {
                    MostrarMensajeSiNoSeCapturo("Detalles", seSeleccionoElementoProveido);

               }
               MostrarMensajeSiNoSeCapturo("Importe", seCapturoImporte);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                textBox3.Text = (SumarContenidoEnGrid()).ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count >= 1)
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

                            if (cantidadEstaEnFormatoValido && EsPositivo(textBox3.Text) )
                            {
                                DateTime fechaDeCobroParam = (radioButton2.Checked == true) ? SqlDateTime.MinValue.Value : dateTimePicker2.Value;
                                bool usarEnCalculosReporteEgresosIngresos = (comboBox2.SelectedIndex == 0) ? true : false;

                                string mensaje = Cheque__DescripcionDeCheque_ConceptoEnCheque_Update(Int32.Parse(textBox5.Text), textBox1.Text.Trim(), textBox2.Text, cantidadDecimal, dateTimePicker1.Value,
                                fechaDeCobroParam, dataGridView1, usarEnCalculosReporteEgresosIngresos, ClsLogin.Id);


                                if (mensaje.Contains("ok"))
                                {
                                    MessageBox.Show("Registros guardados exitosamente", "Resultado de operación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    LimpiarGroupBoxDatosDeCheque();
                                    LimpiarGroupBoxAniadirConceptos();
                                    LimpiarGroupBoxDescripcion();
                                    LimpiarGroupBoxUsarEnCalculoReporteIngresosEgresos();

                                    textBox1.ReadOnly = false;
                                    button2.Enabled = false;
                                    button5.Enabled = false;
                                    button4.Enabled = true;
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

            catch (System.Data.SqlClient.SqlException ex)
            {
                ClsMyException clsMyException = new ClsMyException();
                string res = clsMyException.FormarTextoDeSqlException(ex);

                MessageBox.Show(res, "Reglas de operación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Esta usted seguro que desea cancelar la edición actual?", "Guardar cambios", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                LimpiarGroupBoxDatosDeCheque();
                LimpiarGroupBoxAniadirConceptos();
                LimpiarGroupBoxDescripcion();
                LimpiarGroupBoxUsarEnCalculoReporteIngresosEgresos();

                textBox1.ReadOnly = false;
                button2.Enabled = false;
                button5.Enabled = false;
                button4.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmBuscarYSeleccionarNombreProveedor frmBuscarYSeleccionarNombreProveedor = new FrmBuscarYSeleccionarNombreProveedor();
            frmBuscarYSeleccionarNombreProveedor.AddSubscriptor(this);
            frmBuscarYSeleccionarNombreProveedor.ShowDialog(this);
            frmBuscarYSeleccionarNombreProveedor.Dispose();
            Activate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FrmBeneficiarioChequeBuscarYSeleccionar frmBeneficiarioChequeBuscarYSeleccionar = new FrmBeneficiarioChequeBuscarYSeleccionar();
            frmBeneficiarioChequeBuscarYSeleccionar.AddSubscriptor(this);
            frmBeneficiarioChequeBuscarYSeleccionar.ShowDialog(this);
            frmBeneficiarioChequeBuscarYSeleccionar.Dispose();
            Activate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == false)
            {   //entonces radioButton 4 esta seleccionado
                comboBox1.Enabled = false;
            }

            if (radioButton3.Checked == true)
            {
                comboBox1.Enabled = true;
            }
        }


    }
}
