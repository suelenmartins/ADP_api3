using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using ADP_api.Models;
using System.Text;

namespace ADP_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ADPController : ControllerBase
    {

        static HttpClient client = new HttpClient();

        private readonly ILogger<ADPController> _logger;

        public ADPController(ILogger<ADPController> logger)
        {
            _logger = logger;
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            _logger.LogInformation("Rodando logger...");
            return Ok();
        }

        static async Task<Instruction> GetInstructionAsync(string path)
        {
            Instruction instruction = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                instruction = await response.Content.ReadAsAsync<Instruction>();
            }
            return instruction;
        }

        static async Task<string> PostResultAsync(string path, HttpContent c)
        {
            var response = string.Empty;

            HttpResponseMessage result = await client.PostAsync(path, c);
            
            result.EnsureSuccessStatusCode();

            if (result.IsSuccessStatusCode)
            {
                response = await result.Content.ReadAsAsync<string>();
            }

            return response;
        }
        

        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://interview.adpeai.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                Instruction instruction = null;

                // Get the instruction
                instruction = await GetInstructionAsync("api/v1/get-task");

                // Calculation
                switch (instruction.Operation)
                {
                    case "multiplication":
                        instruction.Result = (instruction.Left * instruction.Right);
                        break;
                    case "addition":
                        instruction.Result = instruction.Left + instruction.Right;
                        break;
                    case "remainder":
                        instruction.Result = (instruction.Left % instruction.Right);
                        break;
                    case "subtraction":
                        instruction.Result = instruction.Left - instruction.Right;
                        break;
                    case "division":
                        if (instruction.Right == 0)
                        {
                            instruction.Result = 0;
                        }
                        else
                        {
                            instruction.Result = (instruction.Left / instruction.Right);
                        }
                        break;
                    default:
                        instruction.Result = instruction.Left + instruction.Right;
                        break;
                }

                

                var payload = "{\"id\":\"" + instruction.Id + "\",\"result\":" + instruction.Result + "}";

                string strPayload = JsonConvert.SerializeObject(payload);
                
                HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                var t = Task.Run(() => PostResultAsync("api/v1/get-task", c));
                t.Wait();



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);


            }
        }

    }
}
