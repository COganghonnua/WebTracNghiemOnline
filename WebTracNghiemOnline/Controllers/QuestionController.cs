using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository questionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<Question> logger;

        public QuestionController(IQuestionRepository questionRepository, IMapper mapper, ILogger<Question> logger)
        {
            this.questionRepository = questionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var questionsDomain = await questionRepository.GetAllAsync();

                return Ok(mapper.Map<IEnumerable<QuestionDTO>>(questionsDomain));
            }catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching all question");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id) 
        {
            try
            {
                var questionsDomain = await questionRepository.GetByIdAsync(id);
                if (questionsDomain == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<QuestionDTO>(questionsDomain));
            }catch(Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching question with ID: {QuestionId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }          
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequestDto createQuestionRequestDto)
        {
            try
            {
          
                var questionDomain = mapper.Map<Question>(createQuestionRequestDto);

                var questionCreated = await questionRepository.CreateAsync(questionDomain);

                var questionDto = mapper.Map<QuestionDTO>(questionCreated);

                return CreatedAtAction(nameof(GetById), new { id = questionDto.QuestionId }, questionDto);
            }catch(Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a new question");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateQuestionRequestDto updateQuestionRequestDto)
        {
            try
            {
                var questionDomain = mapper.Map<Question>(updateQuestionRequestDto);

                questionDomain = await questionRepository.UpdateAsync(id, questionDomain);

                if (questionDomain == null)
                {
                    return NotFound();
                }

                var questionDto = mapper.Map<QuestionDTO>(questionDomain);

                return Ok(questionDto);
            }catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }catch(Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating question with ID: {QuestionId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var questionDomain = await questionRepository.DeleteAsync(id);
                if (questionDomain == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<QuestionDTO>(questionDomain));

            }catch(KeyNotFoundException ex)
            {
                return NotFound($"{ex.Message}");
            }catch(Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting question with ID: {QuestionId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
