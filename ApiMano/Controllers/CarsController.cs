using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace ApiMano.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        //Conexion a la base de datos
        private readonly SqlConnection cnn = new SqlConnection("Data Source=DESKTOP-51CMLRF\\SQLEXPRESS;Initial Catalog=apiamano;Integrated Security=True");
    

        // GET: api/<CarsController>
        [HttpGet]
        public IEnumerable<Cars> Get() //el get no va en try catch
        {
            List<Cars> Cars = new List<Cars>();//Lista vacia que trae los datos que se cargen en la bd
            cnn.Open();
            const string query = "select * from Cars";//seleccion de todos los elementos de la tabla Autos
            SqlCommand cmd = new SqlCommand(query, cnn);
            SqlDataReader reader = cmd.ExecuteReader();//crear un reader para leer lo que traigo de la BD
            while (reader.Read())
            {
                Cars car = new Cars//creo un auto por cada entrada en la tabla base de datos 
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Model = Convert.ToString(reader["model"]),
                    Fuel = Convert.ToString(reader["fuel"]),
                    Type = Convert.ToInt32(reader["type"]),
                    Brand = Convert.ToString(reader["brand"]),
                    Price = float.Parse((string)reader["price"]),
                };
                
                Cars.Add(car);//agrego el auto que creado a la lista de autos
            }
            cnn.Close();//cierre de  la conexion con la BD
            return Cars; //Retorno la lista final formada con todos los items de la tabla Autos
        }

        // GET api/<CarsController>/5
        [HttpGet("{id}")] //get por ID trae solo un auto que especifiques la id
        public Cars Get(int id)
        {
            Cars Cars = new Cars(); //creo el auto que voy a devolver
            cnn.Open();
            const string query = "select * from Cars where id=@id"; //selecciono especificamente 1 auto donde las id coincidan
            SqlCommand cmd = new SqlCommand(query, cnn);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cars car = new Cars //creo un auto auxiliar que relleno con los datos que traigo de la tabla
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Model = Convert.ToString(reader["model"]),
                    Fuel = Convert.ToString(reader["fuel"]),
                    Type = Convert.ToInt32(reader["type"]),
                    Brand = Convert.ToString(reader["brand"]),
                    Price = float.Parse((string)reader["price"]),
                };
                Cars = car;//guardas el auto auxiliar en el auto que voy a devolver, porque al crearlo dentro del while, no puede salir
            }
            cnn.Close();
            return Cars; //regreso el auto creado al principio que ahora es el que traes de la BD
        }

        // POST api/<CarsController>
        [HttpPost]
        public ActionResult Post(Cars car) //post > inserta un auto nuevo a la BD
        {
            try //el post si va en un try catch porque puede fallar, se pueden cargar mal datos
            {
                cnn.Open();
                const string query = "INSERT INTO  Cars (model,fuel,type,brand,price) VALUES (@model,@fuel,@type,@brand,@price) ";
                //creo un  insert a la base de datos 
                SqlCommand cmd = new SqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@model", car.Model);
                cmd.Parameters.AddWithValue("@fuel", car.Fuel);
                cmd.Parameters.AddWithValue("@type", car.Type);
                cmd.Parameters.AddWithValue("@brand", car.Brand);
                cmd.Parameters.AddWithValue("@price", car.Price);
                cmd.ExecuteNonQuery();
                cnn.Close();
                return Ok(this.LastAdded()); //si funciona bien devuelve los datos del ultimo auto guardado
                 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);//si hay un error en vez de regresar un auto, devuelve el codigo del error
            }
        }
        private Cars LastAdded() //esta es una funcion extra que solo sirve como auxiliar para 
            //la funcion post, por eso es private, es interna a la clase esta. 
        {
            Cars Cars = new Cars();
            cnn.Open();
            const string query = "select top 1 * from Cars order by id desc";//seleccionas el top 1 osea el primero solamente
            //de la BD pero al poner order by id desc invertis la tabla, por lo que el primero es el ultimo
            //por ende te trae solo al ultimo auto agregado
            SqlCommand cmd = new SqlCommand(query, cnn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cars car = new Cars
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Model = Convert.ToString(reader["model"]),
                    Fuel = Convert.ToString(reader["fuel"]),
                    Type = Convert.ToInt32(reader["type"]),
                    Brand = Convert.ToString(reader["brand"]),
                    Price = float.Parse((string)reader["price"]),
                };
                Cars = car; //se iguala el auto que creao al principio con el 
                //auto auxiliar que creo aca adentro del while
            }
            cnn.Close();
            return Cars;
        }
        // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)// el delete borra un auto por id
        {
            cnn.Open();
            const string query = "delete from Cars where id=@id ";
            SqlCommand cmd = new SqlCommand(query, cnn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            cnn.Close();
        }
        
        //[HttpPut("{id}")]
        [HttpPut("{id}")]

        public void Put(int id)//
        {
            cnn.Open();
            const string query = "update from Cars";
            SqlCommand cmd = new SqlCommand(query, cnn);
            cmd.Parameters.AddWithValue("@id", id);
            
            cnn.Close();
        }

       
    }
}