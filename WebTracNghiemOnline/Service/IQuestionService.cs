﻿using WebTracNghiemOnline.DTO;

namespace WebTracNghiemOnline.Service
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDTO>> GetAllQuestionsAsync();
        Task<QuestionDTO?> GetQuestionByIdAsync(int id);
        Task<QuestionDTO> CreateQuestionAsync(CreateQuestionDto createQuestionDto);
        Task UpdateQuestionAsync(int id, UpdateQuestionDto updateQuestionDto);
        Task DeleteQuestionAsync(int id);
        Task<ImportResultDto> ImportQuestionsAsync(IFormFile file, int subjectId);
    }
}
