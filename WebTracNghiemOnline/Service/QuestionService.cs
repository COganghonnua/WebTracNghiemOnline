using AutoMapper;
using OfficeOpenXml; // EPPlus
using System.Text.Json;
using WebTracNghiemOnline.DTO;
using WebTracNghiemOnline.Models;
using WebTracNghiemOnline.Repository;

namespace WebTracNghiemOnline.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, IMapper mapper, IAnswerRepository answerRepository)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
            _answerRepository = answerRepository;   
        }

        public async Task<IEnumerable<QuestionDTO>> GetAllQuestionsAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<QuestionDTO>>(questions);
        }

        public async Task<QuestionDTO?> GetQuestionByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            return question == null ? null : _mapper.Map<QuestionDTO>(question);
        }

        public async Task<QuestionDTO> CreateQuestionAsync(CreateQuestionDto createQuestionDto)
        {
            var question = _mapper.Map<Question>(createQuestionDto);
            var createdQuestion = await _questionRepository.CreateAsync(question);
            return _mapper.Map<QuestionDTO>(createdQuestion);
        }

        public async Task UpdateQuestionAsync(int id, UpdateQuestionDto updateQuestionDto)
        {
            var existingQuestion = await _questionRepository.GetByIdAsync(id);
            if (existingQuestion == null)
            {
                throw new KeyNotFoundException($"Question with ID {id} not found.");
            }

            _mapper.Map(updateQuestionDto, existingQuestion);
            await _questionRepository.UpdateAsync(existingQuestion);
        }

        public async Task DeleteQuestionAsync(int id)
        {
            if (!await _questionRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Question with ID {id} not found.");
            }

            await _questionRepository.DeleteAsync(id);
        }

        // Thêm logic xử lý import file Excel
        public async Task<ImportResultDto> ImportQuestionsAsync1(IFormFile file, int subjectId)
        {
            var result = new ImportResultDto();
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            for (int row = 2; row <= rowCount; row++) // Bỏ qua dòng tiêu đề
            {
                try
                {
                    var questionText = worksheet.Cells[row, 1].Value?.ToString();
                    var explanation = worksheet.Cells[row, 2].Value?.ToString();
                    var difficulty = int.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "0");

                    if (string.IsNullOrWhiteSpace(questionText) || string.IsNullOrWhiteSpace(explanation))
                    {
                        result.Errors.Add(new ImportErrorDto
                        {
                            Row = row,
                            Error = "Missing required fields: QuestionText or Explanation"
                        });
                        continue;
                    }

                    if (!Enum.IsDefined(typeof(DifficultyLevel), difficulty))
                    {
                        result.Errors.Add(new ImportErrorDto
                        {
                            Row = row,
                            Error = $"Invalid difficulty level: {difficulty}. Valid values are 1 (Easy), 2 (Medium), 3 (Hard)."
                        });
                        continue;
                    }

                    var difficultyLevel = (DifficultyLevel)difficulty;

                    var answers = new List<Answer>();
                    for (int col = 4; col <= colCount; col += 2)
                    {
                        var answerText = worksheet.Cells[row, col].Value?.ToString();
                        var isCorrectText = worksheet.Cells[row, col + 1].Value?.ToString();

                        if (!string.IsNullOrWhiteSpace(answerText))
                        {
                            bool isCorrect = false;
                            if (!string.IsNullOrWhiteSpace(isCorrectText))
                            {
                                var normalized = isCorrectText.Trim().ToLower();
                                if (normalized == "true" || normalized == "yes" || normalized == "y" || normalized == "1")
                                {
                                    isCorrect = true;
                                }
                                else if (normalized == "false" || normalized == "no" || normalized == "n" || normalized == "0")
                                {
                                    isCorrect = false;
                                }
                                else
                                {
                                    result.Errors.Add(new ImportErrorDto
                                    {
                                        Row = row,
                                        Error = $"Invalid value '{isCorrectText}' for IsCorrect in column {col + 1}. Allowed values: TRUE/FALSE, Yes/No, 1/0."
                                    });
                                    continue;
                                }
                            }

                            answers.Add(new Answer
                            {
                                AnswerText = answerText,
                                IsCorrect = isCorrect
                            });
                        }
                    }

                    if (!answers.Any(a => a.IsCorrect))
                    {
                        result.Errors.Add(new ImportErrorDto
                        {
                            Row = row,
                            Error = "Each question must have at least one correct answer."
                        });
                        continue;
                    }

                    var question = new Question
                    {
                        QuestionText = questionText,
                        Explanation = explanation,
                        Difficulty = difficultyLevel,
                        SubjectId = subjectId,
                        Answers = answers
                    };

                    await _questionRepository.CreateAsync(question);

                    result.Success.Add(new ImportSuccessDto { Row = row, QuestionText = questionText });
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ImportErrorDto
                    {
                        Row = row,
                        Error = ex.Message
                    });
                }
            }

            return result;
        }
        public async Task<ImportResultDto> ImportQuestionsAsync(List<CreateQuestionDto> questions)
        {
            var result = new ImportResultDto();

            foreach (var questionDto in questions)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(questionDto.QuestionText) || string.IsNullOrWhiteSpace(questionDto.Explanation))
                    {
                        result.Errors.Add(new ImportErrorDto
                        {
                            Error = "Missing required fields: QuestionText or Explanation."
                        });
                        continue;
                    }

                    if (!Enum.IsDefined(typeof(DifficultyLevel), questionDto.Difficulty))
                    {
                        result.Errors.Add(new ImportErrorDto
                        {
                            Error = $"Invalid difficulty level: {questionDto.Difficulty}."
                        });
                        continue;
                    }

                    if (!questionDto.Answers.Any(a => a.IsCorrect))
                    {
                        result.Errors.Add(new ImportErrorDto
                        {
                            Error = "Each question must have at least one correct answer."
                        });
                        continue;
                    }

                    var question = _mapper.Map<Question>(questionDto);
                    await _questionRepository.CreateAsync(question);

                    result.Success.Add(new ImportSuccessDto { QuestionText = question.QuestionText });
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ImportErrorDto { Error = ex.Message });
                }
            }

            return result;
        }


    }
}
