﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.ReportSource;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CapaLogicaNegocios;

namespace CapaPresentacion
{
    public partial class FrmVisorReporteChequesCobrados : Form
    {
        //----------------------Constructor
        public FrmVisorReporteChequesCobrados()
        {
            InitializeComponent();
        }

        //--------------------Methods controller
        private DataTable Cheque_RecuperarDetallesDeChequesCobradosController(DateTime fechaInicio, DateTime fechaFin)
        {
            ClsCheque clsCheque = new ClsCheque();
            clsCheque.FechaAlta = fechaInicio;
            clsCheque.FechaModificacion = fechaFin;

            return (clsCheque.Cheque_RecuperarDetallesDeChequesCobrados());
        }


        //------------------------Utils
        private string MuestraFechaDeBusquedaSinLaHora(DateTime fecha)
        {
            return (fecha.ToShortDateString());
        }


        //----------------------Events
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fechaInicio;
                DateTime fechaFin;


                fechaInicio = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month,
                        dateTimePicker2.Value.Day, 0, 1, 0);

                fechaFin = new DateTime(dateTimePicker3.Value.Year, dateTimePicker3.Value.Month,
                        dateTimePicker3.Value.Day, 23, 59, 58);

                DataTable tablaNatural = Cheque_RecuperarDetallesDeChequesCobradosController(fechaInicio, fechaFin);
                if(tablaNatural.Rows.Count == 0)
                {
                    crystalReportViewer1.ReportSource = null;
                    MessageBox.Show("No se encontraron cheques con fecha de cobro en el rango de fechas solicitado", "Resultado de operación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                else
                {
                    CRReporteChequesCobrados crReporte = new CRReporteChequesCobrados();
                    crReporte.SetDatabaseLogon("sa", "modomixto", "CRUZ2-THINK", "DBCajaCuentas2");
                    crReporte.SetParameterValue("@fechaInicio", fechaInicio);
                    crReporte.SetParameterValue("@fechaFin", fechaFin);

                    TextObject periodoDeBusquedaTextObject = crReporte.ReportDefinition.ReportObjects["Text12"] as TextObject;
                    periodoDeBusquedaTextObject.Text = "periodo " + MuestraFechaDeBusquedaSinLaHora(fechaInicio) + " a " + MuestraFechaDeBusquedaSinLaHora(fechaFin);
                    crystalReportViewer1.ReportSource = crReporte;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fechaInicio;
                DateTime fechaFin;


                fechaInicio = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month,
                    dateTimePicker2.Value.Day, 0, 1, 0);

                fechaFin = new DateTime(dateTimePicker3.Value.Year, dateTimePicker3.Value.Month,
                    dateTimePicker3.Value.Day, 23, 59, 58);



                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string nomArchivo = saveFileDialog1.FileName;

                    //http://aspalliance.com/478_Exporting_to_Excel_in_Crystal_Reports_NET__Perfect_Excel_Exports.3
                    //https://www.c-sharpcorner.com/UploadFile/mahesh/savefiledialog-in-C-Sharp/
                    CRReporteChequesCobradosParaExportar reporte = new CRReporteChequesCobradosParaExportar();
                    reporte.SetDatabaseLogon("sa", "modomixto", "CRUZ2-THINK", "DBCajaCuentas2");
                    reporte.SetParameterValue("@fechaInicio", fechaInicio);
                    reporte.SetParameterValue("@fechaFin", fechaFin);

                    //Ponerle las fechas de busqueda al reporte
                    TextObject periodoDeBusquedaTextObject = reporte.ReportDefinition.ReportObjects["Text19"] as TextObject;
                    periodoDeBusquedaTextObject.Text = MuestraFechaDeBusquedaSinLaHora(fechaInicio) + " a " + MuestraFechaDeBusquedaSinLaHora(fechaFin);


                    // Declare variables and get the export options.
                    ExportOptions exportOpts = new ExportOptions();
                    ExcelFormatOptions excelFormatOpts = new ExcelFormatOptions();
                    DiskFileDestinationOptions diskOpts = new DiskFileDestinationOptions();
                    exportOpts = reporte.ExportOptions;
                    // Set the excel format options.
                    excelFormatOpts.ExcelUseConstantColumnWidth = false;
                    excelFormatOpts.ShowGridLines = true;

                    //exportOpts.ExportFormatType = ExportFormatType.ExcelRecord;
                    exportOpts.ExportFormatType = ExportFormatType.Excel;
                    exportOpts.FormatOptions = excelFormatOpts;
                    // Set the disk file options and export.
                    exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
                    //diskOpts.DiskFileName = "miotroreporttttte.xls";
                    diskOpts.DiskFileName = nomArchivo;
                    exportOpts.DestinationOptions = diskOpts;
                    reporte.Export();

                    MessageBox.Show("Exportacion lista", "Resultado de operación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.Source + " " + ex.StackTrace);
            }
        }
    }
}
