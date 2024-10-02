using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Service
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicDTO>> GetAllTopicsAsync();
        Task<TopicDTO?> GetTopicByIdAsync(int id);
        Task<TopicDTO> CreateTopicAsync(CreateTopicDto createTopicDto);
        Task UpdateTopicAsync(int id, UpdateTopicDto updateTopicDto);
        Task DeleteTopicAsync(int id);


    }
}
