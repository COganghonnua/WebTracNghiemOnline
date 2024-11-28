using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDTO>> GetQuestion(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return Ok(question);
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDTO>> CreateQuestion(CreateQuestionDto createQuestionDto)
        {
            var createdQuestion = await _questionService.CreateQuestionAsync(createQuestionDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.QuestionId }, createdQuestion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto updateQuestionDto)
        {
            try
            {
                await _questionService.UpdateQuestionAsync(id, updateQuestionDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                await _questionService.DeleteQuestionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("importbackend")]
        public async Task<IActionResult> ImportQuestions1(IFormFile file, int subjectId)
        {
            Console.WriteLine($"Received file: {file?.FileName}, SubjectId: {subjectId}");

            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File is missing or empty.");
                return BadRequest("File is required.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                Console.WriteLine("Invalid file extension.");
                return BadRequest("Only .xlsx files are supported.");
            }

            var result = await _questionService.ImportQuestionsAsync1(file, subjectId);

            if (result.Errors.Any())
            {
                Console.WriteLine("Errors during import:", JsonSerializer.Serialize(result.Errors));
                return BadRequest(result); // Trả về danh sách lỗi nếu có
            }

            Console.WriteLine("Import successful:", JsonSerializer.Serialize(result.Success));
            return Ok(result);
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportQuestions([FromBody] List<CreateQuestionDto> questions)
        {
            if (questions == null || !questions.Any())
            {
                return BadRequest("No questions provided.");
            }

            var result = await _questionService.ImportQuestionsAsync(questions);

            if (result.Errors.Any())
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


    }
}
