using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var exams = await _examService.GetAllExamsAsync();
            return Ok(exams);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExamAsync(id);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Message);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateExamDto updateExamDto)
        {
            var result = await _examService.UpdateExamAsync(id, updateExamDto);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPost("random")]
        public async Task<IActionResult> CreateRandomExam([FromBody] CreateExamWithQuestionsDto request)
        {
            var result = await _examService.CreateRandomExamAsync(
                new CreateExamDto
                {
                    ExamName = request.ExamName,
                    Fee = request.Fee,
                    SubjectId = request.SubjectId,
                    Duration = request.Duration,
                    
                },
                request.NumberOfQuestions);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Message, details = result.Data });
            }

            return Ok(result.Data);
        }
        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetExamWithQuestions(int id)
        {
            var examWithQuestions = await _examService.GetExamWithQuestionsAsync(id);
            if (examWithQuestions == null)
            {
                return NotFound(new { Status = 404, Message = "Exam not found or no questions available", Data = (object)null });
            }

            return Ok(new { Status = 200, Message = "Exam questions retrieved successfully", Data = examWithQuestions });
        }
        [HttpPost("{id}/submit")]
        [Authorize]
        public async Task<IActionResult> SubmitExam(int id, [FromBody] SubmitExamDto submitExamDto)
        {
            Console.WriteLine($"Exam ID: {id}");
            Console.WriteLine($"User Answers: {JsonSerializer.Serialize(submitExamDto)}");

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy ID người dùng từ JWT
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authenticated." });
                }

                var result = await _examService.SubmitExamAsync(id, userId, submitExamDto);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = "Exam submitted successfully.",
                    score = result.Data.Score,
                    totalQuestions = result.Data.TotalQuestions,
                    correctAnswers = result.Data.CorrectAnswers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }
        [HttpPost("check")]
        public async Task<IActionResult> CheckExamAnswers([FromBody] ExamSubmissionDto examSubmission)
        {
            var result = await _examService.CheckExamAnswersAsync(examSubmission);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { score = result.Score });
        }

        [HttpGet("{examHistoryId}/details")]
        [Authorize]
        public async Task<IActionResult> GetExamHistoryDetails(int examHistoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _examService.GetExamHistoryDetailsAsync(examHistoryId, userId);
            if (!result.Success) return NotFound(result.Message);

            return Ok(result.Data);
        }


    }



}
