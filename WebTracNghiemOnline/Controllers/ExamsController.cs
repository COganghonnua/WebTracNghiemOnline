using Microsoft.AspNetCore.Mvc;
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
                    SubjectId = request.SubjectId
                },
                request.NumberOfQuestions);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Message, details = result.Data });
            }

            return Ok(result.Data);
        }
    }

   

}
