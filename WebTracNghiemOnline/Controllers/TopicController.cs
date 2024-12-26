using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly ILogger<TopicsController> _logger;

        public TopicsController(ITopicService topicService, ILogger<TopicsController> logger)
        {
            _topicService = topicService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<TopicDTO>>> GetTopics()
        {
            Console.WriteLine("Da vao day roi");
            try
            {
                var topics = await _topicService.GetAllTopicsAsync();
                return Ok(topics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all topics");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDTO>> GetTopic(int id)
        {
            try
            {
                var topic = await _topicService.GetTopicByIdAsync(id);
                if (topic == null)
                {
                    return NotFound();
                }
                return Ok(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching topic with ID: {TopicId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TopicDTO>> CreateTopic(CreateTopicDto createTopicDto)
        {
            try
            {
                var createdTopic = await _topicService.CreateTopicAsync(createTopicDto);
                return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.TopicId }, createdTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new topic");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, UpdateTopicDto updateTopicDto)
        {
            try
            {
                // Cập nhật và trả về topic đã cập nhật
                var updatedTopic = await _topicService.UpdateTopicAsync(id, updateTopicDto);
                return Ok(updatedTopic); // Trả về dữ liệu mới
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating topic with ID: {TopicId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                await _topicService.DeleteTopicAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting topic with ID: {TopicId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
