using AutoMapper;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository answerRepository;
        private readonly IMapper mapper;

        public AnswerService(IAnswerRepository answerRepository, IMapper mapper)
        {
            this.answerRepository = answerRepository;
            this.mapper = mapper;
        }
        public async Task<AnswerDTO> CreateAnswerAsync(CreateAnswerDto createAnswerDto)
        {
            var answerDomain = mapper.Map<Answer>(createAnswerDto);
            var answerCreated = await answerRepository.CreateAsync(answerDomain);
            return mapper.Map<AnswerDTO>(answerCreated);
        }

        public async Task DeleteAnswerAsync(int id)
        {
            if(!await answerRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"answer with ID {id} not found.");
            }
            await answerRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AnswerDTO>> GetAllAnswersAsync()
        {
            var answers = await answerRepository.GetAllAsync();

            return mapper.Map<IEnumerable<AnswerDTO>>(answers);
        }

        public async Task<AnswerDTO?> GetAnswerByIdAsync(int id)
        {
            var answer = await answerRepository.GetByIdAsync(id);
            if (answer == null)
            {
                return null;
            }
            else
            {
                return mapper.Map<AnswerDTO>(answer);
            }
        }

        public async Task UpdateAnswerAsync(int id, UpdateAnswerDto updateAnswerDto)
        {
            var existingAnswer = await answerRepository.GetByIdAsync(id);

            if(existingAnswer == null)
            {
                throw new KeyNotFoundException($"answer with ID {id} not found.");
            }
            mapper.Map(updateAnswerDto, existingAnswer);
            await answerRepository.UpdateAsync(existingAnswer);
        }
    }
}
