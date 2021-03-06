﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework;
using CapaLogicaNegocios;



namespace CapaPresentacion
{
    public partial class FrmPrincipal3 : MetroForm
    {
        private Task<CRReporteVacio> tarea;

        //--------------------Constructor
        public FrmPrincipal3()
        {
            InitializeComponent();

            CargarMetroComBoBox();
            metroComboBox1.SelectedIndex = 0;
            DeshabilitarDeMenuOpciones();
            MostrarUsuarioDeSesionEnMetroLabel();


            Func<CRReporteVacio> funcion = () => {
                FrmVisorReporteTonto tonto = new FrmVisorReporteTonto();
                tonto.Dispose();
                return new CRReporteVacio();
            };

            tarea = Task.Run(funcion);
        }

        //-------------Methods controller
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


        //--------------Utils
        private void CargarMetroComBoBox()
        {
            metroComboBox1.Items.Add("Mas opciones");
            metroComboBox1.Items.Add("Crear producto");
            metroComboBox1.Items.Add("Crear tarifas de producto");
            metroComboBox1.Items.Add("Editar tarifas de producto");
            metroComboBox1.Items.Add("Agregar usuario");
            metroComboBox1.Items.Add("Modificar usuario");
            metroComboBox1.Items.Add("Privilegios de usuario");
            metroComboBox1.Items.Add("Editar datos de conexión");
        }

        private void AbrirFormulario(object formHijo)
        {
            if (this.metroPanelContenedor.Controls.Count > 0)
            {
                //this.metroPanelContenedor.Controls.RemoveAt(0);
                this.metroPanelContenedor.Controls.Clear();
            }

            Form fh = formHijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;

            this.metroPanelContenedor.Controls.Add(fh);
            this.metroPanelContenedor.Tag = fh;
            fh.Show();
        }

        private void DeshabilitarDeMenuOpciones()
        {
            bool respuesta = false;

            //Deshabilitar / Habilitar la opcion Agregar usuario, modificar usuario, privilegios de usuario del combobox
            respuesta = EsActivoModuloController(1) ? true : false;
            if (respuesta)
            { /* permanecen habilitadas los item del combobox  agregar usuario, modificar usuario, 
                privilegios de usuario*/ }
            else
            {
                metroComboBox1.Items.Remove("Agregar usuario");
                metroComboBox1.Items.Remove("Modificar usuario");
                metroComboBox1.Items.Remove("Privilegios de usuario");
            }



            //Habilitar / deshabilitar la opcion Productos y tarifas del menu
            respuesta = EsActivoModuloController(5) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Crear Producto del menu*/}
            else { metroComboBox1.Items.Remove("Crear producto"); }

            //Habilitar / deshabilitar la opcion crear tarifas de Producto, del menu
            respuesta = EsActivoModuloController(6) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion crear tarifas de Producto, del menu*/}
            else { metroComboBox1.Items.Remove("Crear tarifas de producto"); }

