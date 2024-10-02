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

        public async Task UpdateTopicAsync(int id, UpdateTopicDto updateTopicDto)
        {
            var existingTopic = await _topicRepository.GetByIdAsync(id);
            if (existingTopic == null)
            {
                throw new KeyNotFoundException($"Topic with ID {id} not found.");
            }

            _mapper.Map(updateTopicDto, existingTopic);
            await _topicRepository.UpdateAsync(existingTopic);
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
