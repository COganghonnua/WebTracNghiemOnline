using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;
using WebTracNghiemOnline.Service;

namespace WebTracNghiemOnline.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IMapper _mapper;

        public TopicService(ITopicRepository topicRepository, IMapper mapper)
        {
            _topicRepository = topicRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TopicDTO>> GetAllTopicsAsync()
        {
            var topics = await _topicRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TopicDTO>>(topics);
        }

        public async Task<TopicDTO?> GetTopicByIdAsync(int id)
        {
            var topic = await _topicRepository.GetByIdAsync(id);
            return topic == null ? null : _mapper.Map<TopicDTO>(topic);
        }

        public async Task<TopicDTO> CreateTopicAsync(CreateTopicDto createTopicDto)
        {
            var topic = _mapper.Map<Topic>(createTopicDto);
            var createdTopic = await _topicRepository.CreateAsync(topic);
            return _mapper.Map<TopicDTO>(createdTopic);
        }

        public async Task<TopicDTO> UpdateTopicAsync(int id, UpdateTopicDto updateTopicDto)
        {
            var existingTopic = await _topicRepository.GetByIdAsync(id);
            if (existingTopic == null)
            {
                throw new KeyNotFoundException($"Topic with ID {id} not found.");
            }

            // Map dữ liệu từ DTO sang model
            _mapper.Map(updateTopicDto, existingTopic);

            // Cập nhật và lấy lại dữ liệu đã cập nhật
            var updatedTopic = await _topicRepository.UpdateAsync(existingTopic);

            // Map dữ liệu đã cập nhật sang DTO
            return _mapper.Map<TopicDTO>(updatedTopic);
        }


        public async Task DeleteTopicAsync(int id)
        {
            if (!await _topicRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Topic with ID {id} not found.");
            }

            await _topicRepository.DeleteAsync(id);
        }

    }
}
