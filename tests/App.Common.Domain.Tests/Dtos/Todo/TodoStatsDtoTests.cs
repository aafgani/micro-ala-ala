using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using App.Common.Domain.Dtos.Todo;

namespace App.Common.Domain.Tests.Dtos.Todo
{
    public class TodoStatsDtoTests
    {
        [Fact]
        public void TodoStatsDto_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var statsDto = new TodoStatsDto();

            // Assert
            Assert.Equal(0, statsDto.TotalTodos);
            Assert.Equal(0, statsDto.CompletedTodos);
            Assert.Equal(0, statsDto.PendingTodos);
            Assert.Equal(0.0, statsDto.CompletionRate);
            Assert.Null(statsDto.TodaysCreated);
            Assert.Null(statsDto.TodaysCompleted);
        }


        [Fact]
        public void TodoStatsDto_Should_SetAndGetPropertiesCorrectly()
        {
            // Arrange
            var statsDto = new TodoStatsDto();

            // Act
            statsDto.TotalTodos = 100;
            statsDto.CompletedTodos = 75;
            statsDto.PendingTodos = 25;
            statsDto.CompletionRate = 75.0;
            statsDto.TodaysCreated = 5;
            statsDto.TodaysCompleted = 3;

            // Assert
            Assert.Equal(100, statsDto.TotalTodos);
            Assert.Equal(75, statsDto.CompletedTodos);
            Assert.Equal(25, statsDto.PendingTodos);
            Assert.Equal(75.0, statsDto.CompletionRate);
            Assert.Equal(5, statsDto.TodaysCreated);
            Assert.Equal(3, statsDto.TodaysCompleted);
        }

        [Theory]
        [InlineData(0, 0, 0.0)] // No todos
        [InlineData(10, 0, 0.0)] // No completed todos
        [InlineData(10, 5, 50.0)] // Half completed
        [InlineData(10, 10, 100.0)] // All completed
        [InlineData(100, 75, 75.0)] // Typical case
        public void TodoStatsDto_Should_HandleVariousCompletionRateScenarios(
          int totalTodos,
          int completedTodos,
          double expectedCompletionRate)
        {
            // Arrange
            var statsDto = new TodoStatsDto
            {
                TotalTodos = totalTodos,
                CompletedTodos = completedTodos,
                PendingTodos = totalTodos - completedTodos,
                CompletionRate = expectedCompletionRate
            };

            // Assert
            Assert.Equal(totalTodos, statsDto.TotalTodos);
            Assert.Equal(completedTodos, statsDto.CompletedTodos);
            Assert.Equal(totalTodos - completedTodos, statsDto.PendingTodos);
            Assert.Equal(expectedCompletionRate, statsDto.CompletionRate);
        }

        [Fact]
        public void TodoStatsDto_Should_HandleNullablePropertiesCorrectly()
        {
            // Arrange & Act
            var statsDto = new TodoStatsDto
            {
                TotalTodos = 50,
                CompletedTodos = 30,
                PendingTodos = 20,
                CompletionRate = 60.0,
                TodaysCreated = null, // Explicitly set to null
                TodaysCompleted = null
            };

            // Assert
            Assert.Equal(50, statsDto.TotalTodos);
            Assert.Equal(30, statsDto.CompletedTodos);
            Assert.Equal(20, statsDto.PendingTodos);
            Assert.Equal(60.0, statsDto.CompletionRate);
            Assert.Null(statsDto.TodaysCreated);
            Assert.Null(statsDto.TodaysCompleted);
        }

        [Fact]
        public void TodoStatsDto_Should_SerializeToJsonCorrectly()
        {
            // Arrange
            var statsDto = new TodoStatsDto
            {
                TotalTodos = 100,
                CompletedTodos = 75,
                PendingTodos = 25,
                CompletionRate = 75.0,
                TodaysCreated = 5,
                TodaysCompleted = 3
            };

            // Act
            var json = JsonSerializer.Serialize(statsDto, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.Contains("\"totalTodos\":100", json);
            Assert.Contains("\"completedTodos\":75", json);
            Assert.Contains("\"pendingTodos\":25", json);
            Assert.Contains("\"completionRate\":75", json);
            Assert.Contains("\"todaysCreated\":5", json);
            Assert.Contains("\"todaysCompleted\":3", json);
        }

        [Fact]
        public void TodoStatsDto_Should_DeserializeFromJsonCorrectly()
        {
            // Arrange
            var json = """
            {
                "totalTodos": 100,
                "completedTodos": 75,
                "pendingTodos": 25,
                "completionRate": 75.0,
                "todaysCreated": 5,
                "todaysCompleted": 3
            }
            """;

            // Act
            var statsDto = JsonSerializer.Deserialize<TodoStatsDto>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(statsDto);
            Assert.Equal(100, statsDto.TotalTodos);
            Assert.Equal(75, statsDto.CompletedTodos);
            Assert.Equal(25, statsDto.PendingTodos);
            Assert.Equal(75.0, statsDto.CompletionRate);
            Assert.Equal(5, statsDto.TodaysCreated);
            Assert.Equal(3, statsDto.TodaysCompleted);
        }

        [Fact]
        public void TodoStatsDto_Should_DeserializeWithNullValues()
        {
            // Arrange
            var json = """
            {
                "totalTodos": 50,
                "completedTodos": 30,
                "pendingTodos": 20,
                "completionRate": 60.0,
                "todaysCreated": null,
                "todaysCompleted": null
            }
            """;

            // Act
            var statsDto = JsonSerializer.Deserialize<TodoStatsDto>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(statsDto);
            Assert.Equal(50, statsDto.TotalTodos);
            Assert.Equal(30, statsDto.CompletedTodos);
            Assert.Equal(20, statsDto.PendingTodos);
            Assert.Equal(60.0, statsDto.CompletionRate);
            Assert.Null(statsDto.TodaysCreated);
            Assert.Null(statsDto.TodaysCompleted);
        }
    }
}
