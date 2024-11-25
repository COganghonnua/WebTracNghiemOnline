﻿using WebTracNghiemOnline.Models;

namespace WebTracNghiemOnline.Repository
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
