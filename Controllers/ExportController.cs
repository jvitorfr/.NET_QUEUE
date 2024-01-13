using App.Broker;
using App.Domain.Entities;
using App.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Portifolio_queue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExportController(ILogger<Queue> logger, IQueueRepository queueRepository) : ControllerBase
    {

        private readonly ILogger<Queue> _logger = logger;
        private readonly IQueueRepository _repository = queueRepository;

        // ID EXAMPLE: A2A803B3-9B5D-473B-9408-79176080B80D
        [HttpGet("consumed/{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(_repository.Find(id));
        }

        [HttpPost("")]
        public IActionResult Post([FromBody] Queue data)
        {
            var publisher = new Publisher();
            var entity = data;
            entity.Id = Guid.NewGuid();
            _ = publisher.Sender("export", JsonSerializer.Serialize(data));
            return Ok($" Exportação enviada para fila: {entity.Id}");
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var consumer = new Consumer();
                var entity = await consumer.ConsumeAsync("export");

                if (entity != null)
                {
                    _repository.Create(entity);
                    return Ok(entity);
                }

                return NotFound("Nenhuma mensagem encontrada na fila.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar a operação: {ex.Message}");
            }
        }






        /*W
        [HttpGet(Name = "GetQueues")]
        public IEnumerable<Queue> Get([FromBody] object? parameters = null)
        {
            return _repository.FindAll((Func<Queue, bool>?)parameters);
        }
        */
    }
}
