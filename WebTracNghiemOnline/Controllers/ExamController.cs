using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamRepository examRepository;
        private readonly IMapper mapper;
        private readonly ILogger<Exam> logger;

        public ExamController(IExamRepository examRepository, IMapper mapper, ILogger<Exam> logger)
        {
            this.examRepository = examRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            try
            {
                var exams = await examRepository.GetAllAsync();
                return Ok(mapper.Map<IEnumerable<ExamDTO>>(exams));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching all question");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var exam = await examRepository.GetByIdAsync(id);
                if (exam == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<ExamDTO>(exam));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching exam with ID: {ExamId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateExamRequestDto createExamRequestDto)
        {
            try
            {
                var examDomain = mapper.Map<Exam>(createExamRequestDto);

                var examCreated = await examRepository.CreateAsync(examDomain);

                var examDto = mapper.Map<ExamDTO>(examCreated);

                return CreatedAtAction(nameof(GetById), new { id = examDto.ExamId }, examDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a new exam");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateExamRequestDto updateExamRequestDto)
        {
            try
            {
                var examDomain = mapper.Map<Exam>(updateExamRequestDto);
                examDomain = await examRepository.UpdateAsync(id, examDomain);
                if (examDomain == null)
                {
                    return NotFound();
                }
                var examDto = mapper.Map<ExamDTO>(examDomain);
                return Ok(examDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating exam with ID: {ExamId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var examDomain = await examRepository.DeleteAsync(id);
                if (examDomain == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<ExamDTO>(examDomain));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"{ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting exam with ID: {ExamId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
