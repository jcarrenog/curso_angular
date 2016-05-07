using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    using Models;

    [RoutePrefix("items")]
    public class TodoItemController : ApiController
    {
        static int _lastId = 0;
        static readonly List<TodoItem> _baseDatos = new List<TodoItem>();

        [Route("")]
        public TodoItem[] GetAll()
        {
            // No devolvemos un IHttpActionResult porque este método sólo devuelve 200 - Ok
            return DataBase.ToArray();
        }

        [Route("{id}", Name = "GetById")]
        public TodoItem GetById(int id)
        {
            // No devolvemos un IHttpActionResult porque este método sólo devuelve 200 - Ok
            return DataBase.Where(item => item.Id == id).FirstOrDefault();
        }

        [Route(""), HttpPut]
        public IHttpActionResult Update(TodoItem newItem)
        {
            if (newItem == null)
            {
                // Esto no está permitido, así que informo de que la petición está mal formada
                return BadRequest();
            }

            var item = DataBase.Where(itm => itm.Id == newItem.Id).FirstOrDefault();

            if(item == null)
            {
                return NotFound();
            }

            // Sólo modificamos los campos que nos lleguen
            if (!string.IsNullOrEmpty(newItem.Descripcion))
                item.Descripcion = newItem.Descripcion;

            if (newItem.Completada.HasValue)
                item.Completada = newItem.Completada;

            // Devolvemos un 200 - OK, con el recurso modificado
            return Ok(item);
        }

        [Route(""), HttpPost]
        public IHttpActionResult Add(TodoItem newItem)
        {
            if (newItem == null)
            {
                // Esto no está permitido, así que informo de que la petición está mal formada
                return BadRequest();
            }

            // ¿Está intentando crear una tarea que ya tiene ID?
            if (newItem.Id != 0)
            {
                // Si existe, damos un error
                var idx = DataBase.FindIndex(itm => itm.Id == newItem.Id);
                if (idx != -1)
                {
                    return Conflict();
                }
            }

            newItem.Id = GetNextId();
            DataBase.Add(newItem);

            // Devolvemos un 201 - Created, con un enlace al recurso recién creado y el propio recurso
            return Created(Url.Link("GetById", new { id = newItem.Id }), newItem);
        }

        [Route("{id}"), HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var idx = DataBase.FindIndex(item => item.Id == id);
            
            if(idx == -1)
            {
                return NotFound();
            }

            var itemBorrado = DataBase[idx];
            DataBase.RemoveAt(idx);

            // Devolvemos un 200 - OK, con el recurso recíén borrado
            return Ok(itemBorrado);
        }

        private List<TodoItem> DataBase
        {
            get
            {
                return _baseDatos;
            }
        }

        private int GetNextId()
        {
            return ++_lastId;
        }
    }
}