            //Habilitar / deshabilitar la opcion  Modificar tarifas de producto
            respuesta = EsActivoModuloController(7) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Modificar tarifas a Producto, del menu*/}
            else { metroComboBox1.Items.Remove("Editar tarifas de producto"); }


            //Habilitar / deshabilitar la opcion agregar socio del menu
            respuesta = EsActivoModuloController(2) ? true : false;
            if (respuesta)
            {   /*Permanece habilitada la opcion agregar socio*/  }
            else
            { metroTile1.Enabled = false; }


            //Habilitar / deshabilitar la opcion buscar socio del menu
            respuesta = EsActivoModuloController(3) ? true : false;
            if (respuesta)
            {   /*Permanece habilitada la opcion buscar socio*/  }
            else
            { metroTile3.Enabled = false; }


            //Habilitar / deshabilitar la opcion Actualizar socio del menu
            respuesta = EsActivoModuloController(4) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion actualizar socio */  }
            else
            { metroTile2.Enabled = false; }




            //Habilitar / deshabilitar la opcion Abrir caja
            respuesta = EsActivoModuloController(8) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Abrir caja, del menu*/}
            else { metroTile4.Enabled = false; }


            respuesta = EsActivoModuloController(9) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Buscar folios de recibo de pago de socio*/}
            else { metroTile6.Enabled = false; }


            respuesta = EsActivoModuloController(10) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Buscar folios de recibo de pago de todos los socios*/}
            else { metroTile7.Enabled = false; }


            respuesta = EsActivoModuloController(11) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Cancelar folio de recibo de pago */}
            else { metroTile5.Enabled = false; }


            respuesta = EsActivoModuloController(12) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Buscar folios de pago cancelados */}
            else { metroTile8.Enabled = false; }


            respuesta = EsActivoModuloController(13) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Gastos (lo de capturar info de cheques) */}
            else { metroTile11.Enabled = false; }


            respuesta = EsActivoModuloController(14) ? true : false;
            if (respuesta)
            {  /*Permanece habilitada la opcion modificar gastos */}
            else { metroTile14.Enabled = false; }

            respuesta = EsActivoModuloController(15) ? true : false;
            if(respuesta)
            { /*Permanece habilitada la opcion eliminar cheque capturado*/   }
            else { metroTile10.Enabled = false; }


            respuesta = EsActivoModuloController(16) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion cheques no cobrados en el periodo*/ }
            else { metroTile12.Enabled = false; }

            respuesta = EsActivoModuloController(17) ? true : false;
            if (respuesta)
            {  /*Permance habilitada la opcion cheques cobrados en el periodo*/ }
            else { metroTile13.Enabled = false; }

            respuesta = EsActivoModuloController(18) ? true : false;
            if (respuesta)
            {   /*Permanece habilitada la opcion cheques de periodo*/  }
            else { metroTile15.Enabled = false; }


            respuesta = EsActivoModuloController(19) ? true : false;
            if (respuesta)
            {   /*Permanece habilitada la opcion insertar proveedor*/  }
            else { metroTile9.Enabled = false; }

            respuesta = EsActivoModuloController(20) ? true : false;
            if (respuesta)
            {   /*Permanece habilitada la opcion actualizar proveedor*/  }
            else { metroTile16.Enabled = false; }

            respuesta = EsActivoModuloController(21) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion eliminar proveedor*/ }
            else { metroTile17.Enabled = false; }

            respuesta = EsActivoModuloController(22) ? true : false;
            if (respuesta)
            {  /*Permanece habilitada la opcion ver proveedores*/  }
            else { metroTile18.Enabled = false; }

            respuesta = EsActivoModuloController(23) ? true : false;
            if(respuesta)
            {   /*Permanece habilitada la opcion sumar folios de usuario*/ }
            else { metroTile19.Enabled = false; }


            respuesta = EsActivoModuloController(24) ? true : false;
            if (respuesta)
            {   /*Permanece habilitada la opcion ver informe egresos- ingresos*/ }
            else { metroTile21.Enabled = false; }

            respuesta = EsActivoModuloController(25) ? true : false;
            if(respuesta)
            {  /*Permanece habilitada la opción sumar productos*/}
            else { metroTile20.Enabled = false; }

            respuesta = EsActivoModuloController(26) ? true : false;
            if (respuesta)
            {    /*Permanece habilitada la opción Bancos - crear periodo*/ }
            else { metroTile22.Enabled = false; }


            respuesta = EsActivoModuloController(28) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion Bancos - modificar saldo*/}
            else { metroTile24.Enabled = false; }

            respuesta = EsActivoModuloController(29) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opcion: Consultar folios de recibo de pago , imprimir exportar activos*/}
            else { metroTile23.Enabled = false; }


            respuesta = EsActivoModuloController(30) ? true : false;
            if(respuesta)
            { /* Permanece habilitada la opción: Socio, imprimir exportar*/}
            else
            {   metroTile25.Enabled = false;  }

            respuesta = EsActivoModuloController(31) ? true : false;
            if(respuesta)
            { /*Permanece habilitada la opción: Consular folios de recibo de pago, imprimir-exportar activos y cancelados*/}
            else
            {  metroTile26.Enabled = false; }


            respuesta = EsActivoModuloController(32) ? true : false;
            if(respuesta)
            { /*Permanece habilitada la opción: Proveedor, agregar producto o servicio*/}
            else
            {  metroTile27.Enabled = false;   }


            respuesta = EsActivoModuloController(33) ? true : false;
            if(respuesta)
            { /*Permanece habilitada la opción: Proveedor, editar producto o servicio*/}
            else
            { metroTile28.Enabled = false; }


            respuesta = EsActivoModuloController(34) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opción: Beneficiario de cheque, nuevo*/}
            else
            { metroTile30.Enabled = false; }

            respuesta = EsActivoModuloController(35) ? true : false;
            if(respuesta)
            { /*Permanece habilitada la opción: Beneficiario de cheque, Ver todos*/}
            else
            { metroTile31.Enabled = false; }

            respuesta = EsActivoModuloController(36) ? true : false;
            if (respuesta)
            { /* Permanece habilitada la opción: Beneficiario de cheque, Eliminar */}
            else
            { metroTile29.Enabled = false; }

            respuesta = EsActivoModuloController(38) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opción: Informe egresos / ingresos, Informe mensual*/}
            else
            {  metroTile32.Enabled = false; }

            respuesta = EsActivoModuloController(39) ? true : false;
            if(respuesta)
            { /*Permanece habilitada la opción: Informe egresos / ingresos, Total inicial de 
                cheques cobrados de periodos anteriores*/ }
            else
            {  metroTile33.Enabled = false;   }

            //el modulo 40 es el boton Informe egresos-ingresos, exportar a excel
            //el modulo 41 es el boton Informe egresos-ingresos mensual, exportar a excel


            respuesta = EsActivoModuloController(42) ? true : false;
            if (respuesta)
            { /*Permanece habilitada la opción: Informe x elementos de proveedor */
            }
            else
            { metroTile34.Enabled = false; }


            respuesta = EsActivoModuloController(44) ? true : false;
            if(respuesta)
            {/*Permance habilitada la opción: Activos y cancelados de socio*/ }
            else
            { metroTile35.Enabled = false; }


            respuesta = EsActivoModuloController(45) ? true : false;
            if (respuesta)
            {/*Permance habilitada la opción: Consultar folios de recibo de pago - productos pagados y no pagados*/ }
            else
            { metroTile36.Enabled = false; }


            respuesta = EsActivoModuloController(47) ? true : false;
            if (respuesta)
            { /*Permancede habilitada en el combobox el item "Editar datos de conexión*/}
            else
            { metroComboBox1.Items.Remove("Editar datos de conexión");  }

        }

        private void MostrarUsuarioDeSesionEnMetroLabel()
        {
            DataRow filaUnica = (ClsLogin.Usuario).AsEnumerable().Single();
            metroLabel5.Text = "Usuario: " + filaUnica.Field<string>("Usuario");
        }


        //------------------------Events

        private void metroTile3_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSocioBusca());
        }

        private void metroTile2_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSocioActualizar());
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSocioAdd());
        }

        private async void metroTile4_Click(object sender, EventArgs e)
        {
            await tarea;
            AbrirFormulario(new FrmRealizarCobro());
        }

        private void metroTile5_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmCancelarFolioReciboDePago());
        }

        private void metroTile6_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBuscarFoliosDeSocio());
        }

        private void metroTile7_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBuscarFoliosDeTodos());
        }

        private void metroTile8_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBuscarFoliosCanceladosDeTodos());
        }

        private void metroTile11_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmAgregarCheque());
        }

        private void metroTile14_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmActualizarInfoCheque());
        }

        private void metroTile10_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmEliminarChequeCapturado());
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(metroComboBox1.SelectedIndex > 0 )
            {
                if(metroComboBox1.SelectedItem.ToString() == "Crear producto")
                {
                    AbrirFormulario(new FrmProductoCrear());
                }

                else if(metroComboBox1.SelectedItem.ToString() == "Crear tarifas de producto")
                {
                    AbrirFormulario(new FrmCrearTarifaAProducto());
                }

                else if(metroComboBox1.SelectedItem.ToString() == "Editar tarifas de producto")
                {
                    AbrirFormulario(new FrmEditarTarifasDeProducto());
                }

                else if (metroComboBox1.SelectedItem.ToString() == "Agregar usuario")
                {
                    AbrirFormulario(new FrmUsuarioAgregar());
                }

                else if (metroComboBox1.SelectedItem.ToString() == "Modificar usuario")
                {
                    AbrirFormulario(new FrmUsuarioActualizar());
                }

                else if (metroComboBox1.SelectedItem.ToString() == "Privilegios de usuario")
                {
                    AbrirFormulario(new FrmUsuarioAsignarPrivilegios());
                }

                else if (metroComboBox1.SelectedItem.ToString() == "Editar datos de conexión")
                {
                    AbrirFormulario(new FrmConfigurarInfoConexion());
                }

                metroComboBox1.SelectedIndex = 0;
            }
        }

        private void metroTile12_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVisorReporteChequesNoCobrados());
        }


        private void metroTile13_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVisorReporteChequesCobrados());
        }

        private void metroTile15_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVisorReporteChequesDePeriodo());
        }

        private void FrmPrincipal3_Load(object sender, EventArgs e)
        {

        }

        private void metroTile9_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedorAgregar());
        }

        private void metroTile16_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedorActualizar());
        }

        private void metroTile17_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedorEliminar());
        }

        private void metroTile18_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedoresVer());
        }

        private void metroTile19_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSumarFoliosDePagoPorUsuario());
        }

        private void metroTile21_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVisorReporteIngresosEgresos());
        }

        private void metroTile20_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSumarProductosPorProducto());
        }

        private void metroTile22_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBancosCrearPeriodo());
        }


        private void metroTile24_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBancosActualizar());
        }

        private void metroTile23_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBuscarFoliosActivosDeTodosImprimirExportar());
        }

        private void metroTile25_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSocioImprimirExportar());
        }

        private void metroTile26_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBuscarFoliosActivosYCanceladosDeTodosImprimirExportar());
        }

        private void metroTile27_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedorAgregarElemento());
        }

        private void metroTile28_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmProveedorEditarElemento());
        }

        private void metroTile30_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBeneficiarioChequeCrear());
        }

        private void metroTile31_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBeneficiarioChequeVer());
        }

        private void metroTile29_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBeneficiarioChequeEliminar());
        }

        private void metroTile32_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVisorReporteEgresosIngresosMensual());
        }

        private void metroTile33_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmChequesCobradosDePeriodosAnterioresTotalInicial());
        }


        private void metroTile34_Click_1(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmVisorReporteEgresosIngresosMensualAgruparXElementoDeProveedor());
        }

        private void metroTile35_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBuscarFoliosActivosYCanceladosDeSocioImprimirExportar());
        }

        private void metroTile36_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmSocioBuscarProductosPagadosYNoPagados());
        }
    }
}
