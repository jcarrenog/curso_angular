using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Completada { get; set; }
    }
}