﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaAccesoDatos;

namespace CapaLogicaNegocios
{
    public class ClsProductoPosee
    {
        public int IdProducto { set; get; }
        public int IdTarifa { set; get; }
        public int IdUsuarioAlta { set; get; }
        public DateTime FechaAlta { set; get; }
        public int IdUsuarioModifico { set; get; }
        public DateTime FechaModificacion { set; get; }
        public bool Activo { set; get; }

        public ClsManejador CLSManejador { set; get; }


        //-------------Constructor
        public ClsProductoPosee()
        {
            this.IdProducto = 0;
            this.IdTarifa = 0;
            this.IdUsuarioAlta = 0;
            this.FechaAlta = new DateTime();
            this.IdUsuarioModifico = 0;
            this.FechaModificacion = new DateTime();
            this.Activo = true;

            this.CLSManejador = new ClsManejador();
    }

        //----------Methods
        public DataTable ProductoPosee_innerjoin_Tarifas()
        {

            List<ClsParametros> lst = new List<ClsParametros>();
            lst.Add(new ClsParametros("@idProductoBuscado", this.IdProducto));

            return CLSManejador.Listado("ProductoPosee_innerjoin_Tarifas", lst);
        }

        public string ProductoPosee_create()
        {
            string mensaje = "";
            List<ClsParametros> lst = new List<ClsParametros>();

            lst.Add(new ClsParametros("@idProducto", this.IdProducto));
            lst.Add(new ClsParametros("@idTarifa", this.IdTarifa));
            lst.Add(new ClsParametros("@idUsuarioOperador", this.IdUsuarioAlta));

            //Parametro de salida
            lst.Add(new ClsParametros("@mensaje", SqlDbType.VarChar, 50));
            CLSManejador.Ejecutar_sp("ProductoPosee_create", lst);

            //Regresar el valor almacenado en el parametro de salida
            mensaje = lst[3].Valor.ToString();

            return (mensaje);
        }
    }
}
