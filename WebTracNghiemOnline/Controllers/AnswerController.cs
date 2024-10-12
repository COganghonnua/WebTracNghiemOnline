using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService answerService;
        private readonly ILogger<AnswerController> logger;

        public AnswerController(IAnswerService answerService, ILogger<AnswerController> logger)
        {
            this.answerService = answerService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetAnswers()
        {
            try
            {
                var answers = await answerService.GetAllAnswersAsync();
                return Ok(answers);

            } catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching all answers");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerDTO>> GetAnswer(int id)
        {

            try
            {
                //gọi dữ liệu ở repository


               /* --------------------*/



                //logic xử lý dữ liệu.
            }
            catch
            {

            }


            try
            {
                var answer = await answerService.GetAnswerByIdAsync(id);
                if (answer == null)
                {
                    return NotFound();
                }
                return Ok(answer);
            }catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching answer with ID: {AnswerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AnswerDTO>> CreateAnswer(CreateAnswerDto createAnswerDto)
        {
            try
            {
                var answerCreated = await answerService.CreateAnswerAsync(createAnswerDto);
                return CreatedAtAction(nameof(GetAnswer), new {id = answerCreated.AnswerId} ,answerCreated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a new answer");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnswer(int id, UpdateAnswerDto updateAnswerDto)
        {
            try
            {
                await answerService.UpdateAnswerAsync(id, updateAnswerDto);
                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating answer with ID: {AnswerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(int id) 
        {
            try
            {
                await answerService.DeleteAnswerAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting answer with ID: {AnswerId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
